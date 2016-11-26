using System.Collections.Generic;
using System;
using UnityEngine;


namespace SavingLoading {
	
	[Serializable]
	public class ObjectGraph {


		// Vertex set
		private List<Node> vertices = new List<Node> ();

		/// <summary>
		/// The root node.
		/// </summary>
		[SerializeField] private Node rootNode;



		public bool IsObjectInGraph (object obj, out Node node) {
			foreach (Node n in vertices) {
				if (object.ReferenceEquals (n.GetObject (), obj)) {
					node = n;
					return true;
				}
			}

			node = null;
			return false;
		}


		public void AddNode (Node node) {
			vertices.Add (node);
		}


		public object GetRoot () {
			return rootNode.GetObject ();
		}


		public void SetRoot (Node root) {
			rootNode = root;
		}


		public void PrintDebug () {
			Debug.Log ("VERTICES:");
			Debug.Log (string.Format ("Total: {0}", vertices.Count));

			foreach (Node n in vertices)
				Debug.Log (string.Format ("\t{0}", n.ToString ()));
		}



		public static ObjectGraph CreateObjectGraph (object rootObject) {
			ObjectGraph graph = new ObjectGraph ();
			graph.SetRoot (NodeFactory.CreateNodeFor (rootObject, graph));
			return graph;
		}
	}
}
