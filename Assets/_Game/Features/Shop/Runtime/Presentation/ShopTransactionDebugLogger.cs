using System;
using UnityEngine;

public sealed class ShopTransactionDebugLogger : IDisposable
{
    private readonly IEventSubscriber eventSubscriber;
    private bool isDisposed;

    public ShopTransactionDebugLogger(
        IEventSubscriber eventSubscriber
    )
    {
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));

        eventSubscriber.Subscribe<ShopTransactionSucceededEvent>(
            HandleTransactionSucceeded
        );
    }

    private void HandleTransactionSucceeded(
        ShopTransactionSucceededEvent eventData
    )
    {
        ShopTransactionResult result = eventData.Result;

        Debug.Log(
            $"[商店交易] 类型：{result.TransactionType}，" +
            $"物品：{result.ItemId}，数量：{result.TradedAmount}，" +
            $"金币变化：{result.GoldDelta}，余额：{result.BalanceAfter}"
        );
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventSubscriber.Unsubscribe<ShopTransactionSucceededEvent>(
            HandleTransactionSucceeded
        );
        isDisposed = true;
    }
}
