using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

internal static class IrSdkConstants
{
    public const int SdkVersion = 2;
    public const int MaxString = 32;
    public const int MaxDesc = 64;
    public const int MaximumBuffers = 3;

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

internal enum VarType : int
{
    Char = 0,
    Bool,
    Int,
    BitField,
    Float,
    Double,
    ETCount // Index, don't use
}

internal enum StatusField: int
{
    Connected = 1
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe struct VarHeader
{
    public int Type;
    public int Offset;
    public int Count;
    public bool CountAsTime;
    public fixed char Pad[3]; // (16 byte align)
    public fixed char Name[IrSdkConstants.MaxString];
    public fixed char Description[IrSdkConstants.MaxString];
    public fixed char Unit[IrSdkConstants.MaxString];
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe struct VariableBuffer
{
    public int TickCount; // Used to detect changes in data
    public int BufOffset; // Offset from header
    public fixed int Pad[2]; // (16 byte align)
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe struct Header
{
    public int Version; // This API header version, see IrSdkConstants.SdkVersion
    public StatusField Status;
    public int TickRate; // Ticks per second (60 or 360 etc)

    // Session information, updated periodically
    public int SessionInformationUpdate; // Incremented when the session information changes.
    public int SessionInformationLength; // Length in bytes of session information string.
    public int SessionInformationOffset; // Session information, encoded in YAML format.

    // State data, output at TickRate.
    public int NumberOfVariables; // Length of array pointed to by the HeaderOffset.
    public int HeaderOffset; // Offset to VarHeader[NumberOfVariables] array, describes the variables received in VariableBuffer.
    public int NumberOfBuffers; // <= IrSdkConstants.MaximumBuffers
    public int BufferLength; // Length in bytes for one line.
    public fixed int Pad[2]; // (16 byte align)
    public VariableBuffer[] VariableBuffers;
}

