using UnityEngine;
using System.Collections;

public class ToolbarEditMenu : MonoBehaviour
{
    public ToolbarButton gridMarkers;
    public ToolbarButton snapToGrid;
    public ToolbarButton undo;
    public ToolbarButton redo;
    public ToolbarButton detectMonitors;
    public ToolbarButton undetectMonitors;
    public ToolbarButton cut;
    public ToolbarButton copy;
    public ToolbarButton paste;
    public ToolbarButton extraInstance;
    public GameObject grid;
	
	void Update ()
    {
        if (gridMarkers.pressed)
        {
            grid.SetActive(!grid.activeSelf);
        }
        if (snapToGrid.pressed)
        {
            GridSnapper.snapToGrid = !GridSnapper.snapToGrid;
        }
        if (undo.pressed)
        {
            SaveLoadManager.Undo();
        }
        if (redo.pressed)
        {
            SaveLoadManager.Redo();
        }
        if (detectMonitors != null && detectMonitors.pressed)
        {
            CameraFixer.detectMonitors();
        }
        if (undetectMonitors != null && undetectMonitors.pressed)
        {
            CameraFixer.undetectMonitors();
        }
        if (cut.pressed)
        {
            CopyPasteManager.Cut();
        }
        if (copy.pressed)
        {
            CopyPasteManager.Copy();
        }
        if (paste.pressed)
        {
            CopyPasteManager.Paste();
        }
        if (UDPManager.isMain && extraInstance.pressed)
        {
            if (!UDPManager.instance.initiliazed)
            {
                UDPManager.instance.StartMain();
            }
            //boot up exe
            UDPManager.instance.StartExtraInstance();
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SaveLoadManager.Undo();
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SaveLoadManager.Redo();
            }
        }
	}
}
