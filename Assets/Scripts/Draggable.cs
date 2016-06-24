using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour
{
    private Vector3 lastPos;
    public const float MinDist = 1f;

	void Start ()
    {
        lastPos = transform.position;
	}

    public bool setDifferentPosition(Vector3 newPos)
    {
        bool toReturn = false;
        if (newPos.x != lastPos.x || newPos.y != lastPos.y)
        {
            if (Geometry.lengthOfVector3(newPos - lastPos) > MinDist)
            {
                toReturn = true;
            }
        }
        lastPos = newPos;
        return toReturn;
    }
}
