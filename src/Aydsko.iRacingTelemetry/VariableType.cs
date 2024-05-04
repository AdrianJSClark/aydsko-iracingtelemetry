namespace Aydsko.iRacingTelemetry;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Since these values represent these types it is most appropriate to use their names.")]
public enum VariableType
{
    Char = 0,
    Bool = 1,
    Int = 2,
    BitField = 3,
    Float = 4,
    Double = 5,
}
