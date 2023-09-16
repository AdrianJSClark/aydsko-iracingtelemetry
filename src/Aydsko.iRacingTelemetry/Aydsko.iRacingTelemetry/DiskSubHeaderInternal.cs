using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

[StructLayout(LayoutKind.Sequential)]
internal struct DiskSubHeaderInternal
{
    public long SessionStartDate;
    public double SessionStartTime;
    public double SessionEndTime;
    public int SessionLapCount;
    public int SessionRecordCount;
}
