//Used to create a site class containing information about the site
//Pietro Scheepers
//11 November 2025

using UnityEngine;

[System.Serializable]
public class SiteData
{
    public string siteName;
    [TextArea(3, 6)]
    public string description;

    
    public string qrCodeName;
    public double latitude;
    public double longitude;
    public Sprite thumbnail;
}
