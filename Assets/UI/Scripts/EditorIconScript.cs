using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// The script for an icon in the Editor panel of the game.
/// </summary>
public class EditorIconScript : EventTrigger {

	public IEditorDraggable draggableObject;

	public override void OnBeginDrag (PointerEventData data) {
		// Instantiate hovering object
		var hoverer = GameObject.Instantiate (draggableObject.GetHoveringPrefab ());

		// Attach EditorDraggedObject script to hoverer
		var edo = hoverer.AddComponent<EditorDraggedObject> ();
		edo.draggedObject = draggableObject;
	}
}
