using System;

public sealed class QuestRuntimeFactory : IQuestRuntimeFactory
{
    private readonly IInventoryQuery inventoryQuery;
    private readonly IEventSubscriber eventSubscriber;
    private readonly IEventPublisher eventPublisher;

    public QuestRuntimeFactory(
        IInventoryQuery inventoryQuery,
        IEventSubscriber eventSubscriber,
        IEventPublisher eventPublisher
    )
    {
        this.inventoryQuery = inventoryQuery
            ?? throw new ArgumentNullException(nameof(inventoryQuery));
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));
        this.eventPublisher = eventPublisher
            ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    public QuestRuntimeSession Create(
        QuestDefinition definition
    )
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        definition.ValidateOrThrow();

        switch (definition.questType)
        {
            case QuestType.PossessItem:
                return CreatePossessItemSession(definition);

            default:
                throw new InvalidOperationException(
                    $"尚未支持任务类型 {definition.questType}。"
                );
        }
    }

    private QuestRuntimeSession CreatePossessItemSession(
        QuestDefinition definition
    )
    {
        PossessItemQuest quest = new PossessItemQuest(
            definition
        );

        CollectItemQuestTracker tracker =
            new CollectItemQuestTracker(
                quest,
                inventoryQuery,
                eventSubscriber,
                eventPublisher
            );

        return new QuestRuntimeSession(quest, tracker);
    }
}
