using UnityEngine;
using System.Collections;

public class CameraFixer : MonoBehaviour
{
    public static CameraFixer instance;
    public Vector2 minCameraSize;
    private Camera camera;
    private int sw;
    private int sh;
    private ScreenOrientation so;
    public SpriteButton exitButton;
    public int noOfMonitors = 1;
    private int noOfMonitorsOld = 1;
    //private static bool activatedDisplays;
    public Transform stickyPalette;
    public Vector3 stickyPalettePosition;

	void Start ()
    {
        instance = this;

        if (stickyPalette != null)
        {
            stickyPalettePosition = stickyPalette.position - new Vector3(-8.9f, 0f, 0f);
        }

        if (Screen.fullScreen)
        {
            Resolution res = GetNativeRes();
            if (res.width != Screen.width || res.height != Screen.height)
            {
                Screen.SetResolution(res.width, res.height, true);
            }
        }
        //else
        //{
        //    Resolution res = GetNativeRes();
        //    Screen.SetResolution(res.width - 240, res.height - 135, false);
        //}
        camera = GetComponent<Camera>();
        ResizeScreen();

        //if (!activatedDisplays)
        //{
        //    activatedDisplays = true;
        //    for (int i = Display.displays.Length - 1; i >= 0; i--)
        //    {
        //        Display.displays[i].Activate();
        //    }
        //}
	}
	
	void Update ()
    {
        if (sw != Screen.width / noOfMonitors || sh != Screen.height || so != Screen.orientation || noOfMonitorsOld != noOfMonitors)
        {
            ResizeScreen();
        }
        if (stickyPalette != null)
        {
            Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height * 0.5f, 0f));
            left.z = 0f;
            stickyPalette.position = stickyPalettePosition + left;
        }
        if (exitButton != null && exitButton.pressed)
        {
            Application.Quit();
        }
	}

    void ResizeScreen()
    {
        sw = (Screen.width / noOfMonitors);
        sh = Screen.height;
        so = Screen.orientation;
        noOfMonitorsOld = noOfMonitors;
        camera.orthographicSize = 1;
        if (minCameraSize.x > camera.orthographicSize * sw / sh)
        {
            camera.orthographicSize = minCameraSize.x * sh / sw;
        }
        if (minCameraSize.y > camera.orthographicSize)
        {
            camera.orthographicSize = minCameraSize.y;
        }
        transform.position = new Vector3((noOfMonitors - 1) * minCameraSize.x, 0f, transform.position.z);
        if (exitButton != null)
        {
            exitButton.transform.position = camera.ScreenToWorldPoint(new Vector3(sw, sh)) - 0.5f * (Vector3.right + Vector3.up);
            exitButton.transform.position = new Vector3(exitButton.transform.position.x, exitButton.transform.position.y, -8.1f);
        }
    }

    //void OnGUI()
    //{
    //    GUILayout.Label("" + Screen.width + "x" + Screen.height);
    //}

    public static Resolution GetNativeRes()
    {
        int nativeIndex = 0;
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width > Screen.resolutions[nativeIndex].width || Screen.resolutions[i].height > Screen.resolutions[nativeIndex].height)
            {
                nativeIndex = i;
            }
        }
        return Screen.resolutions[nativeIndex];
    }

    public static void detectMonitors()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //should be deleted. written for debug purposes.
            instance.noOfMonitors = 1 + (instance.noOfMonitors % 3);
        }
        else
        {
            instance.noOfMonitors = Display.displays.Length;
        }
        changeNoOfMonitorsResolution();
    }

    public static void undetectMonitors()
    {
        instance.noOfMonitors = 1;
        changeNoOfMonitorsResolution();
    }

    private static void changeNoOfMonitorsResolution()
    {
        Screen.SetResolution(Mathf.RoundToInt(1f * Screen.width * instance.noOfMonitors / instance.noOfMonitorsOld), Screen.height, false);
    }
}
