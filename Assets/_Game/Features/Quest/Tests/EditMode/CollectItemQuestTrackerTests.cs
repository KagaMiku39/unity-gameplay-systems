using NUnit.Framework;

public sealed class CollectItemQuestTrackerTests
{
    [Test]
    public void InventoryReachesTarget_PublishesCompletionOnce()
    {
        MutableInventoryQuery inventoryQuery =
            new MutableInventoryQuery();
        EventBus eventBus = new EventBus();
        PossessItemQuest quest = CreateQuest(10);

        int progressEventCount = 0;
        int completedEventCount = 0;

        eventBus.Subscribe<QuestProgressChangedEvent>(
            eventData => progressEventCount++
        );

        eventBus.Subscribe<QuestCompletedEvent>(
            eventData => completedEventCount++
        );

        CollectItemQuestTracker tracker =
            new CollectItemQuestTracker(
                quest,
                inventoryQuery,
                eventBus,
                eventBus
            );

        tracker.Start();

        inventoryQuery.Amount = 10;
        eventBus.Publish(
            new InventoryItemChangedEvent(1, 10)
        );

        eventBus.Publish(
            new InventoryItemChangedEvent(1, 1)
        );

        Assert.That(progressEventCount, Is.EqualTo(2));
        Assert.That(completedEventCount, Is.EqualTo(1));
        Assert.That(quest.IsCompleted, Is.True);

        tracker.Dispose();
    }

    private static PossessItemQuest CreateQuest(
        int requiredAmount
    )
    {
        QuestDefinition definition = new QuestDefinition
        {
            id = 1,
            title = "Collect Apple",
            description = "Collect apples.",
            questType = QuestType.PossessItem,
            targetItemId = 1,
            requiredAmount = requiredAmount
        };

        return new PossessItemQuest(definition);
    }

    private sealed class MutableInventoryQuery : IInventoryQuery
    {
        public int Amount { get; set; }

        public int GetItemCount(int itemId)
        {
            return Amount;
        }
    }
}
