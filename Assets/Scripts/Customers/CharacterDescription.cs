using UnityEngine;
using Vexe.Runtime.Types;


[CreateAssetMenu (fileName = "New Character Description", menuName = "Create Character Description")]
public class CharacterDescription : BaseScriptableObject {
	
	public string catchPhrase;


	/// <summary>
	/// Place {0} in place of the item the character is offering, and
	/// {1} in the place of the item the character is requesting. {2}
	/// is the count of the offered item and {3} is the count of the
	/// requested item.
	/// </summary>
	public string[] formattedTradingStrings;


	public TradingOffer[] possibleTradingOffers;
}