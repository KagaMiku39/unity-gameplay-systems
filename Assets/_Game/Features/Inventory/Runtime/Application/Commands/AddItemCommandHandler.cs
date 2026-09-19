using System;

public sealed class AddItemCommandHandler
{
    private readonly Inventory inventory;
    private readonly ItemDatabase itemDatabase;
    private readonly IEventPublisher eventPublisher;

    public AddItemCommandHandler(
        Inventory inventory,
        ItemDatabase itemDatabase,
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

        if (itemDatabase == null)
        {
            throw new ArgumentNullException(
                nameof(itemDatabase),
                "物品数据库不能为空。"
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
        this.itemDatabase = itemDatabase;
        this.eventPublisher = eventPublisher;
    }

    public int Handle(AddItemCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command), "指令不能为空");
        }
        ItemDefinition itemDefinition;

        bool found = itemDatabase.TryGetItemById(
            command.ItemId,
            out itemDefinition
        );

        if (found == false)
        {
            throw new InvalidOperationException(
                $"引用了不存在的物品 ID {command.ItemId}"
            );
        }

        int remaining = inventory.AddItem(itemDefinition, command.Amount);
        int addedAmount = command.Amount - remaining;

        if (addedAmount > 0)
        {
            InventoryItemChangedEvent inventoryChangedEvent =
                   new InventoryItemChangedEvent(
                        command.ItemId,
                        addedAmount
            );
            eventPublisher.Publish(inventoryChangedEvent);
        }

        return remaining;
    }
}
