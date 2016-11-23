
using System;
using System.Collections;
using System.Collections.Generic;

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
		Dictionary<NodeType, int> distance = new Dictionary<NodeType, int> ();
		Dictionary<NodeType, float> pathWeight = new Dictionary<NodeType, float> ();
		Dictionary<NodeType, NodeType> previous = new Dictionary<NodeType, NodeType> ();
		PriorityQueue fringe = new PriorityQueue ();
		fringe.Enqueue (start, 0);

		distance [start] = 0;
		pathWeight [start] = 0;

		NodeType bestEndCandidate = start;
		float bestHeuristic = heuristic (start);

		/*
			Algorithm:
				Fringe starts with the start node
				bestEndCandidate = start

				while fringe is not empty:
					node = pop element from fringe

					if heuristic(node) < bestHeuristic:
						bestEndCandidate = node
						bestHeuristic = heuristic(node)

					if node is end node, return path

					for each edge from node to W:
						newDistance = distances[node] + 1
						newPathWeight = pathWeight[node] + edge weight
						
						if newPathWeight < pathWeight[W]:
							distances[W] = newDistance
							pathWeight[W] = newPathWeight
							previous[W] = node
							fringe.EnqueueOrChangeKey(W, newPathWeight + heuristic(W))
							
				return no path
		*/

		while (!fringe.IsEmpty ()) {
			var node = (NodeType) fringe.Dequeue ();

			var nodeHeuristic = heuristic (node);
			if (nodeHeuristic < bestHeuristic) {
				bestEndCandidate = node;
				bestHeuristic = nodeHeuristic;
			}

			if (node.Equals (end)) {
				int pathLength = distance [node];
				NodeType[] path = new NodeType[pathLength];

				for (int i = pathLength - 1; i >= 0; i--) {
					path [i] = node;
					node = previous [node];
				}

				return path;
			}

			foreach (EdgeType edge in neighborsOf (node)) {
				var W = edge.nTo;

				var newDist = distance [node] + 1;
				var newWght = pathWeight [node] + edge.nWeight;

				float wWght = 0;
				if (!pathWeight.TryGetValue (W, out newWght))
					pathWeight [W] = wWght = float.PositiveInfinity;

				if (newWght < wWght) {
					distance [W] = newDist;
					pathWeight [W] = newWght;
					previous [W] = node;
					fringe.EnqueueOrChangeKey (W, -newWght - heuristic (W));
				}
			}
		}


		// Didn't reach end. Return path to "bestEndCandidate"
		NodeType nodec = bestEndCandidate;
		int pathLengthc = distance [nodec];
		NodeType[] pathc = new NodeType[pathLengthc];

		for (int i = pathLengthc - 1; i >= 0; i--) {
			pathc [i] = nodec;
			nodec = previous [nodec];
		}

		return pathc;
	}
}
