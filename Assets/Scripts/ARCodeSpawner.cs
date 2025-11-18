using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class QRPrefabMapping
{
    public string qrCodeName; // must match Reference Image Library
    public GameObject prefab;
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARCodeSpawner : MonoBehaviour
{
    public List<QRPrefabMapping> mappings = new List<QRPrefabMapping>();
    public GameObject defaultPrefab;

    // Distance at which spawned object becomes hidden (meters)
    public float hideDistance = 1f;

    private ARTrackedImageManager trackedImageManager;

    // Track spawned objects
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private Dictionary<string, Transform> trackedImageTransforms = new Dictionary<string, Transform>();

    //
    // NEW — store the most recently spawned model
    public Transform latestSpawnedModel { get; private set; }


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
        // === Added images ===
        foreach (var added in args.added)
        {
            trackedImageTransforms[added.referenceImage.name] = added.transform;
            SpawnOrUpdate(added);
        }

        // === Updated images ===
        foreach (var updated in args.updated)
        {
            string key = updated.referenceImage.name;
            trackedImageTransforms[key] = updated.transform;

            if (updated.trackingState == TrackingState.Tracking)
            {
                SpawnOrUpdate(updated);
            }
            else
            {
                // QR is not tracked -> hide spawned model
                if (spawnedObjects.ContainsKey(key))
                    spawnedObjects[key].SetActive(false);
            }
        }

        // === Removed images ===
        foreach (var removed in args.removed)
        {
            string key = removed.referenceImage.name;

            if (spawnedObjects.ContainsKey(key))
            {
                Destroy(spawnedObjects[key]);
                spawnedObjects.Remove(key);
            }

            if (trackedImageTransforms.ContainsKey(key))
                trackedImageTransforms.Remove(key);
        }
    }


    void Update()
    {
        // Distance-based hiding
        foreach (var pair in spawnedObjects)
        {
            string key = pair.Key;
            GameObject obj = pair.Value;

            if (!trackedImageTransforms.ContainsKey(key)) continue;

            float distance = Vector3.Distance(Camera.main.transform.position, trackedImageTransforms[key].position);

            obj.SetActive(distance <= hideDistance);
        }
    }


    void SpawnOrUpdate(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        GameObject prefabToSpawn = GetPrefabForQRCode(imageName) ?? defaultPrefab;
        if (prefabToSpawn == null) return;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            // First time spawning this QR model
            GameObject newObj = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            newObj.transform.SetParent(trackedImage.transform, true);

            spawnedObjects.Add(imageName, newObj);

            // NEW — record latest spawned model
            latestSpawnedModel = newObj.transform;
        }
        else
        {
            // Already exists -> update its tracking position
            GameObject existing = spawnedObjects[imageName];
            existing.SetActive(true);
            existing.transform.position = trackedImage.transform.position;
            existing.transform.rotation = trackedImage.transform.rotation;

            // NEW — also mark updated objects as "latest"
            latestSpawnedModel = existing.transform;
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
