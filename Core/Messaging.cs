using System;

namespace TailscaleClient.Core;

public class Messaging
{
    public enum MessageKind
    {
        ProfileSwitch, IPNBusUpdate
    }

    public class CoreMessagingEvent
    (MessageKind kind, string key, string value) : EventArgs
    {
        public MessageKind Kind { get; set; } = kind;
        public string Key { get; set; } = key;
        public string Value { get; set; } = value;
    }

    public event EventHandler<CoreMessagingEvent> MessageReceived;

    public void SendMessage(MessageKind kind, string key, string value)
    {
        OnMessageReceived(kind, key, value);
    }

    protected virtual void OnMessageReceived(
        MessageKind kind, string key,
        string value) => MessageReceived?.Invoke(this, new CoreMessagingEvent(kind, key, value));

    private static Messaging _instance;
    public static Messaging Instance
    {
        get
        {
            _instance ??= new Messaging();
            return _instance;
        }
    }
}
