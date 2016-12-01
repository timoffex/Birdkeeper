using UnityEngine;
using UnityEngine.UI;


public class ItemDisplayer : MonoBehaviour {

	public Image itemIcon;
	public Text itemName;

	public void DisplayItem (ItemType item) {
		if (itemIcon != null) itemIcon.sprite = item.Icon;
		if (itemName != null) itemName.text = item.Name;
	}
}
