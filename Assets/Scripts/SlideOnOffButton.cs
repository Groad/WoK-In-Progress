using UnityEngine;
using System.Collections;

public class SlideOnOffButton : EditorButton
{
    public static SlideOnOffButton instance;
    public float xWhenOff;
    private float xWhenOn;
    public float period;
    private float timer;
    public bool isOn;
    public Sprite spriteOn;
    public Sprite spriteOff;

	void Start ()
    {
        instance = this;
        timer = period;
        xWhenOn = transform.position.x;
        isOn = true;
	}
	
	void Update ()
    {
	    if (timer < period)
        {
            timer += Time.deltaTime;

            if (timer >= period)
            {
                if (isOn)
                {
                    transform.position = new Vector3(xWhenOn, transform.position.y, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(xWhenOff, transform.position.y, transform.position.z);
                }
            }
            else
            {
                float xEnd = (isOn ? xWhenOn : xWhenOff);
                float xBegin = (isOn ? xWhenOff : xWhenOn);
                transform.position = new Vector3(Easing.CircEaseOut(timer, xBegin, xEnd - xBegin, period), transform.position.y, transform.position.z);
            }
        }
	}

    public override void press()
    {
        isOn = !isOn;
        if (timer < period)
        {
            timer = period - timer;
        }
        else
        {
            timer = 0f;
        }
        GetComponent<SpriteRenderer>().sprite = (isOn ? spriteOn : spriteOff);
    }
}
