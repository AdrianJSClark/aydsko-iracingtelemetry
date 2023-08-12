using System.Security;

namespace Aydsko.iRacingTelemetry;

public static class YamlParser
{
    public static IReadOnlyDictionary<string, string?> Parse(string yaml)
    {
        var deserializer = new YamlDotNet.Serialization.Deserializer();
        var result = deserializer.Deserialize<Dictionary<string, string?>>(yaml);
        return result;
    }

    public static IEnumerable<YamlToken> Tokenize(ReadOnlyMemory<char> input)
    {
        var results = new List<YamlToken>();
        var start = 0;
        var end = 0;
        var current = YamlTokenType.Unknown;

        foreach (var c in input.Span)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                    if (current != YamlTokenType.NewLine && current != YamlTokenType.Unknown)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                    }
                    current = YamlTokenType.NewLine;
                    end++;
                    continue;

                case ':' when (current == YamlTokenType.QuotedString):
                    end++;
                    continue;

                case ':':
                    results.Add(new(start, end - start, YamlTokenType.Literal));
                    start = end;
                    end++;
                    results.Add(new(start, end - start, YamlTokenType.KeySeparator));
                    start = end;
                    current = YamlTokenType.Unknown;
                    continue;

                case '\'' when (current == YamlTokenType.QuotedString || current == YamlTokenType.Whitespace):
                case '\"' when (current == YamlTokenType.QuotedString || current == YamlTokenType.Whitespace):
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    results.Add(new(start, end - start, YamlTokenType.Quote));
                    start = end;
                    current = current == YamlTokenType.QuotedString ? YamlTokenType.Unknown : YamlTokenType.QuotedString;
                    continue;

                case '\'':
                case '\"':
                    start = end;
                    end++;
                    results.Add(new(start, end - start, YamlTokenType.Quote));
                    current = YamlTokenType.QuotedString;
                    continue;

                case ' ' when (current == YamlTokenType.Whitespace || current == YamlTokenType.QuotedString):
                    end++;
                    continue;

                case ' ' when (current == YamlTokenType.NewLine):
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.Whitespace;
                    continue;

                case ' ':
                    start = end;
                    end++;
                    current = YamlTokenType.Whitespace;
                    continue;

                default:
                    if (current == YamlTokenType.Unknown)
                    {
                        current = YamlTokenType.Literal;
                    }

                    if (current == YamlTokenType.Whitespace || current == YamlTokenType.NewLine)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                        current = YamlTokenType.Literal;
                    }

                    end++;
                    continue;
            }
        }

        if (results.Last() is YamlToken lastToken && lastToken.Start + lastToken.Length != input.Length)
        {
            results.Add(new(start, end - start, current));
        }

        return results;
    }

    public record YamlToken(int Start, int Length, YamlTokenType TokenType);

    public enum YamlTokenType
    {
        Unknown,
        Literal,
        KeySeparator,
        Whitespace,
        Quote,
        QuotedString,
        NewLine,
    }
}
