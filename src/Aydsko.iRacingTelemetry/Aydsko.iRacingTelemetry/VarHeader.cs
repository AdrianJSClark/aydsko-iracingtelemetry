using System.Runtime.InteropServices;

internal static class IrSdkConstants
{
    public const int SdkVersion = 2;
    public const int MaxString = 32;
    public const int MaxDesc = 64;
    public const int MaximumBuffers = 4;

    public static readonly int[] TypeBytes = new[]
    {
        1, // Char
        1, // Bool
        4, // Int
        4, // BitField
        4, // Float
        8 // Double
    };
}

[StructLayout(LayoutKind.Sequential)]
internal struct irsdk_header
{
    public int ver; // this api header version, see IRSDK_VER
    public int status; // bitfield using irsdk_StatusField
    public int tickRate; // ticks per second (60 or 360 etc)

    // session information, updated periodicaly
    public int sessionInfoUpdate;  // Incremented when session info changes
    public int sessionInfoLen;     // Length in bytes of session info string
    public int sessionInfoOffset;  // Session info, encoded in YAML format

    // State data, output at tickRate

    public int numVars;            // length of arra pointed to by varHeaderOffset
    public int varHeaderOffset;    // offset to irsdk_varHeader[numVars] array, Describes the variables received in varBuf

    public int numBuf;             // <= IRSDK_MAX_BUFS (3 for now)
    public int bufLen;             // length in bytes for one line
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] pad1;             // (16 byte align)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaximumBuffers)]
    public irsdk_varBuf[] varBuf;  // buffers of data being written to
}

[StructLayout(LayoutKind.Sequential)]
internal struct irsdk_varBuf
{ 
    public int tickCount; // used to detect changes in data
    public int bufOffset; // offset from header
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public int[] pad; // (16 byte align)
}

[StructLayout(LayoutKind.Sequential)]
internal struct irsdk_varHeader
{
    public int type; // irsdk_VarType
    public int offset; // offset from start of buffer row
    public int count; // number of entries (array), so length in bytes would be "irdskVarTypeBytes[type] * count"

    [MarshalAs(UnmanagedType.I1)]
    public bool countAsTime;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public char[] pad;             // (16 byte align)
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxString)]
    public char[] name;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxDesc)]
    public char[] desc;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxString)]
    public char[] unit;
}

[StructLayout(LayoutKind.Sequential)]
internal struct irsdk_diskSubHeader
{
    public long sessionStartDate;
    public double sessionStartTime;
    public double sessionEndTime;
    public int sessionLapCount;
    public int sessionRecordCount;
}
