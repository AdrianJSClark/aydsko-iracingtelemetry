namespace Aydsko.iRacingTelemetry.Yaml;

public record Token(int Start, int Length);
public record KeyToken(int Start, int Length) : Token(Start, Length);
public record KeySeparatorToken(int Start, int Length) : Token(Start, Length);
public record WhitespaceToken(int Start, int Length) : Token(Start, Length);
public record ValueToken(int Start, int Length) : Token(Start, Length);
public record StartQuoteToken(int Start, int Length) : Token(Start, Length);
public record StringToken(int Start, int Length): Token(Start, Length);
public record EndQuoteToken(int Start, int Length): Token(Start, Length);
public record NewLineToken(int Start, int Length): Token(Start, Length);
public record ListBulletToken(int Start, int Length): Token(Start, Length);

internal enum YamlTokenType
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
