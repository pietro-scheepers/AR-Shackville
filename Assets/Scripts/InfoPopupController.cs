//controls the additional information pop-up per site
//Pietro Scheepers
//11 November 2025

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPopupController : MonoBehaviour
{
    [Header("Popup UI")]
    public GameObject infoPopup;       
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Image thumbnailImage;       
    public Button closeButton;

    void Awake()
    {
        if (infoPopup != null)
            infoPopup.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HideInfo);
    }

    // Show via SiteData
    public void ShowInfo(SiteData site)
    {
        if (site == null || infoPopup == null) return;

        titleText.text = site.siteName;
        descriptionText.text = site.description ?? "";

        if (thumbnailImage != null)
        {
            if (site.thumbnail != null)
            {
                thumbnailImage.sprite = site.thumbnail;
                thumbnailImage.gameObject.SetActive(true);
            }
            else
            {
                thumbnailImage.gameObject.SetActive(false);
            }
        }

        infoPopup.SetActive(true);
    }

    public void ShowInfo(string title, string description)
    {
        if (infoPopup == null) return;
        titleText.text = title;
        descriptionText.text = description;
        if (thumbnailImage != null) thumbnailImage.gameObject.SetActive(false);
        infoPopup.SetActive(true);
    }

    public void HideInfo()
    {
        if (infoPopup == null) return;
        infoPopup.SetActive(false);
    }
}
