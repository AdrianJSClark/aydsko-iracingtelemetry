namespace Aydsko.iRacingTelemetry.Generators.Mapper;

/// <summary>
/// This attribute marks a class or struct as a custom Aydsko data package
/// A mapper for this class will be automatically generated with the name
/// {ClassName}_AydskoMapper
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AydskoDataAttribute : Attribute
{
    public bool AutoMapAllProperties { get; }

    public AydskoDataAttribute(bool autoMapAllProperties = false)
    {
        AutoMapAllProperties = autoMapAllProperties;
    }
}
