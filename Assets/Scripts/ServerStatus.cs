using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerStatus : MonoBehaviour {

	private Text serverStatus;

	// Use this for initialization
	void Awake () {
		serverStatus = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		serverStatus.text = "Server status: " + (TNServerInstance.isActive == true ? "Online" : "Offline");
	}
}
