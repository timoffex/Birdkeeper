using UnityEngine;
using System.Collections;

public class SaveGameScript : MonoBehaviour {

	public void SaveGame () {
		Game.current.Save ();
	}

}
