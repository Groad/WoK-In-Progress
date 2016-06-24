using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace HeathenEngineering.OSK.v2
{
	/// <summary>
	/// On screen keyboard key.
	/// Represents a key and stores values used by the keyboard on press events
	/// </summary>
	[AddComponentMenu("Heathen/OSK/v2/On Screen Keyboard Key (v2.x)")]
	[RequireComponent(typeof(Button))]
	public class OnScreenKeyboardKey : MonoBehaviour
	{
		[HideInInspector]
		public OnScreenKeyboard Keyboard;
		public KeyClass type = KeyClass.String;
		public KeyCode keyCode = KeyCode.A;
		public string LowerCaseValue = "a";
		public string UpperCaseValue = "A";
		public Text Text;
		
		// Use this for initialization
		void Start ()
		{
		}
		
		// Update is called once per frame
		void Update ()
		{
			
		}

		/// <summary>
		/// Returns a string that represents the current case value of the keyboard.
		/// </summary>
		/// <returns>A string that represents the current case value.</returns>
		public override string ToString ()
		{
			if (Keyboard != null) 
			{
				if(type == KeyClass.Return)
					return "\n";
				else
					return Keyboard.IsLowerCase ? LowerCaseValue : UpperCaseValue;
			}
			else 
			{
				Debug.LogError("To String was called on an OnScreenKeyboardKey that has no valid keyboard", this);
				return LowerCaseValue;
			}
		}
		
		/// <summary>
		/// Presses the key.
		/// This can be called to simulate a press event on this specific key
		/// </summary>
		public void PressKey()
		{
			if(Keyboard != null)
			{
				Keyboard.ActiveKey = this;
				Keyboard.ActivateKey();
			}
			else
				Debug.LogError("An OnScreenKeyboardKey was pressed but does not have an owning keyboard; insure the key is a child of an OnScreenKeyboard", this);
		}
		/// <summary>
		/// Sets the case for this key by updating the text mesh object.
		/// Special handlig is done for keys with no upper string value such as space
		/// </summary>
		/// <param name="ToUpper">If set to <c>true</c> to upper.</param>
		public void SetCase(bool ToUpper)
		{
			//Handel space special so it can render text but not have a text value
			if(type == KeyClass.String && string.IsNullOrEmpty(UpperCaseValue.Trim()))
				Text.text = "_";
			else
			{
				if(ToUpper)
					Text.text = UpperCaseValue;
				else
					Text.text = LowerCaseValue;
			}
		}
	}
}