using System.Collections.Concurrent;
using RifeOS.SDK.EventBus;

namespace RifeOS.Host.Services;

public sealed class EventBusService : IEventBus
{
    public static EventBusService Instance { get; } = new();

    private readonly ConcurrentDictionary<Type, List<Delegate>> _subscribers = new();

    public void Publish<T>(T message)
    {
        if (_subscribers.TryGetValue(typeof(T), out var handlers))
        {
            lock (handlers)
            {
                foreach (var handler in handlers.ToArray())
                {
                    if (handler is Action<T> action) action(message);
                }
            }
        }
    }

    public void Subscribe<T>(Action<T> handler)
    {
        var handlers = _subscribers.GetOrAdd(typeof(T), _ => new List<Delegate>());
        lock (handlers) { handlers.Add(handler); }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        if (_subscribers.TryGetValue(typeof(T), out var handlers))
        {
            lock (handlers) { handlers.Remove(handler); }
        }
    }
}