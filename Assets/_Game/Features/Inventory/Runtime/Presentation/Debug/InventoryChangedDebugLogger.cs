using System;
using UnityEngine;

public sealed class InventoryChangedDebugLogger : IDisposable
{
    private readonly IEventSubscriber iEventSubscriber;
    private bool isDisposed;

    public InventoryChangedDebugLogger(
        IEventSubscriber eventSubscriber
    )
    {
        if (eventSubscriber == null)
        {
            throw new ArgumentNullException(nameof(eventSubscriber));
        }

        this.iEventSubscriber = eventSubscriber;

        eventSubscriber.Subscribe<InventoryItemChangedEvent>(
            HandleInventoryChanged
        );
    }

    private void HandleInventoryChanged(
        InventoryItemChangedEvent eventData
    )
    {
        Debug.Log(
            $"[事件通知] 物品 ID：{eventData.ItemId}，" +
            $"数量变化：{eventData.QuantityDelta}"
        );
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        iEventSubscriber.Unsubscribe<InventoryItemChangedEvent>(
            HandleInventoryChanged
        );

        isDisposed = true;
    }
}
