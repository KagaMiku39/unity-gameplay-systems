using NUnit.Framework;

public class InventoryAddItemTests
{
    [Test]
    public void AddItem_FillsExistingStackBeforeUsingEmptySlot()
    {
        Inventory inventory = new Inventory(20);
        ItemDefinition apple = CreateApple();

        int firstRemaining = inventory.AddItem(apple, 7);
        int secondRemaining = inventory.AddItem(apple, 18);

        Assert.That(firstRemaining, Is.Zero);
        Assert.That(secondRemaining, Is.Zero);
        Assert.That(inventory.Slots[0].itemId, Is.EqualTo(apple.id));
        Assert.That(inventory.Slots[0].count, Is.EqualTo(20));
        Assert.That(inventory.Slots[1].itemId, Is.EqualTo(apple.id));
        Assert.That(inventory.Slots[1].count, Is.EqualTo(5));
    }

    [Test]
    public void AddItem_NonStackableItemsUseSeparateSlots()
    {
        Inventory inventory = new Inventory(3);
        ItemDefinition hoe = CreateHoe();

        int remaining = inventory.AddItem(hoe, 2);

        Assert.That(remaining, Is.Zero);
        Assert.That(inventory.Slots[0].itemId, Is.EqualTo(hoe.id));
        Assert.That(inventory.Slots[0].count, Is.EqualTo(1));
        Assert.That(inventory.Slots[1].itemId, Is.EqualTo(hoe.id));
        Assert.That(inventory.Slots[1].count, Is.EqualTo(1));
    }

    [Test]
    public void AddItem_WhenInventoryIsFull_ReturnsRemainingAmount()
    {
        Inventory inventory = new Inventory(2);
        ItemDefinition apple = CreateApple();

        int remaining = inventory.AddItem(apple, 50);

        Assert.That(remaining, Is.EqualTo(10));
        Assert.That(inventory.Slots[0].count, Is.EqualTo(20));
        Assert.That(inventory.Slots[1].count, Is.EqualTo(20));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Constructor_WithNonPositiveCapacity_Throws(int capacity)
    {
        Assert.Throws<System.ArgumentOutOfRangeException>(() => new Inventory(capacity));
    }

    public static ItemDefinition CreateApple()
    {
        return new ItemDefinition
        {
            id = 1,
            name = "Apple",
            itemType = ItemType.Food,
            isStackable = true,
            maxStack = 20
        };
    }

    public static ItemDefinition CreateHoe()
    {
        return new ItemDefinition
        {
            id = 3,
            name = "Hoe",
            itemType = ItemType.Tool,
            isStackable = false,
            maxStack = 1
        };
    }
}
