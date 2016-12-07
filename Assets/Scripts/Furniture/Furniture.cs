using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (RectangularGridObject))]
public class Furniture : MonoBehaviour {


	/// <summary>
	/// The furniture type's unique identifier.
	/// </summary>
	[HideInInspector]
	private uint furnitureTypeUniqueId;
	public uint FurnitureTypeID { get { return furnitureTypeUniqueId; } }


	[SerializeField] private List<uint> attractedCustomers;
	[SerializeField] private List<float> attractedCustomersWeights;



	/// <summary>
	/// Offset of far-away corner on the grid.
	/// </summary>
	public Vector3 gridCornerOffset;

	private RectangularGridObject _myGridInternal;
	public RectangularGridObject MyGrid {
		get {
			if (_myGridInternal == null)
				_myGridInternal = GetComponent<RectangularGridObject> ();
			if (_myGridInternal == null) {
				_myGridInternal = gameObject.AddComponent<RectangularGridObject> ();
				Debug.Log ("Furniture did not have a RectangularGridObject component; adding automatically!");
			}
			return _myGridInternal;
		}
	}
	public int gridX { get { return MyGrid.gridSizeX; } }
	public int gridY { get { return MyGrid.gridSizeY; } }



	public bool renderGrid = false;


	/// <summary>
	/// Used when the furniture is being dragged from the editor.
	/// </summary>
	public Furniture_hovering hoveringPrefab;
	public Sprite icon;


	private Shop shop;
	private Vector3 gridXVec;
	private Vector3 gridYVec;



	private IntPair farCorner;
	private IntPair ShopPosition {
		get {
			return farCorner;
		}
	}



	private SpriteRenderer spriteRenderer;

	void Awake () {
		float gridPerTile = MetaInformation.Instance ().numGridSquaresPerTile;
		gridXVec = MetaInformation.Instance ().tileXVector / gridPerTile;
		gridYVec = MetaInformation.Instance ().tileYVector / gridPerTile;

		if (attractedCustomers == null)
			attractedCustomers = new List<uint> ();
		if (attractedCustomersWeights == null)
			attractedCustomersWeights = new List<float> ();

		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start () {
		if (FurnitureTypeID == 0)
			Debug.LogError ("FurnitureTypeID is 0, so furniture will not be saved. Did you instantiate properly? Use Furniture.InstantiateFurnitureByID ().");
	}


	/// <summary>
	/// Returns objGrid[x,y] considering furniture's rotation. Actually just returns true
	/// if coordinates are in range.
	/// </summary>
	/// <returns><c>true</c>, if grid was gotten, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public bool GetGrid (int x, int y) {
		return (x >= 0 && y >= 0 && x < gridX && y < gridY);
	}


	void Update () {
		spriteRenderer.sortingOrder = 2 * (ShopPosition.x + ShopPosition.y + gridX + gridY - 2);
	}


	public void OnRenderObject () {
		if (renderGrid) {
			Matrix4x4 localToWorld = 
				Matrix4x4.TRS (gridCornerOffset, Quaternion.identity, Vector3.one)
					* transform.localToWorldMatrix;

			GridRenderer.RenderGrid ((x,y) => GetGrid (x,y),
				gridX,
				gridY,
				localToWorld,
				gridXVec,
				gridYVec, Color.white);
		}
	}

	public Sprite GetIcon () {
		return icon;
	}


	public Vector3 GetPivotPosition (Vector3 placementCoords) {
		return placementCoords - gridCornerOffset;
	}

	public IntPair GetStandingPosition () {
		return ShopPosition;
	}


	public bool TrySetPosition (IntPair pos) {
		farCorner = pos;
		UpdateTransformPosition ();
		return true;
	}

	private void UpdateTransformPosition () {
		if (shop != null)
			transform.position = (Vector3)shop.shopToWorldCoordinates (ShopPosition) - gridCornerOffset;
	}

	public IntPair GetPosition () {
		return ShopPosition;
	}

	public bool PlaceAtLocation (Shop shp, IntPair pos) {

		shop = shp;
		if (TrySetPosition (pos)) {
			shop.PlaceFurniture (pos.x, pos.y, this);

			return true;
		} else
			return false;
	}


	public Furniture_hovering GetHoveringPrefab () {
		return hoveringPrefab;
	}



	public IEnumerable<KeyValuePair<uint, float>> GetAttractedCustomers () {
		for (int i = 0; i < attractedCustomers.Count; i++)
			yield return new KeyValuePair<uint, float> (attractedCustomers [i], attractedCustomersWeights [i]);
	}

	public void RemoveAttractedCustomer (uint customerID) {
		int idx = attractedCustomers.IndexOf (customerID);

		if (idx >= 0) {
			attractedCustomers.RemoveAt (idx);
			attractedCustomersWeights.RemoveAt (idx);
		}
	}

	public void AddAttractedCustomer (uint customerID, float weight) {
		RemoveAttractedCustomer (customerID);

		attractedCustomers.Add (customerID);
		attractedCustomersWeights.Add (weight);
	}

	public void SetAttractedCustomerWeight (uint customerID, float weight) {
		int idx = attractedCustomers.IndexOf (customerID);

		attractedCustomersWeights [idx] = weight;
	}



	public static GameObject InstantiateFurnitureByID (uint id) {
		GameObject prefab = MetaInformation.Instance ().GetFurniturePrefabByID (id);

		GameObject furnitureGO = GameObject.Instantiate (prefab) as GameObject;
		furnitureGO.GetComponent<Furniture> ().furnitureTypeUniqueId = id;

		return furnitureGO;
	}
}
