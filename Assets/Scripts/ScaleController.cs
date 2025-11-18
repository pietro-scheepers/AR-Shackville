using UnityEngine;

public class ScaleController : MonoBehaviour
{
    // The currently spawned model
    public Transform currentModel;

    // How fast to scale
    public float scaleStep = 0.1f;

    // Called by the + button
    public void IncreaseSize()
    {
        if (currentModel != null)
            currentModel.localScale += Vector3.one * scaleStep;
    }

    // Called by the - button
    public void DecreaseSize()
    {
        if (currentModel != null)
        {
            currentModel.localScale -= Vector3.one * scaleStep;

            // Prevent shrinking below zero
            if (currentModel.localScale.x < 0.1f)
                currentModel.localScale = Vector3.one * 0.1f;
        }
    }
}
