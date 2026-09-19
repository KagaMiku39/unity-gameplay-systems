public class QuestProgressChangedEvent
{
    public int QuestId { get; }
    public string Title { get; }
    public int CurrentAmount { get; }
    public int RequiredAmount { get; }
    public bool IsCompleted => CurrentAmount >= RequiredAmount;

    public QuestProgressChangedEvent(
        int questId,
        string title,
        int currentAmount,
        int requiredAmount
    )
    {
        QuestId = Guard.Positive(questId, nameof(questId));

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new System.ArgumentException(
                "任务标题不能为空。",
                nameof(title)
            );
        }

        Title = title;
        CurrentAmount = Guard.NonNegative(
            currentAmount,
            nameof(currentAmount)
        );
        RequiredAmount = Guard.Positive(
            requiredAmount,
            nameof(requiredAmount)
        );
    }
}
