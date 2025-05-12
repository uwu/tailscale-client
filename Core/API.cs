﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace TailscaleClient.Core;

internal static class API
{
    private static HttpClient _client = null;

    private static readonly string TailscaleSocket = @"ProtectedPrefix\Administrators\Tailscale\tailscaled";

    private static readonly string DefaultControlURL = "https://controlplane.tailscale.com";

    public static bool Init()
    {
        Debug.WriteLine("Initializing Tailscale API");

        // Check if the Tailscale pipe exists 
        if (Directory.GetFiles($@"\\.\pipe\", "*tailscaled").Length < 1)
        {
            return false;
        }

        var httpHandler = new SocketsHttpHandler
        {
            ConnectCallback =
              async (context, token) =>
              {
                  var pipeClientStream = new NamedPipeClientStream(
                  serverName: ".", pipeName: TailscaleSocket, PipeDirection.InOut,
                  PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
                  await pipeClientStream.ConnectAsync(token);

                  return pipeClientStream;
              },
        };

        _client = new HttpClient(httpHandler)
        {
            BaseAddress = new Uri("http://local-tailscaled.sock"),
            DefaultRequestHeaders = { { "User-Agent", "Go-http-client/1.1" },
                                { "Tailscale-Cap", "95" },  // TODO: Get the real value i just
                                                            // copied it from tailscale-ipn packets
                                { "Accept-Encoding", "gzip" } },
            Timeout = TimeSpan.FromSeconds(2)
        };

        InitializeBusWatcher();
        return true;
    }

    public static void InitializeBusWatcher()
    {
        Task.Run(async () =>
        {
            var mask = (ulong)(Types.NotifyWatchOpt.NotifyInitialState |
                               Types.NotifyWatchOpt.NotifyWatchEngineUpdates |
                               Types.NotifyWatchOpt.NotifyNoPrivateKeys);

            var webRequest =
                new HttpRequestMessage(HttpMethod.Get, $"/localapi/v0/watch-ipn-bus?mask={mask}");
            var response = _client.Send(webRequest, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var stream = response.Content.ReadAsStream();
            using var reader = new StreamReader(stream);

            Debug.WriteLine("[IPN] Listening very carefully...");

            while (true)
            {
                Debug.WriteLine("[IPN] Waiting for message...");
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    Debug.WriteLine("[Critical] EOF on IPN bus... this should NOT happen.");
                    break;
                }

                // parse as json encoded string-string pairs, extract all non-empty fields, and send
                // them to the message bus
                var data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(line);
                if (data == null)
                {
                    Debug.WriteLine("[Critical] Failed to parse IPN bus message.");
                    continue;
                }

                foreach (var (key, value) in data)
                {
                    var valueStr = Convert.ToString(value);
                    if (string.IsNullOrEmpty(valueStr))
                    {
                        continue;
                    }
                    Debug.WriteLine($"[IPN] {key}: {valueStr}");
                    var messageKind = key == "Health" ? Messaging.MessageKind.HealthUpdate : Messaging.MessageKind.IPNBusUpdate;
                    Messaging.Instance.SendMessage(messageKind, key, valueStr);
                }
            }
        });
    }

    public static void Dispose()
    {
        _client.Dispose();
    }

    private static T Execute<T>(HttpRequestMessage webRequest)
    {
        var response = _client.Send(webRequest);
        response.EnsureSuccessStatusCode();

        using var reader = new StreamReader(response.Content.ReadAsStream());
        var body = reader.ReadToEnd();

        // evil trickery to convert to T to string or object, trusting the user (me) to not be
        // stupid please just use JSON objects so it can deserialize properly
        if (typeof(T) == typeof(object) || typeof(T) == typeof(string))
        {
            return (T)Convert.ChangeType(body, typeof(T));
        }

        return JsonSerializer.Deserialize<T>(body);
    }

    public static T GET<T>(string endpoint)
    {
        var webRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return Execute<T>(webRequest);
    }

    public static T PATCH<T, U>(string endpoint, U prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T POST<T, U>(string endpoint, U prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T PUT<T, U>(string endpoint, U prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T DELETE<T, U>(string endpoint, U prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Delete, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static Types.Status GetStatus()
    {
        return GET<Types.Status>("/localapi/v0/status");
    }

    public static Types.Profile GetCurrentUser()
    {
        return GET<Types.Profile>("/localapi/v0/profiles/current");
    }

    public static List<Types.Profile> GetAllProfiles()
    {
        return GET<List<Types.Profile>>("/localapi/v0/profiles/");
    }

    public static Types.MaskedPrefs GetPrefs()
    {
        return GET<Types.MaskedPrefs>("/localapi/v0/prefs");
    }

    public static void SwitchEmptyProfile()
    {
        PUT<object, object>("/localapi/v0/profiles/", new { });
    }

    public static void SwitchProfile(string userId)
    {
        POST<object, object>($"localapi/v0/profiles/{userId}", new { });
    }

    public static void DeleteProfile(string userId)
    {
        DELETE<object, object>($"localapi/v0/profiles/{userId}", new { });
    }

    public static void Start(Types.MaskedPrefs prefs)
    {
        POST<object, object>("/localapi/v0/start", new { UpdatePrefs = prefs });
    }

    public static Types.MaskedPrefs UpdatePrefs(Types.MaskedPrefs prefs)
    {
        return PATCH<Types.MaskedPrefs, Types.MaskedPrefs>("/localapi/v0/prefs", prefs);
    }

    public static string CheckPrefs(Types.MaskedPrefs prefs)
    {
        return POST<string, Types.MaskedPrefs>("/localapi/v0/check-prefs", prefs);
    }

    public static void Logout()
    {
        POST<object, object>("/localapi/v0/logout", new { });
    }

    public static void Login(string controlUrl)
    {
        SwitchEmptyProfile();

        var prefs =
            new Types.MaskedPrefs() { WantRunning = true, ControlURL = controlUrl };
        Start(prefs);
        POST<object, object>("/localapi/v0/login-interactive", new { });
    }

    public static void Login()
    {
        Login(DefaultControlURL);
    }

    public static void Connect()
    {
        var currentPrefs = GetPrefs();
        currentPrefs.WantRunning = true;
        UpdatePrefs(currentPrefs);
    }
    public static void Disconnect()
    {
        UpdatePrefs(new Types.MaskedPrefs { WantRunning = false });
    }
}
