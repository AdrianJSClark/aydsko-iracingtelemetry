namespace Aydsko.iRacingTelemetry;

public enum PitServiceStatus
{
    // Status
    PitSvNone = 0,
    PitSvInProgress,
    PitSvComplete,

    // Errors
    PitSvTooFarLeft = 100,
    PitSvTooFarRight,
    PitSvTooFarForward,
    PitSvTooFarBack,
    PitSvBadAngle,
    PitSvCantFixThat,
}
