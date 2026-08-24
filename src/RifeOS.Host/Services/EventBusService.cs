using System.Collections.Concurrent;
using RifeOS.SDK.EventBus;

namespace RifeOS.Host.Services;

public sealed class EventBusService : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();
    private readonly object _lock = new();

    public async Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        List<Delegate> targets;
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var list)) return;
            targets = [.. list];
        }

        foreach (var handler in targets)
        {
            if (cancellationToken.IsCancellationRequested) break;
            if (handler is Func<TEvent, Task> asyncCallback)
            {
                await asyncCallback(eventData);
            }
        }
    }

    public IDisposable Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var type = typeof(TEvent);
        lock (_lock)
        {
            var list = _handlers.GetOrAdd(type, _ => new List<Delegate>());
            list.Add(handler);
        }

        return new SubscriptionToken(() =>
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(type, out var list))
                {
                    list.Remove(handler);
                }
            }
        });
    }

    private sealed class SubscriptionToken(Action unsubscribe) : IDisposable
    {
        private Action? _unsubscribe = unsubscribe;
        public void Dispose()
        {
            Interlocked.Exchange(ref _unsubscribe, null)?.Invoke();
        }
    }
}
