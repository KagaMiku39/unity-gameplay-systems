using System;
using UnityEngine;

public sealed class InventoryDebugInput : MonoBehaviour
{
    [Header("Debug shortcuts are disabled by default")]
    [SerializeField]
    private bool enableDebugShortcuts;

    [SerializeField]
    private InventoryController inventoryController;

    [SerializeField]
    private int itemId = 1;

    [SerializeField]
    private int amount = 50;

    private void Awake()
    {
        if (inventoryController == null)
        {
            throw new InvalidOperationException($"{nameof(InventoryDebugInput)} 没有配置 {nameof(inventoryController)}");
        }
    }

    private void Update()
    {
        if (enableDebugShortcuts == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            DebugAddItem();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            DebugRemoveItem();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            DebugSwapItem(1, 4);
        }
    }

    private void DebugAddItem()
    {
        AddItemCommand command = new AddItemCommand(itemId, amount);
        int remaining = inventoryController.Execute(command);
        int added = amount - remaining;
        Debug.Log($"请求添加：{itemId} 为 {amount} 个，实际添加：{added}，剩余：{remaining}");
    }

    private void DebugRemoveItem()
    {
        RemoveItemCommand command = new RemoveItemCommand(itemId, amount);
        Debug.Log($"请求删除：{itemId} 为 {amount} 个，结果：{inventoryController.Execute(command)}");
    }

    private void DebugSwapItem(int firstIndex, int secondIndex)
    {
        SwapInventorySlotsCommand command = new SwapInventorySlotsCommand(firstIndex, secondIndex);
        Debug.Log($"请求交换：序号 {firstIndex} 和 {secondIndex}，结果：{inventoryController.Execute(command)}");
    }
}
