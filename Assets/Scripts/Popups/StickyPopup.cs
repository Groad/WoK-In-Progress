using UnityEngine;
using System.Collections;

public class StickyPopup : Popup
{
	public static bool moving = false;
	public static int CurrentFontSize;
	public static int CurrentScale;
	public static bool isUp = false;
	public static float alpha = 0;
	public static Color guiColor = new Color (1f, 1f, 1f, 0f);
	public static GUIStyle style = new GUIStyle();
	public Texture2D buttonBG;
	public Texture2D hoverBG;
	public static StickyScript stickyScript;
	
	void OnEnable ()
	{
		CurrentFontSize = (int)(FullScale * ScaleFactor);
		CurrentScale = (int)(FullFontSize * ScaleFactor);
		style.fontSize = CurrentFontSize;
		style.normal.textColor = guiColor;
		style.alignment = TextAnchor.MiddleCenter;
		style.hover.background = hoverBG;
		
	}
	
	public void BringUpMenu (StickyScript _ss)
	{
		stickyScript = _ss;
		if (isUp)
			StartCoroutine (MenuClose());
		else
			StartCoroutine (MenuOpen());
	}
	
	void OnGUI()
	{
		if (stickyScript == null)
			return;
		style.fontSize = CurrentFontSize;
		style.normal.textColor = guiColor;
		style.normal.background = buttonBG;
		GUI.color = guiColor;
		var centroid = StickyScript.transformToObject(new Vector3(Screen.width / 2, 0, 0), gameObject);
		var scaleX = CurrentScale;
		var scaleY = (int)(CurrentScale * 0.2f);
		var offsetX = (int)(scaleX / 2);
		var offsetY = (int)(scaleY / 2);
		var ySpace = (int)(scaleY / 2) + scaleY;
		var GuiRect = (new Rect(centroid.x - offsetX, centroid.y - offsetY, scaleX, scaleY));
		if (GUI.Button(GuiRect, "Change Color", style))
		{
			var cp = Instantiate (stickyScript.colorPicker.gameObject) as GameObject;
			cp.GetComponent<ColorPicker>().stickyScript = stickyScript;
			cp.GetComponent<ColorPicker>().GO = true;
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Delete Sticky", style))
		{
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Clone Sticky", style))
		{
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
	}
	
	public IEnumerator MenuOpen ()
	{
		if (!moving) 
		{
			moving = true;
			float t = 0f;
			while (t < 1f) 
			{
				t += Time.deltaTime / Duration; // sweeps from 0 to 1 in time seconds
				CurrentScale = (int)Mathf.Lerp (FullScale * ScaleFactor, FullScale, t);
				CurrentFontSize = (int)Mathf.Lerp(FullFontSize * ScaleFactor, FullFontSize, t);
				guiColor.a = t;
				yield return 0;
			}
			moving = false;	
			isUp = true;
		}
	}
	
	public IEnumerator MenuClose ()
	{
		if (!moving) 
		{
			moving = true;
			float t = 0f;
			while (t < 1f) 
			{
				t += Time.deltaTime / Duration; // sweeps from 0 to 1 in time seconds
				CurrentScale = (int)Mathf.Lerp (FullScale, FullScale * ScaleFactor, t);
				CurrentFontSize = (int)Mathf.Lerp(FullFontSize, FullFontSize * ScaleFactor, t);
				guiColor.a = 1.0f - t;
				yield return 0;
			}
			moving = false;	
			isUp = false;
		}
		Destroy (gameObject);
	}
}

