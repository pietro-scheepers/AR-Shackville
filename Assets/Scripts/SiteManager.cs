using System.Collections.Generic;
using UnityEngine;

// Simple singleton to hold your sites list, editable in the Inspector
public class SiteManager : MonoBehaviour
{
    public static SiteManager Instance { get; private set; }

    [Tooltip("Populate the list of sites (name, description, qrCodeName, coords, optional thumbnail)")]
    public List<SiteData> allSites = new List<SiteData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // optional
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public SiteData GetSiteByQRCode(string qrCodeName)
    {
        return allSites.Find(s => s.qrCodeName == qrCodeName);
    }

    public SiteData GetSiteByName(string name)
    {
        return allSites.Find(s => s.siteName == name);
    }
}
