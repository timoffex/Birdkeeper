
using UnityEngine;

/// <summary>
/// Implements a max-heap.
/// </summary>
public class PriorityQueue {

	private class PriorityObject {
		public object obj;
		public float priority;

		public PriorityObject (object el, float pr) {
			priority = pr;
			obj = el;
		}

		public static bool operator < (PriorityObject l, PriorityObject r) {
			return l.priority < r.priority;
		}

		public static bool operator > (PriorityObject l, PriorityObject r) {
			return l.priority > r.priority;
		}
	}

	private const int initHeapCapacity = 4;

	private int heapSize;
	private PriorityObject[] heap;

	public PriorityQueue () {
		heap = new PriorityObject[initHeapCapacity];
		heapSize = 0;
	}


	public int GetSize () {
		return heapSize;
	}

	public bool IsEmpty () {
		return GetSize () <= 0;
	}


	public void Enqueue (object el, float priority) {
		PriorityObject obj = new PriorityObject (el, priority);

		AppendToArray (obj);

		// Percolate-up
		int currentIndex = heapSize - 1;
		int parentIndex = (currentIndex - 1) / 2;

		while (parentIndex >= 0 && heap [parentIndex] < heap [currentIndex]) {
			var temp = heap [parentIndex];
			heap [parentIndex] = heap [currentIndex];
			heap [currentIndex] = temp;

			currentIndex = parentIndex;
			parentIndex = (currentIndex - 1) / 2;
		}
	}

	public object Dequeue () {
		heapSize = heapSize - 1;

		object max = heap [0].obj;

		PriorityObject newElement = heap [heapSize];
		FixHeap (newElement, 0);

		return max;
	}


	/// <summary>
	/// Enqueues the object if it's not already in the queue.
	/// If the object is already in the queue, changes its key.
	/// </summary>
	/// <param name="el">Element.</param>
	/// <param name="priority">New priority.</param>
	public void EnqueueOrChangeKey (object el, float priority) {
		int idx = Find (el);

		if (idx == -1)
			Enqueue (el, priority);
		else {
			float oldPriority = heap [idx].priority;
			var myElement = heap [idx];
			myElement.priority = priority;

			if (priority >= oldPriority) {
				// Increase key

				int parentIdx = (idx - 1) / 2;

				while (heap [parentIdx].priority < priority) {
					// Swap with parent
					heap [idx] = heap [parentIdx];
					heap [parentIdx] = myElement;

					idx = parentIdx;
					parentIdx = (idx - 1) / 2;
				}

			} else {
				// Decrease key

				FixHeap (myElement, idx);
			}
		}
	}

	/// <summary>
	/// Returns whether the specified element is inside the priority queue.
	/// </summary>
	/// <param name="el">Element.</param>
	public bool Contains (object el) {
		return Find (el) != -1;
	}

	/// <summary>
	/// Find the specified el.
	/// 
	/// Returns -1 if not found, else returns el's index in the heap.
	/// </summary>
	/// <param name="el">El.</param>
	private int Find (object el) {
		for (int i = 0; i < heapSize; i++)
			if (heap [i].Equals (el))
				return i;
		return -1;
	}

	private void FixHeap (PriorityObject obj, int index) {
		PriorityObject maxChild;
		int maxIndex;

		if (index * 2 + 1 < heapSize) {
			maxChild = heap [index * 2 + 1];
			maxIndex = index * 2 + 1;

			if (index * 2 + 2 < heapSize) {
				var child = heap [index * 2 + 2];
				if (child > maxChild) {
					maxChild = child;
					maxIndex = index * 2 + 2;
				}
			}

			if (maxChild > obj) {
				heap [index] = maxChild;
				FixHeap (obj, maxIndex);
			} else {
				heap [index] = obj;
			}
		} else {
			// else stop! no children to check; we're a leaf now
			heap [index] = obj;
		}
	}


	private void AppendToArray (PriorityObject el) {
		int cap = heap.GetLength (0);

		if (heapSize + 1 >= cap) {
			PriorityObject[] newHeap = new PriorityObject[cap * 2];

			for (int i = 0; i < heapSize; i++)
				newHeap [i] = heap [i];

			heap = newHeap;
		}

		heap [heapSize] = el;
		heapSize = heapSize + 1;
	}

}
