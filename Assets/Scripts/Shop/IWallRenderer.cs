using UnityEngine;
using System.Collections;

public interface IWallRenderer {

	/// <summary>
	/// Places the left wall.
	/// </summary>
	/// <param name="y">The y coordinate.</param>
	/// <param name="tileCorner">Position of the lower-right corner.</param>
	void PlaceLeftWall (int y, Vector3 tileCorner, Transform parent);


	/// <summary>
	/// Places the right wall.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="tileCorner">Position of the lower-left corner.</param>
	void PlaceRightWall (int x, Vector3 tileCorner, Transform parent);

}
