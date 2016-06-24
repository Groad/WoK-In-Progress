using UnityEngine;
using System.Collections;

public class Canvass : MonoBehaviour
{
    public static Transform cTransform;

    void Awake()
    {
        cTransform = transform;
    }
}
