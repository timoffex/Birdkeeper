using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName="Create Trading System Data Container", fileName="Trading System Data.asset")]
public class TradingDialogUtility : ScriptableObject {


	public AudioClip acceptSound;
	public AudioClip declineSound;


	/// <summary>
	/// Offers the trade. Success delegate is called before the enumerator finishes but after
	/// the trade is performed.
	/// </summary>
	/// <returns>The trade.</returns>
	/// <param name="trade">Trade.</param>
	/// <param name="success">Success.</param>
	public static IEnumerator OfferTrade (TradingOffer trade, System.Action<TradingResult> success) {

		var offerCount = trade.Offer.Count;
		var offerName = trade.Offer.ItemType.Name;
		var requestCount = trade.Request.Count;
		var requestName = trade.Request.ItemType.Name;

		string tradeText = string.Format ("You are being offered {0} {1}{2} for {3} {4}{5}.",
			offerCount, offerName, offerCount > 1 ? "s" : "",
			requestCount, requestName, requestCount > 1 ? "s" : "");

		return OfferTrade (trade, tradeText, success);
	}

	/// <summary>
	/// Offers the trade, presenting the given text. Success delegate is called before the enumerator
	/// finishes but after the trade is performed.
	/// </summary>
	/// <returns>The trade.</returns>
	/// <param name="trade">Trade.</param>
	/// <param name="tradeText">Trade text.</param>
	/// <param name="success">Success.</param>
	public static IEnumerator OfferTrade (TradingOffer trade, string tradeText, System.Action<TradingResult> success) {
		

		var acceptOption = new DialogBox.Choice ("Accept", () => {
			if (PerformTrade (trade)) {
				success (TradingResult.SUCCEED);
				PlayClip (instance.acceptSound);
			} else {
				success (TradingResult.FAILACCEPT);
			}
		});

		var denyOption = new DialogBox.Choice ("Deny", () => {
			success (TradingResult.FAILDENY);
			PlayClip (instance.declineSound);
		});

		yield return DialogSystem.Instance ().DisplayMessageAndChoices (tradeText, acceptOption, denyOption);
	}


	private static bool PerformTrade (TradingOffer trade) {
		if (Game.current.inventory.HasStack (trade.Request)) {
			Game.current.inventory.SubtractStack (trade.Request);
			Game.current.inventory.AddStack (trade.Offer);
			return true;
		} else
			return false;
	}


	private static void PlayClip (AudioClip clip) {
		AudioSource src = FindObjectOfType<AudioSource> ();
		if (src != null)
			src.PlayOneShot (clip);
	}


	private static TradingDialogUtility instance {
		get {
			return Resources.Load<TradingDialogUtility> ("Trading System Data");
		}
	}
}
