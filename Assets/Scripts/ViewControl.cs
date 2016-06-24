using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewControl : MonoBehaviour {

	private Vector3 camCenter;
	private Vector3 lastPosition;


    public static float stickieMaxX;
    public static float stickieMinX;
    public static float stickieMaxY;
    public static float stickieMinY;
    public static int calculateMaxMin;

    public Camera mainPanelCamera;
    public Camera navigatorCamera;
    private const float CameraMovePeriod = 1f;
    private float cameraMoveTimer = CameraMovePeriod;
    private float cameraScale;
    private float cameraScaleFirst;
    private float navCameraScale;
    private float navCameraScaleFirst;
    private Vector3 cameraPos;
    private Vector3 cameraPosFirst;

    private const float MaxZoomOut = 35f;

    private Vector3 navigatorMainPanelBoundScale;
    public Transform navigatorMainPanelBound;
    public static bool dragging;
	
	void OnEnable()
	{
		camCenter = transform.position;

        stickieMaxX = 0f;
        stickieMinX = 0f;
        stickieMaxY = 0f;
        stickieMinY = 0f;
        cameraPos = mainPanelCamera.transform.position;
        cameraScale = mainPanelCamera.orthographicSize;
        //navigatorCamera.transform.position = mainPanelCamera.transform.position;
        navigatorCamera.orthographicSize = mainPanelCamera.orthographicSize * 3f;
        navCameraScale = navigatorCamera.orthographicSize;
        navigatorMainPanelBoundScale = navigatorMainPanelBound.localScale;
	}
	void Update ()
    {
        if (calculateMaxMin > 1)
        {
            stickieMaxX = 0f;
            stickieMinX = 0f;
            stickieMaxY = 0f;
            stickieMinY = 0f;
            calculateMaxMin--;
        }
        else if (calculateMaxMin == 1)
        {
            calculateMaxMin = 0;
            //stickieMaxX += 2f;
            //stickieMinX -= 2f;
            //stickieMaxY += 2f;
            //stickieMinY -= 2f;
            cameraPos = new Vector3((stickieMaxX + stickieMinX) * 0.5f, (stickieMaxY + stickieMinY) * 0.5f, navigatorCamera.transform.position.z);
            cameraPosFirst = navigatorCamera.transform.position;
            float minScaleX = Mathf.Max(10f, (stickieMaxX - stickieMinX) * 0.5f / 1.4f);
            float minScaleY = Mathf.Max(10f, (stickieMaxY - stickieMinY) * 0.5f);
            float newScale = Mathf.Max(minScaleX, minScaleY);
            cameraScaleFirst = mainPanelCamera.orthographicSize;
            navCameraScaleFirst = navigatorCamera.orthographicSize;
            navCameraScale = 3f * Mathf.Max(minScaleX, minScaleY * 0.2f);
            cameraMoveTimer = 0f;
            //newScale = Mathf.Min(MaxZoomOut, newScale);
            //if (newScale > mainPanelCamera.orthographicSize || newScale < mainPanelCamera.orthographicSize * 0.7f)
            //{
            //    cameraScale = newScale;
            //}
            //else
            //{
            //    cameraScale = mainPanelCamera.orthographicSize;
            //}
        }

        if (cameraMoveTimer < CameraMovePeriod)
        {
            cameraMoveTimer += Time.deltaTime;
            if (cameraMoveTimer >= CameraMovePeriod)
            {
                //mainPanelCamera.orthographicSize = cameraScale;
                //mainPanelCamera.transform.position = cameraPos;
                navigatorCamera.orthographicSize = navCameraScale;
                navigatorCamera.transform.position = cameraPos;
            }
            else
            {
                //mainPanelCamera.orthographicSize = Easing.Linear(cameraMoveTimer, cameraScaleFirst, cameraScale - cameraScaleFirst, CameraMovePeriod);
                Vector3 camPos = navigatorCamera.transform.position;
                camPos.x = Easing.SineEaseInOut(cameraMoveTimer, cameraPosFirst.x, cameraPos.x - cameraPosFirst.x, CameraMovePeriod);
                camPos.y = Easing.SineEaseInOut(cameraMoveTimer, cameraPosFirst.y, cameraPos.y - cameraPosFirst.y, CameraMovePeriod);
                if (float.IsInfinity(camPos.x) || float.IsNaN(camPos.x) || float.IsInfinity(camPos.x) || float.IsNaN(camPos.x))
                {
                    cameraMoveTimer = CameraMovePeriod;
                    Debug.Log("Something is wrong with world map camera");
                }
                else
                {
                    navigatorCamera.transform.position = camPos;
                    navigatorCamera.orthographicSize = Easing.Linear(cameraMoveTimer, navCameraScaleFirst, navCameraScale - navCameraScaleFirst, CameraMovePeriod);
                }
            }
        }

        var d = -Input.GetAxis("Mouse ScrollWheel") * 0.5f * mainPanelCamera.orthographicSize;
		if (Input.GetAxis("Mouse ScrollWheel") != 0)
		{
            GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize + d, 1.8f, MaxZoomOut);
            Vector3 deltaMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - ftlGatherer.instance.mainPanel.transform.position;
            deltaMousePos.z = 0f;
            deltaMousePos.x = -deltaMousePos.x;
            mainPanelCamera.transform.position -= d * deltaMousePos * 0.2f;
		}
		var position = ftlGatherer.TransformToMainPanel(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3 deltaPos = Vector3.zero;
		position.z = transform.position.z;
        if (Input.GetMouseButton(2) || dragging)
		{
            //var deltaPos = (position - lastPosition) * -0.02f * mainPanelCamera.orthographicSize / 4f;
            //deltaPos.x = - deltaPos.x;
            deltaPos = position - lastPosition;
			transform.position -= deltaPos;
		}
        lastPosition = position - deltaPos;

        if (ftlGatherer.ActiveNotes == null || ftlGatherer.ActiveNotes.Count == 0)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.position -= 10f * Time.deltaTime * Vector3.right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position -= 10f * Time.deltaTime * Vector3.left;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.position += 10f * Time.deltaTime * Vector3.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position += 10f * Time.deltaTime * Vector3.down;
            }
        }

        navigatorMainPanelBound.localScale = navigatorMainPanelBoundScale * mainPanelCamera.orthographicSize * 0.1f;
	}
}
