using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSentStatus : MonoBehaviour {

	private Text imageStatus;
	
	void Awake () {
		imageStatus = GetComponent<Text> ();
		ServerConnection.ImageSendingStatus += SubmitedImagesCount;
	}
	
	// Update is called once per frame
	void SubmitedImagesCount (string value) {
		imageStatus.text = "Submitted images: " + value;
	}
}
