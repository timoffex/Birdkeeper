using UnityEngine;
using System;
using System.Reflection;
using System.Runtime.Serialization;



namespace SavingLoading {
	public static class NodeFactory {

		/// <summary>
		/// Creates an Node for a given object and adds it to the given
		/// graph.
		/// 
		/// If the node for the object already exists, just returns that node.
		/// 
		/// Three possibilities:
		/// 	1) The object is a primitive type. A NodePrimitive is created.
		/// 	2) The object is a GameObject or is a Component / derived from Component.
		/// 	3) The object is something else. A NodeClass is created.
		/// </summary>
		/// <returns>The node for.</returns>
		/// <param name="obj">Object.</param>
		public static Node CreateNodeFor (object obj, ObjectGraph graph) {

			Node retVal;


			if (obj.GetType ().IsPrimitive)
				retVal = new NodePrimitive (obj, graph);
			else if (typeof(Component).IsInstanceOfType (obj)) {
				Component comp = (Component)obj;
				GameObject gameObject = comp.gameObject;

				if (!graph.IsObjectInGraph (gameObject, out retVal))
					retVal = new NodeGameObject (gameObject, graph);
			} else if (obj is GameObject) {
				if (!graph.IsObjectInGraph (obj, out retVal))
					retVal = new NodeGameObject ((GameObject)obj, graph);
			} else {
				if (!graph.IsObjectInGraph (obj, out retVal))
					retVal = new NodeClass (obj, graph);
			}

			return retVal;
		}
	}
}

