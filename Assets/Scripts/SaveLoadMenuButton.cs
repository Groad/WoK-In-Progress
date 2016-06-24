using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SaveLoadMenuButton : MonoBehaviour
{
    public static SaveLoadMenuButton instance;

    public bool menuOpened;
    public GameObject[] buttonsToEnable;
    private SpriteButton spriteButton;

    void Start()
    {
        instance = this;
        spriteButton = GetComponent<SpriteButton>();
        RefreshButtons();
    }

    public void press()
    {
        menuOpened = !menuOpened;
        RefreshButtons();
    }

    void Update()
    {
        if (spriteButton.pressed)
        {
            press();
        }
    }

    public void RefreshButtons()
    {
        for (int i = 0; i < buttonsToEnable.Length; i++)
        {
            buttonsToEnable[i].GetComponent<SpriteRenderer>().enabled = menuOpened;
            buttonsToEnable[i].GetComponent<Collider>().enabled = menuOpened;
            buttonsToEnable[i].GetComponent<SpriteButton>().pressed = false;
            buttonsToEnable[i].GetComponent<SpriteButton>().pressing = false;
            buttonsToEnable[i].GetComponent<SpriteButton>().enabled = menuOpened;
            buttonsToEnable[i].GetComponentInChildren<MeshRenderer>().enabled = menuOpened;
        }
    }
}
