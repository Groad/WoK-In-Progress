using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour
{

	void Start ()
    {
	
	}
	
	void Update ()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
	}

    void OnGUI()
    {
        GUILayout.Label("" + Input.mousePosition.x + "," + Input.mousePosition.y);
    }
}
