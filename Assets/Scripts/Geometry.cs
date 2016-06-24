using UnityEngine;
using System.Collections;

public class Geometry : MonoBehaviour {

	public static Vector2 normalizeVector2(Vector2 vector, float length)
	{
		if ((vector.x != 0f || vector.y != 0f) && length > 0f)
		{
			vector = vector * (length / lengthOfVector2(vector));
		}
		else if (length == 0f)
		{
			vector = new Vector2(0, 0);
		}
		return vector;
	}
	
	public static Vector3 normalizeVector3(Vector3 vector, float length)
	{
		if ((vector.x != 0f || vector.y != 0f || vector.z != 0f) && length > 0f)
		{
			vector = vector * (length / lengthOfVector3(vector));
		}
		else if (length == 0f)
		{
			vector = new Vector3(0, 0, 0);
		}
		return vector;
	}

	public static Vector2 createVector2(float angle, float length)
	{
		return length * (new Vector2(Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad)));
	}
	
	public static Vector3 createVector3(float angle, float length)
	{
		return length * (new Vector3(Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad), 0));
	}

	public static float lengthOfVector2(Vector2 vector)
	{
		return Mathf.Sqrt (Mathf.Pow (vector.x, 2f) + Mathf.Pow (vector.y, 2f));
	}
	
	public static float lengthOfVector3(Vector3 vector)
	{
		return Mathf.Sqrt (Mathf.Pow (vector.x, 2f) + Mathf.Pow (vector.y, 2f));
	}
	
	public static float lengthOfVector3Real(Vector3 vector)
	{
		return Mathf.Sqrt (Mathf.Pow (vector.x, 2f) + Mathf.Pow (vector.y, 2f) + Mathf.Pow (vector.z, 2f));
	}

	public static float angleOfVector2(Vector2 vector)
	{
		return Mathf.Atan2 (vector.y, vector.x) * Mathf.Rad2Deg;
	}
	
	public static float angleOfVector3(Vector3 vector)
	{
		return Mathf.Atan2 (vector.y, vector.x) * Mathf.Rad2Deg;
	}

	public static float mod(float number, float baseNumber)
	{
		float toReturn = number;
		if (baseNumber > 0)
		{
			while (toReturn < 0)
			{
				toReturn += baseNumber;
			}
			while (toReturn > baseNumber)
			{
				toReturn -= baseNumber;
			}
		}
		return toReturn;
	}
	
	public static int mod(int number, int baseNumber)
	{
		int toReturn = number;
		if (baseNumber > 0)
		{
			while (toReturn < 0)
			{
				toReturn += baseNumber;
			}
			while (toReturn > baseNumber)
			{
				toReturn -= baseNumber;
			}
		}
		return toReturn;
	}
	
	public static float differenceOfAngles(float angle1, float angle2)
    {
        return 180 - Mathf.Abs(Mathf.Abs(angle1 - angle2) - 180);
    }

    public static float differenceOfAnglesNegative(float angle1, float angle2)
    {
        float delta = angle1 - angle2;
        while (delta > 180f)
        {
            delta -= 360f;
        }
        while (delta < -180f)
        {
            delta += 360f;
        }
        return delta;
    }
}
