namespace Aydsko.iRacingTelemetry;

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
