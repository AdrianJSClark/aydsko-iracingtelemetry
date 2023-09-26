namespace Aysdko.iRacingTelemetry.Generators.Mapper;

/// <summary>
/// This attribute marks a class or struct as a custom Aysdko data package
/// A mapper for this class will be automatically generated with the name
/// {ClassName}_AysdkoMapper
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AysdkoDataAttribute : Attribute
{
    public bool AutoMapAllProperties { get; }

    public AysdkoDataAttribute(bool autoMapAllProperties = false)
    {
        AutoMapAllProperties = autoMapAllProperties;
    }
}
