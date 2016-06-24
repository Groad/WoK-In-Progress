using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{

	void Awake ()
    {
        
	}

    public static void Reload()
    {
        ResetChains();
        ReloadChains();
    }

    private static void ResetChains()
    {
        foreach (Transform child in SaveLoadManager.instance.transform)
        {
            child.GetComponent<StickyScript>().chains = new Dictionary<StickyScript, Chain>();
        }
    }

    private static void ReloadChains()
    {
        foreach (Transform child in SaveLoadManager.instance.connections)
        {
            LineScript ls = child.GetComponent<LineScript>();
            GameObject s0 = ls.Endpoint0;
            GameObject s1 = ls.Endpoint1;
            while (s0.GetComponent<StickyScript>() == null)
            {
                s0 = s0.GetComponent<LineScript>().Endpoint0;
            }
            while (s1.GetComponent<StickyScript>() == null)
            {
                s1 = s1.GetComponent<LineScript>().Endpoint1;
            }
            StickyScript ss0 = s0.GetComponent<StickyScript>();
            StickyScript ss1 = s1.GetComponent<StickyScript>();
            //ss0.chains.Add()
            if (!ss0.chains.ContainsKey(ss1) || !ss1.chains.ContainsKey(ss0))
            {
                var newChain = new Chain();
                newChain.Sticky0 = s0;
                newChain.Sticky1 = s1;
                newChain.LinesInChain = new List<LineScript>();
                newChain.LinesInChain.Add(ls);
                ls.ThisChain = newChain;

                GameObject ls0 = ls.Endpoint0;
                GameObject ls1 = ls.Endpoint1;
                while (ls0.GetComponent<LineScript>() != null)
                {
                    LineScript lineScript0 = ls0.GetComponent<LineScript>();
                    newChain.LinesInChain.Add(lineScript0);
                    lineScript0.ThisChain = newChain;
                    ls0 = lineScript0.Endpoint0;
                }
                while (ls1.GetComponent<LineScript>() != null)
                {
                    LineScript lineScript1 = ls1.GetComponent<LineScript>();
                    newChain.LinesInChain.Add(lineScript1);
                    lineScript1.ThisChain = newChain;
                    ls1 = lineScript1.Endpoint1;
                }
                ss0.chains.Add(ss1, newChain);
                ss1.chains.Add(ss0, newChain);
            }
        }
    }
}
