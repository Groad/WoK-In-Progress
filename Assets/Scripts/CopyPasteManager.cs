using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

public class CopyPasteManager : MonoBehaviour
{
    private static string copied;
    public static Vector3 offset = new Vector3(-0.4f, -0.4f, 0f);
    public static int noOfPastesInARowCount = 0;

	void Start ()
    {
        copied = "";
    }
	
	void Update()
    {
	    if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Cut();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Copy();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Paste();
            }
        }
	}

    public static void Cut()
    {
        if (Copy())
        {
            DeleteButton.Delete();
        }
    }

    public static bool Copy()
    {
        if (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0].tag == "StickyNote")
        {
            noOfPastesInARowCount = 0;
            copied = SaveLoadManager.SaveString(false, ftlGatherer.ActiveNotes);
            Clipboard.SetText(copied);                                     
            return true;
        }
        return false;
    }

    public static void Paste()
    {
        if (copied != "")
        {
            noOfPastesInARowCount++;
            SaveLoadManager.LoadedText = copied;
            SaveLoadManager.Load(true);
            SaveLoadManager.MakeAMove("CopyPasteManager Pasted");
        }else                                                               
        {                                                                   
            string clipboard = Clipboard.GetText();                         
            if (clipboard.StartsWith("noOfStickiesInMain"))                 //checks Windows clipboard for stickynote data               
            {
                noOfPastesInARowCount++;                                                             
                SaveLoadManager.LoadedText = clipboard;                     
                SaveLoadManager.Load(true);                                 
                SaveLoadManager.MakeAMove("CopyPasteManager Pasted");       
            }                                                               
        }
    }
}
