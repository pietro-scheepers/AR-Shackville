using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// Simple serializable mapping so you can set prefab per qrCodeName in Inspector
[System.Serializable]
public class QRPrefabMapping
{
    public string qrCodeName;   // must match the name in Reference Image Library
    public GameObject prefab;
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARCodeSpawner : MonoBehaviour
{
    public List<QRPrefabMapping> mappings = new List<QRPrefabMapping>();

    // optional: fallback prefab if nothing matches
    public GameObject defaultPrefab;

    // Reference to MenuController so we can show popup
    public MenuController menuController;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        // Added
        foreach (var added in args.added)
        {
            SpawnOrUpdate(added);
            // open popup for this qr
            menuController?.ShowPopupForQRCode(added.referenceImage.name);
        }

        // Updated
        foreach (var updated in args.updated)
        {
            if (updated.trackingState == TrackingState.Tracking)
            {
                SpawnOrUpdate(updated);
                // Optionally ensure popup for the currently tracked one is visible
                menuController?.ShowPopupForQRCode(updated.referenceImage.name);
            }
            else
            {
                // not tracking: hide associated object but keep it in dictionary if you prefer
                if (spawnedObjects.ContainsKey(updated.referenceImage.name))
                {
                    spawnedObjects[updated.referenceImage.name].SetActive(false);
                }

                // Hide popup if the lost image is the one shown
                menuController?.HidePopup();
            }
        }

        // Removed
        foreach (var removed in args.removed)
        {
            // Destroy spawned object and remove mapping
            if (spawnedObjects.ContainsKey(removed.referenceImage.name))
            {
                Destroy(spawnedObjects[removed.referenceImage.name]);
                spawnedObjects.Remove(removed.referenceImage.name);
            }

            // If popup was showing for this, hide it
            menuController?.HidePopup();
        }
    }

    void SpawnOrUpdate(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        GameObject prefabToSpawn = GetPrefabForQRCode(imageName) ?? defaultPrefab;

        if (prefabToSpawn == null) return;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            GameObject newObj = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            // Parent to the tracked image so it follows automatically (optional)
            newObj.transform.SetParent(trackedImage.transform, true);
            spawnedObjects.Add(imageName, newObj);
        }
        else
        {
            GameObject existing = spawnedObjects[imageName];
            existing.SetActive(true);
            // Update transform so it matches the trackedImage
            existing.transform.position = trackedImage.transform.position;
            existing.transform.rotation = trackedImage.transform.rotation;
        }
    }

    GameObject GetPrefabForQRCode(string qrCodeName)
    {
        foreach (var m in mappings)
        {
            if (m.qrCodeName == qrCodeName)
                return m.prefab;
        }
        return null;
    }
}
