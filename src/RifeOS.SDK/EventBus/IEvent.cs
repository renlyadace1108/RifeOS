namespace RifeOS.SDK.EventBus;

public interface IEvent
{
    DateTime Timestamp { get; }
    string SenderId { get; }
}
