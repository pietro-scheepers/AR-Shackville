using UnityEngine;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    public Transform currentModel;
    public float scaleStep = 0.01f;      // How much to scale per frame
    public float minScale = 0.1f;        // Minimum allowed scale
    public float maxScale = 10f;         // Maximum allowed scale

    // Flags to track button hold
    private bool isIncreasing = false;
    private bool isDecreasing = false;

    void Update()
    {
        if (currentModel == null) return;

        if (isIncreasing)
            ScaleUp();
        if (isDecreasing)
            ScaleDown();
    }

    // Call these from your UI Button OnPointerDown
    public void StartIncreasing() => isIncreasing = true;
    public void StopIncreasing() => isIncreasing = false;

    public void StartDecreasing() => isDecreasing = true;
    public void StopDecreasing() => isDecreasing = false;

    // Internal scaling functions
    private void ScaleUp()
    {
        currentModel.localScale += Vector3.one * scaleStep;
        ClampScale();
    }

    private void ScaleDown()
    {
        currentModel.localScale -= Vector3.one * scaleStep;
        ClampScale();
    }

    private void ClampScale()
    {
        float clamped = Mathf.Clamp(currentModel.localScale.x, minScale, maxScale);
        currentModel.localScale = Vector3.one * clamped;
    }
}
