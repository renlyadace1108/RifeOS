namespace RifeOS.SDK.EventBus;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : IEvent;
    IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
}
