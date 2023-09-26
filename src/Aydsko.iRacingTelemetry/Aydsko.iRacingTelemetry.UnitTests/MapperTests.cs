using Aysdko.iRacingTelemetry.Generators.Mapper;

namespace Aydsko.iRacingTelemetry.UnitTests;
public sealed class MapperTests
{
    private DiskClient _diskClient = default!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var testFilePath = Path.Combine(Environment.CurrentDirectory, "mercedesw12_silverstone 2019 gp 2023-08-02 21-44-30.ibt");
        _diskClient = await DiskClient.OpenFileAsync(testFilePath).ConfigureAwait(false);
    }

    [Test]
    public async Task MapperShouldFillTestClass()
    {
        var mapper = new MapperTestClass_AysdkoMapper(_diskClient.GetHeaderVariables());
        var buffer = await _diskClient.ReadBufferLinesAsync().FirstAsync().ConfigureAwait(false);
        var data = mapper.MapFromBuffer(buffer.Span);
        Assert.Multiple(() =>
        {
            Assert.That(data.SessionTime, Is.EqualTo(TimeSpan.FromSeconds(676.3333330787348)));
            Assert.That(data.SessionState, Is.EqualTo(SessionState.StateRacing));
        });
    }
}

[AysdkoData(autoMapAllProperties: true)]
public class MapperTestClass
{
    public TimeSpan SessionTime { get; set; }
    public SessionState SessionState { get; set; }
}
