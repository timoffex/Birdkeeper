
using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
	[SerializeField]
	private KeyList keys = new KeyList ();

	[SerializeField]
	private ValueList values = new ValueList ();

	// save the dictionary to lists
	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();

		KeyCollection oldKeys = this.Keys;


		foreach (TKey key in oldKeys) {
			TValue value;
			if (this.TryGetValue (key, out value)) {
				keys.Add (key);
				values.Add (value);
			}
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize() {
		this.Clear();

		if (keys.Count != values.Count)
			throw new System.Exception (string.Format ("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
		else {
			for (int i = 0; i < keys.Count; i++)
				this.Add (keys [i], values [i]);
		}
	}


	[Serializable] private class KeyList : List<TKey> { }
	[Serializable] private class ValueList : List<TValue> { }
}
