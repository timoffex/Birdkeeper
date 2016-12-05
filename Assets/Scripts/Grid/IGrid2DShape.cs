
using System.Collections.Generic;

public interface IGrid2DShape {

	IEnumerable<IntPair> GetOccupiedSquares ();
	IEnumerable<IntPair> GetOccupiedXEdges ();
	IEnumerable<IntPair> GetOccupiedYEdges ();

	bool DoesOccupySquare (IntPair offset);
	bool DoesOccupyXEdge (IntPair offset);
	bool DoesOccupyYEdge (IntPair offset);

}