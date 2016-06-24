using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawLine : MonoBehaviour
{
    public List<Vector3> positions;
    private LineRenderer lineRenderer;
    public int colorId;
    public Color color;
    private StickyFingerPaint stickyFingerPaint;
    private StickyInPanel stickyInPanel;
    private SphereCollider coll;
    public float collRadius;

	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (transform.parent != null && transform.parent.GetComponent<StickyFingerPaint>() != null)
        {
            stickyFingerPaint = transform.parent.GetComponent<StickyFingerPaint>();
            if (positions.Count > 0)
            {
                coll = GetComponent<SphereCollider>();
                coll.center = positions[0] - Vector3.forward;
                collRadius = coll.radius * transform.lossyScale.x;
            }
        }
        if (transform.parent != null && transform.parent.GetComponent<StickyInPanel>() != null)
        {
            stickyInPanel = transform.parent.GetComponent<StickyInPanel>();
        }
	}
	
	void Update ()
    {
        lineRenderer.SetVertexCount(positions.Count);
        for (int i = 0; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, localToWorld(positions[i]));
            color.a = 0.5f;
            lineRenderer.SetColors(color, color);
            lineRenderer.SetWidth(0.1f * transform.lossyScale.x / 0.8f, 0.1f * transform.lossyScale.x / 0.8f);
        }
        if (coll != null)
        {
            coll.enabled = (StickyInPanel.mode == StickyInPanel.Mode.drawLine);
            coll.radius = collRadius / transform.lossyScale.x;
        }
	}

    public Vector3 localToWorld(Vector3 v)
    {
        Vector3 v2 = new Vector3(v.x * transform.lossyScale.x, v.y * transform.lossyScale.y, v.z * transform.lossyScale.z);
        v2 += transform.position;
        return v2;
    }

    public Vector3 worldToLocal(Vector3 v)
    {
        Vector3 v2 = v - transform.position;
        v2 = new Vector3(v2.x / transform.lossyScale.x, v2.y / transform.lossyScale.y, v2.z / transform.lossyScale.z);
        return v2;
    }

    void OnMouseOver()
    {
        if (stickyFingerPaint == null)
        {
            if (transform.parent != null && transform.parent.GetComponent<StickyFingerPaint>() != null)
            {
                stickyFingerPaint = transform.parent.GetComponent<StickyFingerPaint>();
            }
        }
        if (stickyFingerPaint != null)
        {
            stickyFingerPaint.ForceMouseOver();
            if (StickyInPanel.mode == StickyInPanel.Mode.drawLine && (Input.GetMouseButton(1) || (Input.GetMouseButton(0) && EraseButton.erasing)))// && stickyFingerPaint.gameObject == NewStickyButton.thisSticky)
            {
                transform.parent.GetComponent<StickyScript>().lastUpdateTime = Time.realtimeSinceStartup;
                Destroy(gameObject);
            }
        }
    }

    void OnMouseDown()
    {
        if (stickyInPanel != null)
        {
            stickyInPanel.ForceMouseDown();
        }
    }

    void OnMouseUp()
    {
        if (stickyInPanel != null)
        {
            stickyInPanel.ForceMouseUp();
        }
    }
}
