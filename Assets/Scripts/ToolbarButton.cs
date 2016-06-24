using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolbarButton : MonoBehaviour
{
    public bool pressed;
    public bool pressing;
    private bool mouseOver;
    public bool interactable;
    private SpriteRenderer sprite;
    private Color cNotInteractable = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color cPressing = new Color(0.3f, 0.3f, 1f, 1f);
    private Color cOver = new Color(1f, 1f, 0.7f, 1f);
    public bool isToolbarMainButton;
    private float toolbarMenuCloseTimer;
    private List<ToolbarButton> buttons;
    private bool closeMenuOnNextFrame;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (isToolbarMainButton)
        {
            buttons = new List<ToolbarButton>();
            foreach (Transform child in transform)
            {
                if (child.GetComponent<ToolbarButton>() != null)
                {
                    buttons.Add(child.GetComponent<ToolbarButton>());
                    child.localPosition = Vector3.down * 0.31f * buttons.Count;
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    void Update()
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

        if (isToolbarMainButton)
        {
            if (closeMenuOnNextFrame)
            {
                closeMenuOnNextFrame = false;
                openCloseMenu(false);
            }

            if (mouseOver && interactable)
            {
                openCloseMenu(true);
                toolbarMenuCloseTimer = 0f;
            }
            else
            {
                bool anyChildButtonsOver = false;
                for (int i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].mouseOver)
                    {
                        anyChildButtonsOver = true;
                        break;
                    }
                }
                if (anyChildButtonsOver)
                {
                    toolbarMenuCloseTimer = 0f;

                    for (int i = 0; i < buttons.Count; i++)
                    {
                        if (buttons[i].pressed)
                        {
                            closeMenuOnNextFrame = true;
                            break;
                        }
                    }
                }
                else if (toolbarMenuCloseTimer > 0.3f)
                {
                    openCloseMenu(false);
                }
                else
                {
                    toolbarMenuCloseTimer += Time.deltaTime;
                }
            }
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

    public void openCloseMenu(bool open)
    {
        if (isToolbarMainButton)
        {
            if (open)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].gameObject.SetActive(false);
                    buttons[i].mouseOver = false;
                    buttons[i].pressed = false;
                    buttons[i].pressing = false;
                }
            }
        }
    }
}
