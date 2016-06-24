using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {

	public static ButtonManager Instance;

	public delegate void ButtonAction();
	public event ButtonAction SendFile = delegate{};

	void Awake () 
	{
		Instance = this;
	}

	public void TryToSendFile()
	{
		SendFile ();
	}
}
