using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
    public bool IsDesktopScene;
    public static bool isDesktopScene;
    public static Vector3 stickyScale;

	void Awake ()
    {
        isDesktopScene = IsDesktopScene;
        stickyScale = transform.localScale;
	}
}
