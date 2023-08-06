namespace Aydsko.iRacingTelemetry.UnitTests;

public class HeaderRead
{
    [Test]
    public async Task Test()
    {
        var testFilePath = Path.Combine(Environment.CurrentDirectory, "mercedesw12_silverstone 2019 gp 2023-08-02 21-44-30.ibt");
        var testFileExists = File.Exists(testFilePath);
        Assert.That(testFileExists, Is.True, "Test file does not exist.");

        using var diskClient = await DiskClient.OpenFileAsync(testFilePath).ConfigureAwait(false);

        Assert.That(diskClient.SessionInfoYaml, Is.Not.Null.Or.Empty);
    }
}
