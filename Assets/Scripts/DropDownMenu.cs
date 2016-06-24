using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
//using System.Windows.Forms;

public class DropDownMenu : Toolbar
{
    public static string[] TopToolbarStrings = { "File" };//, "Edit", "Slice", "Window", "Help" };
    bool FileUp = false;
    private Rect mainRect;
    private Rect thisRect;
    private Rect fileRect;
    private Rect editRect;
    private Rect windowRect;
    private Rect sliceRect;
    private Rect helpRect;
    public GUISkin skin;
    public enum Dropdown { File, None };//, Edit, Window, Slice, Help, None };
    public Dropdown dropdown;
    private int width = 100;
    int index = 100;
    Ray ray;
    bool buttonPressed = false;
    int margin = Mathf.RoundToInt(Screen.width * 0.035f);//5;
    [DllImport("user32.dll")]
    private static extern void SaveFileDialog();
    public static float space = 0;

    public void InspectorWindowT(int id)
    {
        GUI.skin = skin;
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        GUILayout.BeginArea(mainRect);
        {
            GUILayout.BeginHorizontal(GUILayout.Width(width), GUILayout.Height(15));
            {
                GUILayout.Space(space);
                if (GUILayout.Button("<b>File</b>"))
                {
                    fileRect = GUILayoutUtility.GetLastRect();
                    index = 0;
                }
                else if (CheckForHover())
                {
                    fileRect = GUILayoutUtility.GetLastRect();
                    index = 0;
                }
                if (GUILayout.Button("<b>Edit</b>"))
                {
                    editRect = GUILayoutUtility.GetLastRect();
                    index = 1;
                }
                else if (CheckForHover())
                {
                    editRect = GUILayoutUtility.GetLastRect();
                    index = 1;
                }
                if (GUILayout.Button("<b>Slice</b>"))
                {
                    sliceRect = GUILayoutUtility.GetLastRect();
                    index = 2;
                }
                else if (CheckForHover())
                {
                    sliceRect = GUILayoutUtility.GetLastRect();
                    index = 2;
                }
                if (GUILayout.Button("<b>Help</b>"))
                {
                    helpRect = GUILayoutUtility.GetLastRect();
                    index = 3;
                }
                else if (CheckForHover())
                {
                    helpRect = GUILayoutUtility.GetLastRect();
                    index = 3;
                }
                if (GUILayout.Button("<b>Window</b>"))
                {
                    windowRect = GUILayoutUtility.GetLastRect();
                    index = 4;
                }
                else if (CheckForHover())
                {
                    windowRect = GUILayoutUtility.GetLastRect();
                    index = 4;
                }
            }
            GUILayout.EndHorizontal();

            switch (dropdown)
            {
                case Dropdown.File:
                    _File(fileRect, 5);
                    index = 0;
                    break;
                //case Dropdown.Edit:
                //    _Edit(editRect, 1);
                //    index = 1;
                //    break;
                //case Dropdown.Slice:
                //    _Slice(sliceRect, 3);
                //    index = 2;
                //    break;
                //case Dropdown.Help:
                //    _Help(helpRect, 4);
                //    index = 3;
                //    break;
                //case Dropdown.Window:
                //    _Window(windowRect, 3);
                //    index = 4;
                //    break;
                default:
                    break;
            }
        }
        GUILayout.EndArea();
    }

    void Update()
    {
        switch (index)
        {
            case 0:
                dropdown = Dropdown.File;
                break;
            //case 1:
            //    dropdown = Dropdown.Edit;
            //    break;
            //case 2:
            //    dropdown = Dropdown.Slice;
            //    break;
            //case 3:
            //    dropdown = Dropdown.Help;
            //    break;
            //case 4:
            //    dropdown = Dropdown.Window;
            //    break;
            default:
                dropdown = Dropdown.None;
                break;
        }
    }
    public Rect MainRect
    {
        get { return mainRect; }
        set
        {
            mainRect = value;
            thisRect = new Rect(margin, margin, mainRect.width - (margin * 2), mainRect.height - (margin * 2));
        }
    }

    public bool CheckForHover()
    {
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= GUILayoutUtility.GetLastRect().xMax
            && mp.x >= GUILayoutUtility.GetLastRect().xMin
            && mp.y <= GUILayoutUtility.GetLastRect().yMax
            && mp.y >= GUILayoutUtility.GetLastRect().yMin)
        {
            return true;
        }
        return false;
    }
    public void _File(Rect rect, int slots)
    {
        rect.height += slots * rect.height;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(rect.xMin);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Save", "dd"))
                {
                    // Talha - Insert Code To Execute Here
                    SaveLoadManager.SavePress();
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Save As", "dd"))
                {
                    SaveLoadManager.SaveAsPress();
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Clear", "dd"))
                {
                    SaveLoadManager.ClearPress();
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Load", "dd"))
                {
                    SaveLoadManager.LoadPress();
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Quit", "dd"))
                {
                    Application.Quit();
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= rect.xMax
            && mp.x >= rect.xMin
            && mp.y <= rect.yMax
            && mp.y >= rect.yMin)
        {
            index = 0;
        }
        else
        {
            index = 100;
            dropdown = Dropdown.None;
        }
    }

    void _Edit(Rect rect, int slots)
    {
        rect.height += slots * rect.height;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(rect.xMin);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Fix", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= rect.xMax
            && mp.x >= rect.xMin
            && mp.y <= rect.yMax
            && mp.y >= rect.yMin)
        {
            index = 1;
        }
        else
        {
            index = 100;
            dropdown = Dropdown.None;
        }
    }

    void _Slice(Rect rect, int slots)
    {
        rect.height += slots * rect.height;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(rect.xMin);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Slice", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Slicer Panel", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Paths", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= rect.xMax
            && mp.x >= rect.xMin
            && mp.y <= rect.yMax
            && mp.y >= rect.yMin)
        {
            index = 2;
        }
        else
        {
            index = 100;
            dropdown = Dropdown.None;
        }
    }

    void _Help(Rect rect, int slots)
    {
        rect.height += slots * rect.height;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(rect.xMin);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Manual", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Keyboard Commands", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("About", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= rect.xMax
            && mp.x >= rect.xMin
            && mp.y <= rect.yMax
            && mp.y >= rect.yMin)
        {
            index = 3;
        }
        else
        {
            index = 100;
            dropdown = Dropdown.None;
        }
    }
    public void _Window(Rect rect, int slots)
    {
        rect.height += slots * rect.height;
        //rect.x += 2 * rect.width;
        //if (dropdown != Dropdown.File) return;
        GUILayout.BeginHorizontal();
        {
            GUILayout.Space(rect.xMin);
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Inspector", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Voxel Manager", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
                if (GUILayout.Button("Temp History", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }

                if (GUILayout.Button("Main Panel", "dd"))
                {
                    index = 100;
                    dropdown = Dropdown.None;
                    return;
                }
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        var mp = Input.mousePosition;
        mp.y = UnityEngine.Screen.height - mp.y;
        if (mp.x <= rect.xMax
            && mp.x >= rect.xMin
            && mp.y <= rect.yMax
            && mp.y >= rect.yMin)
        {
            index = 4;
        }
        else
        {
            index = 100;
            dropdown = Dropdown.None;
        }
    }
}

