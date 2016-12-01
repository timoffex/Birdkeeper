


[System.Serializable]
public class FurnitureStack {
	private uint fid;
	private int count;


	public FurnitureStack (uint fid, int count) {
		this.fid = fid;
		this.count = count;
	}


	public uint FurnitureID { get { return fid; } }
	public int Count { get { return count; } }




	public void IncrementCount (int ct) {
		count += ct;
	}
}