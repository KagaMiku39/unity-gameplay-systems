using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ItemDatabase
{
    private const string ResourcePath = "Inventory/ItemData";

    private readonly Dictionary<int, ItemDefinition> itemDefinitionsById;

    private ItemDatabase(
        List<ItemDefinition> itemDefinitions
    )
    {
        itemDefinitionsById = new Dictionary<int, ItemDefinition>(
            itemDefinitions.Count
        );

        foreach (ItemDefinition itemDefinition in itemDefinitions)
        {
            if (itemDefinition == null)
            {
                throw new InvalidOperationException(
                    "物品配置中包含 null。"
                );
            }

            itemDefinition.ValidateOrThrow();

            if (itemDefinitionsById.ContainsKey(itemDefinition.id))
            {
                throw new InvalidOperationException(
                    $"物品 ID {itemDefinition.id} 重复。"
                );
            }

            itemDefinitionsById.Add(
                itemDefinition.id,
                itemDefinition
            );
        }
    }

    public static ItemDatabase LoadFromResources()
    {
        TextAsset itemData = Resources.Load<TextAsset>(
            ResourcePath
        );

        if (itemData == null)
        {
            throw new InvalidOperationException(
                $"找不到物品配置 Resources/{ResourcePath}.json。"
            );
        }

        List<ItemDefinition> itemDefinitions;

        try
        {
            itemDefinitions = JsonConvert.DeserializeObject<
                List<ItemDefinition>
            >(itemData.text);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "物品 JSON 格式错误。",
                exception
            );
        }

        if (itemDefinitions == null || itemDefinitions.Count == 0)
        {
            throw new InvalidOperationException(
                "物品配置列表不能为空。"
            );
        }

        return new ItemDatabase(itemDefinitions);
    }

    public bool TryGetItemById(
        int id,
        out ItemDefinition itemDefinition
    )
    {
        id = Guard.Positive(id, nameof(id));

        return itemDefinitionsById.TryGetValue(
            id,
            out itemDefinition
        );
    }
}
