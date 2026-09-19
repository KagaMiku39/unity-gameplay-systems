using NUnit.Framework;

public class InventoryRemoveItemTests
{
    private ItemDefinition hoe = InventoryAddItemTests.CreateHoe();
    private ItemDefinition apple = InventoryAddItemTests.CreateApple();

    [Test]
    public void TryRemoveItem_RemovesAcrossMultipleStacks()
    {
        Inventory inventory = new Inventory(3);
        inventory.AddItem(apple, 35);
        Assert.That(inventory.TryRemoveItem(apple.id, 23), Is.True);
        Assert.That(inventory.GetItemCount(apple.id), Is.EqualTo(12));
        Assert.That(inventory.Slots[0].IsEmpty(), Is.True);
        Assert.That(inventory.Slots[1].count, Is.EqualTo(12));
    }

    [Test]
    public void TryRemoveItem_WhenAmountIsInsufficient_DoesNotChangeInventory()
    {
        Inventory inventory = new Inventory(3);
        inventory.AddItem(apple, 5);
        Assert.That(inventory.TryRemoveItem(apple.id, 6), Is.False);
        Assert.That(inventory.GetItemCount(apple.id), Is.EqualTo(5));
    }

    [Test]
    public void TryRemoveItem_DoesNotRemoveDifferentItems()
    {
        Inventory inventory = new Inventory(3);
        inventory.AddItem(hoe, 1);
        inventory.AddItem(apple, 1);
        inventory.TryRemoveItem(apple.id, 1);
        Assert.That(inventory.GetItemCount(hoe.id), Is.EqualTo(1));
        Assert.That(inventory.Slots[0].count, Is.EqualTo(1));
        Assert.That(inventory.GetItemCount(apple.id), Is.Zero);
    }

    [Test]
    public void TryRemoveItem_WhenRemovingExactAmount_ClearsSlot()
    {
        Inventory inventory = new Inventory(3);
        inventory.AddItem(apple, 1);
        inventory.TryRemoveItem(apple.id, 1);
        Assert.That(inventory.Slots[0].IsEmpty(), Is.True);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void TryRemoveItem_WithInvalidItemId_Throws(int itemId)
    {
        Inventory inventory = new Inventory(3);

        Assert.Throws<System.ArgumentOutOfRangeException>(
            () => inventory.TryRemoveItem(itemId, 1)
        );
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void TryRemoveItem_WithNonPositiveAmount_Throws(int amount)
    {
        Inventory inventory = new Inventory(3);

        Assert.Throws<System.ArgumentOutOfRangeException>(
            () => inventory.TryRemoveItem(1, amount)
        );
    }
}
