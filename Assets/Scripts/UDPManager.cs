using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.IO;

public class UDPManager : MonoBehaviour
{
    public static UDPManager instance;
    public const string MESSAGE_RECEIVED = "MESSAGE_RECEIVED";
    public const string WAITING_FOR_ID = "WAITING_FOR_ID";
    public const string I_AM_ALIVE = "I_AM_ALIVE";
    public const string SET_ID = "SET_ID";

    public static bool isMain = false;
    public static int sendPort;
    public static int listenPort;
    public const int PortGetId = 8051;
    public static int keepThisThreadIdAlive = 0;
    private UdpClient listenClient;
    Thread thread;
    public static string dataString = "N";
    public static string logString = "";
    public static string loadString = "";
    private float timer;
    private const float MessagePeriod = 1f;

    public bool initAsMain;
    private bool sendDataFlag;
    private string dataToSend;
    private float maxMessagesTimer;
    private int messageCount;
    public bool initiliazed;

    //only main instance uses these:
    private List<int> aliveInstances;
    private List<float> aliveInstanceTimers;
    private int specificInstanceIdToSend;

    public Process process;
    private string ExtraInstancePath = "/ExtraInstance/Qnowledge.exe";

    //only extra instances use these:
    public static int instanceId;

    void Start()
    {
        instance = this;
        isMain = initAsMain;
        if (isMain)
        {
            sendPort = 8053;
            listenPort = 8052;
        }
        else
        {
            sendPort = 8052;
            listenPort = PortGetId;

            keepThisThreadIdAlive = 1;
            thread = new Thread(DoWork);
            thread.Start();
            aliveInstances = new List<int>();
            aliveInstanceTimers = new List<float>();
            initiliazed = true;
        }
    }

    public void StartMain()
    {

        keepThisThreadIdAlive = 1;
        thread = new Thread(DoWork);
        thread.Start();
        aliveInstances = new List<int>();
        aliveInstanceTimers = new List<float>();
        initiliazed = true;
    }

    public void StartExtraInstance()
    {
        if (File.Exists(Application.dataPath + ExtraInstancePath))
        {
            process = new Process();
            process.StartInfo.FileName = (Application.dataPath + ExtraInstancePath);
            process.Start();
        }
    }

    void DoWork()
    {
        listenClient = new UdpClient(listenPort);
        var groupEP = new IPEndPoint(IPAddress.Any, listenPort);
        string received_data;
        byte[] receive_byte_array;
        var i = 0;
        while (keepThisThreadIdAlive > 0)
        {
            try
            {
                receive_byte_array = listenClient.Receive(ref groupEP);
                received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                dataString = received_data;
                handleReceivedData();
                i++;
                if (i > 10000)
                    i = 0;
            }
            catch
            {
            }
            Thread.Sleep(0);
        }
    }

    void Update()
    {
        maxMessagesTimer += Time.deltaTime;
        if (maxMessagesTimer >= 2f)
        {
            maxMessagesTimer = 0f;
            messageCount = 0;
        }

        bool messagePeriod = false;
        timer += Time.deltaTime;
        if (timer > MessagePeriod)
        {
            timer = 0f;
            messagePeriod = true;
        }

        if (isMain)
        {
            if (initiliazed)
            {
                for (int i = 0; i < aliveInstanceTimers.Count; i++)
                {
                    if (aliveInstances[i] != -1)
                    {
                        aliveInstanceTimers[i] += Time.deltaTime;
                        if (aliveInstanceTimers[i] > 5f)
                        {
                            aliveInstances[i] = -1;
                        }
                    }
                }
            }
            //send state of the program when an extra instance sent this instance state of the program or a change happened on this instance
        }
        else
        {
            if (messagePeriod)
            {
                if (keepThisThreadIdAlive == 1)
                {
                    sendMessage(WAITING_FOR_ID);
                }
                else if (keepThisThreadIdAlive == 2)
                {
                    sendMessage(instanceId + I_AM_ALIVE);
                }
            }
            //send state of the program when a change happened on this instance
        }
        if (sendDataFlag)
        {
            sendDataFlag = false;
            sendMessage(dataToSend, true);
        }

        if (logString != "")
        {
            UnityEngine.Debug.Log(logString);
            logString = "";
        }

        if (loadString != "")
        {
            if (SaveLoadManager.lastSaveString != loadString)
            {
                SaveLoadManager.Reset();
                SaveLoadManager.LoadedText = loadString;
                SaveLoadManager.Load();
                if (isMain)
                {
                    send(dataString, true);
                }
            }
            loadString = "";
        }

        if (specificInstanceIdToSend != 0)
        {

            specificInstanceIdToSend = 0;
        }
    }

    //void OnGUI()
    //{
    //    if (isMain && Input.GetKeyDown(KeyCode.Q))
    //    {
    //        sendMessage("testing", true);
    //    }
    //    GUILayout.Label("sendPort" + sendPort + " listenPort" + listenPort + "\nLast received message:\n" + dataString);
    //}

    public void sendMessage(string _message, bool highPriority = false)
    {
        //if (messageCount < 2)
        //{
        //    if (highPriority || messageCount == 0)
        //    {
                if (isMain)
                {
                    StartCoroutine(sendMessageFromMain(_message, sendPort - 1));
                }
                else
                {
                    sendMessage(_message, sendPort);
                }
        //    }
        //}
        //messageCount++;
    }

    public IEnumerator sendMessageFromMain(string _message, int port)
    {
        for (int i = 0; i < aliveInstances.Count; i++)
        {
            if (aliveInstances[i] != -1)
            {
                sendMessage(_message, port + aliveInstances[i]);
                UnityEngine.Debug.Log((port + aliveInstances[i]));
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void sendMessage(string _message, int port)
    {
        Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
        ProtocolType.Udp);
        var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(o => o.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();
        IPAddress send_to_address = ip;
        IPEndPoint sending_end_point = new IPEndPoint(send_to_address, port);
        byte[] send_buffer = Encoding.ASCII.GetBytes(_message);
        try
        {
            sending_socket.SendTo(send_buffer, sending_end_point);
        }
        catch { }
    }

    void OnApplicationQuit()
    {
        keepThisThreadIdAlive = 0;
        if (thread != null)
        {
            thread.Abort();
        }
        if (listenClient != null)
        {
            listenClient.Close();
            listenClient = null;
        }
        if (process != null)
        {
            process.Kill();
        }
    }

    void handleReceivedData()
    {
        if (dataString.Length > "noOfStickiesInMain".Length && dataString.Substring(0, "noOfStickiesInMain".Length) == "noOfStickiesInMain")
        {
            //will be the state of the program type of messages
            loadString = dataString;
        }
        else
        {
            if (isMain)
            {
                if (dataString.Contains(MESSAGE_RECEIVED))
                {
                    int id = int.Parse("" + dataString[0]);

                    
                }
                else if (dataString.Contains(I_AM_ALIVE))
                {
                    int id = int.Parse("" + dataString[0]);
                    if (!aliveInstances.Contains(id))
                    {
                        aliveInstances.Add(id);
                        aliveInstanceTimers.Add(0f);
                    }
                }
                else if (dataString.Contains(WAITING_FOR_ID))
                {
                    if (getNewId() == 1)
                    {
                        sendMessage("" + getNewId() + SET_ID, PortGetId);
                    }
                }
            }
            else
            {
                if (dataString.Contains(SET_ID))
                {
                    int id = int.Parse("" + dataString[0]);
                    instanceId = id;
                    sendMessage("" + instanceId + MESSAGE_RECEIVED, true);
                    keepThisThreadIdAlive = 2;
                    listenClient.Close();
                    listenPort = 8052 + instanceId;
                    listenClient = new UdpClient(listenPort);
                }
            }
        }
    }

    public int getNewId()
    {
        int toReturn = 1;
        bool loop = true;
        while (loop)
        {
            loop = false;
            for (int i = 0; i < aliveInstances.Count; i++)
            {
                if (toReturn == aliveInstances[i])
                {
                    toReturn++;
                    loop = true;
                }
            }
        }
        return toReturn;
    }

    public static void send(string _message, bool highPriority)
    {
        instance.sendDataFlag = true;
        instance.dataToSend = _message;
    }
}
