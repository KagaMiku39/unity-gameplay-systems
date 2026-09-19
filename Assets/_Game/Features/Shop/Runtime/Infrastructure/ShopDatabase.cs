using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public sealed class ShopDatabase
{
    private const string ResourcePath = "Shop/ShopData";

    private readonly Dictionary<int, ShopDefinition> shopsById;

    private ShopDatabase(
        List<ShopDefinition> definitions,
        ItemDatabase itemDatabase
    )
    {
        if (itemDatabase == null)
        {
            throw new ArgumentNullException(nameof(itemDatabase));
        }

        shopsById = new Dictionary<int, ShopDefinition>(
            definitions.Count
        );

        foreach (ShopDefinition definition in definitions)
        {
            if (definition == null)
            {
                throw new InvalidOperationException(
                    "商店配置中包含 null。"
                );
            }

            definition.ValidateOrThrow();

            if (shopsById.ContainsKey(definition.id))
            {
                throw new InvalidOperationException(
                    $"商店 ID {definition.id} 重复。"
                );
            }

            foreach (int itemId in definition.itemIds)
            {
                ItemDefinition itemDefinition;
                bool itemFound = itemDatabase.TryGetItemById(
                    itemId,
                    out itemDefinition
                );

                if (itemFound == false)
                {
                    throw new InvalidOperationException(
                        $"商店 {definition.id} 引用了不存在的物品 {itemId}。"
                    );
                }
            }

            shopsById.Add(definition.id, definition);
        }
    }

    public static ShopDatabase LoadFromResources(
        ItemDatabase itemDatabase
    )
    {
        TextAsset shopData = Resources.Load<TextAsset>(
            ResourcePath
        );

        if (shopData == null)
        {
            throw new InvalidOperationException(
                $"找不到商店配置 Resources/{ResourcePath}.json。"
            );
        }

        List<ShopDefinition> definitions;

        try
        {
            definitions = JsonConvert.DeserializeObject<
                List<ShopDefinition>
            >(shopData.text);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "商店 JSON 格式错误。",
                exception
            );
        }

        if (definitions == null || definitions.Count == 0)
        {
            throw new InvalidOperationException(
                "商店配置列表不能为空。"
            );
        }

        return new ShopDatabase(definitions, itemDatabase);
    }

    public bool TryGetShopById(
        int shopId,
        out ShopDefinition definition
    )
    {
        shopId = Guard.Positive(shopId, nameof(shopId));
        return shopsById.TryGetValue(shopId, out definition);
    }
}
