using NUnit.Framework;

public sealed class PossessItemQuestTests
{
    [Test]
    public void UpdateProgress_BelowRequiredAmount_DoesNotComplete()
    {
        PossessItemQuest quest = CreateQuest(10);

        bool justCompleted = quest.UpdateProgress(7);

        Assert.That(justCompleted, Is.False);
        Assert.That(quest.CurrentAmount, Is.EqualTo(7));
        Assert.That(quest.IsCompleted, Is.False);
    }

    [Test]
    public void UpdateProgress_ReachesRequiredAmount_CompletesOnce()
    {
        PossessItemQuest quest = CreateQuest(10);

        bool firstResult = quest.UpdateProgress(15);
        bool secondResult = quest.UpdateProgress(0);

        Assert.That(firstResult, Is.True);
        Assert.That(secondResult, Is.False);
        Assert.That(quest.CurrentAmount, Is.EqualTo(10));
        Assert.That(quest.IsCompleted, Is.True);
    }

    private static PossessItemQuest CreateQuest(
        int requiredAmount
    )
    {
        QuestDefinition definition = new QuestDefinition
        {
            id = 1,
            title = "Collect Apple",
            description = "Collect apples.",
            questType = QuestType.PossessItem,
            targetItemId = 1,
            requiredAmount = requiredAmount
        };

        return new PossessItemQuest(definition);
    }
}
