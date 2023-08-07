namespace Aydsko.iRacingTelemetry.UnitTests;

public class CustomParser
{

    [Test]
    public void CustomParser_HelloWorld()
    {
        var input = "hello: world".AsMemory();
        var output = YamlParser.Parse(input);

        Assert.That(output, Is.Not.Null.Or.Empty);
        Assert.That(output, Has.Count.EqualTo(1));
        Assert.That(output, Contains.Key("hello").WithValue("world"));
    }

    [Test]
    public void CustomParser_TestHelloWorld()
    {
        var input = @"
test:
 hello: 'world'".AsMemory();
        var output = YamlParser.Parse(input);

        Assert.That(output, Is.Not.Null.Or.Empty);
        Assert.That(output, Has.Count.EqualTo(1));
        Assert.That(output, Contains.Key("test:hello").WithValue("world"));
    }

    [Test]
    public void CustomParser_TestHelloWorldWithHeaderAndFooter()
    {
        var input = @"---
test:
 hello: 'world'

...
".AsMemory();
        var output = YamlParser.Parse(input);

        Assert.That(output, Is.Not.Null.Or.Empty);
        Assert.That(output, Has.Count.EqualTo(1));
        Assert.That(output, Contains.Key("test:hello").WithValue("world"));
    }
}
