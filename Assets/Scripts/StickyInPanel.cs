using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickyInPanel : MonoBehaviour 
{
    public enum Mode
    {
        drag,
        drawLine
    }

	private bool dragging = false;
	private Vector3 offset = Vector3.zero;
	private Vector3 startPos;
    public bool sentToPalette;
    public static Mode mode;
    public static int currentColorId = 1;

	void OnEnable()
	{
		startPos = transform.position;
	}

    public void ForceMouseDown()
    {
        OnMouseDown();
    }

    public void ForceMouseUp()
    {
        OnMouseUp();
    }

	void OnMouseDown()
	{
        if (SceneManager.isDesktopScene && (mode == Mode.drag || transform.localScale.x < 0.3f))
        {
            dragging = true;
            offset = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            offset.z = transform.position.z;
            enableColliders(false);
            startPos = transform.position;
            ftlGatherer.ActiveNotes = new List<GameObject>();
            ftlGatherer.ActiveNotes.Add(gameObject);
        }
	}

	void OnMouseUp()
    {
        if (SceneManager.isDesktopScene && (mode == Mode.drag || transform.localScale.x < 0.3f))
        {
            //var pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.transform.tag == "ProjectionSurface")
            {
                var pos = ftlGatherer.TransformToMainPanel(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                pos.z = -20f;
                var newInMainPanel = StickySender.instance.AddSticky(pos);
                GetComponent<StickyScript>().copyAllInfoTo(newInMainPanel.GetComponent<StickyScript>());
                if(newInMainPanel.GetComponent<StickyScript>().spriteIndex == 1)
                {
                    newInMainPanel.transform.Rotate(new Vector3(0f, 0f, 45f));
                    newInMainPanel.GetComponentInChildren<TextMesh>().transform.Rotate(new Vector3(0f, 0f, 45f));
                }
                ftlGatherer.ActiveNotes = new List<GameObject>();
                ftlGatherer.ActiveNotes.Add(newInMainPanel);
                GridSnapper.Snap(newInMainPanel);
                StickySender.currentNote = newInMainPanel;
                GetComponent<StickyScript>().enabled = false;
                Destroy(gameObject);
                //commented out because dragStickies MakeAMove is also called
                //SaveLoadManager.MakeAMove("StickyInPanel0");
            }
            else if (Physics.Raycast(ray, out hit) && hit.transform.tag == "StickyPalette")
            {
                dragging = false;
                sendToPalette(hit.transform.GetComponent<StickyPalette>());
                SaveLoadManager.MakeAMove("StickyInPanel1");
            }
            else
            {
                transform.position = startPos;
                dragging = false;
            }
            enableColliders(true);
        }
	}

    public void sendToPalette(StickyPalette palette)
    {
        transform.position = palette.nextSlot;
        transform.localScale = new Vector3(0.15f, 0.15f, 0.2f);
        enableColliders(true);
        NewStickyButton.thisSticky = null;
        palette.AddToList(gameObject);
        sentToPalette = true;
        foreach (Transform child in transform)
        {
            if (child.tag == "DrawLine")
            {
                child.GetComponent<DrawLine>().collRadius *= 0.2f;
            }
        }
    }

	void Update()
	{
		if (!dragging)
			return;
		var position = Camera.main.ScreenToWorldPoint (Input.mousePosition) + offset;
		position.z = transform.position.z;
		transform.position = position;
	}

    void enableColliders(bool enable)
    {
        foreach(Collider c in GetComponents<Collider>())
        {
            c.enabled = enable;
        }

        foreach (Transform child in transform)
        {
            if (child.tag == "DrawLine")
            {
                child.GetComponent<Collider>().enabled = enable;
            }
        }
    }
}
