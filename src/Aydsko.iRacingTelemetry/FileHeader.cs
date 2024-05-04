using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

[StructLayout(LayoutKind.Sequential)]
internal struct FileHeader
{
    /// <summary>API header version, see <see cref="IrSdkConstants.SdkVersion"/>.</summary>
    public int ApiVersion;
    /// <summary>Current status as a bit field, see <see cref="HeaderStatus"/>.</summary>
    public int Status;
    /// <summary>Ticks per second (60 or 360 etc)</summary>
    public int TickRate;

    // Session information, updated periodically.

    /// <summary>Incremented when session information changes.</summary>
    public int SessionInfoUpdate;
    /// <summary>Length in bytes of session info string.</summary>
    public int SessionInfoLen;
    /// <summary>Start of session information string, encoded in YAML format.</summary>
    public int SessionInfoOffset;

    // State data, output at tickRate

    /// <summary>Length of array pointed to by <see cref="VariableHeaderOffset"/>.</summary>
    public int NumberOfVariables;
    /// <summary>
    /// Offset to an array of <see cref="VariableHeader"/> objects with length indicated by <see cref="NumberOfVariables"/>.
    /// The headers describe the variables received in <see cref="VariableBuffers"/>.
    /// </summary>
    public int VariableHeaderOffset;

    /// <summary>Number of variable buffers.</summary>
    /// <remarks>Should be smaller than or equal to <see cref="IrSdkConstants.MaximumBuffers"/>.</remarks>
    public int NumberOfBuffers;
    /// <summary>Length in bytes for one line.</summary>
    public int BufferLengthBytes;
    /// <summary>No value, 16 byte alignment.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] Padding;
    /// <summary>
    /// Array of buffers containing the values of the variables described in the <see cref="VariableHeader" /> values.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaximumBuffers)]
    public VariableBuffer[] VariableBuffers;
}
