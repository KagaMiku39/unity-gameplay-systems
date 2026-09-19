using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public sealed class WorldItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField]
    private int itemId = 1;

    [SerializeField]
    private int amount = 1;

    [SerializeField]
    private bool collectOnTrigger;

    private ICommandSender commandSender;
    private IEventPublisher eventPublisher;
    private bool isInitialized;
    private bool isCollecting;

    public int ItemId => itemId;
    public int RemainingAmount => amount;
    public string InteractionPrompt =>
        $"Pick up item {itemId} x{amount}";
    public bool CanInteract =>
        collectOnTrigger == false &&
        isInitialized &&
        isCollecting == false &&
        amount > 0;

    private void Awake()
    {
        itemId = Guard.Positive(itemId, nameof(itemId));
        amount = Guard.Positive(amount, nameof(amount));

        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;

        Rigidbody body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
    }

    public void Initialize(
        ICommandSender commandSender,
        IEventPublisher eventPublisher
    )
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                $"拾取物 {name} 不能重复初始化。"
            );
        }

        this.commandSender = commandSender
            ?? throw new ArgumentNullException(nameof(commandSender));
        this.eventPublisher = eventPublisher
            ?? throw new ArgumentNullException(nameof(eventPublisher));
        isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collectOnTrigger == false)
        {
            return;
        }

        if (IsPlayer(other) == false)
        {
            return;
        }

        TryCollect();
    }

    private static bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return true;
        }

        Rigidbody attachedBody = other.attachedRigidbody;
        return attachedBody != null &&
            attachedBody.CompareTag("Player");
    }

    public void Interact()
    {
        if (CanInteract == false)
        {
            return;
        }

        TryCollect();
    }

    public int TryCollect()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                $"拾取物 {name} 尚未初始化。"
            );
        }

        if (isCollecting)
        {
            return 0;
        }

        isCollecting = true;

        try
        {
            int previousAmount = amount;
            int remaining = commandSender.Send(
                new AddItemCommand(itemId, previousAmount)
            );
            int collectedAmount = previousAmount - remaining;

            if (collectedAmount == 0)
            {
                return 0;
            }

            amount = remaining;
            eventPublisher.Publish(
                new WorldItemPickedUpEvent(
                    itemId,
                    collectedAmount,
                    amount
                )
            );

            if (amount == 0)
            {
                Destroy(gameObject);
            }

            return collectedAmount;
        }
        finally
        {
            isCollecting = false;
        }
    }
}
