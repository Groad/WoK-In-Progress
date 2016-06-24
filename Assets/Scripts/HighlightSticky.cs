using UnityEngine;
using System.Collections;

public class HighlightSticky : MonoBehaviour 
{

	void Update()
	{
        //bool isAnActiveNote = false;
        //if (ftlGatherer.ActiveNotes != null)
        //{
        //    for (int i = 0; i < ftlGatherer.ActiveNotes.Count; i++)
        //    {
        //        if (ftlGatherer.ActiveNotes[i] == transform.root.gameObject)
        //        {
        //            isAnActiveNote = true;
        //            break;
        //        }
        //    }
        //}
        if (SceneManager.isDesktopScene && ftlGatherer.isAnActiveNote(transform.parent.gameObject))
		{
			GetComponent<SpriteRenderer>().enabled = true;
		}
		else
		{
			GetComponent<SpriteRenderer>().enabled = false;
		}

	}
}
