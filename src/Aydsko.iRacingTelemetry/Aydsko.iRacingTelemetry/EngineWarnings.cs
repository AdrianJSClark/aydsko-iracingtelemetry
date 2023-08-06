namespace Aydsko.iRacingTelemetry;

[Flags]
public enum EngineWarnings
{
    None = 0,
    WaterTempWarning = 0x01,
    FuelPressureWarning = 0x02,
    OilPressureWarning = 0x04,
    EngineStalled = 0x08,
    PitSpeedLimiter = 0x10,
    RevLimiterActive = 0x20,
    OilTempWarning = 0x40,
}
