using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Vector2 tileXVector { get { return MetaInformation.Instance ().tileXVector; } }
	public Vector2 tileYVector { get { return MetaInformation.Instance ().tileYVector; } }


	public Vector3 GetXVector () {
		return (Vector3)MetaInformation.Instance ().tileXVector;
	}

	public Vector3 GetYVector () {
		return (Vector3)MetaInformation.Instance ().tileYVector;
	}


	public void SetXVector (Vector3 v) {
		MetaInformation.Instance ().tileXVector = (Vector2)v;
	}

	public void SetYVector (Vector3 v) {
		MetaInformation.Instance ().tileYVector = (Vector2)v;
	}

	public float GetPPU () {
		return GetComponent<SpriteRenderer> ().sprite.pixelsPerUnit;
	}

}
