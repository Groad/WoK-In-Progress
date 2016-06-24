using UnityEngine;
using System.Collections;

public class StickyScaler : MonoBehaviour
{
    private StickyScript sticky;
    public Vector3 dragStartPos;
    private SpriteRenderer spriteRenderer;
    public bool mouseOver;
    public bool stickyActive;

	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        sticky = transform.parent.GetComponent<StickyScript>();
	}
	
	void Update ()
    {
        transform.localScale = new Vector3(1f / sticky.transform.lossyScale.x, 1f / sticky.transform.lossyScale.y, 1f);
        spriteRenderer.enabled = mouseOver || stickyActive;
	}

    public void drag(Vector3 pos)
    {
        Vector3 deltaPos = pos - dragStartPos;
        if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
        {
            sticky.customScale += deltaPos.x / 2f;
        }
        else
        {
            sticky.customScale -= deltaPos.y / 2f;
        }
        if (sticky.customScale < 1f)
        {
            sticky.customScale = 1f;
        }
        sticky.scaleStickyForText();
        dragStartPos = pos;
        mouseOver = true;
    }
}
