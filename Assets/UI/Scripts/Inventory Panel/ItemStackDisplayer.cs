using UnityEngine;
using UnityEngine.UI;

public class ItemStackDisplayer : MonoBehaviour {

	public Image itemIcon;
	public Text itemName;
	public Text itemCount;

	public void DisplayStack (ItemStack stack) {
		if (itemIcon != null) itemIcon.sprite = stack.ItemType.Icon;
		if (itemName != null) itemName.text = stack.ItemType.Name;
		if (itemCount != null) itemCount.text = stack.Count.ToString ();
	}
}
