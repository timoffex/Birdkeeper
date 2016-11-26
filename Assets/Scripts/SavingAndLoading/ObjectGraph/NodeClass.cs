using System;
using System.Collections.Generic;
using System.Reflection;

namespace SavingLoading {

	[Serializable]
	public class NodeClass : Node {


		public NodeClass (object obj, ObjectGraph graph) {
			graph.AddNode (this);
			representedObject = obj;

			Type type = obj.GetType ();
			parameters = new List<Parameter> ();

			foreach (FieldInfo field in type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				Parameter param = new Parameter ();
				param.field = field;
				param.value = NodeFactory.CreateNodeFor (field.GetValue (obj), graph);

				parameters.Add (param);
			}
		}


		public struct Parameter {
			public FieldInfo field;
			public Node value;
		}

		public List<Parameter> parameters;
		public Type type;

		protected override object Reconstruct () {
			var constructor = type.GetConstructor (Type.EmptyTypes);

			representedObject = constructor.Invoke (new object [0]);
			foreach (Parameter parameter in parameters)
				parameter.field.SetValue (representedObject, parameter.value.GetObject ());

			return representedObject;
		}


		public override string ToString () {
			return string.Format ("[NodeClass:{0}]", representedObject.GetType ());
		}
	}
}

