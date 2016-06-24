using UnityEngine;
using TNet;
using System.Net;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityTools = TNet.UnityTools;

public class DesktopServerSide : MonoBehaviour
{
    public static DesktopServerSide instance;

    public int serverTcpPort = 5127;
    private bool isEmptyServerList = true;
    public bool isConnectedToServer = false;
    private bool isConnectedToChannel = false;

    public SpriteButton startButton;
    public GameObject greyImage;
    public SpriteButton[] interactableWhenConnected;
    public MonoBehaviour[] enableBehaviorWhenConnected;
    public GameObject[] enableObjectWhenConnected;

    public delegate void ServerEvent(string value);
    public static event ServerEvent UpdateClientCount = delegate { };

    void Start()
    {
        instance = this;
        if (Application.isPlaying)
        {
            Tools.ResolveIPs(null);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            TNManager.StartUDP(Random.Range(10000, 50000));
        }
        ChangeButtonState(false);
    }

    private void ChangeButtonState(bool _startServer)
    {
        startButton.interactable = !_startServer;
        GetComponent<BoxCollider>().enabled = !isConnectedToChannel || !_startServer;
        greyImage.SetActive(!isConnectedToChannel || !_startServer);
        startButton.gameObject.SetActive(!isConnectedToChannel || !_startServer);
        for (int i = 0; i < interactableWhenConnected.Length; i++)
        {
            interactableWhenConnected[i].interactable = isConnectedToChannel && _startServer;
        }
        for (int i = 0; i < enableBehaviorWhenConnected.Length; i++)
        {
            enableBehaviorWhenConnected[i].enabled = isConnectedToChannel && _startServer;
        }
        for (int i = 0; i < enableObjectWhenConnected.Length; i++)
        {
            enableObjectWhenConnected[i].SetActive(isConnectedToChannel && _startServer);
        }
    }

    public void StartServer()
    {
        ChangeButtonState(true);
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

        Debug.Log("Server started");
    }

    void Update()
    {
        if (startButton.pressed)
        {
            StartServer();
        }
        checkServerList();

        if (Input.GetKeyDown(KeyCode.F))
        {
            OnNetworkJoinChannel(true, "forcing start");
        }

        if (isEmptyServerList) return;
        if (!isConnectedToServer) ConnectToServer();
    }

    private void checkServerList()
    {
        isEmptyServerList = (TNLobbyClient.knownServers.list.Count == 0);
    }

    public void StopServer()
    {
        Debug.Log("Stop server");
        ChangeButtonState(false);
        TNServerInstance.Stop();
    }

    private void ConnectToServer()
    {
        Debug.Log("Try connect to server");
        isConnectedToServer = true;
        ServerList.Entry ent = TNLobbyClient.knownServers.list[0];
        TNManager.Connect(ent.internalAddress, ent.internalAddress);
    }

    public void OnNetworkConnect(bool success, string message)
    {
        Debug.Log("Connected to server: " + success);
        if (success)
            ConnectToChannel();
    }

    private void ConnectToChannel()
    {
        TNManager.JoinChannel(9533, "Empty");
    }

    void OnNetworkJoinChannel(bool success, string message)
    {
        if (!isConnectedToChannel)
        {
            Debug.Log("Connected to channel: " + success);
            SaveLoadManager.TempLoad();
        }
        isConnectedToChannel = true;
        ChangeButtonState(true);
    }

    void OnNetworkPlayerJoin(Player player)
    {
        int noOfClients = 0;
        for (int i = 0; i < TNManager.players.Count; i++)
        {
            Debug.Log(TNManager.players[i]);
            if (TNManager.players[i] != null)
            {
                noOfClients++;
            }
        }
        Debug.Log("Client connected. Total: " + noOfClients);
        UpdateClientCount(noOfClients.ToString());
    }

    void OnNetworkPlayerLeave(Player player)
    {
        int noOfClients = 0;
        for (int i = 0; i < TNManager.players.Count; i++)
        {
            Debug.Log(TNManager.players[i]);
            if (TNManager.players[i] != null)
            {
                noOfClients++;
            }
        }
        Debug.Log("Client disconnected. Total: " + noOfClients);
        UpdateClientCount(noOfClients.ToString());
    }

    void OnApplicationQuit()
    {
        TNServerInstance.Stop();
    }
}
