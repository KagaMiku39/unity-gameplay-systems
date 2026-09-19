using System;
using System.Collections.Generic;

public sealed class EventBus : IEventPublisher, IEventSubscriber
{
    private readonly Dictionary<Type, List<Delegate>> handlerLists =
        new Dictionary<Type, List<Delegate>>();

    public void Subscribe<TEvent>(
        Action<TEvent> handler
    ) where TEvent : class
    {
        if (handler == null)
        {
            throw new ArgumentNullException(
                nameof(handler),
                "订阅事件处理器不允许为空"
            );
        }

        Type typeOfEvent = typeof(TEvent);
        List<Delegate> handlerList;

        if (handlerLists.TryGetValue(typeOfEvent, out handlerList) == false)
        {
            handlerList = new List<Delegate>();
            handlerLists.Add(typeOfEvent, handlerList);
        }

        handlerList.Add(handler);
    }

    public void Unsubscribe<TEvent>(
        Action<TEvent> handler
    ) where TEvent : class
    {
        if (handler == null)
        {
            throw new ArgumentNullException(
                nameof(handler),
                "取消订阅事件处理器不允许为空"
            );
        }

        Type typeOfEvent = typeof(TEvent);
        List<Delegate> handlerList;

        if (handlerLists.TryGetValue(typeOfEvent, out handlerList) == false)
        {
            return;
        }

        for (int i = handlerList.Count - 1; i >= 0; i--)
        {
            if (handlerList[i].Equals(handler))
            {
                handlerList.RemoveAt(i);
                break;
            }
        }

        if (handlerList.Count == 0)
        {
            handlerLists.Remove(typeOfEvent);
        }
    }

    public void Publish<TEvent>(
        TEvent eventData
    ) where TEvent : class
    {
        if (eventData == null)
        {
            throw new ArgumentNullException(
                nameof(eventData),
                "发布的事件数据不允许为空"
            );
        }

        Type typeOfEvent = typeof(TEvent);
        List<Delegate> handlerList;

        if (handlerLists.TryGetValue(typeOfEvent, out handlerList) == false)
        {
            return;
        }

        Delegate[] handlersSnapshot = handlerList.ToArray();

        foreach (Delegate registeredHandler in handlersSnapshot)
        {
            if (registeredHandler is not Action<TEvent> handler)
            {
                throw new InvalidOperationException(
                    $"事件处理器类型与 {typeOfEvent.Name} 不匹配。"
                );
            }

            handler(eventData);
        }
    }
}
