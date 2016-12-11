using UnityEngine;
using System.Collections;
using System.Linq;



/// <summary>
/// Walks into shop, pops up a dialog request, walks out after dialog finishes or too much time passes.
/// </summary>
[RequireComponent (typeof (ShopMover))]
public class BasicCustomerController : MonoBehaviour {

	private IntPair startPosition;
	private ShopMover myShopMover;


	public CharacterDescription character;


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

		// Pop up a trading dialog.
		var possibleOffers = character.possibleTradingOffers.Where ((off) => Game.current.inventory.HasItem (off.Request.ItemType)).ToList ();

		TradingOffer trade = possibleOffers [Random.Range (0, possibleOffers.Count)];

		string myTradingText;
		if (character.formattedTradingStrings.Length > 0) {
			var strs = character.formattedTradingStrings;
			var idx = Random.Range (0, strs.Length);
			myTradingText = string.Format (strs [idx], trade.Offer.ItemType.Name, trade.Request.ItemType.Name,
				trade.Offer.Count, trade.Request.Count);
		} else
			myTradingText = "UNIMPLEMENTED TRADING TEXT!!";

		yield return TradingDialogUtility.OfferTrade (trade, myTradingText, (result) => {
			switch (result) {
			case TradingResult.SUCCEED:
				NotificationSystem.ShowNotificationIfPossible (string.Format ("Trade succeeded! You got {0} {1}.", trade.Offer.Count, trade.Offer.ItemType.Name));
				break;
			case TradingResult.FAILACCEPT:
				NotificationSystem.ShowNotificationIfPossible (string.Format ("Couldn't do the trade. You didn't have enough {0}", trade.Request.ItemType.Name));
				break;
			case TradingResult.FAILDENY:
				NotificationSystem.ShowNotificationIfPossible ("Declined trade.");
				break;
			}
		});


		// Return to start position.
		yield return myShopMover.MoveToPosition (startPosition, (suc) => {});

		// Despawn self!
		Destroy (gameObject);
	}

}