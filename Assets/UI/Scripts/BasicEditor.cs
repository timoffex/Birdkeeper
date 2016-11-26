using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;

[System.Serializable]
public class BasicEditor : MonoBehaviour {
	
	/// <summary>
	/// Array of items to be displayed.
	/// </summary>
	[SerializeField, HideInInspector]
	public GameObject[] items;

	// Use this for initialization
	void Start () {
		gameObject.AddComponent<VerticalLayoutGroup> ();

		foreach (GameObject item in items) {
			if (item == null)
				continue;


			var draggable = item.GetComponent<IEditorDraggable> ();

			if (draggable != null)
				CreateIconFor (draggable);
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
