namespace Aydsko.iRacingTelemetry;

public record DiskSubHeader(DateTimeOffset SessionStartInstant, double SessionStart, double SessionEnd, int SessionLapCount, int SessionRecordCount);
