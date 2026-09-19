public sealed class AddItemCommand : ICommand<int>
{
    public int ItemId { get; }
    public int Amount { get; }

    public AddItemCommand(int itemId, int amount)
    {
        ItemId = Guard.Positive(itemId, nameof(itemId));
        Amount = Guard.Positive(amount, nameof(amount));
    }
}