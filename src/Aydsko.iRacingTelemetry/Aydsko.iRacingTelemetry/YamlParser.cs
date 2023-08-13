using Aydsko.iRacingTelemetry.Yaml;

namespace Aydsko.iRacingTelemetry;

public static class YamlParser
{
    public static IEnumerable<Token> Tokenize(ReadOnlyMemory<char> input)
    {
        var results = new List<Token>();
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
                        results.Add(CreateToken(start, end - start, current));
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
                    results.Add(CreateToken(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.KeySeparator;
                    keySeparatorSeen = true;
                    continue;

                case '\'' when current == YamlTokenType.QuotedString:
                case '\"' when current == YamlTokenType.QuotedString:
                    results.Add(CreateToken(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.EndQuote;
                    continue;

                case '\'':
                case '\"':
                    results.Add(CreateToken(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.StartQuote;
                    continue;

                case ' ' when current is YamlTokenType.Whitespace or YamlTokenType.QuotedString or YamlTokenType.Value:
                    end++;
                    continue;

                case ' ' when current is YamlTokenType.NewLine or YamlTokenType.ListBullet:
                    results.Add(CreateToken(start, end - start, current));
                    start = end;
                    end++;
                    keySeparatorSeen = current == YamlTokenType.ListBullet;
                    current = YamlTokenType.Whitespace;
                    continue;

                case ' ':
                    if (current is not YamlTokenType.Whitespace and not YamlTokenType.Unknown)
                    {
                        results.Add(CreateToken(start, end - start, current));
                        start = end;
                    }
                    current = YamlTokenType.Whitespace;
                    end++;
                    continue;

                case '-' when keySeparatorSeen == false && (current == YamlTokenType.Whitespace || current == YamlTokenType.NewLine):
                    results.Add(CreateToken(start, end - start, current));
                    start = end;
                    end++;
                    current = YamlTokenType.ListBullet;
                    continue;

                default:
                    if (current == YamlTokenType.StartQuote)
                    {
                        results.Add(CreateToken(start, end - start, current));
                        start = end;
                        current = YamlTokenType.QuotedString;
                    }
                    else if (current == YamlTokenType.EndQuote)
                    {
                        results.Add(CreateToken(start, end - start, current));
                        start = end;
                        current = YamlTokenType.Unknown;
                    }
                    else if (current == YamlTokenType.Unknown)
                    {
                        current = YamlTokenType.Key;
                    }
                    else if (current == YamlTokenType.KeySeparator)
                    {
                        results.Add(CreateToken(start, end - start, current));
                        start = end;
                        current = YamlTokenType.Value;
                    }
                    else if (current is YamlTokenType.Whitespace or YamlTokenType.NewLine)
                    {
                        results.Add(CreateToken(start, end - start, current));
                        start = end;
                        current = keySeparatorSeen ? YamlTokenType.Value : YamlTokenType.Key;
                    }

                    end++;
                    continue;
            }
        }

        if (results.Last() is Token lastToken && lastToken.Start + lastToken.Length != input.Length)
        {
            results.Add(CreateToken(start, end - start, current));
        }

        return results;
    }

    private static Token CreateToken(int start, int length, YamlTokenType tokenType)
    {
        return tokenType switch
        {
            YamlTokenType.Key => new KeyToken(start, length),
            YamlTokenType.KeySeparator => new KeySeparatorToken(start, length),
            YamlTokenType.Whitespace => new WhitespaceToken(start, length),
            YamlTokenType.Value => new ValueToken(start, length),
            YamlTokenType.StartQuote => new StartQuoteToken(start, length),
            YamlTokenType.QuotedString => new StringToken(start, length),
            YamlTokenType.EndQuote => new EndQuoteToken(start, length),
            YamlTokenType.NewLine => new NewLineToken(start, length),
            YamlTokenType.ListBullet => new ListBulletToken(start, length),
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), tokenType, "Invalid or unknown token type."),
        };
    }
}
