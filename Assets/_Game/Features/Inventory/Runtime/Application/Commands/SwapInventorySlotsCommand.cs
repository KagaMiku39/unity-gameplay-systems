public sealed class SwapInventorySlotsCommand : ICommand<bool>
{
    public int FirstIndex { get; }
    public int SecondIndex { get; }

    public SwapInventorySlotsCommand(
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
