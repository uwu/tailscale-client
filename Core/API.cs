using System;
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

    private static readonly string TailscaleSocket =
        @"ProtectedPrefix\Administrators\Tailscale\tailscaled";

    private static readonly string DefaultControlURL = "https://controlplane.tailscale.com";

    public static void Init()
    {
        Debug.WriteLine("Initializing Tailscale API");

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
                                { "Accept-Encoding", "gzip" } }
        };

        InitializeBusWatcher();
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
                    Messaging.Instance.SendMessage(Messaging.MessageKind.IPNBusUpdate, key, valueStr);
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
        try
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
        catch (JsonException)
        {
            throw new Exception("Failed to parse Tailscale JSON output for " + webRequest.RequestUri);
        }
        catch (HttpRequestException e)
        {
            throw new Exception("Failed to connect to Tailscale service: " + e.Message);
        }
        catch (Exception e)
        {
            throw new Exception(
                $"Unknown failure trying to {webRequest.Method} {webRequest.RequestUri}: {e.Message}");
        }
    }

    public static T GET<T>(string endpoint)
    {
        var webRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
        return Execute<T>(webRequest);
    }

    public static T PATCH<T>(string endpoint, dynamic prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T POST<T>(string endpoint, dynamic prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T PUT<T>(string endpoint, dynamic prefs)
    {
        var webRequest =
            new HttpRequestMessage(HttpMethod.Put, endpoint) { Content = JsonContent.Create(prefs) };
        return Execute<T>(webRequest);
    }

    public static T DELETE<T>(string endpoint, dynamic prefs)
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

    public static Types.Prefs GetPrefs()
    {
        return GET<Types.Prefs>("/localapi/v0/prefs");
    }

    public static void SwitchEmptyProfile()
    {
        PUT<dynamic>("/localapi/v0/profiles/", new { });
    }

    public static void SwitchProfile(string userId)
    {
        POST<dynamic>($"localapi/v0/profiles/{userId}", new { });
    }

    public static void DeleteProfile(string userId)
    {
        DELETE<dynamic>($"localapi/v0/profiles/{userId}", new { });
    }

    public static void Start(Types.Prefs prefs)
    {
        POST<dynamic>("/localapi/v0/start", new { UpdatePrefs = prefs });
    }

    public static Types.Prefs UpdatePrefs(Types.Prefs prefs)
    {
        return PATCH<Types.Prefs>("/localapi/v0/prefs", prefs);
    }

    public static void UpdatePrefs(dynamic prefs)
    {
        PATCH<dynamic>("/localapi/v0/prefs", prefs);
    }

    public static void CheckPrefs(Types.Prefs prefs)
    {
        POST<string>("/localapi/v0/check-prefs", prefs);
    }

    public static void Logout()
    {
        POST<dynamic>("/localapi/v0/logout", new { });
    }

    public static void Login(string controlUrl)
    {
        SwitchEmptyProfile();

        var prefs =
            new Types.Prefs() { WantRunning = true, ControlURL = controlUrl };
        Start(prefs);
        POST<dynamic>("/localapi/v0/login-interactive", new { });
    }

    public static void Login()
    {
        Login(DefaultControlURL);
    }

    public static void Connect()
    {
        var currentPrefs = GetPrefs();
        currentPrefs.WantRunning = true;
        currentPrefs.WantRunningSet = true;
        UpdatePrefs(currentPrefs);
    }
    public static void Disconnect()
    {
        UpdatePrefs(new Types.Prefs { WantRunning = false, WantRunningSet = true });
    }
}
