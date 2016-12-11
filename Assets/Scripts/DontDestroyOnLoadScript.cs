using UnityEngine;
using System.Collections;


/// <summary>
/// Makes the given object persist through scenes.
/// </summary>
public class DontDestroyOnLoadScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (gameObject);
	}
}
