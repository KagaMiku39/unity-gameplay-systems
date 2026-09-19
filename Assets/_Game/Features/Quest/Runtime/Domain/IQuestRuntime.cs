public interface IQuestRuntime
{
    int QuestId { get; }
    string Title { get; }
    string Description { get; }
    int NextQuestId { get; }
    bool IsCompleted { get; }
}
