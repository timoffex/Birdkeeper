using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BasicEditor : MonoBehaviour {

	/// <summary>
	/// Array of items to be displayed.
	/// </summary>
	public IEditorDraggable[] items;
	public GameObject onlyItem;

	// Use this for initialization
	void Start () {
		items = new IEditorDraggable[1];
		items [0] = onlyItem.GetComponent<IEditorDraggable> ();


		gameObject.AddComponent<VerticalLayoutGroup> ();


		foreach (IEditorDraggable item in items) {
			CreateIconFor (item);
		}
	}


	private void CreateIconFor (IEditorDraggable item) {
		var gobj = new GameObject ("EDITOR_ITEM");
		gobj.transform.parent = transform;

		var img = gobj.AddComponent<Image> ();
		img.sprite = item.GetIcon ();


		var iconScript = gobj.AddComponent<EditorIconScript> ();
		iconScript.draggableObject = item;
	}
}
