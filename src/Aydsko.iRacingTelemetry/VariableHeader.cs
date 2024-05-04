using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

[StructLayout(LayoutKind.Sequential)]
public struct VariableHeader : IEquatable<VariableHeader>
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
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = IrSdkConstants.MaxString)]
    public string Name;

    /// <summary>A description for the variable.</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = IrSdkConstants.MaxDesc)]
    public string Description;

    /// <summary>Units for the variable.</summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = IrSdkConstants.MaxString)]
    public string Unit;

    public override readonly bool Equals(object? obj)
    {
        return obj is VariableHeader header && Equals(header);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Type, Offset, Count, CountAsTime, Name, Description, Unit);
    }

    public static bool operator ==(VariableHeader left, VariableHeader right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VariableHeader left, VariableHeader right)
    {
        return !(left == right);
    }

    public readonly bool Equals(VariableHeader other)
    {
        return Type == other.Type
               && Offset == other.Offset
               && Count == other.Count
               && CountAsTime == other.CountAsTime
               && Name == other.Name
               && Description == other.Description
               && Unit == other.Unit;
    }
}
