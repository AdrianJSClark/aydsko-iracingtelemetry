namespace Aydsko.iRacingTelemetry.Examples;
internal class UseMapperExample
{
    public static ExampleData UseMapper(IEnumerable<VariableHeader> headers, Span<byte> buffer)
    {
        // Create new mapper instance: this needs to be run once after connecting to a new session - at least each time the variable header changes
        // The mapper can then be reused during the whole session
        var mapper = new ExampleData_AysdkoMapper(headers);

        // Create a single data frame from the current buffer
        var data = mapper.MapFromBuffer(buffer);
        return data;
    }
}
