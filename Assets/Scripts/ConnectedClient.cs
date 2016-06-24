using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnectedClient : MonoBehaviour {

	private Text clientCount;
	
	// Use this for initialization
	void Awake () {
		clientCount = GetComponent<Text> ();
		ServerFirstSetup.UpdateClientCount += UpdateClients;
	}
	
	// Update is called once per frame
	private void UpdateClients (string value) {
		clientCount.text = "Clients: " + value;
	}
}
