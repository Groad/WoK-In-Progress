using UnityEngine;
using System.Collections;

public class ColorPickerDiscreet : EditorButton
{
    public bool isFill;
    public int colorId;
    public ColorPalette menu;
    public Color[] colors;
    private static ColorPickerDiscreet instance;
    public static int currentColorId = 0;

    void Start()
    {
        instance = this;
        menu.colorPickerDiscreet = this;
        GetComponent<SpriteRenderer>().color = getColor(colorId);
    }

    public override void press()
    {
        if (mousePressed)
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        }
        else //hotkey part
        {
            if (ftlGatherer.ActiveNotes != null)
            {
                colorId = -1;
                bool madeAMove = false;
                for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
                {
                    GameObject activeNote = ftlGatherer.ActiveNotes[i];
                    if (activeNote != null)
                    {
                        StickyScript ss = activeNote.GetComponent<StickyScript>();
                        if (ss != null)
                        {
                            if (colorId == -1)
                            {
                                colorId = ss.noteColorId + 1;
                                if (colorId >= colors.Length)
                                {
                                    colorId = 0;
                                }
                            }
                            if (ss.noteColorId != colorId && ss.gameObject != NewStickyButton.thisSticky)
                            {
                                madeAMove = true;
                            }
                            ss.noteColor = colors[colorId];
                            ss.noteColorId = colorId;
                        }
                    }
                }
                currentColorId = colorId;

                if (madeAMove)
                {
                    SaveLoadManager.MakeAMove("ColorPickerDiscreet.changeColor");
                }
            }
            GetComponent<SpriteRenderer>().color = getColor(colorId);
        }
    }

    public void changeColor()
    {
        if (!isFill)
        {
            StickyInPanel.currentColorId = colorId;
        }
        else
        {
            currentColorId = colorId;
            if (ftlGatherer.ActiveNotes != null)
            {
                bool madeAMove = false;
                for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
                {
                    GameObject activeNote = ftlGatherer.ActiveNotes[i];
                    if (activeNote != null)
                    {
                        StickyScript ss = activeNote.GetComponent<StickyScript>();
                        if (ss != null)
                        {
                            if (ss.noteColorId != colorId && ss.gameObject != NewStickyButton.thisSticky)
                            {
                                madeAMove = true;
                            }
                            ss.noteColor = colors[colorId];
                            ss.noteColorId = colorId;
                        }
                    }
                }

                if (madeAMove)
                {
                    SaveLoadManager.MakeAMove("ColorPickerDiscreet.changeColor");
                }
            }
        }
        GetComponent<SpriteRenderer>().color = getColor(colorId);
    }

    public static Color getColor(int colorId)
    {
        if (instance == null)
        {
            return Color.white;
        }
        return instance.colors[colorId % instance.colors.Length];
    }
}
