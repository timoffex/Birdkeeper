using UnityEngine;
using System.Collections;


public class Furniture : MonoBehaviour, IEditorDraggable {


	/// <summary>
	/// The furniture type's unique identifier.
	/// </summary>
	[HideInInspector]
	private uint furnitureTypeUniqueId;
	public uint FurnitureTypeID { get { return furnitureTypeUniqueId; } }



	public int gridX;
	public int gridY;
	private bool[,] objGrid;

	/// <summary>
	/// Offset of far-away corner on the grid.
	/// </summary>
	public Vector3 gridCornerOffset;


	public bool renderGrid = false;

	public Sprite icon;

	/// <summary>
	/// Used when the furniture is being dragged from the editor.
	/// </summary>
	public GameObject hoveringPrefab;


	private RoomRenderer room;
	private Shop shop;
	private Vector3 gridXVec;
	private Vector3 gridYVec;

	private IntPair shopPosition;
	private IntPair ShopPosition {
		get {
			return shopPosition;
		}

		set {
			shopPosition = value;
			transform.position = (Vector3)shop.shopToWorldCoordinates (shopPosition) - gridCornerOffset;
		}
	}


	private SpriteRenderer spriteRenderer;

	void Awake () {
		room = GameObject.Find ("Room").GetComponent<RoomRenderer> ();
		float gridPerTile = room.GetComponent<Shop> ().numGridTilesPerFloorTile;
		gridXVec = room.generalTile.GetXVector () / gridPerTile;
		gridYVec = room.generalTile.GetYVector () / gridPerTile;


		spriteRenderer = GetComponent<SpriteRenderer> ();

		objGrid = new bool [gridX, gridY];
		for (int x = 0; x < gridX; x++)
			for (int y = 0; y < gridY; y++)
				objGrid [x, y] = true;
	}

	void Start () {
		if (FurnitureTypeID == 0)
			Debug.LogError ("FurnitureTypeID is 0, so furniture will not be saved. Did you instantiate properly? Use Furniture.InstantiateFurnitureByID ().");
	}


	/// <summary>
	/// Returns objGrid[x,y] considering furniture's rotation.
	/// </summary>
	/// <returns><c>true</c>, if grid was gotten, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public bool GetGrid (int x, int y) {
		return objGrid [x, y] && (x >= 0 && y >= 0 && x < gridX && y < gridY);
	}


	void Update () {
		spriteRenderer.sortingOrder = 2 * (ShopPosition.x + ShopPosition.y + gridX + gridY - 2);
		transform.position = (Vector3)shop.shopToWorldCoordinates (ShopPosition) - gridCornerOffset;
	}


	public void OnRenderObject () {
		if (renderGrid) {
			Matrix4x4 localToWorld = 
				Matrix4x4.TRS (gridCornerOffset, Quaternion.identity, Vector3.one)
					* transform.localToWorldMatrix;

			GridRenderer.RenderGrid (objGrid,
				localToWorld,
				gridXVec,
				gridYVec, Color.white);
		}
	}

	public Sprite GetIcon () {
		return icon;
	}

	public bool PlaceCloneAtPosition (Vector3 placementCoords) {
		var shop = GameObject.Find ("Room").GetComponent<Shop> ();
		var clone = GameObject.Instantiate (gameObject).GetComponent<Furniture> ();
		var cornerLocation = shop.worldToShopCoordinates (placementCoords);

		if (clone.PlaceAtLocation (shop, cornerLocation))
			return true;
		else {
			Destroy (clone.gameObject);
			return false;
		}
	}


	public Vector3 GetPivotPosition (Vector3 placementCoords) {
		return placementCoords - gridCornerOffset;
	}

	public IntPair GetStandingPosition () {
		return ShopPosition;
	}

	public bool PlaceAtLocation (Shop shp, IntPair pos) {
		if (shp.CanPlaceFurniture (pos.x, pos.y, this)) {
			shop = shp;
			ShopPosition = pos;
			shop.PlaceFurniture (pos.x, pos.y, this);
			return true;
		} else
			return false;
	}


	public GameObject GetHoveringPrefab () {
		return hoveringPrefab;
	}



	public static GameObject InstantiateFurnitureByID (uint id) {
		GameObject prefab = MetaInformation.Instance ().GetFurniturePrefabByID (id);

		GameObject furnitureGO = GameObject.Instantiate (prefab) as GameObject;
		furnitureGO.GetComponent<Furniture> ().furnitureTypeUniqueId = id;

		return furnitureGO;
	}
}
