using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteButton : EditorButton
{

    public override void press()
	{
        Delete();
	}

    public static void Delete()
    {
        if (ftlGatherer.ActiveNotes != null)
        {
            for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
            {
                if (ftlGatherer.ActiveNotes[i] != null && ftlGatherer.ActiveNotes[i].tag == "StickyNote")
                {
                    GameObject activeNote = ftlGatherer.ActiveNotes[i];
                    var activeSticky = activeNote.GetComponent<StickyScript>();
                    var keys = activeSticky.chains.Keys;
                    var keysList = new List<StickyScript>();

                    foreach (var key in keys)
                    {
                        keysList.Add(key);
                    }
                    foreach (var k in keysList)
                    {
                        activeSticky.destroyConnection(k, true);
                        k.destroyConnection(activeSticky, true);
                    }
                    activeSticky.enabled = false;
                    Destroy(activeNote);
                }
            }
            SaveLoadManager.MakeAMove("DeleteButton");
        }
        ftlGatherer.ActiveNotes = null;
    }
}
