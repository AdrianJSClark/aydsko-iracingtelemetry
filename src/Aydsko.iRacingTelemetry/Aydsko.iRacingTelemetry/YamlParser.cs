using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry;

internal static class YamlParser
{
    public static IReadOnlyDictionary<string, string?> Parse(string yaml)
    {
        var deserializer = new YamlDotNet.Serialization.Deserializer();
        var result = deserializer.Deserialize<Dictionary<string, string?>>(yaml);
        return result;
    }

    public static IReadOnlyDictionary<string, string?> Parse(ReadOnlyMemory<char> yamlChars)
    {
        var result = new Dictionary<string, string>();
        var keyStartIndex = 0;
        var valueStartIndex = -1;

        string? key = null;

        var index = -1;
        foreach (var yamlChar in yamlChars.Span)
        {
            index++;
            switch (yamlChar)
            {
                case ':':
                    key = new string(yamlChars.Slice(keyStartIndex, index - keyStartIndex).Span);
                    keyStartIndex = -1;
                    valueStartIndex = index + 1;
                    break;
                case '\r' or '\n':
                    result.Add(key, new string(yamlChars.Slice(valueStartIndex, index - valueStartIndex).Span));
                    valueStartIndex = -1;
                    key = null;
                    break;
            }
        }

        if (valueStartIndex != -1)
        {
            result.Add(key, new string(yamlChars.Slice(valueStartIndex, index - valueStartIndex).Span));
            valueStartIndex = -1;
            key = null;
        }

        return result;
    }

    private enum YamlTokenType
    {
        Unknown,
        Key,
        KeySeparator,
        Value,
        Whitespace,
    }

    private enum ParseState
    {
        None,
        Space,
        Key,
        KeySep,
        Value,
        NewLine
    }
}
