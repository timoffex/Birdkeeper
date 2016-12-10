using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FurnitureItemDisplayScript : MonoBehaviour {

	public FurnitureItemDisplayer furnitureDisplayPrefab;


	private struct DisplayedFurnitureStack {
		public DisplayedFurnitureStack (FurnitureStack f) {
			fid = f.FurnitureID;
			count = f.Count;
		}

		public uint fid;
		public int count;
	}

	private List<DisplayedFurnitureStack> displayedFurnitureStacks;


	void Start () {
		displayedFurnitureStacks = new List<DisplayedFurnitureStack> ();
		RedrawFurnitureInventory ();
	}



	void Update () {

		// If the data we're displaying is outdated, redraw it.
		if (Game.current.furnitureInventory.GetFurnitureStacks ().Count () != displayedFurnitureStacks.Count)
			RedrawFurnitureInventory ();
		else if (!Game.current.furnitureInventory.GetFurnitureStacks ().All (
			(fstck) => displayedFurnitureStacks.Any (
				(dsp) => dsp.fid == fstck.FurnitureID && dsp.count == fstck.Count)))
			RedrawFurnitureInventory ();
	}


	private void RedrawFurnitureInventory () {
		foreach (Transform child in transform)
			Destroy (child.gameObject);

		displayedFurnitureStacks.Clear ();

		foreach (var furnitureItemStack in Game.current.furnitureInventory.GetFurnitureStacks ()) {
			var displayer = GameObject.Instantiate (furnitureDisplayPrefab, transform) as FurnitureItemDisplayer;
			displayer.DisplayFurnitureItem (furnitureItemStack);

			displayedFurnitureStacks.Add (new DisplayedFurnitureStack (furnitureItemStack));
		}
	}
}
