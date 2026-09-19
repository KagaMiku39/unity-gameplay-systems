using System;
using UnityEngine;

public sealed class GameBootstrapper : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private int capacity = 20;

    [SerializeField]
    private InventoryController inventoryController;

    [SerializeField]
    private InventoryView inventoryView;

    [SerializeField]
    private HotbarController hotbarController;

    [SerializeField]
    private HotbarView hotbarView;

    [Header("Quest")]
    [SerializeField]
    private int questId = 1;

    [SerializeField]
    private QuestView questView;

    [Header("Shop")]
    [SerializeField]
    private int initialGold = 200;

    [SerializeField]
    private ShopController shopController;

    [SerializeField]
    private ShopView shopView;

    private InventoryChangedDebugLogger inventoryChangedDebugLogger;
    private QuestProgressDebugLogger questProgressDebugLogger;
    private QuestRuntimeManager questRuntimeManager;
    private ShopTransactionDebugLogger shopTransactionDebugLogger;
    private WorldItemPickupDebugLogger worldItemPickupDebugLogger;

    private void Awake()
    {
        UiNavigationInput uiNavigationInput =
            GetComponent<UiNavigationInput>();

        if (uiNavigationInput == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} requires " +
                $"{nameof(UiNavigationInput)} on the same GameObject."
            );
        }

        if (inventoryController == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(inventoryController)}。"
            );
        }

        if (inventoryView == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(inventoryView)}。"
            );
        }

        if (hotbarController == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(hotbarController)}。"
            );
        }

        if (hotbarView == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(hotbarView)}。"
            );
        }

        if (questView == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(questView)}。"
            );
        }

        if (shopController == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(shopController)}。"
            );
        }

        if (shopView == null)
        {
            throw new InvalidOperationException(
                $"{nameof(GameBootstrapper)} 未配置 {nameof(shopView)}。"
            );
        }

        ItemDatabase itemDatabase =
            ItemDatabase.LoadFromResources();

        QuestDatabase questDatabase =
            QuestDatabase.LoadFromResources();

        ShopDatabase shopDatabase =
            ShopDatabase.LoadFromResources(itemDatabase);

        Inventory inventory = new Inventory(capacity);
        CommandBus commandBus = new CommandBus();
        EventBus eventBus = new EventBus();

        AddItemCommandHandler addItemCommandHandler =
            new AddItemCommandHandler(
                inventory,
                itemDatabase,
                eventBus
            );

        RemoveItemCommandHandler removeItemCommandHandler =
            new RemoveItemCommandHandler(
                inventory,
                eventBus
            );

        SwapInventorySlotsCommandHandler swapInventorySlotsCommandHandler =
            new SwapInventorySlotsCommandHandler(
                inventory,
                eventBus
            );

        Wallet wallet = new Wallet(initialGold);

        BuyShopItemCommandHandler buyShopItemCommandHandler =
            new BuyShopItemCommandHandler(
                shopDatabase,
                itemDatabase,
                wallet,
                commandBus,
                eventBus
            );

        SellShopItemCommandHandler sellShopItemCommandHandler =
            new SellShopItemCommandHandler(
                shopDatabase,
                itemDatabase,
                wallet,
                commandBus,
                eventBus
            );

        inventoryChangedDebugLogger =
            new InventoryChangedDebugLogger(eventBus);

        commandBus.Register<AddItemCommand, int>(
            addItemCommandHandler.Handle
        );

        commandBus.Register<RemoveItemCommand, bool>(
            removeItemCommandHandler.Handle
        );

        commandBus.Register<SwapInventorySlotsCommand, bool>(
            swapInventorySlotsCommandHandler.Handle
        );

        commandBus.Register<BuyShopItemCommand, ShopTransactionResult>(
            buyShopItemCommandHandler.Handle
        );

        commandBus.Register<SellShopItemCommand, ShopTransactionResult>(
            sellShopItemCommandHandler.Handle
        );

        inventoryController.Initialize(
            inventory,
            commandBus
        );

        inventoryView.Initialize(
            inventoryController,
            itemDatabase,
            eventBus
        );

        shopController.Initialize(commandBus);

        shopView.Initialize(
            shopController,
            shopDatabase,
            itemDatabase,
            eventBus,
            wallet.Balance
        );

        GameplayInputLock gameplayInputLock =
            new GameplayInputLock();
        UiPanelStack uiPanelStack = new UiPanelStack();

        hotbarController.Initialize(
            eventBus,
            gameplayInputLock,
            inventoryController.Slots.Count
        );

        hotbarView.Initialize(
            hotbarController,
            inventoryController,
            itemDatabase,
            eventBus
        );

        uiNavigationInput.Initialize(
            uiPanelStack,
            gameplayInputLock,
            inventoryView
        );

        shopView.Hide();

        SimplePlayerMovement playerMovement =
            FindObjectOfType<SimplePlayerMovement>(true);

        if (playerMovement != null)
        {
            playerMovement.Initialize(gameplayInputLock);
        }

        PlayerRayInteractor playerRayInteractor =
            FindObjectOfType<PlayerRayInteractor>(true);

        if (playerRayInteractor != null)
        {
            playerRayInteractor.Initialize(gameplayInputLock);
        }

        ShopInteractable[] shopInteractables =
            FindObjectsOfType<ShopInteractable>(true);

        foreach (ShopInteractable shopInteractable in shopInteractables)
        {
            shopInteractable.Initialize(
                uiNavigationInput,
                shopView
            );
        }

        shopTransactionDebugLogger =
            new ShopTransactionDebugLogger(eventBus);

        worldItemPickupDebugLogger =
            new WorldItemPickupDebugLogger(eventBus);

        WorldItemPickup[] worldItemPickups =
            FindObjectsOfType<WorldItemPickup>(true);

        foreach (WorldItemPickup worldItemPickup in worldItemPickups)
        {
            worldItemPickup.Initialize(commandBus, eventBus);
        }

        questProgressDebugLogger =
            new QuestProgressDebugLogger(eventBus);

        questView.Initialize(
            eventBus
        );

        QuestRuntimeFactory questRuntimeFactory =
            new QuestRuntimeFactory(
                inventoryController,
                eventBus,
                eventBus
            );

        questRuntimeManager = new QuestRuntimeManager(
            questDatabase,
            questRuntimeFactory,
            eventBus,
            eventBus
        );

        questRuntimeManager.StartQuest(questId);
    }

    private void OnDestroy()
    {
        questRuntimeManager?.Dispose();
        questProgressDebugLogger?.Dispose();
        shopTransactionDebugLogger?.Dispose();
        worldItemPickupDebugLogger?.Dispose();
        inventoryChangedDebugLogger?.Dispose();
    }
}
