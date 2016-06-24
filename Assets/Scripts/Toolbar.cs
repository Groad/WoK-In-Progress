using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Toolbar : MonoBehaviour
{

	void Start ()
    {
        var tbWidth = DropDownMenu.TopToolbarStrings.Length * 120;
        var space = Screen.width * 0.05f;
        DropDownMenu.space = space;
        GetComponent<DropDownMenu>().MainRect = new Rect(2, 0, tbWidth + space, 120);
    }

	void OnGUI ()
    {
        var groupRectT = GetComponent<DropDownMenu>().MainRect;
        GUI.Window(0, groupRectT, GetComponent<DropDownMenu>().InspectorWindowT, "", "Toolbar");
    }
}
