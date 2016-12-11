using UnityEngine;
using System.Collections;


[RequireComponent (typeof (ShopMover)),
	RequireComponent (typeof (ICharacter))]
public class CustomerController : MonoBehaviour {

	private ShopMover myShopMover;
	private ICharacter myCharacter;


	// Use this for initialization
	void Start () {
		myShopMover = GetComponent<ShopMover> ();
		myCharacter = GetComponent<ICharacter> ();
	}


	void Update () {
		
	}


	private IEnumerator CustomerGeneralBehaviour () {
		Game game = Game.current;

		if (game == null) {
			Debug.Log ("Game.current is null; Customer won't do anything.");
			yield break;
		}

		for (int i = 0; i < Random.Range (1, 4); i++) {
			yield return myShopMover.MoveToPosition (myCharacter.WhereToGo ());
			yield return new WaitForSeconds (1 + Random.value * 2);
		}
	
	}


	private IEnumerator VisitRandomFurniture (Game game) {
		IntPair pos = game.furnitureInShop [Random.Range (0, game.furnitureInShop.Count)].furnitureRef.GetStandingPosition ();
		yield return myShopMover.MoveToPosition (pos);
		yield return new WaitForSeconds (1 + Random.value * 2);
	}
}
