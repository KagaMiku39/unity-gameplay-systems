public sealed class RemoveItemCommand : ICommand<bool>
{
    public int ItemId { get; }
    public int Amount { get; }

    public RemoveItemCommand(int itemId, int amount)
    {
        ItemId = Guard.Positive(itemId, nameof(itemId));
        Amount = Guard.Positive(amount, nameof(amount));
    }
}