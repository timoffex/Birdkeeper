﻿using UnityEngine;
using System.Collections;

public class ShopMoverGrid : ShopMover {


	private Shop shop;

	private IntPair farCorner = new IntPair (0, 0);
	private IntPair nearCorner {
		get {
			return new IntPair (farCorner.x + gridWidth - 1, farCorner.y + gridHeight - 1);
		}
	}
	private IntPair middlePosition {
		get {
			return new IntPair ((farCorner.x + nearCorner.x) / 2, (farCorner.y + nearCorner.y) / 2);
		}
	}

	public Vector2 gridOffset = new Vector2 (0, 0);
	public int gridWidth = 1;
	public int gridHeight = 1;


	/// <summary>
	/// In grid squares per second.
	/// </summary>
	public float speed = 10f;


	private SpriteRenderer spriteRenderer;

	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		shop = GameObject.Find ("Room").GetComponent<Shop> (); // TODO SHOP
		StartCoroutine (LateStart ());

	}

	void Update () {
		spriteRenderer.sortingOrder = 2 * (nearCorner.x + nearCorner.y) - 2;
	}

	IEnumerator LateStart () {
		yield return new WaitForEndOfFrame ();
		transform.position = GetShop ().shopToWorldCoordinates (farCorner) - gridOffset;
	}




	public override Shop GetShop () {
		return shop;
	}


	public override IntPair GetPosition () {
		return middlePosition;
	}



	public override IEnumerator MoveToPosition (IntPair pos, SuccessCallback callback = null) {

		IntPair endPoint = pos;//new IntPair (pos.x - (gridWidth+1)/2, pos.y - (gridHeight+1)/2);

		if (!shop.IsPositionInGrid (endPoint)) {
			callback (false);
			yield break;
		}

		int cGridWidth = shop.numGridX - gridWidth + 1;
		int cGridHeight = shop.numGridY - gridHeight + 1;
		bool[,] condensedGrid = new bool[cGridWidth, cGridHeight];

		for (int x = 0; x < cGridWidth; x++) {
			for (int y = 0; y < cGridHeight; y++) {

				condensedGrid [x, y] = false;
				for (int a = x; a < x + gridWidth && !condensedGrid [x, y]; a++)
					for (int b = y; b < y + gridHeight  && !condensedGrid [x, y]; b++)
						if (GetShop ().GetGrid (a, b))
							condensedGrid [x, y] = true;

			}
		}


		IntPair[] path = Pathfinding.FindPath (condensedGrid, farCorner, endPoint);

		if (path == null)
			callback (false);
		else {
			
			for (int i = 0; i < path.GetLength (0); i++) {


				/* Gliding script */
				float dx = farCorner.x - path [i].x;
				float dy = farCorner.y - path [i].y;
				float dist = Mathf.Sqrt (dx * dx + dy * dy);

				Vector3 target = (Vector3)GetShop ().shopToWorldCoordinates (path [i]) - (Vector3)gridOffset;
				Vector3 original = transform.position;

				float timeStart = Time.time;
				float p = 0;

				while (p < 1) {
					p = Mathf.Min (1, (Time.time - timeStart) * speed / dist);
					transform.position = (1 - p) * original + p * target;

					yield return new WaitForEndOfFrame ();
				}
				/* Gliding script */


				farCorner = path [i];
			}

			callback (true);

		}
	}
}