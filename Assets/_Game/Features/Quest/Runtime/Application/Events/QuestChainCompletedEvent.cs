using System;

public sealed class QuestChainCompletedEvent
{
    public int LastQuestId { get; }
    public string LastQuestTitle { get; }

    public QuestChainCompletedEvent(
        int lastQuestId,
        string lastQuestTitle
    )
    {
        LastQuestId = Guard.Positive(
            lastQuestId,
            nameof(lastQuestId)
        );

        if (string.IsNullOrWhiteSpace(lastQuestTitle))
        {
            throw new ArgumentException(
                "最后一个任务的标题不能为空。",
                nameof(lastQuestTitle)
            );
        }

        LastQuestTitle = lastQuestTitle;
    }
}
