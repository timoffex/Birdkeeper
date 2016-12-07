using System;


public abstract class IntPath {
	
	public abstract int Length { get; }
	public abstract IntPair this [int i] { get; }


	public static IntPath operator + (IntPath p1, IntPath p2) {
		return IntPathFactory.AddPaths (p1, p2);
	}
}