namespace Aysdko.iRacingTelemetry.Generators.Mapper;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class AysdkoMapAttribute : Attribute
{
    public string? IrsdkVariableName { get; }

    public AysdkoMapAttribute(string? irsdkVariableName = null)
    {
        IrsdkVariableName = irsdkVariableName;
    }
}
