
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AStar<NodeType> {

	public class EdgeType {
		public EdgeType (NodeType f, NodeType t, float w) {
			nFrom = f;
			nTo = t;
			nWeight = w;

		}

		public NodeType nFrom;
		public NodeType nTo;
		public float nWeight;
	}


	public delegate IEnumerable<EdgeType> NeighborDelegate (NodeType node);


	/// <summary>
	/// Solves the AStar problem given:
	/// 	A function that returns the neighbors of a vertex.
	/// 	A function that takes a vertex and returns its heuristic.
	/// 	A start node and an end node.
	/// Returns a path to the vertex with the smallest heuristic value.
	/// </summary>
	/// <param name="neighborsOf">Function mapping vertices to neighbors.</param>
	/// <param name="heuristic">Function mapping vertices to heuristic values.</param>
	/// <param name="start">Start node.</param>
	/// <param name="end">End node.</param>
	public static NodeType[] Solve (NeighborDelegate neighborsOf, Func<NodeType, float> heuristic, NodeType start, NodeType end) {
		Dictionary<NodeType, int> numSteps = new Dictionary<NodeType, int> ();

		// weight of minimum path to node; !pathWeight.ContainsKey(x) means pathWeight[x] == inf
		Dictionary<NodeType, float> pathWeight = new Dictionary<NodeType, float> ();

		// priority of node
		Dictionary<NodeType, float> priority = new Dictionary<NodeType, float> ();

		// !previous.ContainsKey(x) means x has no parent
		Dictionary<NodeType, NodeType> previous = new Dictionary<NodeType, NodeType> ();

		// !status.ContainsKey(x) means unseen, 'f' means fringe, 't' means tree
		Dictionary<NodeType, char> status = new Dictionary<NodeType, char> ();



		pathWeight [start] = 0;
		priority [start] = 0;
		status [start] = 'f';
		numSteps [start] = 0;

		NodeType bestEndCandidate = start;
		float bestHeuristic = heuristic (start);


//		UnityEngine.Debug.LogFormat ("Goal: {0}", end);

		NodeType minNode;
		while (FindMin (priority, status, heuristic, out minNode)) {

//			UnityEngine.Debug.LogFormat ("Node {0}, priority {1}", minNode, priority[minNode]);

			status [minNode] = 't';
			var nodeHeuristic = heuristic (minNode);


			#region Update best candidate.
			if (nodeHeuristic < bestHeuristic) {
				bestEndCandidate = minNode;
				bestHeuristic = nodeHeuristic;
			}
			#endregion

			#region If we reached end, stop.
			if (minNode.Equals (end)) {
				int pathLength = numSteps [minNode];
				NodeType[] path = new NodeType[pathLength];

				var node = minNode;
				for (int i = pathLength - 1; i >= 0; i--) {
					path [i] = node;
					node = previous [node];
				}

				return path;
			}
			#endregion

			#region Update fringe.
			foreach (EdgeType edge in neighborsOf (minNode)) {
				var nextNode = edge.nTo;

				var newWgt = pathWeight [minNode] + edge.nWeight;
				var newPriority = newWgt + heuristic (nextNode);

				if (!status.ContainsKey (nextNode)) {
					status [nextNode] = 'f';
					numSteps [nextNode] = numSteps [minNode] + 1;
					pathWeight [nextNode] = newWgt;
					priority [nextNode] = newPriority;
					previous [nextNode] = minNode;
				} // else, newWgt is at least oldWgt
			}
			#endregion
		}


		// Didn't reach end. Return path to "bestEndCandidate"
		NodeType nodec = bestEndCandidate;
		int pathLengthc = numSteps [nodec];
		NodeType[] pathc = new NodeType[pathLengthc];

		for (int i = pathLengthc - 1; i >= 0; i--) {
			pathc [i] = nodec;
			nodec = previous [nodec];
		}

		return pathc;
	}


	private static bool FindMin (Dictionary<NodeType, float> weight, Dictionary<NodeType, char> status, Func<NodeType, float> heuristic, out NodeType min) {
		bool found = false;
		float minWgt = float.PositiveInfinity;
		float minHrst = float.PositiveInfinity;
		min = default (NodeType);

		foreach (var kv in status) {
			if (kv.Value == 'f') {
				float wgt = weight [kv.Key];
				float hrst = heuristic (kv.Key);
				if (wgt < minWgt || (wgt == minWgt && hrst < minHrst)) {
					min = kv.Key;
					minWgt = weight [kv.Key];
					minHrst = hrst;
					found = true;
				}
			}
		}

		return found;
	}
}
