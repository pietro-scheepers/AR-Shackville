using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public Transform currentModel;
    public float scaleStep = 0.1f;

    public void IncreaseSize()
    {
        if (currentModel != null)
            currentModel.localScale += Vector3.one * scaleStep;
    }

    public void DecreaseSize()
    {
        if (currentModel != null)
        {
            currentModel.localScale -= Vector3.one * scaleStep;

            if (currentModel.localScale.x < 0.1f)
                currentModel.localScale = Vector3.one * 0.1f;
        }
    }
}
