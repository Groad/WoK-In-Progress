using UnityEngine;
using System.Collections;

public class DontDestr : MonoBehaviour {

	void Start () {
		DontDestroyOnLoad (gameObject);
	}
}
