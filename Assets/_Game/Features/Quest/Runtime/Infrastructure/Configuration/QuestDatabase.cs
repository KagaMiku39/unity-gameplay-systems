using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public sealed class QuestDatabase
{
    private const string ResourcePath = "Quest/QuestData";

    private readonly Dictionary<int, QuestDefinition> definitionsById;

    private QuestDatabase(
        List<QuestDefinition> definitions
    )
    {
        definitionsById = new Dictionary<int, QuestDefinition>(
            definitions.Count
        );

        foreach (QuestDefinition definition in definitions)
        {
            if (definition == null)
            {
                throw new InvalidOperationException(
                    "任务配置中包含 null。"
                );
            }

            definition.ValidateOrThrow();

            if (definitionsById.ContainsKey(definition.id))
            {
                throw new InvalidOperationException(
                    $"任务 ID {definition.id} 重复。"
                );
            }

            definitionsById.Add(definition.id, definition);
        }

        foreach (QuestDefinition definition in definitions)
        {
            if (definition.nextQuestId == 0)
            {
                continue;
            }

            if (definitionsById.ContainsKey(definition.nextQuestId))
            {
                continue;
            }

            throw new InvalidOperationException(
                $"任务 {definition.id} 引用了不存在的下一个任务 " +
                $"{definition.nextQuestId}。"
            );
        }
    }

    public static QuestDatabase LoadFromResources()
    {
        TextAsset questData = Resources.Load<TextAsset>(
            ResourcePath
        );

        if (questData == null)
        {
            throw new InvalidOperationException(
                $"找不到任务配置 Resources/{ResourcePath}.json。"
            );
        }

        List<QuestDefinition> definitions;

        try
        {
            definitions = JsonConvert.DeserializeObject<
                List<QuestDefinition>
            >(questData.text);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "任务 JSON 格式错误。",
                exception
            );
        }

        if (definitions == null || definitions.Count == 0)
        {
            throw new InvalidOperationException(
                "任务配置列表不能为空。"
            );
        }

        return new QuestDatabase(definitions);
    }

    public bool TryGetQuestById(
        int questId,
        out QuestDefinition definition
    )
    {
        questId = Guard.Positive(questId, nameof(questId));

        return definitionsById.TryGetValue(
            questId,
            out definition
        );
    }
}
