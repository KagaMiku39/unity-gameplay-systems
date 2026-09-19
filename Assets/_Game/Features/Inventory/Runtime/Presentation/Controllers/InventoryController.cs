using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour, IInventoryQuery
{
    private Inventory inventory;
    private CommandBus commandBus;
    private bool isInitialized;

    public IReadOnlyList<InventorySlot> Slots
    {
        get
        {
            EnsureInitialized();
            return inventory.Slots;
        }
    }

    public void Initialize(
        Inventory inventory,
        CommandBus commandBus
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "InventoryController 不能重复初始化。"
            );
        }

        if (inventory == null)
        {
            throw new ArgumentNullException(nameof(inventory));
        }

        if (commandBus == null)
        {
            throw new ArgumentNullException(nameof(commandBus));
        }

        this.inventory = inventory;
        this.commandBus = commandBus;
        isInitialized = true;
    }

    public TResult Execute<TResult>(ICommand<TResult> command)
    {
        EnsureInitialized();

        return commandBus.Send<TResult>(command);
    }

    private void EnsureInitialized()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "InventoryController 尚未初始化。"
            );
        }
    }

    public int GetItemCount(int itemId)
    {
        EnsureInitialized();
        return inventory.GetItemCount(itemId);
    }
}
