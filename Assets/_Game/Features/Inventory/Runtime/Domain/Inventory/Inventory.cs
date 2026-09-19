using System;
using System.Collections.Generic;

[Serializable]
public class Inventory
{
    private readonly List<InventorySlot> slots;
    public IReadOnlyList<InventorySlot> Slots => slots;
    public int Count => slots.Count;

    public Inventory(int capacity)
    {
        capacity = Guard.Positive(capacity, nameof(capacity));

        slots = new List<InventorySlot>(capacity);
        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public int AddItem(ItemDefinition itemDefinition, int amount)
    {
        if (itemDefinition == null)
        {
            throw new ArgumentNullException(
                nameof(itemDefinition),
                "物品定义不能为空。"
            );
        }
        amount = Guard.Positive(amount, nameof(amount));
        int remaining = amount;
        List<int> emptySlots = new List<int>();
        for (int i = 0; i < slots.Count; i ++)
        {
            var inventorySlot = slots[i];
            if (inventorySlot.IsEmpty())
            {
                emptySlots.Add(i);
                continue;
            }
            if (itemDefinition.isStackable == false)
            {
                continue;
            }
            if (inventorySlot.count >= itemDefinition.maxStack)
            {
                continue;
            }
            if (inventorySlot.itemId == itemDefinition.id)
            {
                int offset = Math.Min(itemDefinition.maxStack - inventorySlot.count, remaining);
                remaining -= offset;
                inventorySlot.count += offset;
            }
            if (remaining == 0)
            {
                break;
            }
        }
        foreach (int emptySlotIndices in emptySlots)
        {
            if (remaining == 0)
            {
                break;
            }
            var inventorySlot = slots[emptySlotIndices];
            int offset = Math.Min(itemDefinition.isStackable ? itemDefinition.maxStack - inventorySlot.count : 1, remaining);
            remaining -= offset;
            inventorySlot.count = offset;
            slots[emptySlotIndices].itemId = itemDefinition.id;
        }
        return remaining;
    }

    public int GetItemCount(int itemId)
    {
        itemId = Guard.Positive(itemId, nameof(itemId));

        int total = 0;
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemId == itemId)
            {
                total += slot.count;
            }
        }
        return total;
    }

    public bool TryRemoveItem(int itemId, int amount)
    {
        itemId = Guard.Positive(itemId, nameof(itemId));
        amount = Guard.Positive(amount, nameof(amount));

        int total = GetItemCount(itemId);
        if (total < amount)
        {
            return false;
        }
        int remaining = amount;
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemId != itemId) {
                continue;
            }
            if (slot.count > remaining)
            {
                slot.count -= remaining;
                remaining = 0;
                break;
            }
            else
            {
                remaining -= slot.count;
                slot.Clear();
            }
        }
        return true;
    }

    public bool SwapSlots(
        int firstIndex,
        int secondIndex
    )
    {
        if (firstIndex < 0 || firstIndex >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(firstIndex), "不能小于 0 或者大于等于背包大小");
        }

        if (secondIndex < 0 || secondIndex >= Count) {
            throw new ArgumentOutOfRangeException(nameof(secondIndex), "不能小于 0 或者大于等于背包大小");

        }

        if (firstIndex == secondIndex)
        {
            return false;
        }

        var temporary = slots[firstIndex];
        slots[firstIndex] = slots[secondIndex];
        slots[secondIndex] = temporary;

        return true;
    }
}
