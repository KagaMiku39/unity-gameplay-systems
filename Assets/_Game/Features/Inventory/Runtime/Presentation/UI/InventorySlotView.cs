using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TMP_Text itemNameText;

    [SerializeField]
    private TMP_Text countText;

    [SerializeField]
    private TMP_Text shortcutText;

    private static readonly Color SelectedBackgroundColor =
        new Color(1f, 0.68f, 0.2f, 1f);

    private Image backgroundImage;
    private Color normalBackgroundColor;
    private Action<int> clickHandler;
    private int slotIndex = -1;
    private bool isInitialized;

    private void Awake()
    {
        if (itemNameText == null)
        {
            throw new InvalidOperationException($"InventorySlotView 没有配置 {nameof(itemNameText)}");
        }

        if (countText == null)
        {
            throw new InvalidOperationException($"InventorySlotView 没有配置 {nameof(countText)}");
        }

        if (shortcutText == null)
        {
            throw new InvalidOperationException(
                $"InventorySlotView 没有配置 {nameof(shortcutText)}"
            );
        }

        backgroundImage = GetComponent<Image>();

        if (backgroundImage == null)
        {
            throw new InvalidOperationException(
                "InventorySlotView 所在对象必须具有 Image 组件。"
            );
        }

        normalBackgroundColor = backgroundImage.color;
    }

    public void Initialize(
        int slotIndex,
        Action<int> clickHandler,
        string shortcutLabel = null
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "InventorySlotView 不能重复初始化。"
            );
        }

        this.slotIndex = Guard.NonNegative(
            slotIndex,
            nameof(slotIndex)
        );
        this.clickHandler = clickHandler;
        SetShortcutLabel(shortcutLabel);
        SetSelected(false);
        isInitialized = true;
    }

    public void Render(
        InventorySlot slot,
        ItemDefinition definition
    )
    {
        if (slot == null)
        {
            throw new ArgumentNullException(nameof(slot), "不能为空");
        }

        if (slot.IsEmpty())
        {
            itemNameText.text = string.Empty;
            countText.text = string.Empty;
            countText.gameObject.SetActive(false);
            return;
        }

        if (definition == null)
        {
            throw new InvalidOperationException($"数据库中不存在 {slot.itemId}");
        }


        if (definition.id != slot.itemId)
        {
            throw new InvalidOperationException($"{nameof(definition.id)} 与 {nameof(slot.itemId)} 不匹配");
        }

        itemNameText.text = definition.name;

        if (slot.count > 1)
        {
            countText.text = slot.count.ToString();
            countText.gameObject.SetActive(true);
        } else
        {
            countText.text = string.Empty;
            countText.gameObject.SetActive(false);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (backgroundImage == null)
        {
            return;
        }

        backgroundImage.color = isSelected
            ? SelectedBackgroundColor
            : normalBackgroundColor;
    }

    public void SetShortcutLabel(string label)
    {
        bool hasLabel = string.IsNullOrWhiteSpace(label) == false;
        shortcutText.text = hasLabel ? label : string.Empty;
        shortcutText.gameObject.SetActive(hasLabel);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        clickHandler?.Invoke(slotIndex);
    }

    private void OnDestroy()
    {
        clickHandler = null;
    }
}
