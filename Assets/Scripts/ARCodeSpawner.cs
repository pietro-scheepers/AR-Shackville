//Keeps track of the QR code/image that correlates to a specific object to be spawned
//Pietro Scheepers
//31 October 2025

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//Class corresponding to the spawned object
[System.Serializable]
public class QRPrefabMapping
{
    public string qrCodeName;     
    public GameObject prefab;

    public Vector3 positionOffset; 
    public Vector3 rotationOffset;
    public float scale = 1f;       
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

        // QR removed from view -- keep object still spawned
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

    //Spawn a new object in the scene
    void SpawnFresh(ARTrackedImage trackedImage)
    {
        if (currentSpawnedObject != null)
            Destroy(currentSpawnedObject);

        currentSpawnedObject = CreateSpawnedObject(trackedImage);
        currentImageTransform = trackedImage.transform;
    }

    //If the code is repositioned, then update the object position etc
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

    //Create object with the predefined position, scale and rotation
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

    //Get the correct mapping for the scaned image
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
