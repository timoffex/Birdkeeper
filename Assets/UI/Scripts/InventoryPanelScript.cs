using UnityEngine;


public class InventoryPanelScript : MonoBehaviour {

	public ItemStackDisplayer itemStackDisplayPrefab;

	void OnEnable () {
		Rebuild ();
	}


	private void Rebuild () {
		Debug.Log ("InventoryPanelScript::Rebuild()");

		for (int i = 0; i < transform.childCount; i++)
			Destroy (transform.GetChild (i).gameObject);
		
		foreach (ItemStack stack in Game.current.inventory.GetItemStacks ()) {
			ItemStackDisplayer stackObj = GameObject.Instantiate (itemStackDisplayPrefab, transform) as ItemStackDisplayer;
			stackObj.DisplayStack (stack);
		}
	}
}

