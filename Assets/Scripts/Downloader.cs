using UnityEngine;
using TNet;
using System.Net;
using System.IO;
using UnityEngine.UI;

public class Downloader : MonoBehaviour
{
	public Image image;
	
	public delegate void ServerEvent(string value);
	public static event ServerEvent ImageLoaded = delegate{};
	public static event ServerEvent TxtLoaded = delegate{};

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
		if (isImage) {
            Texture2D texture = LoadPNG(SaveLoadManager.saveLoadMediaFolder + fileName);
			Rect rect = new Rect (0, 0, texture.width, texture.height);
			Vector2 vec2 = new Vector2 (0.5f, 0.5f);
			image.sprite = Sprite.Create (texture, rect, vec2);
            ImageLoaded(SaveLoadManager.saveLoadMediaFolder + fileName);
		}
		else
		{
            TxtLoaded(SaveLoadManager.saveLoadMediaFolder + fileName);
		}
	}

	public static Texture2D LoadPNG(string filePath) {
		
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
