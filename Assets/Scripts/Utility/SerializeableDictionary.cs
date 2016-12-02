
using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver {
	[SerializeField]
	private List<TKey> keys = new List<TKey> ();

	[SerializeField]
	private List<TValue> values = new List<TValue> ();

	// save the dictionary to lists
	public void OnBeforeSerialize() {
		lock (keys) { // Lock for thread-safety : can't guarantee OnAfterDeserialize isn't called concurrently
			keys.Clear ();
			values.Clear ();


			KeyCollection oldKeys = this.Keys;
			ValueCollection oldValues = this.Values;

			TKey[] oldKeysArray = new TKey[oldKeys.Count];
			TValue[] oldValuesArray = new TValue[oldValues.Count];

			oldKeys.CopyTo (oldKeysArray, 0);
			oldValues.CopyTo (oldValuesArray, 0);

			for (int i = 0; i < oldKeysArray.Length; i++) {
				keys.Add (oldKeysArray [i]);
				values.Add (oldValuesArray [i]);
			}
		}
	}

	// load dictionary from lists
	public void OnAfterDeserialize() {
		lock (keys) {
			this.Clear ();

			if (keys.Count != values.Count)
				throw new System.Exception (string.Format ("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable."));
			else
				for (int i = 0; i < keys.Count; i++) 
					this.Add (keys [i], values [i]);
		}
	}
}
