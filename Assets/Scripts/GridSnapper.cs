using UnityEngine;
using System.Collections;

public class GridSnapper : MonoBehaviour
{
    public GameObject gridSnap;
    public const float GridWidth = 3f;
    public const int NoOfColumns = 16;
    public const int NoOfRows = 12;
    public bool callOnValidate;
    public static AudioSource audioSource;
    public Transform mainCamera;
    public static bool snapToGrid;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

	void OnValidate () 
    {
        if (callOnValidate)
        {
            callOnValidate = false;
            //for (int i = 0; i < NoOfColumns * 2 + 1; i++)
            //{
            //    for (int j = 0; j < NoOfRows * 2 + 1; j++)
            //    {
            //        GameObject gs = Instantiate(gridSnap, Vector3.zero, Quaternion.identity) as GameObject;
            //        gs.transform.SetParent(transform);
            //        gs.transform.localPosition = new Vector3((i - NoOfColumns) * GridWidth, (j - NoOfRows) * GridWidth, 0f);
            //        gs.name = "Grid" + (i - NoOfColumns) + "," + (j - NoOfRows);
            //    }
            //}
        }
	}
	
	public static void Snap(GameObject go)
    {
        ViewControl.calculateMaxMin = 2;
        Vector3 pos = go.transform.position;
        if (snapToGrid)
        {
            float x = Mathf.RoundToInt(pos.x / GridWidth) * GridWidth;
            float y = Mathf.RoundToInt(pos.y / GridWidth) * GridWidth;
            go.transform.position = new Vector3(x, y, pos.z);
        }
        if (go.GetComponent<Draggable>() != null)
        {
            if (go.GetComponent<Draggable>().setDifferentPosition(go.transform.position))
            {
                ftlGatherer.draggedSomeStickies = true;
            }
            //else
            //{
            //    Debug.Log("there's a problem. " + go.name + " doesn't have Draggable component");
            //}
        }
        audioSource.Play();
    }

    void Update()
    {
        transform.position = new Vector3(GridWidth * Mathf.RoundToInt(mainCamera.position.x / GridWidth),
            GridWidth * Mathf.RoundToInt(mainCamera.position.y / GridWidth), transform.position.z);
    }
}
