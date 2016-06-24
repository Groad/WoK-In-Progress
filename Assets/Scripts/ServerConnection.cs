using UnityEngine;
using UnityEngine.UI;
using TNet;
using System.IO;
using System.Collections;
using UnityTools = TNet.UnityTools;

public class ServerConnection : MonoBehaviour {

	private bool isConnectedToServer = false;
	private bool isEmptyServerList = true;

	public static ServerConnection Instance;

	public delegate void ClientEvent(string value);
	public static event ClientEvent ServerStatus = delegate{};
	public static event ClientEvent ConnectionStatus = delegate{};
	public static event ClientEvent ImageSendingStatus = delegate{};
	public static event ClientEvent TxtSendingStatus = delegate{};

	public Button connectButton;
	public Button disconnectButton;
	public Button sendPNGButton;
	public Button sendTxtButton;

	public static int SubmittedImage = 0;

	void Awake()
	{
		Instance = this;
		UpdateConnectStatus ();
	}

	private void UpdateConnectStatus()
	{
		connectButton.interactable = (!isConnectedToServer && !isEmptyServerList); 
		disconnectButton.interactable = (isConnectedToServer && !isEmptyServerList);
		sendPNGButton.interactable = (isConnectedToServer && !isEmptyServerList);
		sendTxtButton.interactable = (isConnectedToServer && !isEmptyServerList);
	}

	void Start () {
		if (Application.isPlaying)
		{
			Tools.ResolveIPs(null);
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			TNManager.StartUDP(Random.Range(10000, 50000));
		}
	}

	void Update () 
	{
		checkServerList ();
		if (isEmptyServerList) return;
	}

	private void checkServerList()
	{
		if ((TNLobbyClient.knownServers.list.Count == 0) != isEmptyServerList)
		{
			isEmptyServerList = (TNLobbyClient.knownServers.list.Count == 0);
			if (isEmptyServerList) 
			{
				isConnectedToServer = false;
				ConnectionStatus(isConnectedToServer.ToString());
			}
			UpdateConnectStatus();
		}
		ServerStatus (isEmptyServerList.ToString ());
	}

	public void ConnectToServer()
	{
		if (isEmptyServerList || isConnectedToServer) return;
		Debug.Log ("Try connect to server");
		isConnectedToServer = true;
		ConnectionStatus(isConnectedToServer.ToString());
		UpdateConnectStatus ();
		ServerList.Entry ent = TNLobbyClient.knownServers.list [0];
		TNManager.Connect(ent.internalAddress, ent.internalAddress);

	}

	public void DisconnectFromServer()
	{
		isConnectedToServer = false;
		ConnectionStatus(isConnectedToServer.ToString());
		UpdateConnectStatus ();
		//TNManager.LeaveChannel ();
		TNManager.Disconnect ();
	}

	public void OnNetworkConnect (bool success, string message) 
	{
		Debug.Log ("Connected to server: " + success);
		if (success) {
			ConnectToChannel ();
			UpdateConnectStatus ();
		} else {
			DisconnectFromServer();
		}
	}
	
	private void ConnectToChannel()
	{
		TNManager.JoinChannel(9533, "ChannelSceneTabletEugene");
	}

	public void SendRFC (string method, string path, bool isImage)
	{
		Debug.Log ("Try to send RFC "+method+". "+TNManager.players);
		TNObject tno = GetComponent<TNObject> ();
		tno.Send (method, Target.Host, path, isImage);
	}

	public static void ImageAlreadySent()
	{
		SubmittedImage++;
		ImageSendingStatus (SubmittedImage.ToString ());
	}

	void OnApplicationQuit() {
		TNServerInstance.Stop();
	}
}
