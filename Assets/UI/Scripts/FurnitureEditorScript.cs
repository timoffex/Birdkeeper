using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// TODO: Make this script more general.
public class FurnitureEditorScript : MonoBehaviour {

	private readonly float iconWidth = 50;
	private readonly float iconHeight = 50;

	private readonly float iconXSpacing = 10;
	private readonly float iconYSpacing = 10;

	private float iconWidthWithSpacing { get { return iconWidth + iconXSpacing; } }
	private float iconHeightWithSpacing { get { return iconHeight + iconYSpacing; } }

	public GameObject imagePrefab;

	void Start () {
		int x = 0;
		int y = 0;
		int maxX = 3;

		foreach (var kv in MetaInformation.Instance ().GetFurnitureMappings ()) {
			CreateFurnitureIcon (kv.Key, kv.Value.GetComponent<Furniture> ().GetIcon (), x, y);

			x = x + 1;
			if (x > maxX) {
				x = 0;
				y = y + 1;
			}
		}
	}


	private void CreateFurnitureIcon (uint fid, Sprite displayImg, int x, int y) {
		var furniturePrefabScript = MetaInformation.Instance ().GetFurniturePrefabByID (fid).GetComponent<Furniture> ();

		GameObject go = GameObject.Instantiate (imagePrefab);
		Image img = go.GetComponent<Image> ();
		img.rectTransform.SetParent (transform, false);

		// Position icon
		img.rectTransform.anchorMin = new Vector2 (0, 1);
		img.rectTransform.anchorMax = new Vector2 (0, 1);
		img.rectTransform.pivot = new Vector2 (0, 1);
		img.rectTransform.offsetMin = new Vector2 (x * iconWidthWithSpacing, -y * iconHeightWithSpacing - iconHeight);
		img.rectTransform.offsetMax = new Vector2 (x * iconWidthWithSpacing + iconWidth, - y * iconHeightWithSpacing);

		img.sprite = displayImg;



		var eventTrigger = go.AddComponent<EventTrigger> ();
		var onClickTrigger = new EventTrigger.Entry ();
		onClickTrigger.eventID = EventTriggerType.BeginDrag;
		onClickTrigger.callback = new EventTrigger.TriggerEvent ();
		onClickTrigger.callback.AddListener ((data) => {
			var hoverer = GameObject.Instantiate (furniturePrefabScript.GetHoveringPrefab ());
			var fedf = hoverer.AddComponent<FurnitureEditorDraggedFurniture> ();
			fedf.furnitureID = fid;
			fedf.draggedObject = furniturePrefabScript;

			data.Use ();
		});
		eventTrigger.triggers.Add (onClickTrigger);
	}
}
