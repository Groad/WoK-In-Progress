using UnityEngine;
using System.Collections;

public class TextSizeButton : EditorButton
{
    public bool isUp;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.enabled = ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0;
    }

    public override void press()
    {
        if (ftlGatherer.ActiveNotes != null)
        {
            for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
            {
                if (ftlGatherer.ActiveNotes[i] != null && ftlGatherer.ActiveNotes[i].tag == "StickyNote")
                {
                    StickyScript activeSticky = ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>();
                    activeSticky.customTextScale += (isUp ? 0.2f : -0.2f);          //refactored from (isUp ? 1f : -1f) * 0.2f
                    if (activeSticky.customTextScale > StickyScript.MaxCustomTextScale)
                    {
                        activeSticky.customTextScale = StickyScript.MaxCustomTextScale;
                    }
                    else if (activeSticky.customTextScale < StickyScript.MinCustomTextScale)
                    {
                        activeSticky.customTextScale = StickyScript.MinCustomTextScale;
                    }
                }
            }
            SaveLoadManager.MakeAMove("TextSizeButton");
        }
    }
}
