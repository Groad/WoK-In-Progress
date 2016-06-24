using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadedImageListening : MonoBehaviour {

	private Text imagePath;
	
	// Use this for initialization
	void Awake () {
		imagePath = GetComponent<Text> ();
		Downloader.ImageLoaded += ImageLoaded;
	}
	
	void ImageLoaded (string path) {
		imagePath.text = "Image Loaded Path: " + path;
		if (IsInvoking ("ClearPath"))
			CancelInvoke ("ClearPath");
		Invoke ("ClearPath", 20);
	}

	private void ClearPath()
	{
		imagePath.text = "Image Loaded Path: -";
	}
}
