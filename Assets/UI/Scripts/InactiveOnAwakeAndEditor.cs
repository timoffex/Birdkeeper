using UnityEngine;
using System.Collections;

public class InactiveOnAwakeAndEditor : MonoBehaviour {

	#if UNITY_EDITOR
	void OnDrawGizmos () {
		gameObject.SetActive (false);
	}
	#endif

}
