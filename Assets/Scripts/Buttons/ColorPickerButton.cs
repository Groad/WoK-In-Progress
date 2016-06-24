using UnityEngine;
using System.Collections;

public class ColorPickerButton : MonoBehaviour
{
	private ColorPicker cp;

	void Start ()
	{
		cp = transform.root.gameObject.GetComponentInChildren<ColorPicker> ();
	}

	void OnMouseDown ()
	{
		cp.HideColorPicker = !cp.HideColorPicker;
	}
}
