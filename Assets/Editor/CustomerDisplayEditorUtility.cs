using UnityEditor;

using System.Linq;

public static class CustomerDisplayEditorUtility {
	public static uint CustomerDropdown (uint currentCustomer) {
		var customerMappings = MetaInformation.Instance ().GetCustomerIDMappings ().ToList ();

		var customerNames = customerMappings.Select ((kv) => kv.Value.name).ToList ();
		var customerIDs = customerMappings.Select ((kv) => kv.Key).ToList ();


		int curIdx = customerIDs.IndexOf (currentCustomer);


		int newIdx = EditorGUILayout.Popup (curIdx, customerNames.ToArray ());

		return customerIDs [newIdx];
	}
}
