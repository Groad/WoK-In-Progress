using UnityEngine;
using System.Collections;

public class CircleMenuButton : MonoBehaviour
{
    public enum Type
    {
        Color,
        Cut,
        Delete,
        Description,
        Drag,
        Duplicate,
        Link,
        Title,
        InsertSticky,
        InsertHandle,
		ReplaceHandleWithSticky,
        DeleteHandle,
        DeleteChain
    }

    private SpriteRenderer icon;
    private SpriteRenderer bg;
    public Color whiteColor;
    public Color focusColor;
    private bool mouseOver;
    private const float AnimPeriod = 0.5f;
    private float animTimer = AnimPeriod;
    public Sprite[] typeSprites;
    public Type type;
    public CirclePopup menu;
    public static LineScript lineScript;
    public static StickyScript stickyScript;

	void Start ()
    {
        if (icon == null)
        {
            foreach (Transform child in transform)
            {
                bg = child.GetComponent<SpriteRenderer>();
                foreach (Transform child2 in child)
                {
                    icon = child2.GetComponent<SpriteRenderer>();
                }
            }
        }
        
        bg.color = whiteColor;
        icon.color = focusColor;
	}

    void Update()
    {
        if (animTimer < AnimPeriod)
        {
            animTimer += Time.deltaTime;
        }
        if (mouseOver)
        {
            if (animTimer >= AnimPeriod)
            {
                bg.color = focusColor;
                icon.color = whiteColor;
            }
            else
            {
                bg.color = mixColors(focusColor, whiteColor, animTimer / AnimPeriod);
                icon.color = mixColors(whiteColor, focusColor, animTimer / AnimPeriod);
            }

            if (Input.GetMouseButton(0))
            {
                bg.color = new Color(bg.color.r * 0.5f, bg.color.g * 0.5f, bg.color.b * 0.5f, bg.color.a);
                icon.color = new Color(icon.color.r * 0.5f, icon.color.g * 0.5f, icon.color.b * 0.5f, icon.color.a);
            }
        }
        else
        {
            if (animTimer >= AnimPeriod)
            {
                bg.color = whiteColor;
                icon.color = focusColor;
            }
            else
            {
                bg.color = mixColors(whiteColor, focusColor, animTimer / AnimPeriod);
                icon.color = mixColors(focusColor, whiteColor, animTimer / AnimPeriod);
            }
        }
    }

    void OnMouseEnter()
    {
        mouseOver = true;
        if (animTimer >= AnimPeriod)
        {
            animTimer = 0f;
        }
        else
        {
            animTimer = AnimPeriod - animTimer;
        }
    }

    void OnMouseExit()
    {
        mouseOver = false;
        if (animTimer >= AnimPeriod)
        {
            animTimer = 0f;
        }
        else
        {
            animTimer = AnimPeriod - animTimer;
        }
    }

    Color mixColors(Color c1, Color c2, float ratio)
    {
        return new Color(c1.r * ratio + c2.r * (1f - ratio), c1.g * ratio + c2.g * (1f - ratio), c1.b * ratio + c2.b * (1f - ratio), c1.a * ratio + c2.a * (1f - ratio));
    }

    void OnMouseDown()
    {
        switch (type)
        {
            case Type.DeleteHandle:
                lineScript.DeleteHandle(Vector3.zero, false);
                menu.CloseMenu();
                SaveLoadManager.MakeAMove("DeleteHandle");
                break;
            case Type.DeleteChain:
                var sticky0 = lineScript.ThisChain.Sticky0;
                var sticky1 = lineScript.ThisChain.Sticky1;
			    sticky0.GetComponent<StickyScript>().destroyConnection(sticky1.GetComponent<StickyScript>(), true);
			    sticky1.GetComponent<StickyScript>().destroyConnection(sticky0.GetComponent<StickyScript>(), true);
			    foreach (var link in lineScript.ThisChain.LinesInChain)
				    Destroy (link);
                menu.CloseMenu();
                SaveLoadManager.MakeAMove("DeleteChain");
                break;
            case Type.InsertSticky:
                lineScript.SplitLineSticky(transform.position);
                menu.CloseMenu();
                SaveLoadManager.MakeAMove("InsertSticky");
                break;
            case Type.InsertHandle:
                lineScript.SplitLineHandle(lineScript.gameObject.transform.position);
                menu.CloseMenu();
                SaveLoadManager.MakeAMove("InsertHandle");
                break;
		    case Type.ReplaceHandleWithSticky:
			    lineScript.ReplaceHandleWithSticky(transform.position);
			    menu.CloseMenu();
                SaveLoadManager.MakeAMove("ReplaceHandleWithSticky");
			    break;
            default:
                break;
        }
    }

    public void changeType(Type newType)
    {
        type = newType;
        if (icon == null)
        {
            foreach (Transform child in transform)
            {
                bg = child.GetComponent<SpriteRenderer>();
                foreach (Transform child2 in child)
                {
                    icon = child2.GetComponent<SpriteRenderer>();
                }
            }
        }
        icon.sprite = typeSprites[(int)type];
    }
}
