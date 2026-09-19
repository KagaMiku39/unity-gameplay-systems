public sealed class HotbarSelectionChangedEvent
{
    public int PreviousSlotIndex { get; }
    public int CurrentSlotIndex { get; }

    public HotbarSelectionChangedEvent(
        int previousSlotIndex,
        int currentSlotIndex
    )
    {
        PreviousSlotIndex = Guard.NonNegative(
            previousSlotIndex,
            nameof(previousSlotIndex)
        );

        CurrentSlotIndex = Guard.NonNegative(
            currentSlotIndex,
            nameof(currentSlotIndex)
        );
    }
}
