using UnityEngine;
using System.Collections;

public class EraseButton : MonoBehaviour
{
    private StickyInPanel.Mode modeOnClick;
    public static bool erasing;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool inDrawLineMode = (erasing && StickyInPanel.mode == StickyInPanel.Mode.drawLine);
        sprite.color = inDrawLineMode ? Color.grey : Color.white;
    }

	void OnMouseDown ()
    {
        if (StickyInPanel.mode == StickyInPanel.Mode.drag)
        {
            modeOnClick = StickyInPanel.mode;
            StickyInPanel.mode = StickyInPanel.Mode.drawLine;
            erasing = true;
        }
        else if (StickyInPanel.mode == StickyInPanel.Mode.drawLine)
        {
            if (erasing && modeOnClick == StickyInPanel.Mode.drag)
            {
                StickyInPanel.mode = modeOnClick;
            }
            modeOnClick = StickyInPanel.Mode.drawLine;
            erasing = !erasing;
        }
	}
}
