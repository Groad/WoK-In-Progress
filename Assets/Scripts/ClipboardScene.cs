using UnityEngine;
using System.Collections;

public class ClipboardScene : MonoBehaviour
{
    private string label;
    private string systemCopyBufferOld;

	void Start ()
    {
	    label = "";
        systemCopyBufferOld = "";
	}
	
	void Update ()
    {
        if (systemCopyBufferOld != GUIUtility.systemCopyBuffer)
        {
            systemCopyBufferOld = GUIUtility.systemCopyBuffer;
            label = Time.realtimeSinceStartup + "\n" + systemCopyBufferOld;
        }
	}

    void OnGUI()
    {
        GUILayout.Label(label);
    }
}
