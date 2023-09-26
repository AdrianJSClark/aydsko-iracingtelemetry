using Aysdko.iRacingTelemetry.Generators.Mapper;

namespace Aydsko.iRacingTelemetry.Examples;

/// <summary>
/// Example class to show how the user can define the data he wants to be mapped from the iracing sdk
/// </summary>
[AysdkoData]
public sealed class ExampleData
{
    // Auto convert time to TimeSpan
    [AysdkoMap]
    public TimeSpan SessionTime { get; set; }

    // use different number types (as long as they can be converted)
    [AysdkoMap]
    public float Throttle { get; set; }
    [AysdkoMap]
    public double Brake { get; set; }

    // Specify the iracing variable name explicitly when property has different name
    [AysdkoMap("Clutch")]
    public double ClutchPedal { get; set; }
}
