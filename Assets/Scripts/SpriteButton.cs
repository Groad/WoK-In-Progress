using UnityEngine;
using System.Collections;

public class SpriteButton : MonoBehaviour
{
    public bool pressed;
    public bool pressing;
    private bool mouseOver;
    public bool interactable;
    private SpriteRenderer sprite;
    private Color cNotInteractable = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color cPressing = new Color(0.3f, 0.3f, 0.3f, 1f);
    private Color cOver = new Color(0.6f, 0.6f, 0.6f, 1f);

    void OnDisable()
    {
        mouseOver = false;
        pressed = false;
        pressing = false;
    }

	void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
    {
        pressed = interactable && mouseOver && Input.GetMouseButtonDown(0);
        pressing = interactable && mouseOver && Input.GetMouseButton(0);
        if (!interactable)
        {
            sprite.color = cNotInteractable;
        }
        else if (pressing)
        {
            sprite.color = cPressing;
        }
        else if (mouseOver)
        {
            sprite.color = cOver;
        }
        else
        {
            sprite.color = Color.white;
        }
	}

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
}
