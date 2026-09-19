using System;

public sealed class QuestRuntimeSession
{
    public IQuestRuntime Quest { get; }
    public IQuestTracker Tracker { get; }

    public QuestRuntimeSession(
        IQuestRuntime quest,
        IQuestTracker tracker
    )
    {
        Quest = quest
            ?? throw new ArgumentNullException(nameof(quest));
        Tracker = tracker
            ?? throw new ArgumentNullException(nameof(tracker));

        if (Quest.QuestId != Tracker.QuestId)
        {
            throw new InvalidOperationException(
                "Quest 与 Tracker 的任务 ID 不一致。"
            );
        }
    }
}
