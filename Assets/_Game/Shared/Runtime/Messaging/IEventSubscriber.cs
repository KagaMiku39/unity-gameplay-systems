using System;

public interface IEventSubscriber
{
    void Subscribe<TEvent>(Action<TEvent> handler)
        where TEvent : class;

    void Unsubscribe<TEvent>(Action<TEvent> handler)
        where TEvent : class;
}
