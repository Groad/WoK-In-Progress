using UnityEngine;
using System.Collections;

public class TestScene : MonoBehaviour
{


	void Start ()
    {
	
	}
	
	void OnGUI ()
    {
        if (GUI.Button(new Rect(0, 0, 300, 100), "Desktop App"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("DesktopScene");
        }
        if (GUI.Button(new Rect(0, 100, 300, 100), "Tablet App"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TabletScene");
        }
        //if (GUI.Button(new Rect(0, 200, 300, 100), "Desktop Extra Instance"))
        //{
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("ExtraDesktopScene");
        //}
	}
}
