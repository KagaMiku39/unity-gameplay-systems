using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShopView : MonoBehaviour, IUiPanel
{
    [SerializeField]
    private int shopId = 1;

    [SerializeField]
    private int transactionAmount = 1;

    [SerializeField]
    private Image itemIcon;

    [SerializeField]
    private TMP_Text productNameText;

    [SerializeField]
    private TMP_Text priceText;

    [SerializeField]
    private TMP_Text balanceText;

    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private Button previousButton;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button buyButton;

    [SerializeField]
    private Button sellButton;

    private ShopController shopController;
    private ItemDatabase itemDatabase;
    private IEventSubscriber eventSubscriber;
    private ShopDefinition shopDefinition;
    private int currentItemIndex;

    public bool IsVisible => gameObject.activeSelf;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(
        ShopController shopController,
        ShopDatabase shopDatabase,
        ItemDatabase itemDatabase,
        IEventSubscriber eventSubscriber,
        int initialBalance
    )
    {
        this.shopController = shopController
            ?? throw new ArgumentNullException(nameof(shopController));

        if (shopDatabase == null)
        {
            throw new ArgumentNullException(nameof(shopDatabase));
        }

        this.itemDatabase = itemDatabase
            ?? throw new ArgumentNullException(nameof(itemDatabase));
        this.eventSubscriber = eventSubscriber
            ?? throw new ArgumentNullException(nameof(eventSubscriber));

        ValidateReferences();
        transactionAmount = Guard.Positive(
            transactionAmount,
            nameof(transactionAmount)
        );

        bool shopFound = shopDatabase.TryGetShopById(
            shopId,
            out shopDefinition
        );

        if (shopFound == false)
        {
            throw new InvalidOperationException(
                $"ShopView cannot find shop {shopId}."
            );
        }

        previousButton.onClick.AddListener(ShowPreviousItem);
        nextButton.onClick.AddListener(ShowNextItem);
        buyButton.onClick.AddListener(BuyCurrentItem);
        sellButton.onClick.AddListener(SellCurrentItem);

        eventSubscriber.Subscribe<WalletBalanceChangedEvent>(
            HandleWalletBalanceChanged
        );

        RenderBalance(initialBalance);
        RenderCurrentItem();
        statusText.text = "Choose Buy or Sell";
    }

    private void ShowPreviousItem()
    {
        currentItemIndex--;

        if (currentItemIndex < 0)
        {
            currentItemIndex = shopDefinition.itemIds.Count - 1;
        }

        RenderCurrentItem();
    }

    private void ShowNextItem()
    {
        currentItemIndex =
            (currentItemIndex + 1) % shopDefinition.itemIds.Count;
        RenderCurrentItem();
    }

    private void BuyCurrentItem()
    {
        ShopTransactionResult result = shopController.Execute(
            new BuyShopItemCommand(
                shopId,
                GetCurrentItemId(),
                transactionAmount
            )
        );

        RenderResult(result);
    }

    private void SellCurrentItem()
    {
        ShopTransactionResult result = shopController.Execute(
            new SellShopItemCommand(
                shopId,
                GetCurrentItemId(),
                transactionAmount
            )
        );

        RenderResult(result);
    }

    private int GetCurrentItemId()
    {
        return shopDefinition.itemIds[currentItemIndex];
    }

    private void RenderCurrentItem()
    {
        ItemDefinition itemDefinition;
        bool found = itemDatabase.TryGetItemById(
            GetCurrentItemId(),
            out itemDefinition
        );

        if (found == false)
        {
            throw new InvalidOperationException(
                $"Shop item {GetCurrentItemId()} does not exist."
            );
        }

        productNameText.text = itemDefinition.name;
        priceText.text =
            $"Buy: {itemDefinition.buyPrice}  " +
            $"Sell: {itemDefinition.sellPrice}";

        Sprite icon = Resources.Load<Sprite>(
            itemDefinition.iconPath
        );

        itemIcon.sprite = icon;
        itemIcon.enabled = icon != null;
    }

    private void RenderResult(ShopTransactionResult result)
    {
        switch (result.Status)
        {
            case ShopTransactionStatus.Success:
                statusText.text =
                    $"Success: {result.TradedAmount} item(s)";
                break;

            case ShopTransactionStatus.PartialSuccess:
                statusText.text =
                    $"Inventory space limited: bought {result.TradedAmount}";
                break;

            case ShopTransactionStatus.InsufficientFunds:
                statusText.text = "Not enough gold";
                break;

            case ShopTransactionStatus.InventoryFull:
                statusText.text = "Inventory is full";
                break;

            case ShopTransactionStatus.InsufficientItems:
                statusText.text = "Not enough items in inventory";
                break;

            case ShopTransactionStatus.ItemCannotBeSold:
                statusText.text = "This item cannot be sold";
                break;

            case ShopTransactionStatus.ItemNotSoldHere:
                statusText.text = "This shop does not trade this item";
                break;

            default:
                statusText.text = "Transaction failed";
                break;
        }
    }

    private void HandleWalletBalanceChanged(
        WalletBalanceChangedEvent eventData
    )
    {
        RenderBalance(eventData.CurrentBalance);
    }

    private void RenderBalance(int balance)
    {
        balanceText.text = $"Gold: {balance}";
    }

    private void ValidateReferences()
    {
        if (
            itemIcon == null ||
            productNameText == null ||
            priceText == null ||
            balanceText == null ||
            statusText == null ||
            previousButton == null ||
            nextButton == null ||
            buyButton == null ||
            sellButton == null
        )
        {
            throw new InvalidOperationException(
                "ShopView Inspector references are incomplete."
            );
        }
    }

    private void OnDestroy()
    {
        previousButton?.onClick.RemoveListener(ShowPreviousItem);
        nextButton?.onClick.RemoveListener(ShowNextItem);
        buyButton?.onClick.RemoveListener(BuyCurrentItem);
        sellButton?.onClick.RemoveListener(SellCurrentItem);

        eventSubscriber?.Unsubscribe<WalletBalanceChangedEvent>(
            HandleWalletBalanceChanged
        );
    }
}
