public interface IQuestRuntimeFactory
{
    QuestRuntimeSession Create(
        QuestDefinition definition
    );
}
