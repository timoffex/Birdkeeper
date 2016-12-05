
using System.Collections;

public interface ICharacter {


	/// <summary>
	/// Returns the position where the character wants to go.
	/// </summary>
	IntPair WhereToGo ();

	/// <summary>
	/// Started when the player initiates a dialog.
	/// </summary>
	IEnumerator OnPlayerInitiatesDialog ();
}