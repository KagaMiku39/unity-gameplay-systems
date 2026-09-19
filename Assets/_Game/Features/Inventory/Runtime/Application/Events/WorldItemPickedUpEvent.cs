public sealed class WorldItemPickedUpEvent
{
    public int ItemId { get; }
    public int CollectedAmount { get; }
    public int RemainingWorldAmount { get; }

    public WorldItemPickedUpEvent(
        int itemId,
        int collectedAmount,
        int remainingWorldAmount
    )
    {
        ItemId = Guard.Positive(itemId, nameof(itemId));
        CollectedAmount = Guard.Positive(
            collectedAmount,
            nameof(collectedAmount)
        );
        RemainingWorldAmount = Guard.NonNegative(
            remainingWorldAmount,
            nameof(remainingWorldAmount)
        );
    }
}
