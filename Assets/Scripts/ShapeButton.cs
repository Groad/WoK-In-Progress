using UnityEngine;
using System.Collections;

public class ShapeButton : EditorButton
{
    public override void press()
    {
        if (NewStickyButton.thisSticky != null)
        {
            nextShape(NewStickyButton.thisSticky.GetComponent<StickyScript>());
        }
        else if (ftlGatherer.ActiveNotes != null)
        {
            int shapeId = -1;
            for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
            {
                if (ftlGatherer.ActiveNotes[i] != null)
                {
                    if (shapeId == -1)
                    {
                        shapeId = ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>().spriteIndex;
                    }
                    else
                    {
                        ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>().spriteIndex = shapeId;
                    }
                    nextShape(ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>());
                }
            }
            SaveLoadManager.MakeAMove("ShapeButton");
        }
    }

    public static void nextShape(StickyScript sticky)
    {
        if (sticky.spriteIndex < StickyScript.MaxSpriteIndex)
        {
            sticky.spriteIndex++;
            switch (sticky.spriteIndex)
            {
                case 0: //square
                    sticky.GetComponent<CapsuleCollider>().enabled = false;
                    sticky.GetComponent<BoxCollider>().enabled = true;
                    break;

                case 1: //diamond
                    sticky.transform.Rotate(new Vector3(0f, 0f, 45f));
                    if(sticky.GetComponent<StickyInPanel>() != null)
                    {
                        sticky.GetComponentInChildren<TextMesh>().transform.Rotate(new Vector3(0f, 0f, -45f));
                    }else { sticky.GetComponentInChildren<TextMesh>().transform.Rotate(new Vector3(0f, 0f, 45f)); }
                    break;

                case 2: //circle
                    sticky.GetComponent<BoxCollider>().enabled = false;
                    sticky.transform.Rotate(new Vector3(0f, 0f, -45f));
                    if (sticky.GetComponent<StickyInPanel>() != null)
                    {
                        sticky.GetComponentInChildren<TextMesh>().transform.Rotate(new Vector3(0f, 0f, 45f));
                    }else { sticky.GetComponentInChildren<TextMesh>().transform.Rotate(new Vector3(0f, 0f, -45f)); }
                    sticky.GetComponent<CapsuleCollider>().enabled = true;
                    break;

                case 3: //rectangle
                    sticky.GetComponent<CapsuleCollider>().enabled = false;
                    sticky.GetComponent<BoxCollider>().enabled = true;
                    break;
            }
        }
        else
        {
            sticky.spriteIndex = 0;
        }
    }
}
