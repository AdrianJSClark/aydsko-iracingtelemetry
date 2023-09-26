namespace Aydsko.iRacingTelemetry.Generators.Attributes;

/// <summary>Mark a class to map to iRacing Telemetry variables.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AydskoDataAttribute : Attribute
{
    /// <summary>Indicates that all properties of the class should be mapped with their names.</summary>
    public bool AutoMapAllProperties { get; }

    /// <summary>Creates the attribute.</summary>
    /// <param name=""autoMapAllProperties"">Indicates that all properties of the class should be mapped with their names.</param>
    public AydskoDataAttribute(bool autoMapAllProperties = false)
    {
        AutoMapAllProperties = autoMapAllProperties;
    }
}
