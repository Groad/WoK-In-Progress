using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadedTxtListening : MonoBehaviour {

	private Text txtPath;
	
	// Use this for initialization
	void Awake () {
		txtPath = GetComponent<Text> ();
		Downloader.TxtLoaded += TxtLoaded;
	}

	void TxtLoaded (string path) {
		txtPath.text = "Txt Loaded Path: " + path;
		if (IsInvoking ("ClearPath"))
			CancelInvoke ("ClearPath");
		Invoke ("ClearPath", 20);
	}
	
	private void ClearPath()
	{
		txtPath.text = "Txt Loaded Path: -";
	}
}
