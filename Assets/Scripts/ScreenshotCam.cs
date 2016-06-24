using UnityEngine;
using System.Collections;

public class ScreenshotCam : MonoBehaviour
{
    public Camera parentCam;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cam.orthographicSize = parentCam.orthographicSize;
    }
}
