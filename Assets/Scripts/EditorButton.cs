using UnityEngine;
using System.Collections;

public class EditorButton : MonoBehaviour
{
    public bool mousePressed = false;

	void OnMouseDown ()
    {
        mousePressed = true;
        press();
        mousePressed = false;
	}

    //will be overridden.
    public virtual void press()
    {
        Debug.Log("pressed EditorButton");
    }
}
