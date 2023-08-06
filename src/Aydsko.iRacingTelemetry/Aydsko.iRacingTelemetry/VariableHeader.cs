namespace Aydsko.iRacingTelemetry;

public record Variable(string? Name, string? Description, string? Unit);

#pragma warning disable CA1819 // Properties should not return arrays

public record VariableChar(string? Name, string? Description, string? Unit, char[] Value) : Variable (Name, Description, Unit);
public record VariableBool(string? Name, string? Description, string? Unit, bool[] Value) : Variable(Name, Description, Unit);
public record VariableInt(string? Name, string? Description, string? Unit, int[] Value) : Variable(Name, Description, Unit);
public record VariableBitField(string? Name, string? Description, string? Unit, int[] Value) : Variable(Name, Description, Unit);
public record VariableFloat(string? Name, string? Description, string? Unit, float[] Value) : Variable(Name, Description, Unit);
public record VariableDouble(string? Name, string? Description, string? Unit, double[] Value) : Variable(Name, Description, Unit);

#pragma warning restore CA1819 // Properties should not return arrays
