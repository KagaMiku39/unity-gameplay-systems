using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class SimplePlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    private Rigidbody body;
    private Vector3 movement;
    private IGameplayInputGate inputGate;
    private bool isInitialized;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Initialize(IGameplayInputGate inputGate)
    {
        if (isInitialized)
        {
            throw new InvalidOperationException(
                "SimplePlayerMovement cannot be initialized twice."
            );
        }

        this.inputGate = inputGate
            ?? throw new ArgumentNullException(nameof(inputGate));
        isInitialized = true;
    }

    private void Update()
    {
        if (isInitialized == false || inputGate.IsBlocked)
        {
            movement = Vector3.zero;
            return;
        }

        movement = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (movement.sqrMagnitude > 0f)
        {
            transform.forward = movement;
        }
    }

    private void FixedUpdate()
    {
        if (isInitialized == false || inputGate.IsBlocked)
        {
            movement = Vector3.zero;
            return;
        }

        body.MovePosition(
            body.position + movement * speed * Time.fixedDeltaTime
        );
    }

    private void Start()
    {
        if (isInitialized == false)
        {
            throw new InvalidOperationException(
                "SimplePlayerMovement has not been initialized."
            );
        }
    }
}
