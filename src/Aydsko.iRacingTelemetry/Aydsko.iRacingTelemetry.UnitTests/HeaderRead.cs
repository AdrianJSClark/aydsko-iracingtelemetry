namespace Aydsko.iRacingTelemetry.UnitTests;

public class HeaderRead
{
    private DiskClient _diskClient = default!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var testFilePath = Path.Combine(Environment.CurrentDirectory, "mercedesw12_silverstone 2019 gp 2023-08-02 21-44-30.ibt");
        _diskClient = await DiskClient.OpenFileAsync(testFilePath).ConfigureAwait(false);
    }

    [Test]
    public async Task SessionRecordsAreAllParsed()
    {
        var sessionRecords = await _diskClient.ReadVariableLinesAsync().ToArrayAsync().ConfigureAwait(false);
        Assert.That(sessionRecords, Has.Length.EqualTo(_diskClient.DiskSubHeader.SessionRecordCount));
    }

    [Test]
    public Task SessionInfoYamlExists()
    {
        Assert.That(_diskClient.SessionInfoYaml, Is.Not.Null.Or.Empty);
        return Verify(_diskClient.SessionInfoYaml);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _diskClient?.Dispose();
    }
}
