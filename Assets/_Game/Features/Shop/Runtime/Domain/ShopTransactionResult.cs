public sealed class ShopTransactionResult
{
    public ShopTransactionType TransactionType { get; }
    public ShopTransactionStatus Status { get; }
    public int ShopId { get; }
    public int ItemId { get; }
    public int RequestedAmount { get; }
    public int TradedAmount { get; }
    public int GoldDelta { get; }
    public int BalanceAfter { get; }

    public bool IsSuccess =>
        Status == ShopTransactionStatus.Success ||
        Status == ShopTransactionStatus.PartialSuccess;

    public ShopTransactionResult(
        ShopTransactionType transactionType,
        ShopTransactionStatus status,
        int shopId,
        int itemId,
        int requestedAmount,
        int tradedAmount,
        int goldDelta,
        int balanceAfter
    )
    {
        TransactionType = transactionType;
        Status = status;
        ShopId = Guard.Positive(shopId, nameof(shopId));
        ItemId = Guard.Positive(itemId, nameof(itemId));
        RequestedAmount = Guard.Positive(
            requestedAmount,
            nameof(requestedAmount)
        );
        TradedAmount = Guard.NonNegative(
            tradedAmount,
            nameof(tradedAmount)
        );
        BalanceAfter = Guard.NonNegative(
            balanceAfter,
            nameof(balanceAfter)
        );

        if (TradedAmount > RequestedAmount)
        {
            throw new System.ArgumentOutOfRangeException(
                nameof(tradedAmount),
                tradedAmount,
                "实际交易数量不能超过请求数量。"
            );
        }

        GoldDelta = goldDelta;
    }
}
