using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

[StructLayout(LayoutKind.Sequential)]
public struct VariableHeader
{
    /// <summary>Type of data in this variable.</summary>
    /// <seealso cref="VariableType" />
    public int Type;
    
    /// <summary>Offset from start of the buffer row.</summary>
    public int Offset;

    /// <summary>Number of entries (array).</summary>
    /// <remarks>Length in bytes would be <see cref="IrSdkConstants.TypeBytes"/> indexed by the <see cref="Type"/> value multiplied by this field.</remarks>
    public int Count;

    [MarshalAs(UnmanagedType.I1)]
    public bool CountAsTime;

    /// <summary>No value, 16 byte alignment.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public char[] Padding;

    /// <summary>Name of the variable.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxString)]
    public char[] Name;

    /// <summary>A description for the variable.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxDesc)]
    public char[] Description;

    /// <summary>Units for the variable.</summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = IrSdkConstants.MaxString)]
    public char[] Unit;
}
