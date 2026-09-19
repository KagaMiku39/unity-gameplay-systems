using System;
using UnityEngine;

public sealed class QuestProgressDebugLogger : IDisposable
{
    private readonly IEventSubscriber eventSubscriber;
    private bool isDisposed;

    public QuestProgressDebugLogger(
        IEventSubscriber eventSubscriber
    )
    {
        if (eventSubscriber == null)
        {
            throw new ArgumentNullException(nameof(eventSubscriber));
        }

        this.eventSubscriber = eventSubscriber;

        eventSubscriber.Subscribe<QuestStartedEvent>(
            HandleQuestStarted
        );

        eventSubscriber.Subscribe<QuestProgressChangedEvent>(
            HandleQuestProgressChanged
        );

        eventSubscriber.Subscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );

        eventSubscriber.Subscribe<QuestChainCompletedEvent>(
            HandleQuestChainCompleted
        );
    }

    private void HandleQuestStarted(
        QuestStartedEvent eventData
    )
    {
        Debug.Log(
            $"[任务开始] {eventData.Title}（ID：{eventData.QuestId}）"
        );
    }

    private void HandleQuestCompleted(
        QuestCompletedEvent eventData
    )
    {
        Debug.Log(
            $"[任务完成] {eventData.Title}（ID：{eventData.QuestId}）"
        );
    }

    private void HandleQuestProgressChanged(
        QuestProgressChangedEvent eventData
    )
    {
        Debug.Log(
            $"[任务进度] {eventData.Title}（ID：{eventData.QuestId}），" +
            $"进度：{eventData.CurrentAmount}/{eventData.RequiredAmount}，" +
            $"是否完成：{eventData.IsCompleted}"
        );
    }

    private void HandleQuestChainCompleted(
        QuestChainCompletedEvent eventData
    )
    {
        Debug.Log(
            $"[任务链完成] 最后完成：{eventData.LastQuestTitle}" +
            $"（ID：{eventData.LastQuestId}）"
        );
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        eventSubscriber.Unsubscribe<QuestStartedEvent>(
            HandleQuestStarted
        );

        eventSubscriber.Unsubscribe<QuestProgressChangedEvent>(
            HandleQuestProgressChanged
        );

        eventSubscriber.Unsubscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );

        eventSubscriber.Unsubscribe<QuestChainCompletedEvent>(
            HandleQuestChainCompleted
        );

        isDisposed = true;
    }
}
