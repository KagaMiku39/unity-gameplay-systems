using System;

public sealed class Wallet
{
    public int Balance { get; private set; }

    public Wallet(int initialBalance)
    {
        Balance = Guard.NonNegative(
            initialBalance,
            nameof(initialBalance)
        );
    }

    public bool TrySpend(int amount)
    {
        amount = Guard.Positive(amount, nameof(amount));

        if (Balance < amount)
        {
            return false;
        }

        Balance -= amount;
        return true;
    }

    public void Deposit(int amount)
    {
        amount = Guard.Positive(amount, nameof(amount));
        Balance = checked(Balance + amount);
    }
}
