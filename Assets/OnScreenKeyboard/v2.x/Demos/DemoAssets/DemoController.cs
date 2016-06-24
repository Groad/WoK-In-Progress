using UnityEngine;
using UnityEngine.UI;
using HeathenEngineering.OSK.v2;
using System.Collections;
using UnityEngine.EventSystems;

public class DemoController : MonoBehaviour 
{
	public Text outputText;
	public OnScreenKeyboard keyboard;
	
	// Use this for initialization
	void Start () 
	{
		if (keyboard != null) 
		{
			keyboard.KeyPressed += new KeyboardEventHandler(keyboardKeyPressed);
			EventSystem.current.SetSelectedGameObject(keyboard.ActiveKey.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void keyboardKeyPressed(OnScreenKeyboard sender, OnScreenKeyboardArguments args)
	{
		switch (args.KeyPressed.type) 
		{
		case KeyClass.Backspace:
			if(outputText.text.Length > 0)
				outputText.text = outputText.text.Substring(0, outputText.text.Length -1);
			break;
		case KeyClass.Return:
			outputText.text += args.KeyPressed.ToString();
			break;
		case KeyClass.Shift:
			//No need to do anything here as the keyboard will sort that on its own
			break;
		case KeyClass.String:
			outputText.text += args.KeyPressed.ToString();
			break;
		}
	}
}
