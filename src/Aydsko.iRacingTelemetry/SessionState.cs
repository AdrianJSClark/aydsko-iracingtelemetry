namespace Aydsko.iRacingTelemetry;

#pragma warning disable CA1724 // Type names should not conflict
public enum SessionState
{
    StateInvalid,
    StateGetInCar,
    StateWarmup,
    StateParadeLaps,
    StateRacing,
    StateCheckered,
    StateCoolDown
}
#pragma warning restore CA1724
