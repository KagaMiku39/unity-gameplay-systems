using System;

public sealed class SwapInventorySlotsCommandHandler
{
    private readonly Inventory inventory;
    private readonly IEventPublisher eventPublisher;

    public SwapInventorySlotsCommandHandler(
        Inventory inventory,
        IEventPublisher eventPublisher
    )
    {
        if (inventory == null)
        {
            throw new ArgumentNullException(
                nameof(inventory),
                "背包不能为空。"
            );
        }

        if (eventPublisher == null)
        {
            throw new ArgumentNullException(
                nameof(eventPublisher),
                "事件发布器不能为空。"
            );
        }

        this.inventory = inventory;
        this.eventPublisher = eventPublisher;
    }

    public bool Handle(SwapInventorySlotsCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(
                nameof(command),
                "指令不能为空。"
            );
        }

        bool swapped = inventory.SwapSlots(
            command.FirstIndex,
            command.SecondIndex
        );

        if (swapped)
        {
            InventorySlotsSwappedEvent slotsSwappedEvent =
                new InventorySlotsSwappedEvent(
                    command.FirstIndex,
                    command.SecondIndex
                );

            eventPublisher.Publish(slotsSwappedEvent);
        }

        return swapped;
    }
}
