using UnityEngine;
using System.Collections;

public class PanelCamera : MonoBehaviour
{
    public Camera mainPanelCamera;
    private Camera cam;
    public int panelId;

	void Start ()
    {
        cam = GetComponent<Camera>();
	}
	
	void Update ()
    {
        cam.orthographicSize = mainPanelCamera.orthographicSize;
        transform.position = mainPanelCamera.transform.position + panelId * Vector3.left * mainPanelCamera.orthographicSize * 32f / 9f;
	}
}
