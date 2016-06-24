using UnityEngine;
using UnityEngine.UI;
using TNet;
using System.IO;
using System.Collections;
using UnityTools = TNet.UnityTools;

public class TabletServerSide : MonoBehaviour {
    private bool isConnectedToServer = false;
    private bool isEmptyServerList = true;

    public static TabletServerSide Instance;

    public delegate void ClientEvent(string value);
    public static event ClientEvent ServerStatus = delegate { };
    public static event ClientEvent ConnectionStatus = delegate { };
    public static event ClientEvent ImageSendingStatus = delegate { };
    public static event ClientEvent TxtSendingStatus = delegate { };

    public SpriteButton connectButton;
    public SpriteButton turnKeyboardOnButton;
    public GameObject greyImage;
    //public Button disconnectButton;
    //public Button sendPNGButton;
    //public Button sendTxtButton;

    public static TouchScreenKeyboard touchScreenKeyboard;

    public static Collider inputKiller;
    public static int SubmittedImage = 0;
    private static bool _fileBrowserOpen;
    public static bool fileBrowserOpen
    {
        get
        {
            return _fileBrowserOpen;
        }
        set
        {
            _fileBrowserOpen = value;
            if (inputKiller != null)
            {
                inputKiller.enabled = value;
            }
            if (Instance != null)
            {
                Instance.UpdateConnectStatus();
            }
        }
    }

    void Awake()
    {
        Instance = this;
    }

    private void UpdateConnectStatus()
    {
        bool touchScreenKeyboardVisible = false;
#if UNITY_ANDROID
        touchScreenKeyboardVisible = TouchScreenKeyboard.visible;
#endif
        connectButton.interactable = (!isConnectedToServer && !isEmptyServerList);
        connectButton.gameObject.SetActive(!isConnectedToServer);
        greyImage.gameObject.SetActive(!isConnectedToServer);
        if (TabletKeyboardController.instance != null)
        {
            TabletKeyboardController.instance.gameObject.SetActive(isConnectedToServer && !fileBrowserOpen && !TouchScreenKeyboard.isSupported);
        }
        turnKeyboardOnButton.gameObject.SetActive(isConnectedToServer && !fileBrowserOpen && TouchScreenKeyboard.isSupported && !touchScreenKeyboardVisible);
        GetComponent<BoxCollider>().enabled = !isConnectedToServer;
        //disconnectButton.interactable = (isConnectedToServer && !isEmptyServerList);
        //sendPNGButton.interactable = (isConnectedToServer && !isEmptyServerList);
        //sendTxtButton.interactable = (isConnectedToServer && !isEmptyServerList);
    }

    void Start()
    {
        UpdateConnectStatus();
        if (Application.isPlaying)
        {
            Tools.ResolveIPs(null);
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            TNManager.StartUDP(Random.Range(10000, 50000));
        }
    }

    void Update()
    {
        //if (isConnectedToServer && !fileBrowserOpen && TouchScreenKeyboard.isSupported)
        //{
        //    TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, true, true, false);
        //}

        if (turnKeyboardOnButton.pressed)
        {
            string text = "";
            if (NewStickyButton.thisSticky != null)
            {
                text = NewStickyButton.thisSticky.name;
            }
            touchScreenKeyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.Default, false, true, false);
        }
        //if (touchScreenKeyboard != null && touchScreenKeyboard.done)
        //{
        //    if (NewStickyButton.thisSticky != null)
        //    {
        //        NewStickyButton.thisSticky.name = touchScreenKeyboard.text;
        //    }
        //}

        if (connectButton.pressed)
        {
            ConnectToServer();
        }
        checkServerList();
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
        ServerStatus(isEmptyServerList.ToString());
    }

    public void ConnectToServer()
    {
        if (isEmptyServerList || isConnectedToServer) return;
        Debug.Log("Try connect to server");
        isConnectedToServer = true;
        ConnectionStatus(isConnectedToServer.ToString());
        UpdateConnectStatus();
        ServerList.Entry ent = TNLobbyClient.knownServers.list[0];
        TNManager.Connect(ent.internalAddress, ent.internalAddress);
    }

    public void DisconnectFromServer()
    {
        isConnectedToServer = false;
        ConnectionStatus(isConnectedToServer.ToString());
        UpdateConnectStatus();
        //TNManager.LeaveChannel ();
        TNManager.Disconnect();
    }

    public void OnNetworkConnect(bool success, string message)
    {
        Debug.Log("Connected to server: " + success);
        if (success)
        {
            ConnectToChannel();
            UpdateConnectStatus();
        }
        else
        {
            DisconnectFromServer();
        }
    }

    private void ConnectToChannel()
    {
        TNManager.JoinChannel(9533, "Empty");
    }

    public void SendRFC(string method, string path, bool isImage)
    {
        Debug.Log("Try to send RFC " + method + ". " + TNManager.players);
        TNObject tno = GetComponent<TNObject>();
        tno.Send(method, Target.Host, path, isImage);
    }

    public static void ImageAlreadySent()
    {
        SubmittedImage++;
        ImageSendingStatus(SubmittedImage.ToString());
    }

    void OnApplicationQuit()
    {
        TNServerInstance.Stop();
    }
}
