using UnityEngine;
using System.Collections;

public class TradingOffer {
	private ItemStack offer;
	private ItemStack request;

	public ItemStack Offer { get { return offer; } }
	public ItemStack Request { get { return request; } }


	private TradingOffer (ItemStack off, ItemStack req) {
		offer = off;
		request = req;
	}


	/// <summary>
	/// Makes the trading offer.
	/// </summary>
	/// <returns>The offer.</returns>
	/// <param name="offeredItems">Offered items.</param>
	/// <param name="requestedItems">Requested items.</param>
	public static TradingOffer MakeOffer (ItemStack offeredItems, ItemStack requestedItems) {
		return new TradingOffer (offeredItems, requestedItems);
	}
}
