using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class LineScript : MonoBehaviour
{
	public enum Type {Line,Handle}
    public Type type;
    public GameObject handle;
    public GameObject linePrefab;
    private bool draggingHandle;
	public LinePopup linePopup;
	public ColorPicker colorPicker;
	private Camera ProjectionCamera;
	[HideInInspector]
	public StickySender stickySender;
	private GameObject ProjectionSurface;
    public GameObject circlePopupLine;
    public GameObject circlePopupHandle;
    public GameObject circlePopupSticky;

    private Texture lineTexture;
    private VectorLine vectorLine;
    private SpriteRenderer arrowHead;                                  
    private BoxCollider lineCollider;                                   

	void OnEnable ()
	{
		Go = false;
		ProjectionSurface = GameObject.Find ("MainPanel");
		ProjectionCamera = GameObject.Find ("MainPanelCamera").GetComponent<Camera>();

        //initializes VectorLine object
        VectorLine.SetCamera3D(ProjectionCamera);
        lineTexture = (Texture) Resources.Load("ThinLine");
        vectorLine = VectorLine.SetLine3D(Color.black, new Vector3[2]);
        vectorLine.lineWidth = 5f;
        vectorLine.material = new Material(Shader.Find("GUI/Text Shader"));
        vectorLine.material.mainTexture = lineTexture;

        stickySender = ProjectionCamera.gameObject.GetComponent<StickySender> ();
		stickySender.lineHit += lineHit;
		if (type == Type.Line)                                                                                      //Now Obsolete LineRenderer Code
		{                                                                                                           //Use Vectrosity and VectorLines
			//LineColor = new Color (0f, 1f, 0f, 1f);
            //GetComponent<LineRenderer>().SetWidth(0.1f, 0.1f);
            //GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended"));
            foreach (Transform child in transform)
            {
               //if (child.GetComponent<LineRenderer>() != null)
                //{
                    //line = child.GetComponent<LineRenderer>();
                //}
                if(child.GetComponent<SpriteRenderer>() != null)        
                {
                    arrowHead = child.GetComponent<SpriteRenderer>();
                    lineCollider = GetComponent<BoxCollider>();
                }                                                          
            }
            //if (line != null)
            //{
                //line.material = new Material(Shader.Find("Particles/Alpha Blended"));
            //}
		}
	}

    void OnDisable()
    {
        stickySender.lineHit -= lineHit;
        VectorLine.Destroy(ref vectorLine);
    }

	public GameObject Endpoint0;// { get; set; }
	public GameObject Endpoint1;// { get; set; }
	public Color LineColor { get; set; }
	public bool Go { get; set; }
	public Chain ThisChain { get; set; }

	void Update ()
	{
		if (!Go)
			return;
		if (Endpoint0 != null && Endpoint1 != null)
		{
            var pos0 = Endpoint0.transform.position;
			var pos1 = Endpoint1.transform.position;
			pos0.z = -50f;
			pos1.z = -50f;
			var centroid = (pos0 + pos1) / 2;
			transform.position = centroid;
			var angle = Mathf.Atan2(pos1.y - pos0.y, pos1.x-pos0.x)*180 / Mathf.PI;
			transform.localEulerAngles  = new Vector3 (0, 0, angle);
            RaycastHit[] hits = Physics.RaycastAll(new Ray(pos1, pos0 - pos1));
            foreach (RaycastHit hit in hits)
            {   
                if(hit.collider == Endpoint0.GetComponent<BoxCollider>())
                {
                    arrowHead.transform.position = hit.point;
                    arrowHead.transform.localPosition += new Vector3(0.5f, 0f, 0f);
                    break;
                }
                else if(hit.collider == Endpoint0.GetComponent<CapsuleCollider>())
                {
                    arrowHead.transform.position = hit.point;
                    arrowHead.transform.localPosition += new Vector3(0.5f, 0f, 0f);
                    break;
                }
                else if(hit.collider == Endpoint0.GetComponent<MeshCollider>())
                {
                    arrowHead.transform.position = hit.point;
                    arrowHead.transform.localPosition += new Vector3(0.5f, 0f, 0f);
                    break;
                }
            }
            //Sets new end points for vectorLine and redraws
            List<Vector3> linePositions = new List<Vector3>();
            linePositions.Add(pos0 + Vector3.back * 0.1f);
            linePositions.Add(pos1 + Vector3.back * 0.1f);
            vectorLine.points3 = linePositions;
            vectorLine.Draw3D();
            //Sets size of line collider used for click detection
            lineCollider.size = new Vector3(Vector3.Distance(pos0, pos1), 0.75f, 0.75f);
        }
	}

	public void SplitLineHandle (Vector3 position)
	{
		position.z = -26f;
		var tmpEndpoint1 = Endpoint1;
        Endpoint1 = Instantiate(handle, position, Quaternion.identity) as GameObject;
        Endpoint1.transform.parent = SaveLoadManager.instance.handles;
        GridSnapper.Snap(Endpoint1);

		LineScript h = Endpoint1.GetComponent<LineScript>();       
		h.Endpoint0 = gameObject;                                  
        var lineObject = Instantiate(linePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        lineObject.transform.parent = SaveLoadManager.instance.connections;
		LineScript ls = lineObject.GetComponent<LineScript>();      
		ls.Endpoint0 = Endpoint1;          
		ls.Endpoint1 = tmpEndpoint1;       
        foreach(Transform r in ls.transform)
        {                   
            if(r.name == "ArrowHead")
            {
                r.GetComponent<SpriteRenderer>().enabled = false;       //Turns off arrow sprite pointing at handles
            }
        }
		ls.Go = true;
		ls.enabled = true;
		h.Endpoint1 = lineObject.gameObject;    
		h.ThisChain = ThisChain;
		ls.ThisChain = ThisChain;
		var index = ThisChain.LinesInChain.IndexOf (GetComponent<LineScript> ());
		//ThisChain.LinesInChain.Remove(GetComponent<LineScript>());
		ThisChain.LinesInChain.Insert (index + 1, ls);
		ThisChain.LinesInChain.Insert (index + 1, h);
	}
	
	public void DeleteHandle (Vector3 _position, bool _replaceWithSticky)
	{
		if (Endpoint0 == null)
			return;
		LineScript line0 = Endpoint0.GetComponent<LineScript>();
		if (Endpoint1.GetComponent<LineScript>() != null)
		{
			LineScript line1 = Endpoint1.GetComponent<LineScript>();
			line0.Endpoint1 = line1.Endpoint1;
		}
		if (_replaceWithSticky)
			line0.SplitLineSticky (_position);
		Destroy(Endpoint1);
		Destroy(gameObject);
	}

	public void SplitLineSticky (Vector3 position)
	{
		position.z = -20;
		var startingSticky = ThisChain.Sticky0;
		var endingSticky = ThisChain.Sticky1;
		var thisLineScript = GetComponent<LineScript> ();
		var index = ThisChain.LinesInChain.IndexOf (thisLineScript);
		var s = ProjectionCamera.gameObject.GetComponent<StickySender>().AddSticky (position);

        GridSnapper.Snap(s);
		var lineObject = Instantiate(linePrefab, Vector3.zero, Quaternion.identity) as LineRenderer;
		LineScript ls = lineObject.GetComponent<LineScript>();
		ls.Endpoint0 = s;
		ls.Endpoint1 = Endpoint1;
		ls.Go = true;
		ls.enabled = true;

		var Chain1 = new Chain ();
		Chain1.LinesInChain = new List<LineScript> ();
		Chain1.LinesInChain.Add (ls);
		for (int i = index + 1; i < ThisChain.LinesInChain.Count; i++)
			Chain1.LinesInChain.Add (ThisChain.LinesInChain [i]);

		Endpoint1 = s;
		for (int i = index + 1; i < ThisChain.LinesInChain.Count; i++)
			ThisChain.LinesInChain.RemoveAt (i);
		startingSticky.GetComponent<StickyScript> ().RerouteConnection (s.GetComponent<StickyScript> (), ThisChain, Chain1, endingSticky.GetComponent<StickyScript> (), null);
	}

	public void ReplaceHandleWithSticky (Vector3 position)
	{
		DeleteHandle (position, true);
	}

	private void lineHit (GameObject _hit, StickySender.ButtonClicked _button)
	{
		if (this == null)
			return;
		if (_hit == null || _hit != gameObject)
			return;
		if (_button == StickySender.ButtonClicked.Left)
		{
		}
		if (_button == StickySender.ButtonClicked.Right)
		{
			var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			position.x += 1.0f;
			if (type == Type.Line)
			{
                Instantiate(circlePopupLine, position, Quaternion.identity);
                CircleMenuButton.lineScript = this;
			}
			else if (type == Type.Handle)
			{
                Instantiate(circlePopupHandle, position, Quaternion.identity);
                CircleMenuButton.lineScript = this;
			}
		}
	}
}
