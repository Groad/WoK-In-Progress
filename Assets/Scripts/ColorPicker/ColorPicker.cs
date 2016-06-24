using UnityEngine;
using System.Collections;

public class ColorPicker : MonoBehaviour {

	public Texture2D colorSpace;
	public Texture2D alphaGradient;
	public string Title = "Color Picker";
	private Vector2 startPos = new Vector2();
	public GameObject receiver;
	public string colorSetFunctionName = "OnSetNewColor";
	public string colorGetFunctionName = "OnGetColor";
	public bool useExternalDrawer = false;
	public int drawOrder = 0;
	[HideInInspector]
	public StickyScript stickyScript { get; set; }
	private Color TempColor = new Color (1f,1f,1f,1f); 
	private Color SelectedColor;

	static ColorPicker activeColorPicker = null;
	public bool GO { get; set; }

	public enum Type {Sticky,Line,Handle}
	[HideInInspector]
	public static Type type;
	public ftlGatherer gatherer;
	public bool HideColorPicker 
	{ 
		get
		{
//			if (gatherer.ActiveNote != null && TempColor != null)
//			{
//				if (gatherer.ActiveNote.GetComponent<StickyScript>() != null)
//					gatherer.ActiveNote.GetComponent<StickyScript>().noteColor = TempColor;
//				else if (gatherer.ActiveNote.GetComponent<LineScript>() != null)
//				{
//					gatherer.ActiveNote.GetComponent<LineScript>().LineColor = TempColor;
//				}
//			}
			return hideCP;
		}
		set
		{
			hideCP = value;
		}
	}

	enum ESTATE
	{
		Hidden,
		Showed,
		Showing,
		Hidding
	}; 
	ESTATE mState = ESTATE.Hidden;
	
	int sizeFull = 120;
	int sizeHidden = 120;
	float animTime = 0.25f;
	float dt = 0;
	bool hideCP = false;

	float sizeCurr = 0;
	float alphaGradientHeight = 16;

	GUIStyle titleStyle = null;
	Color textColor = Color.black;
	Texture2D txColorDisplay;

	string txtR, txtG, txtB, txtA;
	float valR, valG, valB, valA;
	
	public void NotifyColor(Color color)
	{
		SetColor(color);
		SelectedColor = color;
		UpdateColorEditFields(false);
		UpdateColorSliders(false);
	}

	void Start()
	{
		HideColorPicker = true;
		var panel = transform.root.gameObject;
        //print (panel.name);
		var center = panel.transform.position;
		var screenPointCenter = Camera.main.WorldToScreenPoint (center);
		sizeCurr = sizeHidden;
		screenPointCenter.x -= (sizeCurr / 2 + 30);
		screenPointCenter.y += (sizeCurr / 2 + 30);

		startPos = new Vector2 (screenPointCenter.x, Screen.height - screenPointCenter.y);

		txColorDisplay = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		if(receiver)
		{
			receiver.SendMessage(colorGetFunctionName, this, SendMessageOptions.DontRequireReceiver);
		}
	}


	void OnGUI()
	{
		if(!useExternalDrawer && !HideColorPicker)
		{
			_DrawGUI();
		}
	}

	void UpdateColorSliders(bool isFocused)
	{
		if(!isFocused)
		{
			valR = TempColor.r;
			valG = TempColor.g;
			valB = TempColor.b;
			valA = TempColor.a;
		}
		else
		{
			SetColor(new Color(valR, valG, valB, valA));
		}
	}

	void UpdateColorEditFields(bool isFocused)
	{
		if(!isFocused)
		{
			txtR = (255 * TempColor.r).ToString();
			txtG = (255 * TempColor.g).ToString();
			txtB = (255 * TempColor.b).ToString();
			txtA = (255 * TempColor.a).ToString();
		}
		else
		{
			byte r = 0;
			byte g = 0;
			byte b = 0;
			byte a = 0;
			if(!string.IsNullOrEmpty(txtR)) {
				r = byte.Parse(txtR, System.Globalization.NumberStyles.Any);
			}
			if(!string.IsNullOrEmpty(txtG)) {
				g = byte.Parse(txtG, System.Globalization.NumberStyles.Any);
			}
			if(!string.IsNullOrEmpty(txtB)) {
				b = byte.Parse(txtB, System.Globalization.NumberStyles.Any);
			}
			if(!string.IsNullOrEmpty(txtA)) {
				a = byte.Parse(txtA, System.Globalization.NumberStyles.Any);
			}
			SetColor(new Color32(r, g, b, a));
		}
	}

	// Update is called once per frame
	public void _DrawGUI () 
	{
		if (titleStyle == null) {
			titleStyle = new GUIStyle (GUI.skin.label);
			titleStyle.normal.textColor = textColor;
		}

		Rect rectColorEdit = new Rect(startPos.x + sizeCurr + 10, startPos.y + 30, 40, 140);
		Rect rectColorSlider = new Rect(startPos.x + sizeCurr + 50, startPos.y + 30, 60, 140);

		//GUI.Label(new Rect(startPos.x + sizeCurr + 60, startPos.y, 200, 30), Title, titleStyle);

		GUI.DrawTexture(new Rect(startPos.x + sizeCurr + 10, startPos.y, 40, 20), txColorDisplay);
		if(GUI.Button(new Rect(startPos.x + sizeCurr + 10, startPos.y + sizeCurr, 60, 20), "Apply"))
		{
            if (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0)
            {
                GameObject activeNote = ftlGatherer.ActiveNotes[0];
                if (activeNote.GetComponent<StickyScript>() != null)
                {
                    activeNote.GetComponent<StickyScript>().noteColor = TempColor;
                }
                else if (activeNote.GetComponent<LineScript>() != null)
                    activeNote.GetComponent<LineScript>().LineColor = TempColor;
            }
			TempColor = new Color (1f, 1f, 1f, 1f);
			SelectedColor = TempColor;
			HideColorPicker = true;
		}

		//update scaling states
		if(mState == ESTATE.Showing)
		{
			sizeCurr = Mathf.Lerp(sizeHidden, sizeFull, dt/animTime);
			if(dt/animTime > 1.0f) {
				mState = ESTATE.Showed;
			}
			dt += Time.deltaTime;
		}
		if(mState == ESTATE.Hidding)
		{
			sizeCurr = Mathf.Lerp(sizeFull, sizeHidden, dt/animTime);
			if(dt/animTime > 1.0f) {
				mState = ESTATE.Hidden;
			}
			dt += Time.deltaTime;
		}
		//draw color picker
		Rect rect = new Rect(startPos.x, startPos.y, sizeCurr, sizeCurr);
		GUI.DrawTexture(rect, colorSpace);

		float alphaGradHeight = alphaGradientHeight * (sizeCurr/sizeFull);
		Vector2 startPosAlpha = startPos + new Vector2(0, sizeCurr);
		Rect rectAlpha = new Rect(startPosAlpha.x, startPosAlpha.y, sizeCurr, alphaGradHeight);
		GUI.DrawTexture(rectAlpha, alphaGradient);

		Rect rectFullSize = new Rect(startPos.x, startPos.y, sizeCurr, sizeCurr + alphaGradHeight);

		Vector2 mousePos = Event.current.mousePosition;
		Event e = Event.current;
		bool isLeftMBtnClicked = e.type == EventType.mouseUp;
		bool isLeftMBtnDragging = e.type == EventType.MouseDrag;
		bool openCondition = (rectFullSize.Contains(e.mousePosition) && (((e.type == EventType.MouseUp || e.type == EventType.mouseDrag || e.type == EventType.MouseMove) && e.isMouse)));
		bool closeCondition = isLeftMBtnClicked || (!rectFullSize.Contains(e.mousePosition)) && (e.isMouse && (e.type == EventType.MouseMove || e.type == EventType.MouseDown));
		if(openCondition && (activeColorPicker == null || activeColorPicker.mState == ESTATE.Hidden))
		{
			if(mState == ESTATE.Hidden)
			{
				mState = ESTATE.Showing;
				activeColorPicker = this;
				dt = 0;
			}
		}
		if(closeCondition)
		{
			if(mState == ESTATE.Showed)
			{
				if(isLeftMBtnClicked)
				{
					ApplyColor();
				}
				else
				{
					SetColor(SelectedColor);
				}

				mState = ESTATE.Hidding;
				dt = 0;
			}
		}
		if(mState == ESTATE.Showed)
		{
			if(rect.Contains(e.mousePosition))
			{
				float coeffX = colorSpace.width / sizeCurr;
				float coeffY = colorSpace.height / sizeCurr;
				Vector2 localImagePos = (mousePos - startPos);
				Color res = colorSpace.GetPixel((int)(coeffX * localImagePos.x), colorSpace.height - (int)(coeffY * localImagePos.y)-1);
				SetColor(res);
				if(isLeftMBtnDragging )
				{
					ApplyColor();
				}
				UpdateColorEditFields(false);
				UpdateColorSliders(false);
			}
			else if(rectAlpha.Contains(e.mousePosition))
			{
				float coeffX = alphaGradient.width / sizeCurr;
				float coeffY = alphaGradient.height / sizeCurr;
				Vector2 localImagePos = (mousePos - startPosAlpha);
				Color res = alphaGradient.GetPixel((int)(coeffX * localImagePos.x), colorSpace.height - (int)(coeffY * localImagePos.y)-1);
				Color curr = GetColor();
				curr.a = res.r;
				SetColor(curr);
				if(isLeftMBtnDragging)
				{
					ApplyColor();
				}
				UpdateColorEditFields(false);
				UpdateColorSliders(false);
			}
			else if(rectColorEdit.Contains(e.mousePosition))
			{
				UpdateColorEditFields(true);
				UpdateColorSliders(false);
			}
			else if(rectColorSlider.Contains(e.mousePosition))
			{
				UpdateColorEditFields(false);
				UpdateColorSliders(true);
			}
			else
			{
				SetColor(SelectedColor);

			}
		}
	}

	public void SetColor(Color color)
	{
		TempColor = color;
		if(txColorDisplay != null)
		{
			txColorDisplay.SetPixel(0, 0, TempColor);
			txColorDisplay.Apply();
		}
	}

	public Color GetColor()
	{
		return TempColor;
	}

	public void SetTitle(string title, Color textColor)
	{
		this.Title = title;
		this.textColor = textColor;
	}

	public void ApplyColor()
	{
	}
}
