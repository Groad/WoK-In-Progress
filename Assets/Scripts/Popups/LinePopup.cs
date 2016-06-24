using UnityEngine;
using System.Collections;

public class LinePopup : Popup
{
	private bool moving = false;
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
		{
//			lineScript.gameObject.GetComponent<Collider> ().enabled = true;
//			lineScript.enabled = true;
			StartCoroutine (MenuClose());
		}
		else
		{
//			lineScript.enabled = false;
//			lineScript.gameObject.GetComponent<Collider> ().enabled = false;
			StartCoroutine (MenuOpen());
		}
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
		var ySpace = (int)(scaleY / 4) + scaleY;
		var GuiRect = (new Rect(centroid.x - offsetX, centroid.y - offsetY, scaleX, scaleY));
		if (GUI.Button(GuiRect, "Insert Handle", style))
		{
			lineScript.SplitLineHandle (lineScript.gameObject.transform.position);
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Insert Sticky", style))
		{
			lineScript.SplitLineSticky(lineScript.gameObject.transform.position);
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Delete Chain", style))
		{
			var sticky0 = lineScript.ThisChain.Sticky0;
			var sticky1 = lineScript.ThisChain.Sticky1;
			sticky0.GetComponent<StickyScript>().destroyConnection(sticky1.GetComponent<StickyScript>(), true);
			sticky1.GetComponent<StickyScript>().destroyConnection(sticky0.GetComponent<StickyScript>(), true);
			foreach (var link in lineScript.ThisChain.LinesInChain)
				Destroy (link);
			StartCoroutine (MenuClose());
		}
		GuiRect.y += ySpace;
		if (GUI.Button(GuiRect, "Cancel", style))
		{
			Destroy(gameObject);
		}
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
				CurrentScale = (int)Mathf.Lerp(FullScale * ScaleFactor, FullScale, t);
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
				CurrentScale = (int)Mathf.Lerp(FullScale, FullScale * ScaleFactor, t);
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
