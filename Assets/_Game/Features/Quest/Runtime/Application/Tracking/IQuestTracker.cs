using System;

public interface IQuestTracker : IDisposable
{
    int QuestId { get; }
    void Start();
}
