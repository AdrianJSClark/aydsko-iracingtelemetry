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
        var keySeparatorSeen = false;

        foreach (var c in input.Span)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                    if (current is not YamlTokenType.NewLine and not YamlTokenType.Unknown)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                    }
                    current = YamlTokenType.NewLine;
                    end++;
                    keySeparatorSeen = false;
                    continue;

                case ':' when current == YamlTokenType.QuotedString:
                    end++;
                    continue;

                case ':':
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.KeySeparator;
                    keySeparatorSeen = true;
                    continue;

                case '\'' when current == YamlTokenType.QuotedString:
                case '\"' when current == YamlTokenType.QuotedString:
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.EndQuote;
                    continue;

                case '\'':
                case '\"':
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.StartQuote;
                    continue;

                case ' ' when current is YamlTokenType.Whitespace or YamlTokenType.QuotedString or YamlTokenType.Value:
                    end++;
                    continue;

                case ' ' when current is YamlTokenType.NewLine or YamlTokenType.ListBullet:
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    keySeparatorSeen = current == YamlTokenType.ListBullet;
                    current = YamlTokenType.Whitespace;
                    continue;

                case ' ':
                    if (current is not YamlTokenType.Whitespace and not YamlTokenType.Unknown)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                    }
                    current = YamlTokenType.Whitespace;
                    end++;
                    continue;

                case '-' when keySeparatorSeen == false && (current == YamlTokenType.Whitespace || current == YamlTokenType.NewLine):
                    results.Add(new(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.ListBullet;
                    continue;

                default:
                    if (current == YamlTokenType.StartQuote)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                        current = YamlTokenType.QuotedString;
                    }
                    else if (current == YamlTokenType.EndQuote)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                        current = YamlTokenType.Unknown;
                    }
                    else if (current == YamlTokenType.Unknown)
                    {
                        current = YamlTokenType.Key;
                    }
                    else if (current == YamlTokenType.KeySeparator)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                        current = YamlTokenType.Value;
                    }
                    else if (current is YamlTokenType.Whitespace or YamlTokenType.NewLine)
                    {
                        results.Add(new(start, end - start, current));
                        start = end;
                        current = keySeparatorSeen ? YamlTokenType.Value : YamlTokenType.Key;
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
        Key,
        KeySeparator,
        Whitespace,
        Value,
        StartQuote,
        QuotedString,
        EndQuote,
        NewLine,
        ListBullet,
    }
}
