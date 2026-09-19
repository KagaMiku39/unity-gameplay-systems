using System;
using System.Collections.Generic;

public sealed class QuestRuntimeManager : IDisposable
{
    private readonly QuestDatabase questDatabase;
    private readonly IQuestRuntimeFactory questRuntimeFactory;
    private readonly IEventSubscriber eventSubscriber;
    private readonly IEventPublisher eventPublisher;
    private readonly HashSet<int> completedQuestIds =
        new HashSet<int>();

    private IQuestRuntime activeQuest;
    private IQuestTracker activeTracker;
    private bool isStarted;
    private bool isDisposed;

    public IQuestRuntime ActiveQuest => activeQuest;

    public QuestRuntimeManager(
        QuestDatabase questDatabase,
        IQuestRuntimeFactory questRuntimeFactory,
        IEventSubscriber eventSubscriber,
        IEventPublisher eventPublisher
    )
    {
        this.questDatabase = questDatabase
            ?? throw new ArgumentNullException(nameof(questDatabase));
        this.questRuntimeFactory = questRuntimeFactory
            ?? throw new ArgumentNullException(
                nameof(questRuntimeFactory)
            );
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));
        this.eventPublisher = eventPublisher
            ?? throw new ArgumentNullException(nameof(eventPublisher));

        eventSubscriber.Subscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );
    }

    public void StartQuest(int questId)
    {
        EnsureNotDisposed();

        if (isStarted)
        {
            throw new InvalidOperationException(
                "任务运行管理器已经启动。"
            );
        }

        isStarted = true;
        StartQuestInternal(questId);
    }

    private void StartQuestInternal(int questId)
    {
        if (completedQuestIds.Contains(questId))
        {
            throw new InvalidOperationException(
                $"任务链形成循环，再次指向了任务 {questId}。"
            );
        }

        QuestDefinition definition;
        bool found = questDatabase.TryGetQuestById(
            questId,
            out definition
        );

        if (found == false)
        {
            throw new InvalidOperationException(
                $"找不到要启动的任务 ID {questId}。"
            );
        }

        QuestRuntimeSession nextSession =
            questRuntimeFactory.Create(definition);

        activeQuest = nextSession.Quest;
        activeTracker = nextSession.Tracker;

        eventPublisher.Publish(
            new QuestStartedEvent(
                activeQuest.QuestId,
                activeQuest.Title,
                activeQuest.Description
            )
        );

        activeTracker.Start();
    }

    private void HandleQuestCompleted(
        QuestCompletedEvent eventData
    )
    {
        if (activeQuest == null)
        {
            return;
        }

        if (eventData.QuestId != activeQuest.QuestId)
        {
            return;
        }

        int completedQuestId = activeQuest.QuestId;
        string completedQuestTitle = activeQuest.Title;
        int nextQuestId = activeQuest.NextQuestId;

        completedQuestIds.Add(completedQuestId);

        activeTracker.Dispose();
        activeTracker = null;
        activeQuest = null;

        if (nextQuestId == 0)
        {
            eventPublisher.Publish(
                new QuestChainCompletedEvent(
                    completedQuestId,
                    completedQuestTitle
                )
            );
            return;
        }

        StartQuestInternal(nextQuestId);
    }

    private void EnsureNotDisposed()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(
                nameof(QuestRuntimeManager)
            );
        }
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        activeTracker?.Dispose();
        eventSubscriber.Unsubscribe<QuestCompletedEvent>(
            HandleQuestCompleted
        );

        isDisposed = true;
    }
}
