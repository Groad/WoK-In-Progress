using UnityEngine;
using System.Collections;

public class ColorPaletteButton : EditorButton
{
    private int _color = 0;
    public int color
    {
        get
        {
            return _color;
        }
        set
        {
            _color = value;
            GetComponent<SpriteRenderer>().color = ColorPickerDiscreet.getColor(_color);
        }
    }

	void Start ()
    {
	    
	}

    public override void press()
    {
        ftlGatherer.pressedEditorButton = true;
        StickyFingerPaint.painting = false;
        ColorPickerDiscreet colorPickerDiscreet = transform.parent.GetComponent<ColorPalette>().colorPickerDiscreet;
        colorPickerDiscreet.colorId = color;
        colorPickerDiscreet.changeColor();
        transform.parent.gameObject.SetActive(false);
	}
}
