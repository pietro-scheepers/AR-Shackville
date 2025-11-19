using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class QRPrefabMapping
{
    public string qrCodeName;      // Must match Reference Image Library
    public GameObject prefab;

    public Vector3 positionOffset; // Local offset from QR center
    public Vector3 rotationOffset; // Euler angle offset
    public float scale = 1f;       // Uniform scale
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARCodeSpawner : MonoBehaviour
{
    public ScaleController scaleController;

    public List<QRPrefabMapping> mappings;
    public GameObject defaultPrefab;

    public float hideDistance = 1f;

    private ARTrackedImageManager trackedImageManager;

    private GameObject currentSpawnedObject = null;
    private Transform currentImageTransform = null;

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
        // NEW QR detected
        foreach (var added in args.added)
            SpawnFresh(added);

        // Existing QR repositioned
        foreach (var updated in args.updated)
        {
            if (updated.trackingState == TrackingState.Tracking)
                UpdateExisting(updated);
            else if (currentSpawnedObject != null)
                currentSpawnedObject.SetActive(false);
        }

        // QR removed from view
        foreach (var removed in args.removed)
        {
            if (currentSpawnedObject != null)
            {
                Destroy(currentSpawnedObject);
                currentSpawnedObject = null;
            }
            currentImageTransform = null;
        }
    }

    void Update()
    {
        if (currentSpawnedObject == null || currentImageTransform == null)
            return;

        float distance = Vector3.Distance(
            Camera.main.transform.position,
            currentImageTransform.position);

        currentSpawnedObject.SetActive(distance <= hideDistance);
    }

    // -----------------------------------------------------
    // Spawn a NEW object (destroy previous one)
    // -----------------------------------------------------
    void SpawnFresh(ARTrackedImage trackedImage)
    {
        if (currentSpawnedObject != null)
            Destroy(currentSpawnedObject);

        currentSpawnedObject = CreateSpawnedObject(trackedImage);
        currentImageTransform = trackedImage.transform;
    }

    // -----------------------------------------------------
    // Update position + rotation of existing object
    // -----------------------------------------------------
    void UpdateExisting(ARTrackedImage trackedImage)
    {
        if (currentSpawnedObject == null)
        {
            SpawnFresh(trackedImage);
            return;
        }

        var mapping = GetMapping(trackedImage.referenceImage.name);

        Vector3 pos = trackedImage.transform.position;
        Quaternion rot = trackedImage.transform.rotation;

        if (mapping != null)
        {
            rot *= Quaternion.Euler(mapping.rotationOffset);
            pos += rot * mapping.positionOffset;
        }

        currentSpawnedObject.transform.position = pos;
        currentSpawnedObject.transform.rotation = rot;

        currentSpawnedObject.SetActive(true);

        latestSpawnedModel = currentSpawnedObject.transform;

        if (scaleController != null)
            scaleController.currentModel = latestSpawnedModel;

        currentImageTransform = trackedImage.transform;
    }

    // -----------------------------------------------------
    // Instantiate object with offset + scale
    // -----------------------------------------------------
    GameObject CreateSpawnedObject(ARTrackedImage trackedImage)
    {
        var mapping = GetMapping(trackedImage.referenceImage.name);

        GameObject prefab = mapping?.prefab ?? defaultPrefab;
        if (prefab == null) return null;

        Vector3 pos = trackedImage.transform.position;
        Quaternion rot = trackedImage.transform.rotation;

        if (mapping != null)
        {
            rot *= Quaternion.Euler(mapping.rotationOffset);
            pos += rot * mapping.positionOffset;
        }

        GameObject obj = Instantiate(prefab, pos, rot);

        if (mapping != null)
            obj.transform.localScale = Vector3.one * mapping.scale;

        obj.transform.SetParent(trackedImage.transform, true);

        latestSpawnedModel = obj.transform;

        if (scaleController != null)
            scaleController.currentModel = latestSpawnedModel;

        return obj;
    }

    // -----------------------------------------------------
    // Lookup mapping
    // -----------------------------------------------------
    QRPrefabMapping GetMapping(string qrName)
    {
        foreach (var m in mappings)
        {
            if (m.qrCodeName == qrName)
                return m;
        }
        return null;
    }
}
