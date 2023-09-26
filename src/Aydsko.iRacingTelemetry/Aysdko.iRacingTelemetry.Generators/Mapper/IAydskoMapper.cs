namespace Aydsko.iRacingTelemetry.Generators.Mapper;

public interface IAydskoMapper<T> where T : class
{
    T MapFromBuffer(Span<byte> buffer);
}
