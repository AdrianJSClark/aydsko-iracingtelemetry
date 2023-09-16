using Aydsko.iRacingTelemetry.Yaml;

namespace Aydsko.iRacingTelemetry.UnitTests;

public class CustomParser
{
    [Test, TestCaseSource(nameof(TokenizeTestCases))]
    public Token[] TokenizeTest(string input)
    {
        return YamlParser.TokenizeStateBased(input.AsMemory()).ToArray();
    }

    private static IEnumerable<TestCaseData> TokenizeTestCases()
    {
        yield return new("hello:world")
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 5),
                new KeySeparatorToken(5, 1),
                new ValueToken(6, 5),
            }
        };

        yield return new("hello: world")
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 5),
                new KeySeparatorToken(5, 1),
                new WhitespaceToken(6, 1),
                new ValueToken(7, 5),
            }
        };

        yield return new("hello: 'world'")
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 5),
                new KeySeparatorToken(5, 1),
                new WhitespaceToken(6, 1),
                new StartQuoteToken(7, 1),
                new StringToken(8, 5),
                new EndQuoteToken(13, 1),
            }
        };

        yield return new("hello: \"world\"")
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 5),
                new KeySeparatorToken(5, 1),
                new WhitespaceToken(6, 1),
                new StartQuoteToken(7, 1),
                new StringToken(8, 5),
                new EndQuoteToken(13, 1),
            }
        };

        yield return new("""
                         container:
                          key1: value1
                          key2: value2
                         """)
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 9),
                new KeySeparatorToken(9, 1),
                new NewLineToken(10, 2),
                new WhitespaceToken(12, 1),
                new KeyToken(13, 4),
                new KeySeparatorToken(17,1),
                new WhitespaceToken(18, 1),
                new ValueToken(19,6),
                new NewLineToken(25, 2),
                new WhitespaceToken(27, 1),
                new KeyToken(28, 4),
                new KeySeparatorToken(32,1),
                new WhitespaceToken(33, 1),
                new ValueToken(34,6),
            }
        };

        yield return new("""
                         container:
                          key1: 'value1'
                          key2: "value2"
                         """)
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 9),
                new KeySeparatorToken(9, 1),
                new NewLineToken(10, 2),
                new WhitespaceToken(12, 1),
                new KeyToken(13, 4),
                new KeySeparatorToken(17,1),
                new WhitespaceToken(18, 1),
                new StartQuoteToken(19, 1),
                new StringToken(20, 6),
                new EndQuoteToken(26, 1),
                new NewLineToken(27, 2),
                new WhitespaceToken(29, 1),
                new KeyToken(30, 4),
                new KeySeparatorToken(34,1),
                new WhitespaceToken(35, 1),
                new StartQuoteToken(36, 1),
                new StringToken(37, 6),
                new EndQuoteToken(43, 1),
            }
        };

        yield return new("""
                         list:
                          - value1
                          - value2
                         """)
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 4),
                new KeySeparatorToken(4, 1),
                new NewLineToken(5, 2),
                new WhitespaceToken(7, 1),
                new ListBulletToken(8, 1),
                new WhitespaceToken(9,1),
                new ValueToken(10,6),
                new NewLineToken(16,2),
                new WhitespaceToken(18,1),
                new ListBulletToken(19, 1),
                new WhitespaceToken(20,1),
                new ValueToken(21,6),
            }
        };

        yield return new("""
                         CarSetup:
                          UpdateCount: 8
                          TiresAero:
                           TireCompound:
                            TireCompound: Soft
                           LeftFrontTire:
                            StartingPressure: 151.7 kPa
                            LastHotPressure: 22.8 psi
                            LastTempsOMI: 115C, 121C, 127C
                            TreadRemaining: 91%, 86%, 86%
                          Chassis:
                           LeftFront:
                            CornerWeight: 1964 N
                            Camber: -3.48 deg
                          DriveBrake:
                           BrakeSystemConfig:
                            BaseBrakeBias: 57.0% (BBAL)
                            FineBrakeBias: 0.0 (BB+/BB-)
                         """)
        {
            ExpectedResult = new Token[]
            {
                new KeyToken(0, 8),
                new KeySeparatorToken(8, 1),
                new NewLineToken(9, 2), // CarSetup:
                new WhitespaceToken(11,1),
                new KeyToken(12,11),
                new KeySeparatorToken(23,1),
                new WhitespaceToken(24,1),
                new ValueToken(25,1),
                new NewLineToken(26,2), // UpdateCount: 8
                new WhitespaceToken(28,1),
                new KeyToken(29,9),
                new KeySeparatorToken(38,1),
                new NewLineToken(39,2), // TiresAero:
                new WhitespaceToken(41,2),
                new KeyToken(43,12),
                new KeySeparatorToken(55,1),
                new NewLineToken(56,2), // TireCompound:
                new WhitespaceToken(58,3),
                new KeyToken(61,12),
                new KeySeparatorToken(73,1),
                new WhitespaceToken(74,1),
                new ValueToken(75,4),
                new NewLineToken(79,2), // TireCompound: Soft
                new WhitespaceToken(81,2),
                new KeyToken(83,13),
                new KeySeparatorToken(96,1),
                new NewLineToken(97,2), // LeftFrontTire:
                new WhitespaceToken(99,3),
                new KeyToken(102,16),
                new KeySeparatorToken(118,1),
                new WhitespaceToken(119,1),
                new ValueToken(120,9),
                new NewLineToken(129,2), // StartingPressure: 151.7 kPa
                new WhitespaceToken(131,3),
                new KeyToken(134,15),
                new KeySeparatorToken(149,1),
                new WhitespaceToken(150,1),
                new ValueToken(151,8),
                new NewLineToken(159,2), // LastHotPressure: 22.8 psi
                new WhitespaceToken(161,3),
                new KeyToken(164,12),
                new KeySeparatorToken(176,1),
                new WhitespaceToken(177,1),
                new ValueToken(178,16),
                new NewLineToken(194,2), // LastTempsOMI: 115C, 121C, 127C
                new WhitespaceToken(196,3),
                new KeyToken(199,14),
                new KeySeparatorToken(213,1),
                new WhitespaceToken(214,1),
                new ValueToken(215,13),
                new NewLineToken(228,2), // TreadRemaining: 91%, 86%, 86%
                new WhitespaceToken(230,1),
                new KeyToken(231,7),
                new KeySeparatorToken(238,1),
                new NewLineToken(239,2), // Chassis:
                new WhitespaceToken(241,2),
                new KeyToken(243,9),
                new KeySeparatorToken(252,1),
                new NewLineToken(253,2), // LeftFront:
                new WhitespaceToken(255,3),
                new KeyToken(258,12),
                new KeySeparatorToken(270,1),
                new WhitespaceToken(271,1),
                new ValueToken(272,6),
                new NewLineToken(278,2), // CornerWeight: 1964 N
                new WhitespaceToken(280,3),
                new KeyToken(283,6),
                new KeySeparatorToken(289,1),
                new WhitespaceToken(290,1),
                new ValueToken(291,9),
                new NewLineToken(300,2), // Camber: -3.48 deg
                new WhitespaceToken(302,1),
                new KeyToken(303,10),
                new KeySeparatorToken(313,1),
                new NewLineToken(314,2), // DriveBrake:
                new WhitespaceToken(316,2),
                new KeyToken(318,17),
                new KeySeparatorToken(335,1),
                new NewLineToken(336,2), // BrakeSystemConfig:
                new WhitespaceToken(338,3),
                new KeyToken(341,13),
                new KeySeparatorToken(354,1),
                new WhitespaceToken(355,1),
                new ValueToken(356,12),
                new NewLineToken(368,2), // BaseBrakeBias: 57.0% (BBAL)
                new WhitespaceToken(370,3),
                new KeyToken(373,13),
                new KeySeparatorToken(386,1),
                new WhitespaceToken(387,1),
                new ValueToken(388,13), // FineBrakeBias: 0.0 (BB+/BB-)
            }
        };
    }
}
