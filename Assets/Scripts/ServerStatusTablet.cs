using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ServerStatusTablet : MonoBehaviour {
	
	private Text serverStatus;
	
	// Use this for initialization
	void Awake () {
		serverStatus = GetComponent<Text> ();
		ServerConnection.ServerStatus += UpdateServerStatus;
	}
	
	// Update is called once per frame
	void UpdateServerStatus (string value) {
		serverStatus.text = "Server status: " + (value == "False" ? "Online" : "Offline");
	}
}