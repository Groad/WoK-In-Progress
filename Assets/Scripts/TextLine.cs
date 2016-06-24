using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextLine : MonoBehaviour
{
    public Text text;
    public StickyScript ss;
    private StickyInPanel sip;
    public int lineNo;
    private const float XDelta = 20f;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        sip = ss.GetComponent<StickyInPanel>();
    }
	
	void Update ()
    {
	    if (ss == null)
        {
            Destroy(gameObject);
        }
        else
        {
            LocateText();
        }
	}

    public void LocateText()
    {
        float scale = ss.transform.lossyScale.x;
        if (sip != null || ss.isPanelShowOff)
        {
            transform.position = Camera.main.WorldToScreenPoint(ss.transform.position);
        }
        else
        {
            scale *= 5f / StickySender.instance.GetComponent<Camera>().orthographicSize;
            transform.position = StickySender.instance.ReverseTransform(ss.transform.position);
        }
        transform.localScale = scale * Vector3.one * 0.1f;
        //transform.position += Vector3.up * XDelta * (ss.noOfLines * 0.5f - (lineNo + 0.5f)) * scale;
    }
}
