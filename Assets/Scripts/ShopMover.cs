using UnityEngine;
using System.Collections;

/// <summary>
/// Defines methods for moving an object between shop locations.
/// </summary>
public abstract class ShopMover : MonoBehaviour {


	/// <summary>
	/// Returns the shop the mover is inside.
	/// </summary>
	/// <returns>The shop.</returns>
	public abstract Shop GetShop ();


	/// <summary>
	/// Returns position inside the shop.
	/// </summary>
	/// <returns>The position.</returns>
	public abstract IntPair GetPosition ();


	/// <summary>
	/// Sets the ShopMover's position. Warning: this CAN fail.
	/// </summary>
	/// <param name="pos">Position.</param>
	public abstract void SetPosition (IntPair pos);


	public delegate void SuccessCallback (bool success);

	/// <summary>
	/// Moves to the specified position and calls the callback with true on success or false on failure.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="callback">Callback that takes a boolean which is "true" on success and "false" otherwise.</param>
	public abstract IEnumerator MoveToPosition (IntPair pos, SuccessCallback callback = null);
}
