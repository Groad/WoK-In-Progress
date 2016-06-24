using UnityEngine;
using TNet;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class DesktopDownloader : MonoBehaviour {

    private StickyScript lastCreatedSticky;

    public AudioClip audioGotFileFromTablet;

    public delegate void ServerEvent(string value);
    public static event ServerEvent ImageLoaded = delegate { };
    public static event ServerEvent TxtLoaded = delegate { };

    void Start()
    {

    }

    [RFC]
    void SavedFile(string filePath, bool isImage)
    {
        string fileName = filePath;
        if (filePath.Contains("/") || filePath.Contains("\\"))
        {
            char[] delimiters = new char[] { '\\', '/' };
            string[] parts = filePath.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                fileName = parts[parts.Length - 1];
            }
        }
        if (!File.Exists(SaveLoadManager.saveLoadMediaFolder + fileName))
        {
            File.Copy(filePath, SaveLoadManager.saveLoadMediaFolder + fileName);
            File.Delete(filePath);
        }
        Debug.Log("File saved: " + SaveLoadManager.saveLoadMediaFolder + fileName);
        if (isImage)
        {
            Texture2D texture = LoadPNG(SaveLoadManager.saveLoadMediaFolder + fileName);
            if (lastCreatedSticky != null)
            {
                lastCreatedSticky.putPhoto(texture, true);
                lastCreatedSticky.imagePath = fileName;
            }
            else
            {
                GameObject sticky = NewStickyButton.instance.CreateSticky();
                lastCreatedSticky = sticky.GetComponent<StickyScript>();
                lastCreatedSticky.putPhoto(texture, true);
                lastCreatedSticky.imagePath = fileName;
                lastCreatedSticky.GetComponent<StickyInPanel>().sendToPalette(GameObject.Find("StickyPalette").GetComponent<StickyPalette>());
                lastCreatedSticky = null;
                AudioSource.PlayClipAtPoint(audioGotFileFromTablet, Vector3.zero);
            }
            ImageLoaded(SaveLoadManager.saveLoadMediaFolder + fileName);
        }
        else
        {
            string fileData;

            if (File.Exists(SaveLoadManager.saveLoadMediaFolder + fileName))
            {
                fileData = File.ReadAllText(SaveLoadManager.saveLoadMediaFolder + fileName);
                GameObject sticky = NewStickyButton.instance.CreateSticky();
                lastCreatedSticky = sticky.GetComponent<StickyScript>();
                lastCreatedSticky.readStickyInfo(fileData);
                lastCreatedSticky.GetComponent<StickyInPanel>().sendToPalette(GameObject.Find("StickyPalette").GetComponent<StickyPalette>());
                if (!lastCreatedSticky.isTherePhoto)
                {
                    lastCreatedSticky = null;
                }
                AudioSource.PlayClipAtPoint(audioGotFileFromTablet, Vector3.zero);
            }
            TxtLoaded(SaveLoadManager.saveLoadMediaFolder + filePath);
        }
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }
}
