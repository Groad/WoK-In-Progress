using UnityEngine;
using System.Collections;

public class ColorPalette : MonoBehaviour
{
    public ColorPickerDiscreet colorPickerDiscreet;

	void Start ()
    {
        int i = 0;
	    foreach (Transform child in transform)
        {
            child.GetComponent<ColorPaletteButton>().color = i;
            i++;
        }
	}
	
	void Update ()
    {
	
	}
}
