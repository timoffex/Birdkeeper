using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogChoice : MonoBehaviour {

	public Text textObject;

	public void SetText (string txt) {
		textObject.text = txt;
	}
}
