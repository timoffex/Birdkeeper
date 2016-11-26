using UnityEngine;
using System.Collections;

public class LoadGameScript : MonoBehaviour {

	public void LoadGame () {
		Game.current.Load ();
	}

}

