using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class ShopInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private string prompt = "Open Farm Shop";

    private IUiNavigator uiNavigator;
    private IUiPanel shopPanel;
    private bool isInitialized;

    public string InteractionPrompt => prompt;

    public bool CanInteract => isInitialized;

    public void Initialize(
        IUiNavigator uiNavigator,
        IUiPanel shopPanel
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "ShopInteractable cannot be initialized twice."
            );
        }

        this.uiNavigator = uiNavigator
            ?? throw new ArgumentNullException(nameof(uiNavigator));
        this.shopPanel = shopPanel
            ?? throw new ArgumentNullException(nameof(shopPanel));
        isInitialized = true;
    }

    public void Interact()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "ShopInteractable has not been initialized."
            );
        }

        uiNavigator.TryOpenRootPanel(shopPanel);
    }
}
