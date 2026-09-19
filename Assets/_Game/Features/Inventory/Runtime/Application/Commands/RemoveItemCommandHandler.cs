using System;

public sealed class RemoveItemCommandHandler
{
    private readonly Inventory inventory;
    private readonly IEventPublisher eventPublisher;

    public RemoveItemCommandHandler(
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

    public bool Handle(RemoveItemCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "指令不能为空");
        }

        bool isRemoved = inventory.TryRemoveItem(command.ItemId, command.Amount);

        if (isRemoved)
        {
            InventoryItemChangedEvent inventoryChangedEvent =
                   new InventoryItemChangedEvent(
                        command.ItemId,
                        -command.Amount
            );
            eventPublisher.Publish(inventoryChangedEvent);
        }

        return isRemoved;
    }
}
