using NUnit.Framework;

public sealed class ShopTransactionTests
{
    [Test]
    public void BuyItem_WithEnoughGold_AddsItemAndChargesWallet()
    {
        TestContext context = CreateContext(2, 100);

        ShopTransactionResult result = context.CommandBus.Send(
            new BuyShopItemCommand(1, 1, 3)
        );

        Assert.That(result.Status, Is.EqualTo(
            ShopTransactionStatus.Success
        ));
        Assert.That(context.Inventory.GetItemCount(1), Is.EqualTo(3));
        Assert.That(context.Wallet.Balance, Is.EqualTo(85));
    }

    [Test]
    public void BuyItem_WhenInventoryPartiallyFits_ChargesOnlyAddedAmount()
    {
        TestContext context = CreateContext(1, 1000);

        ShopTransactionResult result = context.CommandBus.Send(
            new BuyShopItemCommand(1, 1, 25)
        );

        Assert.That(result.Status, Is.EqualTo(
            ShopTransactionStatus.PartialSuccess
        ));
        Assert.That(result.TradedAmount, Is.EqualTo(20));
        Assert.That(context.Inventory.GetItemCount(1), Is.EqualTo(20));
        Assert.That(context.Wallet.Balance, Is.EqualTo(900));
    }

    [Test]
    public void BuyItem_WithoutEnoughGold_DoesNotChangeInventory()
    {
        TestContext context = CreateContext(2, 10);

        ShopTransactionResult result = context.CommandBus.Send(
            new BuyShopItemCommand(1, 1, 3)
        );

        Assert.That(result.Status, Is.EqualTo(
            ShopTransactionStatus.InsufficientFunds
        ));
        Assert.That(context.Inventory.GetItemCount(1), Is.EqualTo(0));
        Assert.That(context.Wallet.Balance, Is.EqualTo(10));
    }

    [Test]
    public void SellItem_WithEnoughItems_RemovesItemAndAddsGold()
    {
        TestContext context = CreateContext(2, 100);
        context.CommandBus.Send(
            new AddItemCommand(1, 5)
        );

        ShopTransactionResult result = context.CommandBus.Send(
            new SellShopItemCommand(1, 1, 2)
        );

        Assert.That(result.Status, Is.EqualTo(
            ShopTransactionStatus.Success
        ));
        Assert.That(context.Inventory.GetItemCount(1), Is.EqualTo(3));
        Assert.That(context.Wallet.Balance, Is.EqualTo(104));
    }

    private static TestContext CreateContext(
        int capacity,
        int initialGold
    )
    {
        ItemDatabase itemDatabase =
            ItemDatabase.LoadFromResources();
        ShopDatabase shopDatabase =
            ShopDatabase.LoadFromResources(itemDatabase);
        Inventory inventory = new Inventory(capacity);
        Wallet wallet = new Wallet(initialGold);
        EventBus eventBus = new EventBus();
        CommandBus commandBus = new CommandBus();

        AddItemCommandHandler addHandler =
            new AddItemCommandHandler(
                inventory,
                itemDatabase,
                eventBus
            );
        RemoveItemCommandHandler removeHandler =
            new RemoveItemCommandHandler(
                inventory,
                eventBus
            );
        BuyShopItemCommandHandler buyHandler =
            new BuyShopItemCommandHandler(
                shopDatabase,
                itemDatabase,
                wallet,
                commandBus,
                eventBus
            );
        SellShopItemCommandHandler sellHandler =
            new SellShopItemCommandHandler(
                shopDatabase,
                itemDatabase,
                wallet,
                commandBus,
                eventBus
            );

        commandBus.Register<AddItemCommand, int>(
            addHandler.Handle
        );
        commandBus.Register<RemoveItemCommand, bool>(
            removeHandler.Handle
        );
        commandBus.Register<BuyShopItemCommand, ShopTransactionResult>(
            buyHandler.Handle
        );
        commandBus.Register<SellShopItemCommand, ShopTransactionResult>(
            sellHandler.Handle
        );

        return new TestContext(
            inventory,
            wallet,
            commandBus
        );
    }

    private sealed class TestContext
    {
        public Inventory Inventory { get; }
        public Wallet Wallet { get; }
        public CommandBus CommandBus { get; }

        public TestContext(
            Inventory inventory,
            Wallet wallet,
            CommandBus commandBus
        )
        {
            Inventory = inventory;
            Wallet = wallet;
            CommandBus = commandBus;
        }
    }
}
