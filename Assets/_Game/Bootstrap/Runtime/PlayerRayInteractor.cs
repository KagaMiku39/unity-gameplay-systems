using System;
using UnityEngine;

public sealed class PlayerRayInteractor : MonoBehaviour
{
    [SerializeField]
    private Transform rayOrigin;

    [SerializeField]
    private float interactionDistance = 2.5f;

    [SerializeField]
    private LayerMask interactableLayers = ~0;

    [SerializeField]
    private InteractionPromptView promptView;

    [SerializeField]
    private Camera worldCamera;

    private IGameplayInputGate inputGate;
    private IInteractable currentTarget;
    private bool isInitialized;

    public IInteractable CurrentTarget => currentTarget;

    public void Initialize(IGameplayInputGate inputGate)
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "PlayerRayInteractor cannot be initialized twice."
            );
        }

        this.inputGate = inputGate
            ?? throw new ArgumentNullException(nameof(inputGate));

        if (rayOrigin == null)
        {
            throw new InvalidOperationException(
                "PlayerRayInteractor requires a ray origin."
            );
        }

        if (promptView == null)
        {
            throw new InvalidOperationException(
                "PlayerRayInteractor requires an interaction prompt view."
            );
        }

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (worldCamera == null)
        {
            throw new InvalidOperationException(
                "PlayerRayInteractor requires a world camera."
            );
        }

        if (interactionDistance <= 0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(interactionDistance),
                interactionDistance,
                "Interaction distance must be greater than zero."
            );
        }

        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized == false)
        {
            return;
        }

        if (inputGate.IsBlocked)
        {
            SetCurrentTarget(null, default);
            return;
        }

        RefreshTarget();

        if (
            currentTarget != null &&
            currentTarget.CanInteract &&
            Input.GetKeyDown(KeyCode.F)
        )
        {
            currentTarget.Interact();
            RefreshTarget();
        }
    }

    private void RefreshTarget()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        Debug.DrawRay(
            ray.origin,
            ray.direction * interactionDistance,
            Color.cyan
        );

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(
            ray,
            out hit,
            interactionDistance,
            interactableLayers,
            QueryTriggerInteraction.Collide
        );

        if (hitSomething == false)
        {
            SetCurrentTarget(null, default);
            return;
        }

        IInteractable target = FindInteractable(hit.collider);

        if (target == null)
        {
            SetCurrentTarget(null, default);
            return;
        }

        Vector3 markerWorldPosition = hit.collider.bounds.center;
        Vector3 markerScreenPosition =
            worldCamera.WorldToScreenPoint(markerWorldPosition);

        if (markerScreenPosition.z <= 0f)
        {
            SetCurrentTarget(null, default);
            return;
        }

        SetCurrentTarget(target, markerScreenPosition);
    }

    private static IInteractable FindInteractable(Collider collider)
    {
        MonoBehaviour[] behaviours =
            collider.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IInteractable interactable)
            {
                return interactable;
            }
        }

        return null;
    }

    private void SetCurrentTarget(
        IInteractable target,
        Vector2 markerScreenPosition
    )
    {
        if (target != null && target.CanInteract == false)
        {
            target = null;
        }

        currentTarget = target;

        if (currentTarget == null)
        {
            promptView.Clear();
            return;
        }

        promptView.Render(
            currentTarget.InteractionPrompt,
            markerScreenPosition
        );
    }

    private void OnDisable()
    {
        if (promptView != null)
        {
            promptView.Clear();
        }
    }

    private void Start()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "PlayerRayInteractor has not been initialized."
            );
        }
    }
}
