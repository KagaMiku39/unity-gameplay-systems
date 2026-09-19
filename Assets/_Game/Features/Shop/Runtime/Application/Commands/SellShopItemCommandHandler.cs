using System;

public sealed class SellShopItemCommandHandler
{
    private readonly ShopDatabase shopDatabase;
    private readonly ItemDatabase itemDatabase;
    private readonly Wallet wallet;
    private readonly ICommandSender commandSender;
    private readonly IEventPublisher eventPublisher;

    public SellShopItemCommandHandler(
        ShopDatabase shopDatabase,
        ItemDatabase itemDatabase,
        Wallet wallet,
        ICommandSender commandSender,
        IEventPublisher eventPublisher
    )
    {
        this.shopDatabase = shopDatabase
            ?? throw new ArgumentNullException(nameof(shopDatabase));
        this.itemDatabase = itemDatabase
            ?? throw new ArgumentNullException(nameof(itemDatabase));
        this.wallet = wallet
            ?? throw new ArgumentNullException(nameof(wallet));
        this.commandSender = commandSender
            ?? throw new ArgumentNullException(nameof(commandSender));
        this.eventPublisher = eventPublisher
            ?? throw new ArgumentNullException(nameof(eventPublisher));
    }

    public ShopTransactionResult Handle(
        SellShopItemCommand command
    )
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        ShopDefinition shop;
        bool shopFound = shopDatabase.TryGetShopById(
            command.ShopId,
            out shop
        );

        if (shopFound == false)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.ShopNotFound
            );
        }

        if (shop.ContainsItem(command.ItemId) == false)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.ItemNotSoldHere
            );
        }

        ItemDefinition itemDefinition;
        bool itemFound = itemDatabase.TryGetItemById(
            command.ItemId,
            out itemDefinition
        );

        if (itemFound == false)
        {
            throw new InvalidOperationException(
                $"商店引用的物品 {command.ItemId} 不存在。"
            );
        }

        if (itemDefinition.sellPrice == 0)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.ItemCannotBeSold
            );
        }

        bool removed = commandSender.Send(
            new RemoveItemCommand(
                command.ItemId,
                command.Amount
            )
        );

        if (removed == false)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.InsufficientItems
            );
        }

        int previousBalance = wallet.Balance;
        int revenue = checked(
            itemDefinition.sellPrice * command.Amount
        );
        wallet.Deposit(revenue);

        ShopTransactionResult result =
            new ShopTransactionResult(
                ShopTransactionType.Sell,
                ShopTransactionStatus.Success,
                command.ShopId,
                command.ItemId,
                command.Amount,
                command.Amount,
                revenue,
                wallet.Balance
            );

        eventPublisher.Publish(
            new WalletBalanceChangedEvent(
                previousBalance,
                wallet.Balance
            )
        );
        eventPublisher.Publish(
            new ShopTransactionSucceededEvent(result)
        );

        return result;
    }

    private ShopTransactionResult CreateFailure(
        SellShopItemCommand command,
        ShopTransactionStatus status
    )
    {
        return new ShopTransactionResult(
            ShopTransactionType.Sell,
            status,
            command.ShopId,
            command.ItemId,
            command.Amount,
            0,
            0,
            wallet.Balance
        );
    }
}
