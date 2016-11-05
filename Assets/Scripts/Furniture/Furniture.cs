using UnityEngine;
using System.Collections;

public class Furniture : MonoBehaviour, IEditorDraggable {

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

	private IntPair shopPosition {
		get { return shop.worldToShopCoordinates (transform.position + gridCornerOffset); }
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
		spriteRenderer.sortingOrder = 2 * (shopPosition.x + shopPosition.y);
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

	public bool PlaceCloneAtMousePosition () {
		var shop = GameObject.Find ("Room").GetComponent<Shop> ();
		var clone = GameObject.Instantiate (gameObject).GetComponent<Furniture> ();
		var cornerLocation = shop.worldToShopCoordinates (GetHoverPositionFromMouse ());

		if (clone.PlaceAtLocation (shop, cornerLocation))
			return true;
		else {
			Destroy (clone.gameObject);
			return false;
		}
	}


	public Vector3 GetHoverPositionFromMouse () {
		return Camera.main.ScreenToWorldPoint (Input.mousePosition) - gridCornerOffset;
	}


	public bool PlaceAtLocation (Shop shp, IntPair pos) {
		if (shp.CanPlaceFurniture (pos.x, pos.y, this)) {
			shop = shp;
			transform.position = (Vector3)shop.shopToWorldCoordinates (pos) - gridCornerOffset;
			shop.PlaceFurniture (pos.x, pos.y, this);
			return true;
		} else
			return false;
	}


	public GameObject GetHoveringPrefab () {
		return hoveringPrefab;
	}
}
