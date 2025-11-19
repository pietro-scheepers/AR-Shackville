//Scales spawned objects smaller or bigger
//Pietro Scheepers
//19 November2025

using UnityEngine;
using UnityEngine.UI;

public class ScaleController : MonoBehaviour
{
    public Transform currentModel;
    public float scaleStep = 0.01f;      
    public float minScale = 0.1f;        
    public float maxScale = 10f;         

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

    public void StartIncreasing() => isIncreasing = true;
    public void StopIncreasing() => isIncreasing = false;

    public void StartDecreasing() => isDecreasing = true;
    public void StopDecreasing() => isDecreasing = false;

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

    //Makes sure the size does not go bigger or smaller than max and min -- this can be changed
    private void ClampScale()
    {
        float clamped = Mathf.Clamp(currentModel.localScale.x, minScale, maxScale);
        currentModel.localScale = Vector3.one * clamped;
    }
}
