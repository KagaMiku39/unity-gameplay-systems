using System;
using UnityEngine;

public sealed class UiNavigationInput : MonoBehaviour, IUiNavigator
{
    private readonly object inputLockOwner = new object();

    private UiPanelStack panelStack;
    private IGameplayInputLock gameplayInputLock;
    private IUiPanel inventoryPanel;
    private bool isInitialized;

    public int OpenPanelCount => panelStack?.Count ?? 0;

    public void Initialize(
        UiPanelStack panelStack,
        IGameplayInputLock gameplayInputLock,
        IUiPanel inventoryPanel
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "UiNavigationInput cannot be initialized twice."
            );
        }

        this.panelStack = panelStack
            ?? throw new ArgumentNullException(nameof(panelStack));
        this.gameplayInputLock = gameplayInputLock
            ?? throw new ArgumentNullException(nameof(gameplayInputLock));
        this.inventoryPanel = inventoryPanel
            ?? throw new ArgumentNullException(nameof(inventoryPanel));

        inventoryPanel.Hide();
        isInitialized = true;
    }

    public void ToggleInventory()
    {
        EnsureInitialized();

        if (panelStack.IsTop(inventoryPanel))
        {
            TryCloseTopPanel();
            return;
        }

        TryOpenRootPanel(inventoryPanel);
    }

    public bool TryOpenRootPanel(IUiPanel panel)
    {
        EnsureInitialized();

        if (panel == null)
        {
            throw new ArgumentNullException(nameof(panel));
        }

        if (panelStack.Count > 0)
        {
            return false;
        }

        panelStack.Push(panel);
        SynchronizeGameplayInputLock();
        return true;
    }

    public bool TryCloseTopPanel()
    {
        EnsureInitialized();
        bool closed = panelStack.TryPop();
        SynchronizeGameplayInputLock();
        return closed;
    }

    private void Update()
    {
        if (isInitialized == false)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryCloseTopPanel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
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
                "UiNavigationInput has not been initialized."
            );
        }
    }

    private void SynchronizeGameplayInputLock()
    {
        if (panelStack.Count > 0)
        {
            gameplayInputLock.Acquire(inputLockOwner);
            return;
        }

        gameplayInputLock.Release(inputLockOwner);
    }

    private void OnDestroy()
    {
        gameplayInputLock?.Release(inputLockOwner);
    }
}
