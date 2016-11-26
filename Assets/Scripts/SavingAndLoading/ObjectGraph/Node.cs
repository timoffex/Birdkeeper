using System;


namespace SavingLoading {
	
	[Serializable]
	public abstract class Node {
		/// <summary>
		/// The object represented by this graph node.
		/// </summary>
		[NonSerialized] protected object representedObject;


		/// <summary>
		/// Returns the represented object, or reconstructs it first if
		/// it is null.
		/// </summary>
		/// <returns>The object.</returns>
		public object GetObject () {
			if (representedObject == null)
				representedObject = Reconstruct ();

			return representedObject;
		}



		protected abstract object Reconstruct ();


		public sealed override bool Equals (object obj) {
			if (obj is Node)
				return Object.ReferenceEquals (((Node)obj).representedObject, representedObject);
			return false;
		}

		public sealed override int GetHashCode () {
			return representedObject.GetHashCode ();
		}
	}
}