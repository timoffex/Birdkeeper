using System;


/// <summary>
/// Mark fields that may differ from the object's prefab. Those fields
/// are saved separately.
/// </summary>
[AttributeUsage (AttributeTargets.Field)]
public class UniqueToObject : Attribute {
	
}

