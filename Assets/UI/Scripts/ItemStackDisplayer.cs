using UnityEngine;
using UnityEngine.UI;

public class ItemStackDisplayer : MonoBehaviour {

	public Image itemIcon;
	public Text itemName;
	public Text itemCount;

	public void DisplayStack (ItemStack stack) {
		itemIcon.sprite = stack.ItemType.Icon;
		itemName.text = stack.ItemType.Name;
		itemCount.text = stack.Count.ToString ();
	}
}
