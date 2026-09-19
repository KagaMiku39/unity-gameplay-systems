public sealed class SellShopItemCommand
    : ICommand<ShopTransactionResult>
{
    public int ShopId { get; }
    public int ItemId { get; }
    public int Amount { get; }

    public SellShopItemCommand(
        int shopId,
        int itemId,
        int amount
    )
    {
        ShopId = Guard.Positive(shopId, nameof(shopId));
        ItemId = Guard.Positive(itemId, nameof(itemId));
        Amount = Guard.Positive(amount, nameof(amount));
    }
}
