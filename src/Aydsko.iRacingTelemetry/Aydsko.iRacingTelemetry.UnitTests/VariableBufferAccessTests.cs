namespace Aydsko.iRacingTelemetry.UnitTests;
public sealed class VariableBufferAccessTests
{
    [Test]
    [TestCase(1.0, (int) 1)]
    [TestCase(1, (float)1.0)]
    [TestCase("1", (float)1.0)]
    [TestCase(1, (TrackLocation)1)]
    public void SafeConvertTypeShouldReturnExpectedValue<T>(object value, T expected)
    {
        T typedValue = VariableBufferAccess.SafeConvertType<T>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(42, "00:00:42")]
    [TestCase("42", "00:00:42")]
    public void SafeConvertTypeShouldConvertToTimeSpan(object value, TimeSpan expected)
    {
        TimeSpan typedValue = VariableBufferAccess.SafeConvertType<TimeSpan>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(1.0, (int)1)]
    [TestCase(1, (float)1.0)]
    [TestCase("1", (float)1.0)]
    [TestCase(1, (TrackLocation)1)]
    public void SafeConvertTypeShouldReturnNullabeValue<T>(object value, T expected) where T : struct
    {
        T? typedValue = VariableBufferAccess.SafeConvertType<T?>(value);
        Assert.That(typedValue, Is.EqualTo(expected));
    }
}
