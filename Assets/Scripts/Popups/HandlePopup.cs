using UnityEngine;
using System.Collections;

public class HandlePopup : Popup
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
	public static LineScript lineScript;
	
	void OnEnable ()
	{
		CurrentFontSize = (int)(FullScale * ScaleFactor);
		CurrentScale = (int)(FullFontSize * ScaleFactor);
		style.fontSize = CurrentFontSize;
		style.normal.textColor = guiColor;
		style.alignment = TextAnchor.MiddleCenter;
		style.hover.background = hoverBG;
		
	}
	
	public void BringUpMenu (LineScript _ls)
	{
		lineScript = _ls;
		if (isUp)
			StartCoroutine (MenuClose());
		else
			StartCoroutine (MenuOpen());
	}
	
	void OnGUI()
	{
		if (lineScript == null)
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
		if (GUI.Button(GuiRect, "Delete Handle", style))
		{
			lineScript.DeleteHandle (Vector3.zero, false);
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Insert Sticky", style))
		{
			lineScript.SplitLineSticky(transform.position);
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Delete Chain", style))
		{
			var sticky0 = lineScript.ThisChain.Sticky0;
			var sticky1 = lineScript.ThisChain.Sticky1;
			lineScript = null;
			sticky0.GetComponent<StickyScript>().destroyConnection(sticky1.GetComponent<StickyScript>(), true);
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
