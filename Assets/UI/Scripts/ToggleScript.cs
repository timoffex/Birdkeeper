using UnityEngine;
using System.Collections;

public class ToggleScript : MonoBehaviour {

	public void ToggleObject (GameObject obj) {
		obj.SetActive (!obj.activeSelf);
	}
}
