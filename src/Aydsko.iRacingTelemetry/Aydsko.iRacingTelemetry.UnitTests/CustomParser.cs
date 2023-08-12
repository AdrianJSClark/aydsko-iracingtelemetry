namespace Aydsko.iRacingTelemetry.UnitTests;

public class CustomParser
{

    [Test, TestCaseSource(nameof(TokenizeTestCases))]
    public YamlParser.YamlToken[] TokenizeTest(string input)
    {
        return YamlParser.Tokenize(input.AsMemory()).ToArray();
    }

    private static IEnumerable<TestCaseData> TokenizeTestCases()
    {
        yield return new("hello:world")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Literal),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 5, YamlParser.YamlTokenType.Literal),
            }
        };
        yield return new("hello: world")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Literal),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 1, YamlParser.YamlTokenType.Whitespace),
                new(7, 5, YamlParser.YamlTokenType.Literal),
            }
        };
        yield return new("hello: 'world'")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Literal),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 1, YamlParser.YamlTokenType.Whitespace),
                new(7, 1, YamlParser.YamlTokenType.Quote),
                new(8, 5, YamlParser.YamlTokenType.QuotedString),
                new(13, 1, YamlParser.YamlTokenType.Quote),
            }
        };
        yield return new("hello: \"world\"")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Literal),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 1, YamlParser.YamlTokenType.Whitespace),
                new(7, 1, YamlParser.YamlTokenType.Quote),
                new(8, 5, YamlParser.YamlTokenType.QuotedString),
                new(13, 1, YamlParser.YamlTokenType.Quote),
            }
        };
        yield return new("""
                         container:
                          key1: value1
                          key2: value2
                         """)
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 9, YamlParser.YamlTokenType.Literal),
                new(9, 1, YamlParser.YamlTokenType.KeySeparator),
                new(10, 2, YamlParser.YamlTokenType.NewLine),
                new(12, 1, YamlParser.YamlTokenType.Whitespace),
                new(13, 4, YamlParser.YamlTokenType.Literal),
                new(17,1, YamlParser.YamlTokenType.KeySeparator),
                new(18, 1, YamlParser.YamlTokenType.Whitespace),
                new(19,6, YamlParser.YamlTokenType.Literal),
                new(25, 2, YamlParser.YamlTokenType.NewLine),
                new(27, 1, YamlParser.YamlTokenType.Whitespace),
                new(28, 4, YamlParser.YamlTokenType.Literal),
                new(32,1, YamlParser.YamlTokenType.KeySeparator),
                new(33, 1, YamlParser.YamlTokenType.Whitespace),
                new(34,6, YamlParser.YamlTokenType.Literal),
            }
        };
        yield return new("""
                         container:
                          key1: 'value1'
                          key2: "value2"
                         """)
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 9, YamlParser.YamlTokenType.Literal),
                new(9, 1, YamlParser.YamlTokenType.KeySeparator),
                new(10, 2, YamlParser.YamlTokenType.NewLine),
                new(12, 1, YamlParser.YamlTokenType.Whitespace),
                new(13, 4, YamlParser.YamlTokenType.Literal),
                new(17,1, YamlParser.YamlTokenType.KeySeparator),
                new(18, 1, YamlParser.YamlTokenType.Whitespace),
                new(19, 1, YamlParser.YamlTokenType.Quote),
                new(20, 6, YamlParser.YamlTokenType.QuotedString),
                new(26, 1, YamlParser.YamlTokenType.Quote),
                new(27, 2, YamlParser.YamlTokenType.NewLine),
                new(29, 1, YamlParser.YamlTokenType.Whitespace),
                new(30, 4, YamlParser.YamlTokenType.Literal),
                new(34,1, YamlParser.YamlTokenType.KeySeparator),
                new(35, 1, YamlParser.YamlTokenType.Whitespace),
                new(36, 1, YamlParser.YamlTokenType.Quote),
                new(37, 6, YamlParser.YamlTokenType.QuotedString),
                new(43, 1, YamlParser.YamlTokenType.Quote),
            }
        };
    }
}
