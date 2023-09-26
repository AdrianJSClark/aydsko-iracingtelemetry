namespace Aydsko.iRacingTelemetry.Generators.Mapper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AydskoMapAttribute : Attribute
{
    public string? IrsdkVariableName { get; }

    public AydskoMapAttribute(string? irsdkVariableName = null)
    {
        IrsdkVariableName = irsdkVariableName;
    }
}
