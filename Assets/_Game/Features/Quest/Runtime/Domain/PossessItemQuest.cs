public sealed class PossessItemQuest : IQuestRuntime
{
    public int QuestId { get; }
    public string Title { get; }
    public string Description { get; }
    public int TargetItemId { get; }
    public int RequiredAmount { get; }
    public int NextQuestId { get; }
    public int CurrentAmount { get; private set; }
    public bool IsCompleted { get; private set; }

    public PossessItemQuest(
        QuestDefinition definition
    )
    {
        if (definition == null)
        {
            throw new System.ArgumentNullException(nameof(definition));
        }

        definition.ValidateOrThrow();

        if (definition.questType != QuestType.PossessItem)
        {
            throw new System.InvalidOperationException(
                $"任务 {definition.id} 不是持有物品类型。"
            );
        }

        QuestId = definition.id;
        Title = definition.title;
        Description = definition.description;
        TargetItemId = definition.targetItemId;
        RequiredAmount = definition.requiredAmount;
        NextQuestId = definition.nextQuestId;
    }

    public bool UpdateProgress(int actualAmount)
    {
        actualAmount = Guard.NonNegative(
            actualAmount,
            nameof(actualAmount)
        );

        if (IsCompleted)
        {
            return false;
        }

        CurrentAmount = System.Math.Min(
            actualAmount,
            RequiredAmount
        );

        if (CurrentAmount < RequiredAmount)
        {
            return false;
        }

        IsCompleted = true;
        return true;
    }
}
