using System;

public sealed class CollectItemQuestTracker : IQuestTracker
{
    private readonly PossessItemQuest quest;
    private readonly IInventoryQuery inventoryQuery;
    private readonly IEventSubscriber eventSubscriber;
    private readonly IEventPublisher eventPublisher;

    private bool isStarted;
    private bool isDisposed;

    public int QuestId => quest.QuestId;

    public CollectItemQuestTracker(
        PossessItemQuest quest,
        IInventoryQuery inventoryQuery,
        IEventSubscriber eventSubscriber,
        IEventPublisher eventPublisher
    )
    {
        if (quest == null)
        {
            throw new ArgumentNullException(nameof(quest));
        }

        if (inventoryQuery == null)
        {
            throw new ArgumentNullException(nameof(inventoryQuery));
        }

        if (eventSubscriber == null)
        {
            throw new ArgumentNullException(nameof(eventSubscriber));
        }

        if (eventPublisher == null)
        {
            throw new ArgumentNullException(nameof(eventPublisher));
        }

        this.quest = quest;
        this.inventoryQuery = inventoryQuery;
        this.eventSubscriber = eventSubscriber;
        this.eventPublisher = eventPublisher;
    }

    public void Start()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(
                nameof(CollectItemQuestTracker)
            );
        }

        if (isStarted)
        {
            throw new InvalidOperationException(
                "任务 Tracker 不能重复启动。"
            );
        }

        eventSubscriber.Subscribe<InventoryItemChangedEvent>(
            HandleInventoryChanged
        );

        isStarted = true;
        RefreshProgress();
    }

    private void HandleInventoryChanged(
        InventoryItemChangedEvent eventData
    )
    {
        if (quest.IsCompleted)
        {
            return;
        }

        if (eventData.ItemId != quest.TargetItemId)
        {
            return;
        }

        RefreshProgress();
    }

    private void RefreshProgress()
    {
        int actualAmount = inventoryQuery.GetItemCount(
            quest.TargetItemId
        );

        bool justCompleted = quest.UpdateProgress(
            actualAmount
        );

        QuestProgressChangedEvent progressChangedEvent =
            new QuestProgressChangedEvent(
                quest.QuestId,
                quest.Title,
                quest.CurrentAmount,
                quest.RequiredAmount
            );

        eventPublisher.Publish(progressChangedEvent);

        if (justCompleted == false)
        {
            return;
        }

        QuestCompletedEvent completedEvent =
            new QuestCompletedEvent(
                quest.QuestId,
                quest.Title
            );

        eventPublisher.Publish(completedEvent);
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        if (isStarted)
        {
            eventSubscriber.Unsubscribe<InventoryItemChangedEvent>(
                HandleInventoryChanged
            );
        }

        isDisposed = true;
    }
}
