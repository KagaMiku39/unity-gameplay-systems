public sealed class WalletBalanceChangedEvent
{
    public int PreviousBalance { get; }
    public int CurrentBalance { get; }
    public int Delta => CurrentBalance - PreviousBalance;

    public WalletBalanceChangedEvent(
        int previousBalance,
        int currentBalance
    )
    {
        PreviousBalance = Guard.NonNegative(
            previousBalance,
            nameof(previousBalance)
        );
        CurrentBalance = Guard.NonNegative(
            currentBalance,
            nameof(currentBalance)
        );
    }
}
