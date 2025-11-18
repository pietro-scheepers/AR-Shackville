using UnityEngine;
using UnityEngine.InputSystem; // Import the Input System namespace (if using new input system)

public class SelectionManager : MonoBehaviour
{
    // Reference to the ScaleController to set the current model
    public ScaleController scaleController;
    public Camera arCamera; // Reference to your AR camera

    // Use the new Unity Input System for touch handling
    private InputAction touchAction;

    void Awake()
    {
        // Basic setup for the new Input System (adjust based on your actual setup)
        // This assumes you have an Input Action Asset set up.
        // For a simple single touch setup without an asset:
        touchAction = new InputAction(binding: "*/{PrimaryTouchContact}");
        touchAction.performed += ctx => HandleTouch(ctx);
        touchAction.Enable();
    }

    void OnDestroy()
    {
        touchAction.Disable();
    }

    private void HandleTouch(InputAction.CallbackContext context)
    {
        // Get the touch position from the screen
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object is one of your spawned models
            if (hit.collider.CompareTag("Selectable"))
            {
                // Set the ScaleController's current model to the tapped object
                if (scaleController != null)
                {
                    scaleController.currentModel = hit.transform;
                    Debug.Log("Selected object: " + hit.transform.name);
                    // You can add visual feedback here (e.g., a highlight/outline)
                }
            }
        }
    }
}
