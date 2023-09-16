using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

[StructLayout(LayoutKind.Sequential)]
internal struct VariableBuffer
{ 
    /// <summary>Used to detect changes in the data.</summary>
    public int TickCount;
    /// <summary>Buffer offset from the header.</summary>
    public int BufferOffset;
    /// <summary>No value, 16 byte alignment.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] Padding; // (16 byte align)
}
