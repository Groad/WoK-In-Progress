using UnityEngine;
using System.Collections;

public class MainStickyReceiver : MonoBehaviour
{
    public static NetworkView instance;
    public bool isMainPanel;
    public GameObject sticky;

    [RPC]
    public void ReceiveSticky(string _stickyText)
    {
        if (Network.isServer)
        {
            GameObject s = Instantiate(sticky, new Vector3(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f), 0f), Quaternion.identity) as GameObject;
			s.GetComponent<StickyScript>().StickyText = _stickyText;
//			s.AddComponent<StickyView>();
//            s.GetComponent<StickyView>().text = _stickyText;
        }
    }

	void Awake ()
    {
        instance = GetComponent<NetworkView>();
	}
	
	void Update ()
    {
	
	}
}
