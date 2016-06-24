using UnityEngine;
using System.Collections;

public class Hotkey : MonoBehaviour
{
    public string hoverExplanation;
    public KeyCode key;
    private bool mouseOver;
    private float mouseOverTimer;
    private const float MouseOverExplanation = 0.5f;
    private GameObject explanationBox;
    private static GameObject explanationBoxPrefab;
    public bool allowedWhenSlidedOff;

    void Start()
    {
        if (explanationBoxPrefab == null)
        {
            explanationBoxPrefab = Resources.Load("explanationBox") as GameObject;
        }
    }

	void Update ()
    {
	    if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(key))
        {
            if (SlideOnOffButton.instance.isOn || allowedWhenSlidedOff)
            {
                GetComponent<EditorButton>().press();
            }
        }

        if (mouseOver)
        {
            mouseOverTimer += Time.deltaTime;
            if (mouseOverTimer > MouseOverExplanation)
            {
                if (explanationBox == null)
                {
                    explanationBox = Instantiate(explanationBoxPrefab);
                    TextMesh textMesh = explanationBox.GetComponentInChildren<TextMesh>();
                    SpriteRenderer spriteRenderer = explanationBox.GetComponentInChildren<SpriteRenderer>();
                    if (hoverExplanation == "")
                    {
                        textMesh.text = "Hotkey: CTRL+" + key.ToString();
                        int textLength = textMesh.text.Length + 2;
                        spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x * textLength, spriteRenderer.transform.localScale.y, spriteRenderer.transform.localScale.z);
                    }
                    else
                    {
                        textMesh.text = "Hotkey: CTRL+" + key.ToString();
                        int textLength = textMesh.text.Length + 2;
                        textMesh.text = hoverExplanation + "\n" + textMesh.text;
                        if (hoverExplanation.Length > textLength)
                        {
                            textLength = hoverExplanation.Length;
                        }
                        spriteRenderer.transform.localScale = new Vector3(spriteRenderer.transform.localScale.x * textLength, spriteRenderer.transform.localScale.y * 2f, spriteRenderer.transform.localScale.z);
                    }
                }
                explanationBox.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.right * 10);
                explanationBox.transform.position = new Vector3(explanationBox.transform.position.x, explanationBox.transform.position.y, -8f);
            }
        }
        else if (explanationBox != null)
        {
            Destroy(explanationBox);
        }
	}

    void OnMouseEnter()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
}
