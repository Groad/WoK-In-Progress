using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class StickyScript : MonoBehaviour
{
    public bool isPanelShowOff;

	public static bool moving = false;
	public static TouchScreenKeyboard keyboard;
	public static bool closed = false;
	public LineRenderer line;
	public ColorPicker colorPicker;
	public StickyPopup stickyPopup;
	public GameObject stickySenderObject;
	[HideInInspector]
	public StickySender stickySender;
	private GameObject ProjectionSurface;
	private Camera ProjectionCamera;
    public static StickyPopup _stickyPopup;
    private TextMesh textMesh;
    private List<TextMesh> shadowTextMeshes;
    public bool shadowsOn;

	public static Vector3 LargeScale = new Vector3();
	public static Vector3 SmallScale = new Vector3();
	public float shrinkFactor = 0.2f;
	public float time = 0.2f;
    public int noteColorId;
	public Color noteColor = new Color(1f, 1f, 1f, 1f);
	public static Color pressedColor = new Color (0.6f, 1f, 0.6f, 1f);
	public static float timeCounter = 0;
	public static bool soundEffects = true;
	public static int fontSize = 48;
	public static GUIStyle keyTextStyle;
	
	public static byte[] receivedBytes;
	public static byte[] N;
	public static Texture2D texi;
	public static Texture2D receivedTexture;

	public Texture2D MyTexture {get; set; }
	public string StickyText { get; internal set; }
	public string User { get; internal set; }
	public DateTime DateTime { get; internal set; }
    private static GameObject lastDrawnSticky;

    private float lastTimeEditedTex;
    private float shapeUpdateTime;
    public float customTextScale = 1;
    public const float MinCustomTextScale = 0.4f;
    public const float MaxCustomTextScale = 4f;

    public float realScale = 1f;//read only

    public const int MaxSpriteIndex = 3;
    private int _spriteIndex;
    public int spriteIndex
    {
        get
        {
            return _spriteIndex;
        }
        set
        {
            if (_spriteIndex != value)
            {
                shapeUpdateTime = Time.realtimeSinceStartup;
            }
            _spriteIndex = value;
            GetComponent<SpriteRenderer>().sprite = spritesSticky[_spriteIndex];
            isAssymmetricScaling = assymmetricScale[_spriteIndex];
            foreach (Transform child in transform)
            {
                if (child.name == "Highlight")
                {
                    child.GetComponent<SpriteRenderer>().sprite = spritesHighlight[_spriteIndex];
                }
            }
            if (shapeUpdateTime > lastTimeEditedTex)
            {
                editTexForShape();
            }
        }
    }
    public Sprite[] spritesSticky;
    public Sprite[] spritesHighlight;
    public Texture2D[] masks;
    public bool[] assymmetricScale;
    public int photoRotateCount;
    private bool isAssymmetricScaling;

    public float lastUpdateTime;
    private float lastUpdateTimeVectorInfo;
    private string stickyInfo;

    public SpriteRenderer photo;
    public bool isTherePhoto;
    private const float PhotoTexWidth = 257f;
    private Vector2[] MaskBounds;
    private float photoUpdateTime;
    public Texture2D uneditedTex;
    public Texture2D editedTex;

    public string imagePath = "";

    public int noOfLines;
    private int noOfLinesOld;
    //private TextLine textUI;
    public GameObject textPrefab;
    private Vector3 firstScale;
    private Vector3 textMeshFirstScale;
    public int textPointer;
    private bool refreshTextScale;

    public bool ableToScale;
    public GameObject scalerPrefab;
    private StickyScaler scaler;
    public float customScale = 1f;
    public const float PhotoZ = 0.01f;
    public const float FingerPaintZ = 0.05f;
    private const float CustomScaleMax = 5f;
    public static bool theresNewTextOnStickies = false;
    private float textHorSize;
    private float textVerSize;
    private float fontSizeLatest = FontSize;
    public SpriteRenderer rectangleGreySprite;
    private GameObject highlight;
    public int layerNo;
    public const int MaxLayerNo = 2;

	void OnEnable()
	{
        refreshTextScale = true;
        firstScale = transform.localScale;
        if (SceneManager.isDesktopScene)
        {
		    ProjectionSurface = GameObject.Find ("MainPanel");
		    ProjectionCamera = GameObject.Find ("MainPanelCamera").GetComponent<Camera>();
		    stickySender = ProjectionCamera.gameObject.GetComponent<StickySender> ();
        }

        //GameObject t = Instantiate(textPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //t.transform.SetParent(Canvass.cTransform);
        //textUI = t.GetComponent<TextLine>();
        //textUI.text.text = name;
        //textUI.ss = this;
        //textUI.lineNo = 0;

        //MaskBounds = new Vector2[] { new Vector2(255f, 255f), new Vector2(298f, 298f), new Vector2(281f, 281f), new Vector2(263f, 236f) };
        MaskBounds = new Vector2[] { new Vector2(255f, 255f), new Vector2(380f, 380f), new Vector2(281f, 281f), new Vector2(263f, 236f) };

        textMesh = GetComponentInChildren<TextMesh>();
        shadowTextMeshes = new List<TextMesh>();
        foreach (Transform child in textMesh.transform)
        {
            shadowTextMeshes.Add(child.GetComponent<TextMesh>());
        }
        textHorSize = textMesh.fontSize;
        textVerSize = textMesh.fontSize;
        textMeshFirstScale = textMesh.transform.localScale;
		LargeScale = transform.localScale;
		SmallScale = LargeScale * shrinkFactor;
		keyTextStyle = new GUIStyle();
		keyTextStyle = new GUIStyle();
		keyTextStyle.alignment = TextAnchor.MiddleCenter;
        //ftlGatherer.ActiveNotes = new List<GameObject>();
        //ftlGatherer.ActiveNotes.Add(gameObject);
        if (SceneManager.isDesktopScene)
        {
            stickySender.stickyHit += stickyHit;
        }
        HighlightSticky highlightScript = GetComponentInChildren<HighlightSticky>();
        if (highlightScript != null)
        {
            highlight = highlightScript.gameObject;
        }
        scaleStickyForText();
	}

    void Start()
    {
        textPointer = name.Length;
        scaleStickyForText();

        if (ableToScale)
        {
            GameObject scalerGO = Instantiate(scalerPrefab) as GameObject;
            scalerGO.transform.parent = transform;
            scalerGO.transform.localScale = Vector3.one;
            scalerGO.transform.localPosition = Vector3.forward * 1f + (Vector3.right + Vector3.down) * 1.1f;
            if (spriteIndex == 1)
            {
                scalerGO.transform.Rotate(new Vector3(0f, 0f, 45f));
            }
            scaler = scalerGO.GetComponent<StickyScaler>();
        }
    }
	
	void OnDisable()
    {
        if (SceneManager.isDesktopScene)
        {
            stickySender.stickyHit -= stickyHit;
        }
	}

	public void press()
	{
		if (!Input.GetMouseButton(1))
        {
            selectedForConnection = null;
            StickyText = name;
            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
            timeCounter = Time.realtimeSinceStartup;
            //ftlGatherer.ActiveNotes = new List<GameObject>();
            //ftlGatherer.ActiveNotes.Add(gameObject);
        }
	}
	
	public static Vector3 transformToObject (Vector3 _vertex, GameObject _object)
	{
		var ratio = new Vector2 (_object.GetComponent<Collider>().bounds.size.x / Screen.width , _object.GetComponent<Collider>().bounds.size.y / Screen.height);
		_vertex.x = _object.GetComponent<Collider>().bounds.center.x + (ratio.x * _vertex.x) - _object.GetComponent<Collider>().bounds.size.x / 2;
		_vertex.y = -_object.GetComponent<Collider>().bounds.center.y + (ratio.y * _vertex.y) - _object.GetComponent<Collider>().bounds.size.y / 2;
		_vertex = Camera.main.WorldToScreenPoint (_vertex);		
		return _vertex;
	}

	void Update()
    {
        GetComponent<SpriteRenderer>().color = (selectedForConnection == this ? Color.red : noteColor);
        if (isPanelShowOff)
        {
            if (NewStickyButton.thisSticky != null && ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0] != null && NewStickyButton.thisSticky != ftlGatherer.ActiveNotes[0])
            {
                Destroy(NewStickyButton.thisSticky);
            }
            bool drawPanelSticky = (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0] != null && ftlGatherer.ActiveNotes[0].GetComponent<StickyScript>() != null && (ftlGatherer.ActiveNotes[0].GetComponent<StickyInPanel>() == null || ftlGatherer.ActiveNotes[0].GetComponent<StickyInPanel>().sentToPalette));
            if (drawPanelSticky)
            {
                if (ftlGatherer.ActiveNotes != null && ftlGatherer.ActiveNotes.Count > 0 && ftlGatherer.ActiveNotes[0] != null)
                {
                    GameObject activeNote = ftlGatherer.ActiveNotes[0];
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    GetComponent<SpriteRenderer>().enabled = true;
                    StickyScript an = activeNote.GetComponent<StickyScript>();
                    name = an.name;
                    textPointer = an.textPointer;
                    string stickyTextFirstPart = name.Substring(0, textPointer);
                    string stickyTextSecondPart = name.Substring(textPointer, name.Length - textPointer);
                    textMesh.text = stickyTextFirstPart + "<color=" + ((Time.realtimeSinceStartup % 1f < 0.5f) ? "#000000ff" : "#ffffffff") + ">|</color>" + stickyTextSecondPart;
                    textMesh.fontSize = an.textMesh.fontSize;
                    if (shadowsOn)
                    {
                        for (int i = 0; i < shadowTextMeshes.Count; i++)
                        {
                            shadowTextMeshes[i].text = textMesh.text;
                            shadowTextMeshes[i].fontSize = textMesh.fontSize;
                        }
                    }
                    noteColor = an.noteColor;
                    customTextScale = an.customTextScale;
                    noteColorId = an.noteColorId;
                    spriteIndex = an.spriteIndex;
                    getFontSize(name.Split('\n'));
                    scaleStickyForText();
                    if (lastDrawnSticky != activeNote)
                    {
                        if (!an.isTherePhoto)
                        {
                            deletePhoto();
                        }
                        photoUpdateTime = 0f;
                        shapeUpdateTime = 0f;
                        lastDrawnSticky = activeNote;
                        GetComponent<StickyScript>().getDrawLines(activeNote.transform);
                    }
                    else if (an != null)
                    {
                        if (an != null && lastUpdateTime != an.lastUpdateTime && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
                        {
                            if (lastUpdateTime > an.lastUpdateTime)
                            {
                                //an.readStickyInfo(stickyInfoToString(), lastUpdateTime);
                                an.GetComponent<StickyScript>().getDrawLines(transform);
                            }
                            else
                            {
                                //readStickyInfo(an.stickyInfoToString(), an.lastUpdateTime);
                                GetComponent<StickyScript>().getDrawLines(an.transform);
                            }
                        }
                    }
                    SyncPhotoInfo(this, an);
                }
            }
            else
            {
                textMesh.text = "";
                if (shadowsOn)
                {
                    for (int i = 0; i < shadowTextMeshes.Count; i++)
                    {
                        shadowTextMeshes[i].text = textMesh.text;
                    }
                }
                lastDrawnSticky = null;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            if (ViewControl.calculateMaxMin > 0)
            {
                if (minX() < ViewControl.stickieMinX)
                {
                    ViewControl.stickieMinX = minX();
                }
                else if (maxX() > ViewControl.stickieMaxX)
                {
                    ViewControl.stickieMaxX = maxX();
                }
                if (minY() < ViewControl.stickieMinY)
                {
                    ViewControl.stickieMinY = minY();
                }
                else if (maxY() > ViewControl.stickieMaxY)
                {
                    ViewControl.stickieMaxY = maxY();
                }
            }

            if (ftlGatherer.isAnActiveNote(gameObject) && (FindBox.instance == null || !FindBox.instance.enabled))
            {
#if UNITY_ANDROID
                if (TabletServerSide.touchScreenKeyboard != null && TouchScreenKeyboard.visible)
                {
                    name = TabletServerSide.touchScreenKeyboard.text;
                    textPointer = name.Length;
                }
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                string stickyText0 = name.Substring(0, textPointer);
                string stickyText1 = name.Substring(textPointer, name.Length - textPointer);
                foreach (char c in Input.inputString)
                {
                    if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl))
                    {
                        if (c == '\b')
                        {
                            if (stickyText0.Length > 0)
                            {
                                stickyText0 = stickyText0.Substring(0, stickyText0.Length - 1);
                                theresNewTextOnStickies = true;
                            }
                        }
                        else if (c == '\n' || c == '\r')
                        {
                            stickyText0 += "\n";
                            theresNewTextOnStickies = true;
                        }
                        else
                        {
                            stickyText0 += c;
                            theresNewTextOnStickies = true;
                        }
                    }
                }
                textPointer = stickyText0.Length;
                name = stickyText0 + stickyText1;
                StickyText = name;
#endif
                string[] textLines = name.Split('\n');
                noOfLines = textLines.Length;
                if (isPanelShowOff && !GetComponent<SpriteRenderer>().enabled)
                {
                    noOfLines = 0;
                    textMesh.text = "";
                    if (shadowsOn)
                    {
                        for (int i = 0; i < shadowTextMeshes.Count; i++)
                        {
                            shadowTextMeshes[i].text = textMesh.text;
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        textPointer = Mathf.Max(textPointer - 1, 0);
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        textPointer = Mathf.Min(textPointer + 1, name.Length);
                    }

                    string stickyTextFirstPart = name.Substring(0, textPointer);
                    string stickyTextSecondPart = name.Substring(textPointer, name.Length - textPointer);
                    textMesh.text = stickyTextFirstPart + "<color=" + ((Time.realtimeSinceStartup % 1f < 0.5f) ? "#000000ff" : "#ffffffff") + ">|</color>" + stickyTextSecondPart;
                    
                }
                /*textMesh.fontSize = */getFontSize(textLines);
                scaleStickyForText();
                if (shadowsOn)
                {
                    for (int i = 0; i < shadowTextMeshes.Count; i++)
                    {
                        shadowTextMeshes[i].text = textMesh.text;
                        shadowTextMeshes[i].fontSize = textMesh.fontSize;
                    }
                }

                refreshTextScale = true;

                if (ableToScale)
                {
                    scaler.stickyActive = true;
                }
            }
            else
            {
                if (FindBox.instance != null && FindBox.instance.enabled)
                {
                    int index = FindBox.IndexOf(name, FindBox.instance.text);
                    if (index != -1)
                    {
                        string textMeshText = name.Substring(0, index) + "<color=#ff0000ff>" + name.Substring(index, FindBox.instance.text.Length) + "</color>" + name.Substring(index + FindBox.instance.text.Length);
                        textMesh.text = textMeshText;
                    }
                    else
                    {
                        textMesh.text = name;
                    }
                }
                else
                {
                    textMesh.text = name;
                }
                if (refreshTextScale)
                {
                    refreshTextScale = false;
                    //textUI.text.text = name;
                    textMesh.text = name;
                    string[] textLines = name.Split('\n');
                    /*textMesh.fontSize = */getFontSize(textLines);
                    scaleStickyForText();
                }
                if (ableToScale)
                {
                    scaler.stickyActive = false;
                }
                if (shadowsOn)
                {
                    for (int i = 0; i < shadowTextMeshes.Count; i++)
                    {
                        shadowTextMeshes[i].text = name;
                        shadowTextMeshes[i].fontSize = textMesh.fontSize;
                    }
                }
            }
        }
        if (shadowsOn)
        {
            shadowTextMeshes[0].transform.position = textMesh.transform.position - 0.02f * Vector3.right - 0.02f * Vector3.up;
            shadowTextMeshes[1].transform.position = textMesh.transform.position - 0.02f * Vector3.right - 0.02f * Vector3.up;
            shadowTextMeshes[2].transform.position = textMesh.transform.position - 0.02f * Vector3.right - 0.02f * Vector3.up;
            shadowTextMeshes[3].transform.position = textMesh.transform.position - 0.02f * Vector3.right - 0.02f * Vector3.up;
        }
	}

    public void scaleStickyForText()
    {
        if (GetComponent<StickyInPanel>() == null && !isPanelShowOff)
        {
            float fontSize = fontSizeLatest;
            float fontSizeRatio = FontSize / fontSize;
            realScale = Mathf.Max(customScale, fontSizeRatio);
            Vector3 newScale = firstScale * realScale;
            float fontSizeRatioHor = FontSize / textHorSize;
            float fontSizeRatioVer = FontSize / textVerSize;
            if (isAssymmetricScaling)
            {
                newScale = new Vector3(Mathf.Max(customScale, fontSizeRatioHor), Mathf.Max(customScale, fontSizeRatioVer), 1f);
            }
            if (!float.IsInfinity(newScale.x) && !float.IsNaN(newScale.x))
            {
                if ((!isAssymmetricScaling && customScale > fontSizeRatio) || (isAssymmetricScaling && (customScale > fontSizeRatioHor || customScale > fontSizeRatioVer)))
                {
                    textMesh.transform.localScale = customTextScale * textMeshFirstScale / customScale * fontSizeRatio;
                    if (isAssymmetricScaling)
                    {
                        if (customScale < fontSizeRatioHor)
                        {
                            //Debug.Log(">hor" + UnityEngine.Random.value);
                            textMesh.transform.localScale = new Vector3(textMesh.transform.localScale.x / fontSizeRatioHor * customScale, textMesh.transform.localScale.y, textMesh.transform.localScale.z);
                        }
                        if (customScale < fontSizeRatioVer)
                        {
                            //Debug.Log(">ver" + UnityEngine.Random.value);
                            textMesh.transform.localScale = new Vector3(textMesh.transform.localScale.x, textMesh.transform.localScale.y / fontSizeRatioVer * customScale, textMesh.transform.localScale.z);
                        }
                    }
                }
                else
                {
                    if (isAssymmetricScaling)
                    {
                        //Debug.Log("<" + UnityEngine.Random.value);
                        textMesh.transform.localScale = new Vector3(customTextScale * textMeshFirstScale.x / fontSizeRatioHor * fontSizeRatio, customTextScale * textMeshFirstScale.y / fontSizeRatioVer * fontSizeRatio, 1f);
                    }
                    else
                    {
                        textMesh.transform.localScale = customTextScale * textMeshFirstScale;
                    }
                }
                textMesh.transform.localScale *= fontSizeLatest / FontSize; 
                textMesh.transform.localScale = new Vector3(textMesh.transform.localScale.x, textMesh.transform.localScale.y, 1f);
                transform.localScale = new Vector3(newScale.x, newScale.y, 1f);
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<DrawLine>() != null)
                    {
                        child.position = new Vector3(child.position.x, child.position.y, transform.position.z + FingerPaintZ * numberSign(child.localPosition.z));
                    }
                    else if (child.name.Contains("Photo"))
                    {
                        child.position = new Vector3(child.position.x, child.position.y, transform.position.z + PhotoZ * numberSign(child.localPosition.z));
                    }
                }
            }
        }
        else
        {
            if (isAssymmetricScaling)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x * OblongShapeConst, transform.localScale.z);
                textMesh.transform.localScale = customTextScale * new Vector3(textMeshFirstScale.x, textMeshFirstScale.y / OblongShapeConst, textMeshFirstScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.z);
                textMesh.transform.localScale = customTextScale * textMeshFirstScale;
            }
            textMesh.transform.localScale *= fontSizeLatest / FontSize;
            textMesh.transform.localScale = new Vector3(textMesh.transform.localScale.x, textMesh.transform.localScale.y, 1f);
        }

        rectangleGreySprite.gameObject.SetActive(isAssymmetricScaling);
        if (isAssymmetricScaling)
        {
            if (GetComponent<StickyInPanel>() == null && !isPanelShowOff)
            {
                rectangleGreySprite.transform.position = transform.position - Vector3.forward * 0.001f;
            }
            else
            {
                rectangleGreySprite.transform.position = transform.position + Vector3.forward * 0.001f;
            }
            rectangleGreySprite.transform.localScale = new Vector3(1f + 0.25f / transform.lossyScale.x, 1f + 0.25f / transform.lossyScale.y, 1f);
            //rectangleGreySprite.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            if (highlight != null)
            {
                highlight.transform.localPosition = rectangleGreySprite.transform.localPosition * 2f;
                highlight.transform.localScale = new Vector3(1f + 0.5f / transform.lossyScale.x, 1f + 0.5f / transform.lossyScale.y, 1f);
                //highlight.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
        }
        else
        {
            if (highlight != null)
            {
                highlight.transform.localScale = Vector3.one;
            }
        }
    }

    public static float numberSign(float n)
    {
        if (n >= 0f)
        {
            return 1f;
        }
        return -1f;
    }

    public static StickyScript selectedForConnection;
    public Dictionary<StickyScript, Chain> chains;
    public Color lineColor = Color.white;
    public static bool shouldUpdateLines = false;

    void Awake()
    {
        noteColor = ColorPickerDiscreet.getColor(0);
        chains = new Dictionary<StickyScript, Chain>();
        name = "";
    }

    public void createConnection(StickyScript _sticky)
    {
		selectedForConnection = null;
        if (!chains.ContainsKey(_sticky))
        {
			addLineRenderer(_sticky);
        }
    }

    public void destroyConnection(StickyScript _sticky, bool _destroy)
    {
        if (chains.ContainsKey(_sticky))
        {
			if (_destroy)
			foreach (var link in chains[_sticky].LinesInChain)
			{
				if (link != null && link.gameObject != null)
					Destroy (link.gameObject);
			}
			chains.Remove(_sticky);
        }
        if (_sticky.chains.ContainsKey(this))
        {
			if (_destroy)
			foreach (var link in _sticky.chains[this].LinesInChain)
			{
				if (link != null && link.gameObject != null)
					Destroy (link.gameObject);
			}
			_sticky.chains.Remove(this);
		}
    }

	public void RerouteConnection (StickyScript _newSticky, Chain _chain0, Chain _chain1, StickyScript _otherSticky, GameObject _delete)
	{
		if (chains.ContainsKey(_otherSticky))
		{
			if (_delete != null)
				Destroy (_delete);
			foreach (var link0 in _chain0.LinesInChain)
				link0.ThisChain = _chain0;
			foreach (var link1 in _chain1.LinesInChain)
				link1.ThisChain = _chain1;
			_chain0.Sticky0 = gameObject;
			_chain0.Sticky1 = _newSticky.gameObject;
			_chain1.Sticky0 = _newSticky.gameObject;
			_chain1.Sticky1 = _otherSticky.gameObject;
			chains.Remove (_otherSticky);
			chains.Add (_newSticky, _chain0);
			_otherSticky.chains.Remove(this);
			_otherSticky.chains.Add (_newSticky, _chain1);
		}
	}

    public void addLineRenderer(StickyScript _sticky)
    {
		var lineObject = Instantiate (line, Vector3.zero, Quaternion.identity) as LineRenderer;
        LineScript lineScript = lineObject.GetComponent<LineScript>();
        lineScript.transform.SetParent(SaveLoadManager.instance.connections);
        lineScript.Endpoint0 = this.gameObject;
        lineScript.Endpoint1 = _sticky.gameObject;
        lineScript.Go = true;
        lineScript.enabled = true;
		var newChain = new Chain ();
		newChain.Sticky0 = gameObject;
		newChain.Sticky1 = _sticky.gameObject;
		newChain.LinesInChain = new List<LineScript> ();
		newChain.LinesInChain.Add (lineObject.GetComponent<LineScript> ());
		lineObject.GetComponent<LineScript> ().ThisChain = newChain;
		if (!chains.ContainsKey (_sticky))
			chains.Add (_sticky, newChain);
		else
			chains [_sticky] = newChain;
		if (!_sticky.chains.ContainsKey (this))
			_sticky.chains.Add (this, newChain);
		else
			_sticky.chains [this] = newChain;
    }

	private void stickyHit (GameObject _hit, StickySender.ButtonClicked _button)
	{
		if (gameObject != null && _hit != gameObject)
			return;
		if (_button == StickySender.ButtonClicked.Left)
		{
		}
		if (_button == StickySender.ButtonClicked.Right)
		{
			if (selectedForConnection == null)
			{
				selectedForConnection = this;
			}
			else
			{
				if (selectedForConnection != this)
				{
					if (chains.ContainsKey(selectedForConnection))
					{
						destroyConnection(selectedForConnection, true);
					}
					else
					{
						createConnection(selectedForConnection);
                    }
                    SaveLoadManager.MakeAMove("StickyScript0");
				}
				selectedForConnection = null;
			}
		}
	}

    public void getDrawLines(Transform other)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "DrawLine")
            {
                Destroy(child.gameObject);
            }
        }
        List<Transform> lines = new List<Transform>();
        foreach (Transform child in other)
        {
            if (child.tag == "DrawLine")
            {
                lines.Add(child);
            }
        }
        for (int i = 0; i < lines.Count; i++)
        {
            GameObject line = Instantiate(lines[i].gameObject) as GameObject;
            line.name = "DrawLine";
            line.transform.SetParent(transform);
            GetComponent<StickyFingerPaint>().setDrawLineScale(line);
        }
        if (other.GetComponent<StickyScript>() != null)
        {
            lastUpdateTime = other.GetComponent<StickyScript>().lastUpdateTime;
        }
    }

    public void copyAllInfoTo(StickyScript other)
    {
        other.StickyText = StickyText;
        other.name = name;
        other.noteColorId = noteColorId;
        other.noteColor = noteColor;
        other.spriteIndex = spriteIndex;
        other.customScale = customScale;
        other.customTextScale = customTextScale;
        other.getDrawLines(transform);
        other.uneditedTex = uneditedTex;
        other.photoRotateCount = photoRotateCount;
        other.layerNo = layerNo;
        givePhotoInfo(other);
    }

    public static void SyncPhotoInfo(StickyScript s0, StickyScript s1)
    {
        if (s0.photoUpdateTime > s1.photoUpdateTime)
        {
            s0.givePhotoInfo(s1);
        }
        else if (s1.photoUpdateTime > s0.photoUpdateTime)
        {
            s1.givePhotoInfo(s0);
        }
    }

    public void givePhotoInfo(StickyScript other)
    {
        other.imagePath = imagePath;
        if (isTherePhoto)
        {
            other.putPhoto(photo.sprite);
        }
        else
        {
            other.deletePhoto();
        }
        other.photoUpdateTime = photoUpdateTime;
    }

    public string stickyInfoToString(bool includePosition = false)
    {
        stickyInfo = "";
        stickyInfo += gameObject.name;
        stickyInfo += "<endOfText>" + "\n";
        stickyInfo += "n" + noteColorId + "\n";
        stickyInfo += "s" + spriteIndex + "\n";
        stickyInfo += "p" + (isTherePhoto ? "1" : "0") + "\n";
        stickyInfo += "k" + customScale + "\n";
        stickyInfo += "t" + customTextScale + "\n";
        stickyInfo += "r" + photoRotateCount + "\n";
        stickyInfo += "l" + layerNo + "\n";
        if (includePosition)
        {
            stickyInfo += "x" + transform.position.x + "\n";
            stickyInfo += "y" + transform.position.y + "\n";
            stickyInfo += "z" + transform.position.z + "\n";
        }
        int colorId = -1;
        foreach (Transform child in transform)
        {
            if (child.tag == "DrawLine")
            {
                DrawLine drawLine = child.GetComponent<DrawLine>();
                if (colorId != drawLine.colorId)
                {
                    colorId = drawLine.colorId;
                    stickyInfo += "c" + colorId + "\n";
                }
                stickyInfo += "" + drawLine.positions[0].x + "," + drawLine.positions[0].y + "," + drawLine.positions[1].x + "," + drawLine.positions[1].y + "\n";
            }
        }

        lastUpdateTimeVectorInfo = lastUpdateTime;
        return stickyInfo;
    }

    public void readStickyInfo(string newStickyInfo, bool includePosition = false)
    {
        stickyInfo = newStickyInfo;
        Vector3 pos = Vector3.zero;

        foreach (Transform child in transform)
        {
            if (child.tag == "DrawLine")
            {
                Destroy(child.gameObject);
            }
        }

        string[] basicContent = stickyInfo.Split(new string[] { "<endOfText>" }, StringSplitOptions.None);
        if (basicContent.Length >= 1)
        {
            StickyText = basicContent[0];
            textMesh.text = basicContent[0];
            if (shadowsOn)
            {
                for (int i = 0; i < shadowTextMeshes.Count; i++)
                {
                    shadowTextMeshes[i].text = textMesh.text;
                    shadowTextMeshes[i].fontSize = textMesh.fontSize;
                }
            }
            gameObject.name = basicContent[0];
        }
        if (basicContent.Length >= 2)
        {
            string[] lines = basicContent[1].Split('\n');
            int colorId = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("n"))
                {
                    string intString = lines[i].Remove(0, 1);
                    noteColorId = int.Parse(intString);
                    noteColor = ColorPickerDiscreet.getColor(noteColorId);
                }
                else if (lines[i].Contains("s"))
                {
                    string intString = lines[i].Remove(0, 1);
                    spriteIndex = int.Parse(intString);
                }
                else if (lines[i].Contains("p"))
                {
                    string intString = lines[i].Remove(0, 1);
                    isTherePhoto = (int.Parse(intString) == 1);
                }
                else if (lines[i].Contains("k"))
                {
                    string floatString = lines[i].Remove(0, 1);
                    customScale = float.Parse(floatString);
                }
                else if (lines[i].Contains("t"))
                {
                    string floatString = lines[i].Remove(0, 1);
                    customTextScale = float.Parse(floatString);
                }
                else if (lines[i].Contains("r"))
                {
                    string intString = lines[i].Remove(0, 1);
                    photoRotateCount = int.Parse(intString);
                }
                else if (lines[i].Contains("c"))
                {
                    string intString = lines[i].Remove(0, 1);
                    colorId = int.Parse(intString);
                }
                else if (includePosition && lines[i].Contains("x"))
                {
                    string floatString = lines[i].Remove(0, 1);
                    pos.x = float.Parse(floatString);
                }
                else if (includePosition && lines[i].Contains("y"))
                {
                    string floatString = lines[i].Remove(0, 1);
                    pos.y = float.Parse(floatString);
                }
                else if (includePosition && lines[i].Contains("z"))
                {
                    string floatString = lines[i].Remove(0, 1);
                    pos.z = float.Parse(floatString);
                }
                else if (lines[i].Contains("l"))
                {
                    string intString = lines[i].Remove(0, 1);
                    layerNo = int.Parse(intString);
                }
                else
                {
                    string[] floatStrings = lines[i].Split(',');
                    if (floatStrings.Length == 4)
                    {
                        Vector3 pos0 = new Vector3(float.Parse(floatStrings[0]), float.Parse(floatStrings[1]), 0f);
                        Vector3 pos1 = new Vector3(float.Parse(floatStrings[2]), float.Parse(floatStrings[3]), 0f);
                        GetComponent<StickyFingerPaint>().CreateDrawLine(colorId, true, pos0, pos1);
                    }
                }
            }
        }
        if (includePosition)
        {
            transform.position = pos;
        }
        //if (updateTime == 0f)
        //{
            lastUpdateTimeVectorInfo = Time.realtimeSinceStartup;
        //}
        //else
        //{
        //    lastUpdateTimeVectorInfo = updateTime;
        //}
        lastUpdateTime = lastUpdateTimeVectorInfo;
    }

    public void putPhoto(Texture2D tex, bool doAutoRotate = false)
    {
        uneditedTex = tex;
        TextureScale.Bilinear(uneditedTex, 400, 400);
        if (!doAutoRotate)
        {
            ImageImportSettings.instance.setActive(this);
        }
        else
        {
            rotateTexture(uneditedTex, photoRotateCount);
            editTexForShape();
        }
    }

    public void putPhoto(Sprite sprite)
    {
        if (photo == null)
        {
            photo = (new GameObject()).AddComponent<SpriteRenderer>();
            photo.transform.SetParent(transform);
            if (GetComponent<StickyInPanel>() != null || isPanelShowOff)
            {
                photo.transform.localPosition = -Vector3.forward * 0.01f;
            }
            else
            {
                photo.transform.localPosition = Vector3.forward * 0.01f;
                photo.transform.localEulerAngles = Vector3.up * 180f;
            }
            photo.name = "Photo";
        }
        photo.sprite = sprite;
        photo.transform.localScale = new Vector3(MaskBounds[spriteIndex].x / sprite.texture.width, MaskBounds[spriteIndex].y / sprite.texture.height, 1f) * 1.05f;
        photo.enabled = true;
        isTherePhoto = true;
        photoUpdateTime = Time.realtimeSinceStartup;
    }

    public void deletePhoto()
    {
        if (photo != null)
        {
            Destroy(photo.gameObject);
        }
        isTherePhoto = false;
        photoUpdateTime = Time.realtimeSinceStartup;
    }

    public void clearPhoto()
    {
        if (photo != null)
        {
            photo.enabled = false;
        }
        isTherePhoto = false;
        photoUpdateTime = Time.realtimeSinceStartup;
    }

    public void editTexForShape()
    {
        if (uneditedTex != null)
        {
            editedTex = copyOf(uneditedTex);
            if (spriteIndex != 0)
            {
                TextureScale.Bilinear(editedTex, (int)MaskBounds[spriteIndex].x, (int)MaskBounds[spriteIndex].y);
                editedTex = MaskTexture(editedTex);
            }

            Rect rect = new Rect(0, 0, editedTex.width, editedTex.height);
            Vector2 vec2 = new Vector2(0.5f, 0.5f);
            putPhoto(Sprite.Create(editedTex, rect, vec2));
            lastTimeEditedTex = Time.realtimeSinceStartup;

            SaveLoadManager.MakeAMove("StickyScript1");
        }
    }

    public Texture2D MaskTexture(Texture2D tex)
    {
        Texture2D mask = masks[spriteIndex];
        Texture2D tex2 = new Texture2D(tex.width, tex.height);
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Color c0 = tex.GetPixel(i, j);
                c0.a = mask.GetPixel((int)i * mask.width / tex.width, (int)j * mask.height / tex.height).a;
                tex2.SetPixel(i, j, c0);
            }
        }
        tex2.Apply();
        return tex2;
    }

    public static Texture2D copyOf(Texture2D tex)
    {
        Texture2D newTex = new Texture2D(tex.width, tex.height);
        newTex.SetPixels32(tex.GetPixels32());
        //newTex.LoadImage(tex.EncodeToJPG());
        newTex.Apply();
        return newTex;
    }

    private const float MaxCharsInSizeWithoutScaling = 10f;
    private const float MaxNoOfLinesWithoutScaling = 4.2f;
    private const float FontSize = 65f;
    public const float OblongShapeConst = 0.8f;

    public void getFontSize(string[] lines)
    {
        float maxCharsInSizeWithoutScaling = MaxCharsInSizeWithoutScaling / customTextScale;
        float maxNoOfLinesWithoutScaling = MaxNoOfLinesWithoutScaling / customTextScale;
        if (isAssymmetricScaling)
        {
            maxNoOfLinesWithoutScaling *= OblongShapeConst;
        }
        int maxLineLength = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length > maxLineLength)
            {
                maxLineLength = lines[i].Length;
            }
        }
        textHorSize = (maxLineLength > maxCharsInSizeWithoutScaling ? FontSize * maxCharsInSizeWithoutScaling / maxLineLength : FontSize * (isAssymmetricScaling ? OblongShapeConst : 1f));
        textVerSize = (lines.Length > maxNoOfLinesWithoutScaling ? FontSize * maxNoOfLinesWithoutScaling / lines.Length : FontSize);
        fontSizeLatest = Mathf.Min(textHorSize, textVerSize);
        //return Mathf.RoundToInt(Mathf.Min(textHorSize, textVerSize));
    }

    public static Texture2D rotateImage(Texture2D image, bool isRight)
    {
        Color32[] pixels = image.GetPixels32();
        if (isRight)
        {
            pixels = RotateMatrixRight(pixels, image.width);
        }
        else
        {
            pixels = RotateMatrixLeft(pixels, image.width);
        }
        image.SetPixels32(pixels);
        return image;
    }

    static Color32[] RotateMatrixRight(Color32[] matrix, int n)
    {
        Color32[] ret = new Color32[n * n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[(n - j - 1) * n + i] = matrix[i * n + j];
            }
        }

        return ret;
    }

    static Color32[] RotateMatrixLeft(Color32[] matrix, int n)
    {
        Color32[] ret = new Color32[n * n];

        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                ret[i * n + j] = matrix[(n - j - 1) * n + i];
            }
        }

        return ret;
    }

    public static void rotateTexture(Texture2D tex, int rotateCount)
    {
        int rc = rotateCount % 4;
        while (rc > 0)
        {
            rc--;
            tex = StickyScript.rotateImage(tex, true);
        }
        while (rc < 0)
        {
            rc++;
            tex = StickyScript.rotateImage(tex, false);
        }
    }

    public float minX()
    {
        return transform.position.x - 2f * realScale;
    }

    public float minY()
    {
        return transform.position.y - 2f * realScale;
    }

    public float maxX()
    {
        return transform.position.x + 2f * realScale;
    }

    public float maxY()
    {
        return transform.position.y + 2f * realScale;
    }
}
