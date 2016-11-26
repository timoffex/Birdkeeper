using System;

namespace SavingLoading {
	[Serializable]
	public class NodePrimitive : Node {


		public NodePrimitive (object val, ObjectGraph graph) {
			graph.AddNode (this);
			representedObject = val;
			value = val;
		}


		public object value;


		protected override object Reconstruct () {
			return representedObject = value;
		}

		public override string ToString () {
			return value.ToString ();
		}
	}
}

