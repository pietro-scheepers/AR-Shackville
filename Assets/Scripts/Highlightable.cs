using UnityEngine;

public class Highlightable : MonoBehaviour
{
    private Material[] originalMats;
    private Material outlineMat;

    void Awake()
    {
        // Create outline material (simple emissive material)
        outlineMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        outlineMat.EnableKeyword("_EMISSION");
        outlineMat.SetColor("_EmissionColor", Color.yellow * 1.5f);
    }

    public void ApplyHighlight()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (var rend in renderers)
        {
            originalMats = rend.materials;

            Material[] newMats = new Material[originalMats.Length + 1];
            originalMats.CopyTo(newMats, 0);
            newMats[newMats.Length - 1] = outlineMat;

            rend.materials = newMats;
        }
    }

    public void RemoveHighlight()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (var rend in renderers)
        {
            if (originalMats != null)
            {
                rend.materials = originalMats;
            }
        }
    }
}
