using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Vector3 xVectorPix;
	public Vector3 yVectorPix;


	public Vector3 GetXVector () {
		return xVectorPix / GetPPU ();
	}

	public Vector3 GetYVector () {
		return yVectorPix / GetPPU ();
	}


	public void SetXVector (Vector3 v) {
		xVectorPix = v * GetPPU ();
	}

	public void SetYVector (Vector3 v) {
		yVectorPix = v * GetPPU ();
	}

	public float GetPPU () {
		return GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit;
	}

}
