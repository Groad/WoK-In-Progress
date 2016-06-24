using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewStickyButton : EditorButton 
{
	public GameObject stickyInPanel;
	private Vector3 panelPosition;
	public static GameObject thisSticky;
    public static NewStickyButton instance;
	
	void Start () 
	{
        instance = this;
		var panel = transform.parent;
        panelPosition = panel.position;// - panel.GetComponent<SpriteRenderer> ().bounds.extents;
        panelPosition.x += 0.25f;
		panelPosition.z -= 2f;
	}

	public void removeFromPanel ()
	{
		thisSticky = null;
	}

    public GameObject CreateSticky()
    {
        return Instantiate(stickyInPanel, panelPosition, Quaternion.identity) as GameObject;
    }

    public void ForceMouseDown()
    {
        press();
    }

    public override void press()
    {
        if (thisSticky == null)
        {
            thisSticky = CreateSticky();
            StickyScript ss = thisSticky.GetComponent<StickyScript>();
            ss.noteColorId = ColorPickerDiscreet.currentColorId;
            ss.noteColor = ColorPickerDiscreet.getColor(ColorPickerDiscreet.currentColorId);
        }
        thisSticky.transform.parent = transform.parent;
        if (!SceneManager.isDesktopScene)
        {
            thisSticky.transform.localScale = Vector3.one * 0.8f;
            TabletKeyboardController.sticky = thisSticky.GetComponent<StickyScript>();
            //TabletKeyboardController.outputText = thisSticky.GetComponentInChildren<TextMesh>();
        }
        ftlGatherer.ActiveNotes = new List<GameObject>();
        ftlGatherer.ActiveNotes.Add(thisSticky);
        StickySender.currentNote = thisSticky;
    }
}
