using System;

public sealed class QuestCompletedEvent
{
    public int QuestId { get; }
    public string Title { get; }

    public QuestCompletedEvent(
        int questId,
        string title
    )
    {
        QuestId = Guard.Positive(
            questId,
            nameof(questId)
        );

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(
                "任务标题不能为空。",
                nameof(title)
            );
        }

        Title = title;
    }
}
