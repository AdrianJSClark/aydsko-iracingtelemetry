namespace Aydsko.iRacingTelemetry;

public record Variable(string? Name, string? Description, string? Unit);

public record Variable<T>(string? Name, string? Description, string? Unit, T Value) : Variable(Name, Description, Unit);
