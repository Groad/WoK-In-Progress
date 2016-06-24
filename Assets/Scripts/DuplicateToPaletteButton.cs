using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuplicateToPaletteButton : EditorButton
{
    public Transform stickyParent;

    public override void press()
    {
        //if (NewStickyButton.thisSticky != null)
        //{
        //    GameObject thisStickyIsThisSticky = NewStickyButton.thisSticky;
        //    GameObject duplicate = NewStickyButton.instance.CreateSticky();
        //    //duplicate.transform.parent = stickyParent;
        //    //NewStickyButton.thisSticky.GetComponent<StickyScript>().copyAllInfoTo(duplicate.GetComponent<StickyScript>());
        //    thisStickyIsThisSticky.GetComponent<StickyScript>().copyAllInfoTo(duplicate.GetComponent<StickyScript>());
        //    duplicate.GetComponent<StickyInPanel>().sendToPalette(StickyPalette.instance);
        //    ftlGatherer.ActiveNotes = new List<GameObject>();
        //    ftlGatherer.ActiveNotes.Add(duplicate);
        //    StickySender.currentNote = duplicate;
        //    StickyInPanel.mode = StickyInPanel.Mode.drag;
        //    NewStickyButton.thisSticky = thisStickyIsThisSticky;
        //    SaveLoadManager.MakeAMove("DuplicateToPaletteButton0");
        //    Debug.Log("NewStickyButton.thisSticky exists, so I duplicated that.");
        //}
        //else 
            if (ftlGatherer.ActiveNotes != null)
        {
            GameObject thisStickyIsThisSticky = NewStickyButton.thisSticky;
            List<GameObject> newActiveNotes = new List<GameObject>();
            for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
            {
                if (ftlGatherer.ActiveNotes[i] != null && ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>() != null)
                {
                    GameObject duplicate = NewStickyButton.instance.CreateSticky();
                    //duplicate.transform.parent = stickyParent;
                    ftlGatherer.ActiveNotes[i].GetComponent<StickyScript>().copyAllInfoTo(duplicate.GetComponent<StickyScript>());
                    duplicate.GetComponent<StickyInPanel>().sendToPalette(StickyPalette.instance);
                    NewStickyButton.thisSticky = null;
                    newActiveNotes.Add(duplicate);
                    StickySender.currentNote = duplicate;
                }
                ftlGatherer.ActiveNotes = newActiveNotes;
            }
            SaveLoadManager.MakeAMove("DuplicateToPaletteButton1");
            Debug.Log("I duplicated ftlGatherer.ActiveNotes.");
            NewStickyButton.thisSticky = thisStickyIsThisSticky;
        }
    }
}
