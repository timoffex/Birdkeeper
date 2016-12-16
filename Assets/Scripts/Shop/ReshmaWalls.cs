using UnityEngine;
using System.Linq;

public class ReshmaWalls : MonoBehaviour, IWallRenderer {

	public WallQualities[] leftWalls;
	public WallQualities[] rightWalls;


	public void PlaceLeftWall (int p, Vector3 tileCorner, Transform parent) {

		int idx = 0;
		int nextWallPosition = 0;
		while (nextWallPosition < p) {
			if (leftWalls [idx++ % leftWalls.Length].doubleWall)
				nextWallPosition += 2;
			else
				nextWallPosition += 1;
		}

		if (nextWallPosition == p) {
			var wallPrefab = leftWalls [idx % leftWalls.Length];

			int total = Game.current.shopSizeY;
			int endPosition = nextWallPosition + (wallPrefab.doubleWall ? 2 : 1);

			if (endPosition <= total) {
				PlaceWall (p, 0, tileCorner, wallPrefab.gameObject, parent);
			} else {
				wallPrefab = leftWalls.Where ((wall) => !wall.doubleWall).First ();
				PlaceWall (p, 0, tileCorner, wallPrefab.gameObject, parent);
			}
		}
	}

	public void PlaceRightWall (int p, Vector3 tileCorner, Transform parent) {
		var wallPrefab = rightWalls[p % rightWalls.Length];

		PlaceWall (0, p, tileCorner, wallPrefab.gameObject, parent);
	}


	private void PlaceWall (int x, int y, Vector3 tileCorner, GameObject wallPrefab, Transform parent) {
		var wall = GameObject.Instantiate (wallPrefab, tileCorner, Quaternion.identity, parent) as GameObject;

		var sr = wall.GetComponent<SpriteRenderer> ();
		sr.sortingLayerName = "Room Tiles";
		sr.sortingOrder = 6 * (x + y);
	}

}
