namespace Aydsko.iRacingTelemetry;

[Flags]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "Value \"Disconnected\" does mean \"None\".")]
public enum HeaderStatus
{
    Disconnected = 0,
    Connected = 1,
}
