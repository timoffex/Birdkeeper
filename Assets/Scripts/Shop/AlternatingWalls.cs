using UnityEngine;


public class AlternatingWalls : MonoBehaviour, IWallRenderer {

	public GameObject[] leftWalls;
	public GameObject[] rightWalls;


	public void PlaceLeftWall (int p, Vector3 tileCorner, Transform parent) {
		var wallPrefab = leftWalls[p % leftWalls.Length];

		PlaceWall (p, 0, tileCorner, wallPrefab, parent);
	}

	public void PlaceRightWall (int p, Vector3 tileCorner, Transform parent) {
		var wallPrefab = rightWalls[p % rightWalls.Length];

		PlaceWall (0, p, tileCorner, wallPrefab, parent);
	}


	private void PlaceWall (int x, int y, Vector3 tileCorner, GameObject wallPrefab, Transform parent) {
		var wall = GameObject.Instantiate (wallPrefab, tileCorner, Quaternion.identity, parent) as GameObject;

		var sr = wall.GetComponent<SpriteRenderer> ();
		sr.sortingLayerName = "Room Tiles";
		sr.sortingOrder = 6 * (x + y);
	}

}
