﻿using UnityEngine;
using System.Collections;

public class Furniture_hovering : MonoBehaviour {

	/// <summary>
	/// Used to automatically set gridX, gridY and gridCornerOffset.
	/// </summary>
	public Furniture originalFurniture;

	public int gridX { get { return originalFurniture.gridX; } }
	public int gridY { get { return originalFurniture.gridY; } }

	/// <summary>
	/// Offset of far-away corner on the grid.
	/// </summary>
	public Vector3 gridCornerOffset { get { return originalFurniture.gridCornerOffset; } }


	private Vector3 gridXVec;
	private Vector3 gridYVec;


	void Awake () {
		float gridPerTile = MetaInformation.Instance ().numGridSquaresPerTile;
		gridXVec = MetaInformation.Instance ().tileXVector / gridPerTile;
		gridYVec = MetaInformation.Instance ().tileYVector / gridPerTile;
	}

	public void OnRenderObject () {
		
		Matrix4x4 localToWorld = 
			Matrix4x4.TRS (gridCornerOffset, Quaternion.identity, Vector3.one)
			* transform.localToWorldMatrix;
		
		GridRenderer.RenderGrid (gridX, gridY,
			localToWorld,
			gridXVec,
			gridYVec, Color.white);
	
	}
}
