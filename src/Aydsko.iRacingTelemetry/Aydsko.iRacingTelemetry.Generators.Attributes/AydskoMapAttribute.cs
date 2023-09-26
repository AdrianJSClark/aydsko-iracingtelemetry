namespace Aydsko.iRacingTelemetry.Generators.Attributes;

/// <summary>Mark a property to map to an iRacing Telemetry variable.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AydskoMapAttribute : Attribute
{
    /// <summary>Name of the variable within the iRacing Telemetry.</summary>
    public string? IrsdkVariableName { get; }

    /// <summary>Constructs the attribute.</summary>
    /// <param name="irsdkVariableName">Name of the variable within the iRacing Telemetry.</param>
    public AydskoMapAttribute(string? irsdkVariableName = null)
    {
        IrsdkVariableName = irsdkVariableName;
    }
}
