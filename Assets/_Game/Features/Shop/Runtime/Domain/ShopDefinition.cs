using System;
using System.Collections.Generic;

[Serializable]
public sealed class ShopDefinition
{
    public int id;
    public string name;
    public List<int> itemIds;

    public void ValidateOrThrow()
    {
        if (id <= 0)
        {
            throw new InvalidOperationException(
                "商店 ID 必须大于 0。"
            );
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                $"商店 {id} 的名称不能为空。"
            );
        }

        if (itemIds == null || itemIds.Count == 0)
        {
            throw new InvalidOperationException(
                $"商店 {id} 至少需要配置一个物品。"
            );
        }

        HashSet<int> uniqueItemIds = new HashSet<int>();

        foreach (int itemId in itemIds)
        {
            if (itemId <= 0)
            {
                throw new InvalidOperationException(
                    $"商店 {id} 包含非法物品 ID。"
                );
            }

            if (uniqueItemIds.Add(itemId) == false)
            {
                throw new InvalidOperationException(
                    $"商店 {id} 重复配置了物品 {itemId}。"
                );
            }
        }
    }

    public bool ContainsItem(int itemId)
    {
        itemId = Guard.Positive(itemId, nameof(itemId));
        return itemIds.Contains(itemId);
    }
}
