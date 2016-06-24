using UnityEngine;
using System.Collections;

public class StickyFingerPaint : MonoBehaviour
{
    public GameObject drawLinePrefab;
    public DrawLine currentDrawLine;
    private Vector3 drawLineLastPos;
    public static bool painting;

	void Start ()
    {
        
	}
	
	void Update ()
    {

    }

    void OnMouseOver()
    {
        if (StickyInPanel.mode == StickyInPanel.Mode.drawLine && transform.lossyScale.x > 0.5f)
        {
            if (Input.GetMouseButtonUp(0) && !EraseButton.erasing)
            {
                currentDrawLine = null;
                painting = false;
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = transform.position.z - 0.005f;
            if (Input.GetMouseButtonDown(0) && !EraseButton.erasing)
            {
                drawLineLastPos = mousePos;
                painting = true;
            }
            else if (Input.GetMouseButton(0) && !EraseButton.erasing)
            {
                if (painting && Geometry.lengthOfVector3(drawLineLastPos - mousePos) > 0.05f)
                {
                    Vector3 localMousePos = transform.InverseTransformPoint(mousePos);
                    if (Mathf.Abs(localMousePos.x) < 1.22f && Mathf.Abs(localMousePos.y) < 1.22f)
                    {
                        CreateDrawLine(StickyInPanel.currentColorId, false, drawLineLastPos, mousePos);
                        drawLineLastPos = mousePos;
                        GetComponent<StickyScript>().lastUpdateTime = Time.realtimeSinceStartup;
                    }
                }

                //if (currentDrawLine == null)
                //{
                //    currentDrawLine = (Instantiate(drawLinePrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<DrawLine>();
                //    currentDrawLine.name = "DrawLine";
                //    currentDrawLine.color = Color.black;
                //    currentDrawLine.transform.localScale = Vector3.one;
                //    currentDrawLine.transform.SetParent(transform);
                //    currentDrawLine.transform.localPosition = -Vector3.forward * 0.05f;
                //    currentDrawLine.positions.Add(currentDrawLine.worldToLocal(mousePos));
                //    currentDrawLine.color = ColorPickerDiscreet.getColor(StickyInPanel.currentColorId);
                //    drawLineLastPos = mousePos;
                //}
                //else
                //{
                //    if (Geometry.lengthOfVector3(drawLineLastPos - mousePos) > 0.05f)
                //    {
                //        currentDrawLine.positions.Add(currentDrawLine.worldToLocal(mousePos));
                //        drawLineLastPos = mousePos;
                //    }
                //}
            }
        }
    }

    public void CreateDrawLine(int colorId, bool areLocalPositions, Vector3 pos0, Vector3 pos1)
    {
        currentDrawLine = (Instantiate(drawLinePrefab, transform.position, Quaternion.identity) as GameObject).GetComponent<DrawLine>();
        currentDrawLine.name = "DrawLine";
        currentDrawLine.transform.SetParent(transform);
        setDrawLineScale(currentDrawLine.gameObject);
        if (areLocalPositions)
        {
            Vector3 v0 = pos0;
            Vector3 v1 = pos1;
            v0.z = v1.z = 0f;
            currentDrawLine.positions.Add(v0);
            currentDrawLine.positions.Add(v1);
        }
        else
        {
            Vector3 v0 = currentDrawLine.worldToLocal(pos0);
            Vector3 v1 = currentDrawLine.worldToLocal(pos1);
            v0.z = v1.z = 0f;
            currentDrawLine.positions.Add(v0);
            currentDrawLine.positions.Add(v1);
        }
        currentDrawLine.colorId = colorId;
        currentDrawLine.color = ColorPickerDiscreet.getColor(colorId);
    }

    void OnMouseExit()
    {
        currentDrawLine = null;
    }

    public void ForceMouseOver()
    {
        OnMouseOver();
    }

    public void setDrawLineScale(GameObject line)
    {
        if (GetComponent<StickyScript>().isPanelShowOff || GetComponent<StickyInPanel>() != null)
        {
            line.transform.localScale = Vector3.one;
            line.transform.localPosition = -Vector3.forward * StickyScript.FingerPaintZ;
        }
        else
        {
            line.transform.localScale = new Vector3(-1f, 1f, 1f);
            line.transform.localPosition = Vector3.forward * StickyScript.FingerPaintZ;
        }
    }
}
