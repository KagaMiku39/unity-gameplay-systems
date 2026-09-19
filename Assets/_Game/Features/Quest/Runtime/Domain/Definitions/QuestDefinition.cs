using System;

[Serializable]
public sealed class QuestDefinition
{
    public int id;
    public string title;
    public string description;
    public QuestType questType;
    public int targetItemId;
    public int requiredAmount;
    public int nextQuestId;

    public void ValidateOrThrow()
    {
        if (id <= 0)
        {
            throw new InvalidOperationException(
                "任务 ID 必须大于 0。"
            );
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException(
                $"任务 {id} 的标题不能为空。"
            );
        }

        if (questType == QuestType.None)
        {
            throw new InvalidOperationException(
                $"任务 {id} 没有配置有效的任务类型。"
            );
        }

        if (questType == QuestType.PossessItem)
        {
            if (targetItemId <= 0)
            {
                throw new InvalidOperationException(
                    $"任务 {id} 的目标物品 ID 必须大于 0。"
                );
            }

            if (requiredAmount <= 0)
            {
                throw new InvalidOperationException(
                    $"任务 {id} 的需求数量必须大于 0。"
                );
            }
        }

        if (nextQuestId < 0)
        {
            throw new InvalidOperationException(
                $"任务 {id} 的下一个任务 ID 不能小于 0。"
            );
        }

        if (nextQuestId == id)
        {
            throw new InvalidOperationException(
                $"任务 {id} 不能把自己配置为下一个任务。"
            );
        }
    }
}
