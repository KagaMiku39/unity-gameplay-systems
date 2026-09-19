using System;

public sealed class BuyShopItemCommandHandler
{
    private readonly ShopDatabase shopDatabase;
    private readonly ItemDatabase itemDatabase;
    private readonly Wallet wallet;
    private readonly ICommandSender commandSender;
    private readonly IEventPublisher eventPublisher;

    public BuyShopItemCommandHandler(
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
        BuyShopItemCommand command
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

        int requestedCost = checked(
            itemDefinition.buyPrice * command.Amount
        );
        int previousBalance = wallet.Balance;

        if (wallet.TrySpend(requestedCost) == false)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.InsufficientFunds
            );
        }

        int remaining;

        try
        {
            remaining = commandSender.Send(
                new AddItemCommand(
                    command.ItemId,
                    command.Amount
                )
            );
        }
        catch
        {
            wallet.Deposit(requestedCost);
            throw;
        }

        int tradedAmount = command.Amount - remaining;
        int refund = checked(
            remaining * itemDefinition.buyPrice
        );

        if (refund > 0)
        {
            wallet.Deposit(refund);
        }

        if (tradedAmount == 0)
        {
            return CreateFailure(
                command,
                ShopTransactionStatus.InventoryFull
            );
        }

        ShopTransactionStatus status = remaining == 0
            ? ShopTransactionStatus.Success
            : ShopTransactionStatus.PartialSuccess;

        ShopTransactionResult result =
            new ShopTransactionResult(
                ShopTransactionType.Buy,
                status,
                command.ShopId,
                command.ItemId,
                command.Amount,
                tradedAmount,
                -checked(tradedAmount * itemDefinition.buyPrice),
                wallet.Balance
            );

        PublishSuccessfulTransaction(
            result,
            previousBalance
        );

        return result;
    }

    private ShopTransactionResult CreateFailure(
        BuyShopItemCommand command,
        ShopTransactionStatus status
    )
    {
        return new ShopTransactionResult(
            ShopTransactionType.Buy,
            status,
            command.ShopId,
            command.ItemId,
            command.Amount,
            0,
            0,
            wallet.Balance
        );
    }

    private void PublishSuccessfulTransaction(
        ShopTransactionResult result,
        int previousBalance
    )
    {
        eventPublisher.Publish(
            new WalletBalanceChangedEvent(
                previousBalance,
                wallet.Balance
            )
        );

        eventPublisher.Publish(
            new ShopTransactionSucceededEvent(result)
        );
    }
}
