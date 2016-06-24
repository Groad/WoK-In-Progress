using UnityEngine;
using UnityEngine.UI;
using HeathenEngineering.OSK.v2;
using System.Collections;
using UnityEngine.EventSystems;

public class TabletKeyboardController : MonoBehaviour
{
    public static StickyScript sticky;
    public OnScreenKeyboard keyboard;
    public static TabletKeyboardController instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (keyboard != null)
        {
            keyboard.KeyPressed += new KeyboardEventHandler(keyboardKeyPressed);
            EventSystem.current.SetSelectedGameObject(keyboard.ActiveKey.gameObject);
        }
    }

    void Update()
    {

    }

    void keyboardKeyPressed(OnScreenKeyboard sender, OnScreenKeyboardArguments args)
    {
        if (sticky != null)
        {
            string text = sticky.gameObject.name;
            switch (args.KeyPressed.type)
            {
                case KeyClass.Backspace:
                    if (text.Length > 0)
                        text = text.Substring(0, text.Length - 1);
                    break;
                case KeyClass.Return:
                    text += args.KeyPressed.ToString();
                    break;
                case KeyClass.Shift:
                    //No need to do anything here as the keyboard will sort that on its own
                    break;
                case KeyClass.String:
                    text += args.KeyPressed.ToString();
                    break;
            }
            sticky.gameObject.name = text;
            sticky.textPointer = text.Length;
            sticky.StickyText = text;
        }
    }
}
