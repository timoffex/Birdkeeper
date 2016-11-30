using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent (typeof (ShopMover))]
public class AIMoveRandomlyAndOfferTrades : MonoBehaviour {

	private ShopMover myShopMover;

	void Awake () {
		myShopMover = GetComponent<ShopMover> ();
	}


	void Start () {
		StartCoroutine (BeginAI ());
	}


	private IEnumerator BeginAI () {

		while (true) {
			switch (Random.Range (0, 2)) {
			case 0:
				// Offer a trade.
				var info = MetaInformation.Instance ();

				if (info != null) {
					int numMappings = info.GetNumberOfRegisteredItems ();

					if (numMappings != 0) {
						ItemType offType = info.GetItemTypeMappings ().ElementAt (Random.Range (0, numMappings)).Value;
						ItemType reqType = info.GetItemTypeMappings ().ElementAt (Random.Range (0, numMappings)).Value;

						int numOfferedItem = Random.Range (1, 10);
						int numRequestItem = Random.Range (1, 10);

						TradingOffer offer = TradingOffer.MakeOffer (
							                     new ItemStack (offType, numOfferedItem),
							                     new ItemStack (reqType, numRequestItem));

						bool didSucceed = false;
						yield return TradingDialogUtility.OfferTrade (offer, (success) => {
							didSucceed = success;
						});


						if (didSucceed)
							yield return DialogSystem.Instance ().DisplayMessage ("Trade succeeded.");
						else
							yield return DialogSystem.Instance ().DisplayMessage ("Trade failed.");


						break;
					} else
						goto case 1;
				} else {
					Debug.Log ("No MetaInformation instance found. AI will not make a random trade offer.");
					goto case 1;
				}


			case 1:
				// Move to a random piece of furniture in the shop.
				Shop shop = myShopMover.GetShop ();

				Furniture target = shop.GetFurnitureAtIndex (Random.Range (0, shop.GetFurnitureAmount ()));

				yield return myShopMover.MoveToPosition (target.GetStandingPosition (), (succ) => {});

				break;
			}
		}
	}
}
