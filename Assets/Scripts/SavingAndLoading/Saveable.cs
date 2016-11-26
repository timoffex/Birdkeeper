using System;


/// <summary>
/// Place on any class that should be saved when the game is saved.
/// </summary>
[AttributeUsage (AttributeTargets.Class)]
public class Saveable : Attribute {

	private bool savePublic;


	public bool SavePublic {
		get { return savePublic; }
	}
		

	/// <param name="savePublic">If set to <c>false</c>, public attributes are not saved (default is true).</param>
	public Saveable (bool savePublic = true) {
		this.savePublic = savePublic;
	}
}
