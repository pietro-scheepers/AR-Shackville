//Not used
//Determines which object was selected in the scene
//Pietro Scheepers
//18 November 2025

using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : MonoBehaviour
{
    public ScaleController scaleController;
    public Camera arCamera;

    private InputAction touchAction;

    private Highlightable lastHighlighted;

    void Awake()
    {
        touchAction = new InputAction(binding: "<Touchscreen>/primaryTouch/press");
        touchAction.performed += ctx => OnTouch();
        touchAction.Enable();
    }

    void OnDestroy()
    {
        touchAction.Disable();
    }

    private void OnTouch()
    {
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;
        if (!touch.press.isPressed) return;

        Vector2 tapPos = touch.position.ReadValue();

        Ray ray = arCamera.ScreenPointToRay(tapPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Selectable"))
            {
                Transform selected = hit.transform;

                // Update ScaleController target
                scaleController.currentModel = selected;

                // Remove highlight from previous
                if (lastHighlighted != null)
                    lastHighlighted.RemoveHighlight();

                // Add highlight to new
                Highlightable h = selected.GetComponent<Highlightable>();
                if (h == null)
                    h = selected.gameObject.AddComponent<Highlightable>();

                h.ApplyHighlight();
                lastHighlighted = h;

                Debug.Log("Selected: " + selected.name);
            }
        }
    }
}
