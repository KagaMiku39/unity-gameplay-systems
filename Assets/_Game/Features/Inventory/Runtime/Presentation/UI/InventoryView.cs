using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class InventoryView : MonoBehaviour, IUiPanel
{
    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private InventorySlotView slotPrefab;

    private InventoryController inventoryController;
    private ItemDatabase itemDatabase;
    private IEventSubscriber eventSubscriber;

    private List<InventorySlotView> inventorySlotViewsList;

    private bool isInitialized;
    private int pendingSwapSlotIndex = -1;

    public bool IsVisible => gameObject.activeSelf;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClearPendingSwap();
        gameObject.SetActive(false);
    }

    public void Initialize(
        InventoryController inventoryController,
        ItemDatabase itemDatabase,
        IEventSubscriber eventSubscriber
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException("InventoryView 不能重复初始化。");
        }

        if (inventoryController == null)
        {
            throw new ArgumentNullException(nameof(inventoryController), "初始化不允许为空");
        }
        if (itemDatabase == null)
        {
            throw new ArgumentNullException(nameof(itemDatabase), "初始化不允许为空");
        }
        if (eventSubscriber == null)
        {
            throw new ArgumentNullException(nameof(eventSubscriber), "初始化不允许为空");
        }
        if (content == null)
        {
            throw new InvalidOperationException(
                $"InventoryView 没有配置 {nameof(content)}。"
            );
        }
        if (slotPrefab == null)
        {
            throw new InvalidOperationException(
                $"InventoryView 没有配置 {nameof(slotPrefab)}。"
            );
        }

        this.inventoryController = inventoryController;
        this.itemDatabase = itemDatabase;
        this.eventSubscriber = eventSubscriber;

        CreateSlotViews();
        RefreshAll();

        this.eventSubscriber.Subscribe<InventoryItemChangedEvent>(
            OnInventoryItemChanged
        );

        this.eventSubscriber.Subscribe<InventorySlotsSwappedEvent>(
            OnInventorySlotsSwapped
        );

        isInitialized = true;
    }

    private void CreateSlotViews()
    {
        int capacity = inventoryController.Slots.Count;
        inventorySlotViewsList = new List<InventorySlotView>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            InventorySlotView slotView = Instantiate(
                slotPrefab,
                content
            );

            slotView.Initialize(
                i,
                HandleSlotClicked
            );

            inventorySlotViewsList.Add(slotView);
        }
    }

    private void HandleSlotClicked(int slotIndex)
    {
        if (pendingSwapSlotIndex < 0)
        {
            pendingSwapSlotIndex = slotIndex;
            inventorySlotViewsList[slotIndex].SetSelected(true);
            return;
        }

        if (pendingSwapSlotIndex == slotIndex)
        {
            ClearPendingSwap();
            return;
        }

        int firstIndex = pendingSwapSlotIndex;
        ClearPendingSwap();

        inventoryController.Execute(
            new SwapInventorySlotsCommand(
                firstIndex,
                slotIndex
            )
        );
    }

    private void ClearPendingSwap()
    {
        if (
            pendingSwapSlotIndex >= 0 &&
            inventorySlotViewsList != null &&
            pendingSwapSlotIndex < inventorySlotViewsList.Count
        )
        {
            inventorySlotViewsList[
                pendingSwapSlotIndex
            ].SetSelected(false);
        }

        pendingSwapSlotIndex = -1;
    }

    private void RefreshAll()
    {
        IReadOnlyList<InventorySlot> slots = inventoryController.Slots;

        if (inventorySlotViewsList.Count != slots.Count)
        {
            throw new InvalidOperationException(
                "背包数据格子数量与 UI 格子数量不一致。"
            );
        }

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot slot = slots[i];
            InventorySlotView slotView = inventorySlotViewsList[i];

            if (slot.IsEmpty())
            {
                slotView.Render(slot, null);
                continue;
            }

            ItemDefinition itemDefinition;
            bool found = itemDatabase.TryGetItemById(
                slot.itemId,
                out itemDefinition
            );

            if (found == false)
            {
                throw new InvalidOperationException(
                    $"渲染背包格子 {i} 时，找不到物品 ID {slot.itemId}。"
                );
            }

            slotView.Render(slot, itemDefinition);
        }
    }

    private void OnInventoryItemChanged(
        InventoryItemChangedEvent eventData
    )
    {
        RefreshAll();
    }

    private void OnInventorySlotsSwapped(
        InventorySlotsSwappedEvent eventData
    )
    {
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (eventSubscriber != null)
        {
            eventSubscriber.Unsubscribe<InventoryItemChangedEvent>(
                OnInventoryItemChanged
            );

            eventSubscriber.Unsubscribe<InventorySlotsSwappedEvent>(
                OnInventorySlotsSwapped
            );
        }
    }
}
