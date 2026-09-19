using System;

public sealed class QuestStartedEvent
{
    public int QuestId { get; }
    public string Title { get; }
    public string Description { get; }

    public QuestStartedEvent(
        int questId,
        string title,
        string description
    )
    {
        QuestId = Guard.Positive(questId, nameof(questId));

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "任务标题不能为空。",
                nameof(title)
            );
        }

        Title = title;
        Description = description ?? string.Empty;
    }
}
