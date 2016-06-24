using UnityEngine;
using System.Collections;

public class FingerPaintButton : EditorButton
{
    private SpriteRenderer sprite;
    public SpriteRenderer panel;
    public SpriteRenderer colorButton;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool inDrawLineMode = (/*NewStickyButton.thisSticky != null && */StickyInPanel.mode == StickyInPanel.Mode.drawLine);
        //colorButton.color = inDrawLineMode ? ColorPickerDiscreet.getColor(StickyInPanel.currentColorId) : Color.white;
        panel.color = inDrawLineMode ? Color.grey : Color.white;
        sprite.color = panel.color;
    }

    public override void press()
    {
        if (StickyInPanel.mode == StickyInPanel.Mode.drawLine)
        {
            StickyInPanel.mode = StickyInPanel.Mode.drag;
        }
        else if (StickyInPanel.mode == StickyInPanel.Mode.drag)
        {
            StickyInPanel.mode = StickyInPanel.Mode.drawLine;
        }
    }
}
