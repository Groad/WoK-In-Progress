using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickyPalette : MonoBehaviour 
{
    public static StickyPalette instance;
	public List <GameObject> stickiesInPalette = new List<GameObject> ();
	private Vector3 farLeft;

    void Awake()
    {
        instance = this;
    }

	void OnEnable()
	{
		farLeft = transform.position;
		farLeft.x -= GetComponent<BoxCollider> ().bounds.extents.x;
        farLeft.x += 0.45f;
        farLeft.y += 4.25f;
		farLeft.z -= 0.1f;
	}

	public Vector3 nextSlot
	{
		get
		{
			var position = farLeft;
			position.y -= 0.5f * stickiesInPalette.Count;
			return position;
		}
	}

	public void AddToList (GameObject _sticky)
	{
        _sticky.transform.SetParent(SaveLoadManager.instance.stickiesInPalette);
		if (!stickiesInPalette.Contains(_sticky))
        {
            bool foundEmptySpot = false;
            for (int i = 0; i < stickiesInPalette.Count; i++)
            {
                if (stickiesInPalette[i] == null)
                {
                    foundEmptySpot = true;
                    stickiesInPalette[i] = _sticky;
                    break;
                }
            }
            if (!foundEmptySpot)
            {
                stickiesInPalette.Add(_sticky);
            }
        }
        for (int i = 0; i < stickiesInPalette.Count; i++)
        {
            if (stickiesInPalette[i] != null)
            {
                stickiesInPalette[i].transform.position = farLeft - 0.5f * (i % 18) * Vector3.up + (i / 18) * 0.5f * Vector3.right;
            }
        }
	}

	public delegate void StickyPaletteEventHandler (bool on);
	
	public static event StickyPaletteEventHandler mouseOverStickyPalette;
	
	public static void MouseOverStickyPalette (bool on)
	{
		if (mouseOverStickyPalette != null)
		{
			mouseOverStickyPalette(on);
		}
	}
	
	void OnMouseEnter ()
	{
		MouseOverStickyPalette (true);
	}

	void OnMouseExit ()
	{
		MouseOverStickyPalette (false);
	}
}