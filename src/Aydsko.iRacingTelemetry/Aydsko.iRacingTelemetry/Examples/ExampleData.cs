using Aydsko.iRacingTelemetry.Generators.Mapper;

namespace Aydsko.iRacingTelemetry.Examples;

/// <summary>
/// Example class to show how the user can define the data he wants to be mapped from the iracing sdk
/// </summary>
[AydskoData]
public sealed class ExampleData
{
    // Auto convert time to TimeSpan
    [AydskoMap]
    public TimeSpan SessionTime { get; set; }

    // use different number types (as long as they can be converted)
    [AydskoMap]
    public float Throttle { get; set; }
    [AydskoMap]
    public double Brake { get; set; }

    // Specify the iracing variable name explicitly when property has different name
    [AydskoMap("Clutch")]
    public double ClutchPedal { get; set; }
}
