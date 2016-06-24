using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CirclePopup : MonoBehaviour
{
    private Vector3 scale;
    private Vector3 buttonScale;
    private List<CircleMenuButton> buttons;
    private float animTimer;
    private const float AnimPeriod0 = 0.35f;
    private const float AnimPeriod1 = 1.05f;
    private bool closingIn;
    private const float buttonDistance = 1.818f;
    public GameObject buttonPrefab;
    public List<CircleMenuButton.Type> buttonTypes;
    public static CirclePopup instance;

	void Start ()
    {
        if (instance != null)
        {
            instance.CloseMenu();
        }
        instance = this;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -5f;
        transform.position = mousePos;
        buttons = new List<CircleMenuButton>();
        buttonScale = buttonPrefab.transform.localScale;
        for (int i = 0; i < buttonTypes.Count; i++)
        {
            CircleMenuButton b = (Instantiate(buttonPrefab, transform.position - Vector3.forward * 0.1f + Geometry.createVector3(360f * i / buttonTypes.Count, buttonDistance), Quaternion.identity) as GameObject).GetComponent<CircleMenuButton>();
            b.transform.SetParent(transform);
            b.transform.localScale = Vector3.zero;
            b.menu = this;
            b.changeType(buttonTypes[i]);
            buttons.Add(b);
        }
        scale = transform.localScale;
        transform.localScale = Vector3.zero;
    }
	
	void Update ()
    {
	    if (closingIn && animTimer > 0f)
        {
            animTimer -= Time.deltaTime;
            if (animTimer <= 0f)
            {
                Destroy(gameObject);
            }
        }
        else if (!closingIn && (animTimer < AnimPeriod0 || animTimer < AnimPeriod1))
        {
            animTimer += Time.deltaTime;
        }

        if (animTimer <= 0f)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.localScale = Vector3.zero;
            }
            transform.localScale = Vector3.zero;
        }
        else
        {
            if (animTimer <= AnimPeriod0)
            {
                transform.localScale = scale * Easing.Linear(animTimer, 0f, 1f, AnimPeriod0);
            }
            else
            {
                transform.localScale = scale;
            }
            if (closingIn)
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].transform.localScale = buttonScale;
                }
            }
            else
            {
                if (animTimer <= AnimPeriod1)
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        buttons[i].transform.localScale = scale.x / transform.localScale.x * buttonScale * Easing.BounceEaseOut(animTimer, 0f, 1f, AnimPeriod1);
                    }
                }
                else
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        buttons[i].transform.localScale = buttonScale;
                    }
                }
            }
        }
	}

    public void OpenUpMenu()
    {
        if (closingIn)
        {
            if (animTimer < 0f)
            {
                animTimer = 0f;
            }
            closingIn = false;
        }
    }

    public void CloseMenu()
    {
        if (!closingIn)
        {
            if (animTimer > AnimPeriod0)
            {
                animTimer = AnimPeriod0;
            }
            closingIn = true;
        }
    }

    void OnMouseDown()
    {
        CloseMenu();
    }
}
