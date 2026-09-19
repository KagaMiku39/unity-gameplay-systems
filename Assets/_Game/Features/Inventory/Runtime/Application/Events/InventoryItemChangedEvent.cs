using System;

public sealed class InventoryItemChangedEvent
{
    public int ItemId { get; }
    public int QuantityDelta { get; }

    public InventoryItemChangedEvent(int itemId, int quantityDelta)
    {
        ItemId = Guard.Positive(itemId, nameof(itemId));

        if (quantityDelta == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityDelta), quantityDelta, "不能为 0");
        }

        QuantityDelta = quantityDelta;
    }
}
