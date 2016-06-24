using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FindBox : EditorButton
{
    public static FindBox instance;
    public string text;
    private string lastText;
    private TextMesh textMesh;
    private int textPointer;

	void Start ()
    {
        instance = this;
        text = "";
        lastText = "";
        textPointer = 0;
        textMesh = GetComponentInChildren<TextMesh>();
        press();
	}
	
	void Update ()
    {
        //type text
        string stickyText0 = text.Substring(0, textPointer);
        string stickyText1 = text.Substring(textPointer, text.Length - textPointer);
        foreach (char c in Input.inputString)
        {
            if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
            {
                if (c == '\b')
                {
                    if (stickyText0.Length > 0)
                    {
                        stickyText0 = stickyText0.Substring(0, stickyText0.Length - 1);
                    }
                    else
                    {
                        stickyText0 = "";
                        stickyText1 = "";
                    }
                }
                else if (c == '\n' || c == '\r')
                {
                    stickyText0 += "\n";
                }
                else
                {
                    stickyText0 += c;
                }
            }
        }
        textPointer = stickyText0.Length;
        text = stickyText0 + stickyText1;

        string stickyTextFirstPart = text.Substring(0, textPointer);
        string stickyTextSecondPart = text.Substring(textPointer, text.Length - textPointer);
        textMesh.text = stickyTextFirstPart + "<color=" + ((Time.realtimeSinceStartup % 1f < 0.5f) ? "#000000ff" : "#ffffffff") + ">|</color>" + stickyTextSecondPart;
        textMesh.fontSize = getFontSize(text.Split('\n'));

	    if (text != lastText)
        {
            lastText = text;
            textMesh.text = lastText;
            if (text != "")
            {
                //search
                ftlGatherer.ActiveNotes = new List<GameObject>();
                foreach (Transform child in SaveLoadManager.instance.transform)
                {
                    if (Contains(child.name, text))
                    {
                        ftlGatherer.ActiveNotes.Add(child.gameObject);
                    }
                }
                foreach (Transform child in SaveLoadManager.instance.stickiesInPalette)
                {
                    if (Contains(child.name, text))
                    {
                        ftlGatherer.ActiveNotes.Add(child.gameObject);
                    }
                }
            }
        }
	}

    public override void press()
    {
        if (!mousePressed)
        {
            enabled = !enabled;
            GetComponent<SpriteRenderer>().enabled = enabled;
            GetComponent<Collider>().enabled = enabled;
            textMesh.gameObject.SetActive(enabled);
            textMesh.text = "";
            text = "";
            textPointer = 0;
        }
    }
    private const float MaxCharsInSizeWithoutScaling = 12;
    private const float MaxNoOfLinesWithoutScaling = 1.2f;
    private const float FontSize = 35f;
    public int getFontSize(string[] lines)
    {
        int maxLineLength = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > maxLineLength)
            {
                maxLineLength = lines[i].Length;
            }
        }
        float horSize = (maxLineLength > MaxCharsInSizeWithoutScaling ? FontSize * MaxCharsInSizeWithoutScaling / maxLineLength : FontSize);
        float verSize = (lines.Length > MaxNoOfLinesWithoutScaling ? FontSize * MaxNoOfLinesWithoutScaling / lines.Length : FontSize);
        return Mathf.RoundToInt(Mathf.Min(horSize, verSize));
    }

    public static bool Contains(string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
    {
        return source.IndexOf(toCheck, comp) >= 0;
    }

    public static int IndexOf(string source, string toCheck, StringComparison comp = StringComparison.OrdinalIgnoreCase)
    {
        return source.IndexOf(toCheck, comp);
    }
}
