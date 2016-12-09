using UnityEngine;


[CreateAssetMenu (fileName = "New Character Description", menuName = "Create Character Description")]
public class CharacterDescription : ScriptableObject {

	public string catchPhrase;


	/// <summary>
	/// Place {0} in place of the item the character is offering, and
	/// {1} in the place of the item the character is requesting.
	/// </summary>
	public string formattedTradingString;


	public TradingOffer[] possibleTradingOffers;
}