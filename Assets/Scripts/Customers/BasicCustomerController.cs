using UnityEngine;
using System.Collections;



/// <summary>
/// Walks into shop, pops up a dialog request, walks out after dialog finishes or too much time passes.
/// </summary>
[RequireComponent (typeof (ShopMover))]
public class BasicCustomerController : MonoBehaviour {

	private IntPair startPosition;
	private ShopMover myShopMover;

	void Start () {
		myShopMover = GetComponent<ShopMover> ();

		startPosition = myShopMover.GetPosition ();
	
		StartCoroutine (BasicCustomerRoutine ());
	}

	private IEnumerator BasicCustomerRoutine () {
		Game game = Game.current;

		if (game == null) {
			Debug.Log ("Game.current is null; Customer won't do anything.");
			yield break;
		}


		// Walk to a random furniture.
		bool succeededMovingIntoShop = false;

		while (!succeededMovingIntoShop) {
			var allFurniture = game.furnitureInShop;

			if (allFurniture.Count > 0) {
				IntPair randomFurniturePosition = allFurniture [Random.Range (0, allFurniture.Count)].furnitureRef.GetStandingPosition ();
				yield return myShopMover.MoveToPosition (randomFurniturePosition, (suc) => succeededMovingIntoShop = suc);
			} else {
				yield return myShopMover.MoveToPosition (new IntPair (0, 0));
				succeededMovingIntoShop = true;
			}

			if (!succeededMovingIntoShop)
				yield return new WaitForSeconds (0.1f); // to prevent rapid looping
		}

		// Pop up a dialog.
		DialogSystem dialogSys = DialogSystem.Instance ();
		if (dialogSys == null) {
			Debug.Log ("DialogSystem not found. Customer won't say anything.");
		} else {
			yield return dialogSys.DisplayMessage ("This is a default message!");
		}


		// Return to start position.
		bool succeededReturningToStart = false;

		while (!succeededReturningToStart) {
			yield return myShopMover.MoveToPosition (startPosition, (suc) => succeededReturningToStart = suc);

			if (!succeededReturningToStart)
				yield return new WaitForSeconds (0.1f); // to prevent rapid looping
		}


		// Despawn self!
		Destroy (gameObject);
	}

}