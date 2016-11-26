using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;
using System.Collections.Generic;

namespace SavingLoading {

	[Serializable]
	public class NodeGameObject : Node {


		public NodeGameObject (GameObject gameObject, ObjectGraph graph) {
			graph.AddNode (this);
			representedObject = gameObject;


			components = new List<ObjectComponent> ();


			name = gameObject.name;
			foreach (Component component in gameObject.GetComponents (typeof(Component)))
				components.Add (new ObjectComponent (component, graph));
		}



		public struct Parameter {
			public FieldInfo field;
			public Node value;
		}


		public struct ObjectComponent {

			public ObjectComponent (Component comp, ObjectGraph graph) {
				type = comp.GetType ();
				componentParameters = new List<Parameter> ();

				foreach (FieldInfo field in type.GetFields (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
					Parameter param = new Parameter ();
					param.field = field;
					param.value = NodeFactory.CreateNodeFor (field.GetValue (comp), graph);
					componentParameters.Add (param);
				}
			}

			public Type type;
			public List<Parameter> componentParameters;


			public override string ToString () {
				return type.Name;
			}
		}


		public List<ObjectComponent> components;
		public string name;



		protected override object Reconstruct () {
			GameObject obj = new GameObject ();

			foreach (ObjectComponent comp in components) {
				var c = obj.AddComponent (comp.type);

				foreach (Parameter param in comp.componentParameters)
					param.field.SetValue (c, param.value.GetObject ());
			}


			representedObject = obj;
			return representedObject;
		}

		public override string ToString (){
			string componentInfo = "";
			foreach (ObjectComponent comp in components)
				componentInfo += string.Format ("\t{0}\n", comp.ToString ());
			
			return string.Format ("[NodeGameObject:{0}]\n{1}", name, componentInfo);
		}
	}
}
