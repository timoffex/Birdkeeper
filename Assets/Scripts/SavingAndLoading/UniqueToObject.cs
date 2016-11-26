using System;


/// <summary>
/// Mark fields that may differ from the object's prefab. Those fields
/// are saved.
/// 
/// 
/// If the field is a List or Dictionary type, each of its entries is
/// saved according to the following rules.
/// 
/// If the field is a reference to an instance of a class that does not extend
/// from MonoBehaviour (and is therefore not on a GameObject), the instance is
/// saved according to the following rules.
/// 
/// 
/// If the field is a primitive, it is serialized normally and restored
/// when the object is reloaded.
/// 
/// If the field is a reference to an instantiated GameObject or a MonoBehaviour
/// on an instantiated GameObject, the reference is saved and the object
/// is saved too (the object is only saved once, so it is okay to have multiple
/// references to a single object). The GameObject's transform position and rotation
/// are also saved and restored on load.
/// 
/// If the field is a reference to anything else, an error is thrown.
/// </summary>
[AttributeUsage (AttributeTargets.Field)]
public class UniqueToObject : Attribute {
	
}

