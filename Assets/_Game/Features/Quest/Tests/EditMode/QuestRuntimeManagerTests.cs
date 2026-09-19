using System.Collections.Generic;
using NUnit.Framework;

public sealed class QuestRuntimeManagerTests
{
    [Test]
    public void CompletingConfiguredQuests_StartsNextAndCompletesChain()
    {
        QuestDatabase questDatabase =
            QuestDatabase.LoadFromResources();
        MutableInventoryQuery inventoryQuery =
            new MutableInventoryQuery();
        EventBus eventBus = new EventBus();
        List<int> startedQuestIds = new List<int>();
        int chainCompletedCount = 0;

        eventBus.Subscribe<QuestStartedEvent>(
            eventData => startedQuestIds.Add(eventData.QuestId)
        );

        eventBus.Subscribe<QuestChainCompletedEvent>(
            eventData => chainCompletedCount++
        );

        QuestRuntimeFactory questRuntimeFactory =
            new QuestRuntimeFactory(
                inventoryQuery,
                eventBus,
                eventBus
            );

        QuestRuntimeManager manager = new QuestRuntimeManager(
            questDatabase,
            questRuntimeFactory,
            eventBus,
            eventBus
        );

        manager.StartQuest(1);

        inventoryQuery.SetItemCount(1, 10);
        eventBus.Publish(
            new InventoryItemChangedEvent(1, 10)
        );

        inventoryQuery.SetItemCount(2, 20);
        eventBus.Publish(
            new InventoryItemChangedEvent(2, 20)
        );

        Assert.That(
            startedQuestIds,
            Is.EqualTo(new[] { 1, 2 })
        );
        Assert.That(chainCompletedCount, Is.EqualTo(1));
        Assert.That(manager.ActiveQuest, Is.Null);

        manager.Dispose();
    }

    private sealed class MutableInventoryQuery : IInventoryQuery
    {
        private readonly Dictionary<int, int> itemCounts =
            new Dictionary<int, int>();

        public void SetItemCount(int itemId, int amount)
        {
            itemCounts[itemId] = amount;
        }

        public int GetItemCount(int itemId)
        {
            int amount;
            bool found = itemCounts.TryGetValue(
                itemId,
                out amount
            );

            return found ? amount : 0;
        }
    }
}
