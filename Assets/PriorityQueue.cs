
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

	private const int initHeapCapacity = 0;

	private int heapSize;
	private PriorityObject[] heap;

	public PriorityQueue () {
		heap = new PriorityObject[initHeapCapacity];
		heapSize = 0;
	}


	public int GetSize () {
		return heapSize;
	}


	public void Enqueue (object el, float priority) {
		PriorityObject obj = new PriorityObject (el, priority);

		AppendToArray (obj);

		// Percolate-up
		int currentIndex = heapSize - 1;
		int parentIndex = (currentIndex - 1) / 2;

		while (heap [parentIndex] < heap [currentIndex]) {
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
			}
		}

		// else stop! no children to check; we're a leaf now
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
