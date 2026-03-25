using UnityEngine;

[System.Serializable]
public class SiteData
{
    public string siteName;
    [TextArea(3, 6)]
    public string description;

    // The exact name used in your AR Reference Image Library for this QR
    public string qrCodeName;

    // Optional: latitude/longitude if you want Map links
    public double latitude;
    public double longitude;

    // Optional thumbnail to show in popup
    public Sprite thumbnail;
}
