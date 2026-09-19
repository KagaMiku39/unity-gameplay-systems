using System;
using UnityEngine;

public sealed class WorldItemPickupDebugLogger : IDisposable
{
    private readonly IEventSubscriber eventSubscriber;
    private bool isDisposed;

    public WorldItemPickupDebugLogger(
        IEventSubscriber eventSubscriber
    )
    {
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));

        eventSubscriber.Subscribe<WorldItemPickedUpEvent>(
            HandleWorldItemPickedUp
        );
    }

    private void HandleWorldItemPickedUp(
        WorldItemPickedUpEvent eventData
    )
    {
        Debug.Log(
            $"[拾取物品] ID：{eventData.ItemId}，" +
            $"拾取：{eventData.CollectedAmount}，" +
            $"世界剩余：{eventData.RemainingWorldAmount}"
        );
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventSubscriber.Unsubscribe<WorldItemPickedUpEvent>(
            HandleWorldItemPickedUp
        );
        isDisposed = true;
    }
}
