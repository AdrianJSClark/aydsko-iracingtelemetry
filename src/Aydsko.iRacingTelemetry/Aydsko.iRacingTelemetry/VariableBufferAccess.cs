using System.Globalization;
using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;
public static class VariableBufferAccess
{
    public static object GetValue(Span<byte> valueBuffer, VariableHeader header)
    {
        var type = (VariableType)header.Type;
        return (type, header.Count, new string(header.Unit)) switch
        {
            (VariableType.Char, 1, _) => MemoryMarshal.Cast<byte, char>(valueBuffer.Slice(header.Offset, header.Count * 1))[0],
            (VariableType.Bool, 1, _) => MemoryMarshal.Cast<byte, bool>(valueBuffer.Slice(header.Offset, header.Count * 1))[0],
            (VariableType.Int, 1, "irsdk_TrkSurf") => MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, "irsdk_TrkLoc") => (TrackLocation)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, "irsdk_SessionState") => (SessionState)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, "irsdk_PitSvStatus") => (PitServiceStatus)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, "irsdk_CameraState") => (CameraState)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, "irsdk_PaceMode") => (PaceMode)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Int, 1, _) => MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.BitField, 1, "irsdk_Flags") => (GlobalState)MemoryMarshal.Cast<byte, uint>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.BitField, 1, "irsdk_PitSvFlags") => (PitService)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.BitField, 1, "irsdk_EngineWarnings") => (EngineWarnings)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.BitField, 1, _) => MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Float, 1, _) => MemoryMarshal.Cast<byte, float>(valueBuffer.Slice(header.Offset, header.Count * 4))[0],
            (VariableType.Double, 1, _) => MemoryMarshal.Cast<byte, double>(valueBuffer.Slice(header.Offset, header.Count * 8))[0],
            (VariableType.Char, > 1, _) => MemoryMarshal.Cast<byte, char>(valueBuffer.Slice(header.Offset, header.Count * 1)).ToArray(),
            (VariableType.Bool, > 1, _) => MemoryMarshal.Cast<byte, bool>(valueBuffer.Slice(header.Offset, header.Count * 1)).ToArray(),
            (VariableType.Int, > 1, _) => MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4)).ToArray(),
            (VariableType.BitField, > 1, _) => MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(header.Offset, header.Count * 4)).ToArray(),
            (VariableType.Float, > 1, _) => MemoryMarshal.Cast<byte, float>(valueBuffer.Slice(header.Offset, header.Count * 4)).ToArray(),
            (VariableType.Double, > 1, _) => MemoryMarshal.Cast<byte, double>(valueBuffer.Slice(header.Offset, header.Count * 8)).ToArray(),
            _ => throw new InvalidDataException("Unexpected header variable type value: " + header.Type),
        };
    }

    /// <summary>
    /// Read a single variable value from the buffer and convert to target data type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer"></param>
    /// <param name="header"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static T GetTypedValue<T>(Span<byte> buffer, VariableHeader header)
    {
        var value = GetValue(buffer, header);
        return SafeConvertType<T>(value);
    }

    public static Type GetDataType(VariableType variableType) => variableType switch
    {
        VariableType.Char => typeof(char),
        VariableType.Bool => typeof(bool),
        VariableType.Int => typeof(int),
        VariableType.BitField => typeof(int),
        VariableType.Float => typeof(float),
        VariableType.Double => typeof(double),
        _ => typeof(object),
    };

    public static T SafeConvertType<T>(object value)
    {
        var targetType = typeof(T);
        
        // get underlying type in case T is nullable
        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        
        // special handling for enums because Convert.ChangeType throws on trying to convert to enum type
        // but int type and cast to enum works
        if (targetType.IsEnum)
        {
            return (T)Enum.ToObject(targetType, Convert.ToInt32(value, CultureInfo.InvariantCulture));
        }
        
        // special handling for TimeSpans -> iracing gives times in seconds
        if (targetType.Equals(typeof(TimeSpan)))
        {
            return (T)(object)TimeSpan.FromSeconds(Convert.ToInt64(value, CultureInfo.InvariantCulture));
        }
        
        return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}
