using UnityEngine;
using System.Collections;

public interface IEditorDraggable {

	/// <summary>
	/// Returns the icon that should be used when displaying the object in the editor.
	/// </summary>
	/// <returns>The icon.</returns>
	Sprite GetIcon ();



	/// <summary>
	/// Returns the prefab that should be used to visualize the object while it is being dragged.
	/// </summary>
	/// <returns>The hovering prefab.</returns>
	GameObject GetHoveringPrefab ();


	/// <summary>
	/// Places a clone of the object at the current mouse position.
	/// </summary>
	/// <returns> True if placed succesfully, false otherwise. </returns>
	bool PlaceCloneAtPosition (Vector3 placementCoords);

	/// <summary>
	/// Returns the world position where the pivot should be placed if it is
	/// hovering over the mouse coordinates.
	/// </summary>
	/// <param name="placementCoords">The coordinates at which the object will be placed if released</param>
	/// <returns>The position where the pivot should be.</returns>
	Vector3 GetPivotPosition (Vector3 placementCoords);
}
