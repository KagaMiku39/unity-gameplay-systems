
public sealed class InventorySlotsSwappedEvent
{
    public int FirstIndex { get; }
    public int SecondIndex { get; }

    public InventorySlotsSwappedEvent(
        int firstIndex,
        int secondIndex
    )
    {
        FirstIndex = Guard.NonNegative(
            firstIndex,
            nameof(firstIndex)
        );

        SecondIndex = Guard.NonNegative(
            secondIndex,
            nameof(secondIndex)
        );
    }
}
