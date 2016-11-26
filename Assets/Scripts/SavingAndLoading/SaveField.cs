using System;

/// <summary>
/// Place on fields that should be saved inside a Saveable class.
/// </summary>
[AttributeUsage (AttributeTargets.Field)]
public class SaveField : Attribute {
}

