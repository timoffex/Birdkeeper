using UnityEngine;
using System.Collections;

public class RoomRenderer : MonoBehaviour {

	public int roomSizeX;
	public int roomSizeY;


	// Floor tile prefab. Pivot at far-away corner.
	public GameObject floorTile;

	// Left wall tile prefab. Pivot at far-away corner (right corner).
	public GameObject leftWallTile;

	// Right wall tile prefab. Pivot at far-away corner (left corner).
	public GameObject rightWallTile;


	// X direction is down and to the right.
	public Vector2 tileXVector;

	// Y direction is down and to the left.
	public Vector2 tileYVector;

	// Use this for initialization
	void Start () {
		PlaceRoom (roomSizeX, roomSizeY);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlaceRoom (int sizeX, int sizeY) {

		/* Both of the following vectors should point downward, such that
			objects with smaller X or Y values are drawn before objects with larger
			X and Y values.

			Both have magnitude equal to the size of a tile along the
			shop's X and Y axes in world coordinates. */
		Vector2 xVector, yVector;


		xVector = tileXVector;
		yVector = tileYVector;



		// Represents the far corner of the room.
		Vector2 startPos = transform.position;

		// Draw wall tiles.
		for (int x = 0; x < sizeX; x++) {
			Vector2 pos = startPos + x * xVector;

			PlaceLeftWall (pos);
		}

		for (int y = 0; y < sizeY; y++) {
			Vector2 pos = startPos + y * yVector;

			PlaceRightWall (pos);
		}

		// Draw floor tiles.
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				Vector2 pos = startPos + x * xVector + y * yVector;

				PlaceFloor (pos);
			}
		}
	}

	// Places a floor tile with its far-away corner at position pos.
	void PlaceFloor (Vector2 pos) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;

		var tile = GameObject.Instantiate (floorTile, position, rotation, transform) as GameObject;
	}

	// Places a left wall tile with its far-away corner (right corner) at position pos.
	void PlaceLeftWall (Vector2 pos) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;

		var tile = GameObject.Instantiate (leftWallTile, position, rotation, transform) as GameObject;
	}

	// Places a right wall tile with its far-away corner (left corner) at position pos.
	void PlaceRightWall (Vector2 pos) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;

		var tile = GameObject.Instantiate (rightWallTile, position, rotation, transform) as GameObject;
	}
}
