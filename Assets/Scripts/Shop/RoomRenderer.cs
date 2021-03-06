﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Shop))]
public class RoomRenderer : MonoBehaviour {

	// Floor tile prefab. Pivot at far-away corner.
	public GameObject floorTile;

	// Left wall tile prefab. Pivot at far-away corner (right corner).
	public GameObject leftWallTile;

	// Right wall tile prefab. Pivot at far-away corner (left corner).
	public GameObject rightWallTile;


	// Render grid overlay?
	public bool renderGrid = false;


	private Shop shop;

	// Use this for initialization
	void Start () {
		shop = GetComponent<Shop> ();
		PlaceRoom (shop.numTilesX, shop.numTilesY);
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


		xVector = MetaInformation.Instance ().tileXVector;
		yVector = MetaInformation.Instance ().tileYVector;



		// Represents the far corner of the room.
		Vector2 startPos = transform.position;

		int order = 0;


		// Draw floor tiles.
		for (int x = 0; x < sizeX; x++) {
			for (int y = 0; y < sizeY; y++) {
				Vector2 pos = startPos + x * xVector + y * yVector;

				PlaceFloor (pos, order++);
			}
		}



		// Draw wall tiles.
		for (int y = 0; y < sizeY; y++) {
			Vector2 pos = startPos + y * yVector;

			PlaceLeftWall (pos, order++, y);
		}

		for (int x = 0; x < sizeX; x++) {
			Vector2 pos = startPos + x * xVector;

			PlaceRightWall (pos, order++, x);
		}
	}

	// Places a floor tile with its far-away corner at position pos.
	void PlaceFloor (Vector2 pos, int order) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;

		var tile = GameObject.Instantiate (floorTile, position, rotation, transform) as GameObject;
		var sr = tile.GetComponent<SpriteRenderer> ();
		sr.sortingOrder = order;
		sr.sortingLayerName = "RoomTiles";
	}

	// Places a left wall tile with its far-away corner (right corner) at position pos.
	void PlaceLeftWall (Vector2 pos, int order, int y) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;

		var wallRenderer = leftWallTile.GetComponent<IWallRenderer> ();

		if (wallRenderer == null) {
			var tile = GameObject.Instantiate (leftWallTile, position, rotation, transform) as GameObject;
			var sr = tile.GetComponent<SpriteRenderer> ();
			sr.sortingOrder = order;
			sr.sortingLayerName = "RoomTiles";
		} else {
			wallRenderer.PlaceLeftWall (y, pos, transform);
		}
	}

	// Places a right wall tile with its far-away corner (left corner) at position pos.
	void PlaceRightWall (Vector2 pos, int order, int x) {
		var position = new Vector3 (pos.x, pos.y, transform.position.z);
		var rotation = Quaternion.identity;


		var wallRenderer = rightWallTile.GetComponent<IWallRenderer> ();

		if (wallRenderer == null) {
			var tile = GameObject.Instantiate (rightWallTile, position, rotation, transform) as GameObject;
			var sr = tile.GetComponent<SpriteRenderer> ();
			sr.sortingOrder = order;
			sr.sortingLayerName = "RoomTiles";
		} else {
			wallRenderer.PlaceRightWall (x, pos, transform);
		}
	}




	public void OnRenderObject () {
		if (renderGrid) {

			GridRenderer.RenderGrid (shop.GetGrid, shop.numGridX, shop.numGridY,
				transform.localToWorldMatrix,
				MetaInformation.Instance ().tileXVector / MetaInformation.Instance ().numGridSquaresPerTile,
				MetaInformation.Instance ().tileYVector / MetaInformation.Instance ().numGridSquaresPerTile,
				Color.blue);


//
//			// Red around grid tiles
//			GridRenderer.RenderGrid (
//				shop.numTilesX * shop.numGridTilesPerFloorTile,
//				shop.numTilesY * shop.numGridTilesPerFloorTile,
//				transform.localToWorldMatrix,
//				generalTile.GetXVector () / shop.numGridTilesPerFloorTile,
//				generalTile.GetYVector () / shop.numGridTilesPerFloorTile,
//				Color.red);
//
//			// Green around floor tiles
//			GridRenderer.RenderGrid (shop.numTilesX, shop.numTilesY,
//				transform.localToWorldMatrix,
//				generalTile.GetXVector (), generalTile.GetYVector (), Color.green);
		}
	}
}
