namespace Aydsko.iRacingTelemetry.UnitTests;

public sealed class VariableBufferAccessTests
{
    private DiskClient _diskClient = default!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var testFilePath = Path.Combine(Environment.CurrentDirectory, "mercedesw12_silverstone 2019 gp 2023-08-02 21-44-30.ibt");
        _diskClient = await DiskClient.OpenFileAsync(testFilePath).ConfigureAwait(false);
    }

    [Test]
    [TestCase(1.0, (int) 1)]
    [TestCase(1, (float)1.0)]
    [TestCase("1", (float)1.0)]
    [TestCase(1, (TrackLocation)1)]
    public void SafeConvertTypeShouldReturnExpectedValue<T>(object value, T expected)
    {
        var typedValue = VariableBufferAccess.SafeConvertType<T>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(42, "00:00:42")]
    [TestCase("42", "00:00:42")]
    public void SafeConvertTypeShouldConvertToTimeSpan(object value, TimeSpan expected)
    {
        var typedValue = VariableBufferAccess.SafeConvertType<TimeSpan>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(1.0, (int)1)]
    [TestCase(1, (float)1.0)]
    [TestCase("1", (float)1.0)]
    [TestCase(1, (TrackLocation)1)]
    public void SafeConvertTypeShouldReturnNullableValue<T>(object value, T expected) where T : struct
    {
        var typedValue = VariableBufferAccess.SafeConvertType<T?>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("SessionTime", 676.3333330787348)]
    [TestCase("SessionState", SessionState.StateRacing)]
    public async Task GetTypedValueShouldReturnValueFromBuffer<T>(string varHeaderName, T expected)
    {
        var header = _diskClient.GetHeaderVariables().First(x => new string(x.Name) == varHeaderName);
        var buffer = await _diskClient.ReadBufferLinesAsync().FirstAsync().ConfigureAwait(false);
        var value = VariableBufferAccess.GetTypedValue<T>(buffer.Span, header);
        Assert.That(value, Is.EqualTo(expected));
    }
}
