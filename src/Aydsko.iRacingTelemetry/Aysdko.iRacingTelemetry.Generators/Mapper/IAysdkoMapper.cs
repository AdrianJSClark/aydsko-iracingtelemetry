namespace Aysdko.iRacingTelemetry.Generators.Mapper;
public interface IAysdkoMapper<T> where T : class
{
    T MapFromBuffer(Span<byte> buffer);
}
