using System;

[Serializable]
public class InventorySlot
{
    public int itemId;
    public int count;

    public bool IsEmpty()
    {
        return itemId == 0 && count == 0;
    }

    public void Clear()
    {
        itemId = 0;
        count = 0;
    }
}