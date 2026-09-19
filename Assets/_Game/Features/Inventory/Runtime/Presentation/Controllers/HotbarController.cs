using System;
using UnityEngine;

public sealed class HotbarController : MonoBehaviour
{
    private const int MaximumNumberKeyCount = 9;

    [SerializeField]
    [Range(1, MaximumNumberKeyCount)]
    private int visibleSlotCount = 5;

    private IEventPublisher eventPublisher;
    private IGameplayInputGate inputGate;
    private int selectedSlotIndex;
    private bool isInitialized;

    public int VisibleSlotCount
    {
        get
        {
            EnsureInitialized();
            return visibleSlotCount;
        }
    }

    public int SelectedSlotIndex
    {
        get
        {
            EnsureInitialized();
            return selectedSlotIndex;
        }
    }

    public void Initialize(
        IEventPublisher eventPublisher,
        IGameplayInputGate inputGate,
        int inventoryCapacity
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "HotbarController 不能重复初始化。"
            );
        }

        this.eventPublisher = eventPublisher
            ?? throw new ArgumentNullException(nameof(eventPublisher));
        this.inputGate = inputGate
            ?? throw new ArgumentNullException(nameof(inputGate));

        Guard.Positive(inventoryCapacity, nameof(inventoryCapacity));

        if (visibleSlotCount > inventoryCapacity)
        {
            throw new InvalidOperationException(
                $"快捷栏格子数 {visibleSlotCount} 不能超过背包容量 " +
                $"{inventoryCapacity}。"
            );
        }

        selectedSlotIndex = 0;
        isInitialized = true;
    }

    public bool TrySelectSlot(int slotIndex)
    {
        EnsureInitialized();

        if (slotIndex < 0 || slotIndex >= visibleSlotCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(slotIndex),
                slotIndex,
                $"快捷栏索引必须位于 0 到 {visibleSlotCount - 1}。"
            );
        }

        if (inputGate.IsBlocked || slotIndex == selectedSlotIndex)
        {
            return false;
        }

        int previousSlotIndex = selectedSlotIndex;
        selectedSlotIndex = slotIndex;

        eventPublisher.Publish(
            new HotbarSelectionChangedEvent(
                previousSlotIndex,
                selectedSlotIndex
            )
        );

        return true;
    }

    private void Update()
    {
        if (isInitialized == false || inputGate.IsBlocked)
        {
            return;
        }

        for (int i = 0; i < visibleSlotCount; i++)
        {
            KeyCode numberKey = (KeyCode)(
                (int)KeyCode.Alpha1 + i
            );

            if (Input.GetKeyDown(numberKey))
            {
                TrySelectSlot(i);
                return;
            }
        }
    }

    private void Start()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "HotbarController 尚未初始化。"
            );
        }
    }
}
