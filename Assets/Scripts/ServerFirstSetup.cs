using UnityEngine;
using TNet;
using System.Net;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityTools = TNet.UnityTools;

public class ServerFirstSetup : MonoBehaviour {

	public int serverTcpPort = 5127;
	private bool isEmptyServerList = true;
	private bool isConnectedToServer = false;
	private bool isConnectedToChannel = false;

	public Button startButton;
	public Button stopButton;

	public delegate void ServerEvent(string value);
	public static event ServerEvent UpdateClientCount = delegate{};

	void Start ()
	{
		if (Application.isPlaying)
		{
			Tools.ResolveIPs(null);
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			TNManager.StartUDP(Random.Range(10000, 50000));
		}
		ChangeButtonState (false);
	}

	private void ChangeButtonState(bool _startServer)
	{
		startButton.interactable = !_startServer;
		stopButton.interactable = _startServer;
	}

	public void StartServer()
	{
		ChangeButtonState (true);
		int udpPort = Random.Range(10000, 40000);
		TNLobbyClient lobby = GetComponent<TNLobbyClient>();
		
		if (lobby == null)
		{
			TNServerInstance.Start(serverTcpPort, udpPort, "server.dat");
		}
		else
		{
			TNServerInstance.Type type = (lobby is TNUdpLobbyClient) ?
				TNServerInstance.Type.Udp : TNServerInstance.Type.Tcp;
			TNServerInstance.Start(serverTcpPort, udpPort, lobby.remotePort, "server.dat", type);
		}


		Debug.Log ("Server started");
	}

	void Update () 
	{
		checkServerList ();
		if (isEmptyServerList)	return;
		if (!isConnectedToServer) ConnectToServer();
	}

	private void checkServerList()
	{
		isEmptyServerList = (TNLobbyClient.knownServers.list.Count == 0);
	}

	public void StopServer()
	{
		Debug.Log ("Stop server");
		ChangeButtonState (false);
		TNServerInstance.Stop ();
	}

	private void ConnectToServer()
	{
		Debug.Log ("Try connect to server");
		isConnectedToServer = true;
		ServerList.Entry ent = TNLobbyClient.knownServers.list [0];
		TNManager.Connect(ent.internalAddress, ent.internalAddress);
	}

	public void OnNetworkConnect (bool success, string message) 
	{
		Debug.Log ("Connected to server: " + success);
		if (success)
			ConnectToChannel ();
	}

	private void ConnectToChannel()
	{
        TNManager.JoinChannel(9533, "ChannelScenePCEugene");
	}

	void OnNetworkJoinChannel (bool success, string message)
	{
		Debug.Log ("Connected to channel: " + success);
	}

	void OnNetworkPlayerJoin (Player player) 
	{
		Debug.Log ("Client connected. Total: " + TNManager.players.Count);
		UpdateClientCount (TNManager.players.Count.ToString ());
	}

	void OnNetworkPlayerLeave (Player player) 
	{
		Debug.Log ("Client disconnected. Total: " + TNManager.players.Count);
		UpdateClientCount (TNManager.players.Count.ToString ());
	}

	 

	void OnApplicationQuit() {
		TNServerInstance.Stop();
	}
}
