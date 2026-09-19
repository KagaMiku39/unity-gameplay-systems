using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class HotbarView : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;

    [SerializeField]
    private InventorySlotView slotPrefab;

    private HotbarController hotbarController;
    private InventoryController inventoryController;
    private ItemDatabase itemDatabase;
    private IEventSubscriber eventSubscriber;
    private List<InventorySlotView> slotViews;
    private bool isInitialized;

    public void Initialize(
        HotbarController hotbarController,
        InventoryController inventoryController,
        ItemDatabase itemDatabase,
        IEventSubscriber eventSubscriber
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "HotbarView 不能重复初始化。"
            );
        }

        this.hotbarController = hotbarController
            ?? throw new ArgumentNullException(nameof(hotbarController));
        this.inventoryController = inventoryController
            ?? throw new ArgumentNullException(nameof(inventoryController));
        this.itemDatabase = itemDatabase
            ?? throw new ArgumentNullException(nameof(itemDatabase));
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));

        if (content == null)
        {
            throw new InvalidOperationException(
                $"HotbarView 没有配置 {nameof(content)}。"
            );
        }

        if (slotPrefab == null)
        {
            throw new InvalidOperationException(
                $"HotbarView 没有配置 {nameof(slotPrefab)}。"
            );
        }

        CreateSlotViews();
        RefreshAllSlots();
        RefreshSelection();

        eventSubscriber.Subscribe<InventoryItemChangedEvent>(
            HandleInventoryItemChanged
        );
        eventSubscriber.Subscribe<InventorySlotsSwappedEvent>(
            HandleInventorySlotsSwapped
        );
        eventSubscriber.Subscribe<HotbarSelectionChangedEvent>(
            HandleSelectionChanged
        );

        isInitialized = true;
    }

    private void CreateSlotViews()
    {
        int slotCount = hotbarController.VisibleSlotCount;
        slotViews = new List<InventorySlotView>(slotCount);

        ResizeContent(slotCount);

        for (int i = 0; i < slotCount; i++)
        {
            InventorySlotView slotView = Instantiate(
                slotPrefab,
                content
            );

            slotView.Initialize(
                i,
                HandleSlotClicked,
                (i + 1).ToString()
            );

            slotViews.Add(slotView);
        }
    }

    private void ResizeContent(int slotCount)
    {
        HorizontalLayoutGroup layout =
            content.GetComponent<HorizontalLayoutGroup>();
        RectTransform slotRect =
            slotPrefab.GetComponent<RectTransform>();

        if (layout == null || slotRect == null)
        {
            return;
        }

        float width =
            layout.padding.left +
            layout.padding.right +
            slotRect.rect.width * slotCount +
            layout.spacing * (slotCount - 1);
        float height =
            layout.padding.top +
            layout.padding.bottom +
            slotRect.rect.height;

        content.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );
        content.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            height
        );
    }

    private void HandleSlotClicked(int slotIndex)
    {
        hotbarController.TrySelectSlot(slotIndex);
    }

    private void RefreshAllSlots()
    {
        IReadOnlyList<InventorySlot> inventorySlots =
            inventoryController.Slots;

        for (int i = 0; i < slotViews.Count; i++)
        {
            InventorySlot slot = inventorySlots[i];

            if (slot.IsEmpty())
            {
                slotViews[i].Render(slot, null);
                continue;
            }

            ItemDefinition definition;
            bool found = itemDatabase.TryGetItemById(
                slot.itemId,
                out definition
            );

            if (found == false)
            {
                throw new InvalidOperationException(
                    $"渲染快捷栏格子 {i} 时找不到物品 {slot.itemId}。"
                );
            }

            slotViews[i].Render(slot, definition);
        }
    }

    private void RefreshSelection()
    {
        int selectedIndex = hotbarController.SelectedSlotIndex;

        for (int i = 0; i < slotViews.Count; i++)
        {
            slotViews[i].SetSelected(i == selectedIndex);
        }
    }

    private void HandleInventoryItemChanged(
        InventoryItemChangedEvent eventData
    )
    {
        RefreshAllSlots();
    }

    private void HandleInventorySlotsSwapped(
        InventorySlotsSwappedEvent eventData
    )
    {
        RefreshAllSlots();
    }

    private void HandleSelectionChanged(
        HotbarSelectionChangedEvent eventData
    )
    {
        RefreshSelection();
    }

    private void OnDestroy()
    {
        if (eventSubscriber == null)
        {
            return;
        }

        eventSubscriber.Unsubscribe<InventoryItemChangedEvent>(
            HandleInventoryItemChanged
        );
        eventSubscriber.Unsubscribe<InventorySlotsSwappedEvent>(
            HandleInventorySlotsSwapped
        );
        eventSubscriber.Unsubscribe<HotbarSelectionChangedEvent>(
            HandleSelectionChanged
        );
    }
}
