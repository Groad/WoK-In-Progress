using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickySender : MonoBehaviour
{
    public static StickySender instance;
	public static List<GameObject> MainPanelObjects = new List<GameObject>();
	public GameObject PlayerPrefab;
	public GameObject stickyNote;
	public static List<GameObject> stickyNotes = new List<GameObject> ();
	public static GameObject currentNote;
	string ip = "10.0.0.2";
	int port = 64000;
	bool connected;
	bool noteUp = false;
	string text = "";
	
	void Awake()
	{
		Screen.orientation = ScreenOrientation.Portrait;
        Network.InitializeServer(10, port, false);
        instance = this;
	}
	
	void OnEnable()
	{
	}
	public void CreatePlayer(bool _host)
	{
		connected = true;
		var g = (GameObject)Network.Instantiate (PlayerPrefab, transform.position, Quaternion.identity, 0);	
		if (Network.isServer)
		{
			g.GetComponent<NetworkView> ().group = 0;
			g.name = "Host";
		}
		else
		{
			g.GetComponent<NetworkView> ().group = 1;
			g.name = "Tablet";
			g.GetComponent<Camera> ().enabled = true;
		}
	}
	
	void OnDisconnectedFromServer()
	{
		connected = false;
	}
	
	void OnPlayerDisconnected(NetworkPlayer pl)
	{
		Network.DestroyPlayerObjects (pl);
	}
	
	void OnConnectedToServer()
	{
		CreatePlayer (false);
	}
	
	void OnServerInitialized()
	{
		CreatePlayer (true);
	}

	public GameObject AddSticky (Vector3 _pos)
	{
		var newNote = Instantiate(stickyNote, _pos, Quaternion.identity) as GameObject;
        newNote.transform.SetParent(SaveLoadManager.instance.transform);
		newNote.name = "";
		currentNote = newNote;
        ftlGatherer.ActiveNotes = new List<GameObject>();
        ftlGatherer.ActiveNotes.Add(currentNote);
		//newNote.GetComponent<StickyScript> ().stickySender = this;
		noteUp = true;
		if (Network.isServer)
		{
			MainPanelObjects.Add(newNote);
		}
		return newNote;
	}

	void OnGUI()
	{
		if (Input.GetKey (KeyCode.Escape))
			Application.Quit ();
        return;
		if (!connected) 
		{
            ip = GUI.TextField(new Rect(70, 70, 140, 40), ip);
            if (GUI.Button(new Rect(70, 120, 140, 40), "connect"))
            {
                Network.Connect(ip, port);
            }
            if (GameObject.Find("mainStickyReceiver") && GameObject.Find("mainStickyReceiver").GetComponent<MainStickyReceiver>().isMainPanel == true)
            {
                if (GUI.Button(new Rect(70, 170, 140, 40), "host"))
                {
                    Network.InitializeServer(10, port, false);
                }
            }
		} 
		else 
		{
			if (GUI.Button (new Rect (70, 70, 140, 40), "disconnect")) 
			{
				OnPlayerDisconnected(Network.player);
				Network.Disconnect ();
			}
			if (Network.isServer || (!Network.isServer && !noteUp))
			{
				if (GUI.Button(new Rect(70, 120, 200, 32), "new note"))
				{
					var pos = TransformToMainPanel(new Vector3 ( Screen.width / 2, Screen.height / 2, 10));
					pos.z = -20f;
					AddSticky(pos);
				}
			}
            if (!Network.isServer)
            {
                if (currentNote != null && GUI.Button(new Rect(70, 120, 200, 32), "send"))
                {
                    MainStickyReceiver.instance.RPC("ReceiveSticky", RPCMode.AllBuffered, currentNote.name);
                    Destroy(currentNote);
                    //Destroy(sendNote);
                    noteUp = false;
                }
            }
		}
	}

    public Vector3 TransformToMainPanel(Vector3 _screenPos)
    {
        var w = (float)GetComponent<Camera>().targetTexture.width;
        var h = (float)GetComponent<Camera>().targetTexture.height;
        var wRatio = w / Screen.width;
        var hRatio = h / Screen.height;
        _screenPos.x *= wRatio;
        _screenPos.y *= hRatio;
        _screenPos = GetComponent<Camera>().ScreenToWorldPoint(_screenPos);
        return _screenPos;
    }

    public Vector3 ReverseTransform(Vector3 _worldPos)
    {
        _worldPos = GetComponent<Camera>().WorldToScreenPoint(_worldPos);
        var w = (float)GetComponent<Camera>().targetTexture.width;
        var h = (float)GetComponent<Camera>().targetTexture.height;
        var wRatio = w / Screen.width;
        var hRatio = h / Screen.height;
        _worldPos.x /= wRatio * 1.26f;//magic number. don't question its wisdom.

        _worldPos.y /= hRatio;
        
        return _worldPos;
    }

	public enum ButtonClicked {Left,Right,Middle,None}

	public delegate void MainPanelHitEventHandler (GameObject _hit, ButtonClicked _button);

	public event MainPanelHitEventHandler stickyHit;
	public event MainPanelHitEventHandler lineHit;

	public void StickyHit (GameObject _hit, ButtonClicked _button)
	{
		if (stickyHit != null)
		{
			foreach (var popup in GameObject.FindGameObjectsWithTag("Popup"))
			{
				Destroy (popup);
			}
			if (_hit != null)
			stickyHit (_hit, _button);
		}
	}

	public void LineHit (GameObject _hit, ButtonClicked _button)
	{
		if (lineHit != null)
		{
			foreach (var popup in GameObject.FindGameObjectsWithTag("Popup"))
			{
				Destroy (popup);
			}
			lineHit (_hit, _button);
		}
	}

	void Update()
	{
		var pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit) && hit.transform.tag == "ProjectionSurface")
		{
			ray = GetComponent<Camera>().ViewportPointToRay(hit.textureCoord);
            pos.z = 10;
            //pos = TransformToMainPanel(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            pos = ftlGatherer.TransformToMainPanel(pos);
			if (Physics.Raycast(ray, out hit))
			{
				if (Input.GetMouseButtonDown(0))
				{
					switch (hit.transform.tag)
					{
					case ("StickyNote"):
						StickyHit(hit.transform.gameObject, ButtonClicked.Left);
						break;
					case ("Line"):
						if (hit.transform != null)
							LineHit(hit.transform.gameObject, ButtonClicked.Left);
						break;
					case ("Handle"):
						if (hit.transform != null)
							LineHit(hit.transform.gameObject, ButtonClicked.Left);
						break;
					default:
						break;
					}
				}
				if (Input.GetMouseButtonDown(1))
				{
					switch (hit.transform.tag)
					{
					case ("StickyNote"):
						if (hit.transform.gameObject != null)
						StickyHit(hit.transform.gameObject, ButtonClicked.Right);
						break;
					case ("Line"):
						if (hit.transform != null)
						LineHit(hit.transform.gameObject, ButtonClicked.Right);
						break;
					case ("Handle"):
						if (hit.transform != null)
						LineHit(hit.transform.gameObject, ButtonClicked.Right);
						break;
					default:
						break;
					}
				}
			}
		}
	}
}
