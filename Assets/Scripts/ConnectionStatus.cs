using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnectionStatus : MonoBehaviour {

	private Text serverStatus;
	
	// Use this for initialization
	void Awake () {
		serverStatus = GetComponent<Text> ();
		ServerConnection.ConnectionStatus += UpdateConnectionStatus;
	}
	
	// Update is called once per frame
	void UpdateConnectionStatus (string value) {
		serverStatus.text = "Connection status: " + (value == "False" ? "Diconnected" : "Connected");
	}
}
