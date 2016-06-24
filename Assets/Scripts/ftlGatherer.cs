using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript;
using System.Threading;

public class ftlGatherer : MonoBehaviour 
{
    public static ftlGatherer instance;
	public static Dictionary <int, ITouch> ftlTouches = new Dictionary<int, ITouch>();
	public GameObject noteObject;
	public GameObject TouchObject;
	public static Vector3 startPos = new Vector3 ();
	private GameObject activeNote;
    private static List<GameObject> _ActiveNotes;
    public static List<GameObject> ActiveNotes
    {
        get
        {
            return _ActiveNotes;
        }
        set
        {
            //Debug.Log("ActiveNotes set to " + value);
            _ActiveNotes = value;
        }
    }
	public static Vector3 offset;
	public static GameObject touchObject;
	public static float timeButtonHit = 0;
	public bool MainPanelGatherer = false;
	public GameObject ProjectionSurface;
    private Vector3 mousePosLast;
    private List<GameObject> draggingHandles;
    public static bool pressedEditorButton;//to skip touchBegin

    private bool boxSelecting;
    private Vector3 mousePos;
    private Vector3 boxSelectBeginPos;
    private Vector3 boxSelectEndPos;
    private List<LineRenderer> lineRenderers;
    public static bool draggedSomeStickies;

    public StickyScaler stickyScaler;

    public GameObject mainPanel;
    public float mainPanelHeight;
    public Camera mainCamera;
    public GameObject mapPanel;
    public float mapPanelHeight;
    public Camera mapCamera;
    private bool mapCameraControl;
    private static Vector3 mainPanelPos = new Vector3(-1.84f, 0f, 0f);
    private Vector3 mapPanelLastPos;

    void Awake()
    {
        instance = this;
    }

	void OnEnable () 
	{
		startPos = new Vector3 (0f, 3.79f, 0f);
        lineRenderers = new List<LineRenderer>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<LineRenderer>() != null)
            {
                LineRenderer lr = child.GetComponent<LineRenderer>();
                lineRenderers.Add(lr);
                lr.enabled = false;
                lr.material = new Material(Shader.Find("Particles/Alpha Blended"));
            }
        }

		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan += touchesBeganHandler;
			TouchManager.Instance.TouchesEnded += touchesEndedHandler;
			TouchManager.Instance.TouchesMoved += touchesMovedHandler;
			TouchManager.Instance.TouchesCancelled += touchesCancelledHandler;
		}
	}

	void OnDisable()
	{
		if (TouchManager.Instance != null)
		{
			TouchManager.Instance.TouchesBegan -= touchesBeganHandler;
			TouchManager.Instance.TouchesEnded -= touchesEndedHandler;
			TouchManager.Instance.TouchesMoved -= touchesMovedHandler;
			TouchManager.Instance.TouchesCancelled -= touchesCancelledHandler;
		}
	}

	void OnGUI()
	{
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();
	}

	void Update ()
    {
        //map panel camera control
        if (Input.GetMouseButtonDown(0))
        {
            mapCameraControl = false;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == mapPanel)
                {
                    mapPanelLastPos = TransformToMapPanel(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    //mainCamera.transform.position = new Vector3(newMainPanelCamPos.x, newMainPanelCamPos.y, mainCamera.transform.position.z);
                    mapCameraControl = true;
                }
            }
        }
        if (Input.GetMouseButton(0) && mapCameraControl)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == mapPanel)
                {
                    Vector3 newMainPanelCamPos = TransformToMapPanel(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    mainCamera.transform.position += newMainPanelCamPos - mapPanelLastPos;
                    mapPanelLastPos = newMainPanelCamPos;
                    //mainCamera.transform.position = new Vector3(newMainPanelCamPos.x, newMainPanelCamPos.y, mainCamera.transform.position.z);
                }
            }
        }
        //boxSelect
        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == ProjectionSurface)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                boxSelectBeginPos = TransformToMainPanel(mousePos);
                boxSelectEndPos = boxSelectBeginPos;
                boxSelecting = true;

                for (int i = 0; i < lineRenderers.Count; i++)
                {
                    lineRenderers[i].enabled = true;
                }
                locateLineRenderers();
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (boxSelecting)
            {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                boxSelectEndPos = TransformToMainPanel(mousePos);
                locateLineRenderers();
                boxSelectStickies(new Vector3(Mathf.Min(boxSelectBeginPos.x, boxSelectEndPos.x), Mathf.Min(boxSelectBeginPos.y, boxSelectEndPos.y), 0f), new Vector3(Mathf.Max(boxSelectBeginPos.x, boxSelectEndPos.x), Mathf.Max(boxSelectBeginPos.y, boxSelectEndPos.y), 0f));
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            boxSelecting = false;
            for (int i = 0; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }
	}

    private void locateLineRenderers()
    {
        float lineWidth = GetComponent<Camera>().orthographicSize * 0.02f;
        float lineWidthHalf = lineWidth * 0.5f;
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].SetWidth(lineWidth, lineWidth);
        }
        boxSelectBeginPos.z = -12f;
        boxSelectEndPos.z = -12f;
        lineRenderers[0].SetPosition(0, boxSelectBeginPos + Vector3.up * lineWidthHalf);
        lineRenderers[0].SetPosition(1, new Vector3(boxSelectBeginPos.x, boxSelectEndPos.y, boxSelectBeginPos.z) - Vector3.up * lineWidthHalf);
        lineRenderers[1].SetPosition(0, boxSelectBeginPos + Vector3.right * lineWidthHalf);
        lineRenderers[1].SetPosition(1, new Vector3(boxSelectEndPos.x, boxSelectBeginPos.y, boxSelectBeginPos.z) - Vector3.right * lineWidthHalf);
        lineRenderers[2].SetPosition(0, new Vector3(boxSelectEndPos.x, boxSelectBeginPos.y, boxSelectEndPos.z) + Vector3.up * lineWidthHalf);
        lineRenderers[2].SetPosition(1, boxSelectEndPos - Vector3.up * lineWidthHalf);
        lineRenderers[3].SetPosition(0, new Vector3(boxSelectBeginPos.x, boxSelectEndPos.y, boxSelectEndPos.z) + Vector3.right * lineWidthHalf);
        lineRenderers[3].SetPosition(1, boxSelectEndPos - Vector3.right * lineWidthHalf);
    }

    private void boxSelectStickies(Vector3 begin, Vector3 end)
    {
        ActiveNotes = new List<GameObject>();
        foreach (Transform child in SaveLoadManager.instance.transform)
        {
            if (child.position.x >= begin.x && child.position.x <= end.x && child.position.y >= begin.y && child.position.y <= end.y)
            {
                ActiveNotes.Add(child.gameObject);
            }
        }

        if (ActiveNotes.Count == 0)
        {
            //check for handles (pivots)
            foreach (Transform child in SaveLoadManager.instance.handles)
            {
                if (child.position.x >= begin.x && child.position.x <= end.x && child.position.y >= begin.y && child.position.y <= end.y)
                {
                    ActiveNotes.Add(child.gameObject);
                }
            }
        }
    }

    public static bool isEqualVecs(Vector3 v0, Vector3 v1)
    {
        return v0.x == v1.x && v0.y == v1.y && v0.z == v1.z;
    }

	public static void updateTouch(ITouch _touch)
	{
		if (ftlTouches.ContainsKey(_touch.Id))
		ftlTouches[_touch.Id] = _touch;
	}

    public static bool isAnActiveNote(GameObject note, List<GameObject> tempActiveNotes = null)
    {
        if (tempActiveNotes == null)
        {
            if (ActiveNotes != null)
            {
                for (int i = 0; i < ActiveNotes.Count; i++)
                {
                    if (ActiveNotes[i] == note)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < tempActiveNotes.Count; i++)
            {
                if (tempActiveNotes[i] == note)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool allActiveNotesAreNotes()
    {
        if (ActiveNotes != null)
        {
            for (int i = 0; i < ActiveNotes.Count; i++)
            {
                if (ActiveNotes[i] != null && ActiveNotes[i].tag != "StickyNote")
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static Vector3 TransformWithCameraPosition(Vector3 worldPos, Vector3 planePos, float height, Camera cam)
    {
        Vector3 deltaPlanePos = worldPos - planePos;
        deltaPlanePos /= height;
        return cam.transform.position + cam.orthographicSize * (cam.transform.right * deltaPlanePos.x + cam.transform.up * deltaPlanePos.y);
    }

    public static Vector3 TransformToMainPanel(Vector3 worldPos)
    {
        return TransformWithCameraPosition(worldPos, instance.mainPanel.transform.position, instance.mainPanelHeight, instance.mainCamera);
    }

    public static Vector3 TransformToMapPanel(Vector3 worldPos)
    {
        return TransformWithCameraPosition(worldPos, instance.mapPanel.transform.position, -instance.mapPanelHeight, instance.mapCamera);
    }

	#region Event handlers
	
	public void touchesBeganHandler(object sender, TouchEventArgs e)
	{
		foreach (var touch in e.Touches)
		{
			if (ftlTouches.ContainsKey(touch.Id)) return;
			ftlTouches.Add (touch.Id, touch);
			var pos = Camera.main.ScreenToWorldPoint (new Vector3 (touch.Position.x, touch.Position.y, 10));
            if (pressedEditorButton)
            {
                pressedEditorButton = false;
            }
			else if (MainPanelGatherer)
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
				if (Physics.Raycast(ray, out hit) && hit.transform.tag == "ProjectionSurface")
                {
                    stickyScaler = null;
                    draggingHandles = null;
                    List<GameObject> lastActiveNotes = ActiveNotes;
                    ActiveNotes = null;
					ray = GetComponent<Camera>().ViewportPointToRay(hit.textureCoord);
                    pos.z = 10;
                    //pos = GetComponent<StickySender>().TransformToMainPanel(new Vector3 (touch.Position.x, touch.Position.y, 10));
                    pos = TransformToMainPanel(pos);
                    mousePosLast = pos;
                    mousePosLast.z = 0f;
					if (Physics.Raycast(ray, out hit))
					{
						var hitObject = hit.transform.gameObject;
                        if (Input.GetKey(KeyCode.LeftControl))
                        {
                            Debug.Log(hit.transform.name);
                        }
						switch (hitObject.tag)
						{
                            case "StickyNote":
                                //delete handles and lines in ActiveNotes
                                if (lastActiveNotes != null)
                                {
                                    for (int i = 0; i < lastActiveNotes.Count; i++)
                                    {
                                        if (lastActiveNotes[i] != null && (lastActiveNotes[i].tag == "Handle" || lastActiveNotes[i].tag == "Line"))
                                        {
                                            lastActiveNotes[i] = null;
                                        }
                                    }
                                }
                                //add/remove the sticky 
                                activeNote = hitObject;
                                if (isAnActiveNote(hitObject, lastActiveNotes))
                                {
                                    ActiveNotes = lastActiveNotes;
                                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                                    {
                                        ActiveNotes.Remove(activeNote);
                                    }
                                }
                                else
                                {
                                    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                                    {
                                        if (lastActiveNotes == null)
                                        {
                                            ActiveNotes = new List<GameObject>();
                                        }
                                        else
                                        {
                                            ActiveNotes = lastActiveNotes;
                                        }
                                        ActiveNotes.Add(hitObject);
                                    }
                                    else
                                    {
                                        ActiveNotes = new List<GameObject>();
                                        ActiveNotes.Add(hitObject);
                                    }
                                }
                                hit.transform.gameObject.GetComponent<StickyScript>().press();
                                offset = pos - hit.transform.gameObject.transform.position;
                                offset.z = 0;
							    break;
						    case "Line":
							    activeNote = hitObject;
                                ActiveNotes = new List<GameObject>();
							    ActiveNotes.Add(hitObject);
                                break;
                            case "Handle":
                                activeNote = hitObject;
                                if (isAnActiveNote(hitObject, lastActiveNotes))
                                {
                                    ActiveNotes = lastActiveNotes;
                                }
                                else
                                {
                                    ActiveNotes = new List<GameObject>();
                                    ActiveNotes.Add(hitObject);
                                }
                                offset = pos - hit.transform.gameObject.transform.position;
                                offset.z = 0;
                                break;
                            case "StickyScaler":
                                stickyScaler = hit.transform.GetComponent<StickyScaler>();
                                stickyScaler.dragStartPos = pos;
                                offset = pos - hit.transform.gameObject.transform.position;
                                offset.z = 0;
                                ActiveNotes = lastActiveNotes;
                                break;
						    default:
							    activeNote = null;
							    break;
						}
                        ViewControl.dragging = false;
					}
                    else
                    {
                        ViewControl.dragging = true;
                    }
				}
			}
		}
	}

	public void touchesMovedHandler(object sender, TouchEventArgs e)
	{
		foreach (var touch in e.Touches)
		{
			ITouch testTouch;
			if (!ftlTouches.TryGetValue(touch.Id, out testTouch)) return;
            updateTouch(touch);
            //var pos = GetComponent<StickySender>().TransformToMainPanel(new Vector3(touch.Position.x, touch.Position.y, 10));
            var pos = TransformToMainPanel(Camera.main.ScreenToWorldPoint(new Vector3(touch.Position.x, touch.Position.y, 10)));
            if (activeNote != null && isAnActiveNote(activeNote))
            {
                pos.z = activeNote.transform.position.z;
                //pos.x = Mathf.Clamp(pos.x, -16f, 16f);
                //pos.y = Mathf.Clamp(pos.y, -24f, 24f);

                Vector3 deltaVec = pos - mousePosLast;
                mousePosLast = pos;
                deltaVec.z = 0f;
                //deltaVec.x *= 1.27f;

                //activeNote.transform.position = pos - offset;
                if (activeNote.tag == "Handle")
                {
                    //activeNote.transform.position += deltaVec;
                    for (int i = 0; i < ActiveNotes.Count; i++)
                    {
                        if (ActiveNotes[i] != null && ActiveNotes[i].tag == "Handle")
                        {
                            ActiveNotes[i].transform.position += deltaVec;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < ActiveNotes.Count; i++)
                    {
                        if (ActiveNotes[i] != null && ActiveNotes[i].tag == "StickyNote")
                        {
                            ActiveNotes[i].transform.position += deltaVec;
                        }
                    }

                    if (draggingHandles == null)
                    {
                        calculateDraggingHandles();
                    }
                    for (int i = 0; i < draggingHandles.Count; i++)
                    {
                        draggingHandles[i].transform.position += deltaVec;
                    }
                }
			}
            if (stickyScaler != null)
            {
                stickyScaler.drag(pos);
            }
		}
	}

    private void calculateDraggingHandles()
    {
        draggingHandles = getConnectedHandles(ActiveNotes);
    }

    public static List<GameObject> getConnectedHandles(List<GameObject> stickies)
    {
        List<GameObject> handles = new List<GameObject>();
        for (int i = 0; i < stickies.Count; i++)
        {
            if (stickies[i] != null && stickies[i].tag == "StickyNote")
            {
                StickyScript ss0 = stickies[i].GetComponent<StickyScript>();
                for (int j = i + 1; j < stickies.Count; j++)
                {
                    if (stickies[j] != null && stickies[j].tag == "StickyNote")
                    {
                        StickyScript ss1 = stickies[j].GetComponent<StickyScript>();
                        if (ss0.chains.ContainsKey(ss1))
                        {
                            List<LineScript> lines = ss0.chains[ss1].LinesInChain;
                            for (int k = 0; k < lines.Count; k++)
                            {
                                if (lines[k].tag == "Handle")
                                {
                                    handles.Add(lines[k].gameObject);
                                }
                            }
                        }
                    }
                }
            }
        }
        return handles;
    }
	
	public void touchesEndedHandler(object sender, TouchEventArgs e)
	{
		foreach (var touch in e.Touches)
        {
            bool madeAMove =  StickyScript.theresNewTextOnStickies;
            string moveLog = "";
            if (StickyScript.theresNewTextOnStickies)
            {
                StickyScript.theresNewTextOnStickies = false;
                moveLog += " StickyScript.theresNewTextOnStickies";
            }
            ViewControl.dragging = false;
            if (activeNote != null && ActiveNotes != null)
            {
                for (int i = 0; i < ActiveNotes.Count; i++)
                {
                    GridSnapper.Snap(ActiveNotes[i]);
                }
            }
            madeAMove = draggedSomeStickies || madeAMove;
            if (draggedSomeStickies)
            {
                moveLog += " draggedSomeStickies";
                draggedSomeStickies = false;
            }
            draggingHandles = null;
			activeNote = null;
            if (stickyScaler != null)
            {
                ViewControl.calculateMaxMin = 2;
                stickyScaler.mouseOver = false;
                stickyScaler = null;
                madeAMove = true;
            }

            if (madeAMove)
            {
                SaveLoadManager.MakeAMove("ftlGatherer" + moveLog);
            }
			ITouch _touch;
			if (!ftlTouches.TryGetValue(touch.Id, out _touch)) return;
			ftlTouches.Remove(touch.Id);
		}
	}
	
	public void touchesCancelledHandler(object sender, TouchEventArgs e)
	{
		touchesEndedHandler(sender, e);
	}
	
	#endregion
}
