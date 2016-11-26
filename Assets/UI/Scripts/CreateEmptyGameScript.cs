using UnityEngine;

public class CreateEmptyGameScript : MonoBehaviour {
	public void CreateGame () {
		Game.current.CreateEmpty ();
	}
}
