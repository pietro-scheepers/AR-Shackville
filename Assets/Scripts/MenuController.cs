using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MenuController : MonoBehaviour
{
    [Header("UI Prefab & Container")]
    public GameObject listItemPrefab;      // Must have children: "SiteName" (TMP), "InfoButton" (Button), "MapsButton" (Button)
    public Transform contentParent;        // Scroll View -> Viewport -> Content

    [Header("Popup")]
    public InfoPopupController infoPopupController;

    void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        if (SiteManager.Instance == null || listItemPrefab == null || contentParent == null) return;

        // Clear previous children (if any)
        foreach (Transform t in contentParent) Destroy(t.gameObject);

        foreach (SiteData site in SiteManager.Instance.allSites)
        {
            GameObject item = Instantiate(listItemPrefab, contentParent);
            item.SetActive(true);

            // Ensure panel Image is enabled
            Image panelImage = item.GetComponent<Image>();
            if (panelImage != null)
                panelImage.enabled = true;

            // Site Name text
            TMP_Text nameText = item.transform.Find("SiteName")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.enabled = true;
                nameText.text = site.siteName;
            }

            // Info Button
            Button infoButton = item.transform.Find("Info")?.GetComponent<Button>();
            if (infoButton != null)
            {
                infoButton.interactable = true;

                Image infoImg = infoButton.GetComponent<Image>();
                if (infoImg != null) infoImg.enabled = true;

                Button button = infoButton.GetComponent<Button>();
                if (button != null) button.enabled = true;

                // Clear previous listeners to avoid duplicates
                infoButton.onClick.RemoveAllListeners();
                infoButton.onClick.AddListener(() =>
                {
                    Debug.Log("Info clicked for " + site.siteName);
                    infoPopupController.ShowInfo(site);
                });
            }

            // Maps Button
            Button mapsButton = item.transform.Find("Map")?.GetComponent<Button>();
            if (mapsButton != null)
            {
                mapsButton.interactable = true;

                Image mapsImg = mapsButton.GetComponent<Image>();
                if (mapsImg != null) mapsImg.enabled = true;

                Button button = mapsButton.GetComponent<Button>();
                if (button != null) button.enabled = true;

                mapsButton.onClick.RemoveAllListeners();
                mapsButton.onClick.AddListener(() =>
                {
                    Debug.Log("Maps clicked for " + site.siteName);
                    OpenMapsForSite(site);
                });
            }
        }
    }

    void OpenMapsForSite(SiteData site)
    {
        if (site == null) return;

        // Use coordinates only
        if (site.latitude != 0 || site.longitude != 0)
        {
        #if UNITY_IOS
                string appleMaps = $"http://maps.apple.com/?ll={site.latitude},{site.longitude}";
                Application.OpenURL(appleMaps);
        #else
                string googleMaps = $"geo:{site.latitude},{site.longitude}";
                Application.OpenURL(googleMaps);
        #endif
                }
                else
                {
        #if UNITY_IOS
                string appleMaps = $"http://maps.apple.com/?ll=0,0";
                Application.OpenURL(appleMaps);
        #else
                string googleMapsWeb = $"https://www.google.com/maps/search/?api=1&query=0,0";
                Application.OpenURL(googleMapsWeb);
        #endif
        }
    }


    string UnityWebRequestEscape(string value)
    {
        return UnityEngine.Networking.UnityWebRequest.EscapeURL(value);
    }

    // Called by ARCodeSpawner to open popup by qrCodeName
    public void ShowPopupForQRCode(string qrCodeName)
    {
        SiteData site = SiteManager.Instance?.GetSiteByQRCode(qrCodeName);
        if (site != null)
            infoPopupController.ShowInfo(site);
    }

    // Called by ARCodeSpawner to hide the popup (e.g., when image lost)
    public void HidePopup()
    {
        infoPopupController.HideInfo();
    }
}
