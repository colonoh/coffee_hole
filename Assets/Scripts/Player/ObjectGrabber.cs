using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float holdFollowSpeed = 15f;
    [SerializeField] private LayerMask grabbableLayer = ~0; // everything by default

    private InputAction attackAction;
    private Rigidbody heldObject;
    private bool wasGravity;

    private void Awake()
    {
        var playerInput = GetComponentInParent<PlayerInput>();
        attackAction = playerInput.actions["Attack"];
    }

    private void OnEnable()
    {
        // Defer subscription so attackAction is assigned
        if (attackAction != null)
            attackAction.performed += OnAttack;
    }

    private void Start()
    {
        // Subscribe here too in case OnEnable ran before Awake
        if (attackAction != null)
            attackAction.performed -= OnAttack;
        attackAction = GetComponentInParent<PlayerInput>().actions["Attack"];
        attackAction.performed += OnAttack;
    }

    private void OnDisable()
    {
        if (attackAction != null)
            attackAction.performed -= OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        // Don't grab/release while cursor is unlocked
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        if (heldObject != null)
            Release();
        else
            TryGrab();
    }

    private void TryGrab()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, grabRange, grabbableLayer))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                heldObject = rb;
                wasGravity = rb.useGravity;
                rb.useGravity = false;
                // Zero out velocity/spin so object doesn't fight the hold
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                // Not grabbable — try interacting instead
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                interactable?.Interact();
            }
        }
    }

    private void Release()
    {
        if (heldObject == null) return;
        heldObject.useGravity = wasGravity;
        // Object keeps its current velocity (set by FixedUpdate lerping)
        heldObject = null;
    }

    private void FixedUpdate()
    {
        if (heldObject == null) return;

        // If the held object was destroyed or disabled
        if (!heldObject.gameObject.activeInHierarchy)
        {
            heldObject = null;
            return;
        }

        Vector3 holdPoint = transform.position + transform.forward * holdDistance;
        Vector3 toTarget = holdPoint - heldObject.position;

        // Velocity-based following so the object still collides with things
        heldObject.linearVelocity = toTarget * holdFollowSpeed;

        // Dampen rotation so held object doesn't spin wildly
        heldObject.angularVelocity *= 0.5f;
    }
}
