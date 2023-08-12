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
                new(7, 1, YamlParser.YamlTokenType.StartQuote),
                new(8, 5, YamlParser.YamlTokenType.QuotedString),
                new(13, 1, YamlParser.YamlTokenType.EndQuote),
            }
        };
        yield return new("hello: \"world\"")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Literal),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 1, YamlParser.YamlTokenType.Whitespace),
                new(7, 1, YamlParser.YamlTokenType.StartQuote),
                new(8, 5, YamlParser.YamlTokenType.QuotedString),
                new(13, 1, YamlParser.YamlTokenType.EndQuote),
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
                new(19, 1, YamlParser.YamlTokenType.StartQuote),
                new(20, 6, YamlParser.YamlTokenType.QuotedString),
                new(26, 1, YamlParser.YamlTokenType.EndQuote),
                new(27, 2, YamlParser.YamlTokenType.NewLine),
                new(29, 1, YamlParser.YamlTokenType.Whitespace),
                new(30, 4, YamlParser.YamlTokenType.Literal),
                new(34,1, YamlParser.YamlTokenType.KeySeparator),
                new(35, 1, YamlParser.YamlTokenType.Whitespace),
                new(36, 1, YamlParser.YamlTokenType.StartQuote),
                new(37, 6, YamlParser.YamlTokenType.QuotedString),
                new(43, 1, YamlParser.YamlTokenType.EndQuote),
            }
        };
        yield return new("""
                         list:
                          - value1
                          - value2
                         """)
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 4, YamlParser.YamlTokenType.Literal),
                new(4, 1, YamlParser.YamlTokenType.KeySeparator),
                new(5, 2, YamlParser.YamlTokenType.NewLine),
                new(7, 1, YamlParser.YamlTokenType.Whitespace),
                new(8, 1, YamlParser.YamlTokenType.ListBullet),
                new(9,1, YamlParser.YamlTokenType.Whitespace),
                new(10,6,YamlParser.YamlTokenType.Literal),
                new(16,2, YamlParser.YamlTokenType.NewLine),
                new(18,1, YamlParser.YamlTokenType.Whitespace),
                new(19, 1, YamlParser.YamlTokenType.ListBullet),
                new(20,1, YamlParser.YamlTokenType.Whitespace),
                new(21,6,YamlParser.YamlTokenType.Literal),
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
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 8, YamlParser.YamlTokenType.Literal),
                new(8, 1, YamlParser.YamlTokenType.KeySeparator),
                new(9, 2, YamlParser.YamlTokenType.NewLine), // CarSetup:
                new(11,1, YamlParser.YamlTokenType.Whitespace),
                new(12,11, YamlParser.YamlTokenType.Literal),
                new(23,1, YamlParser.YamlTokenType.KeySeparator),
                new(24,1, YamlParser.YamlTokenType.Whitespace),
                new(25,1,YamlParser.YamlTokenType.Literal),
                new(26,2, YamlParser.YamlTokenType.NewLine), // UpdateCount: 8
                new(28,1, YamlParser.YamlTokenType.Whitespace),
                new(29,9, YamlParser.YamlTokenType.Literal),
                new(38,1,YamlParser.YamlTokenType.KeySeparator),
                new(39,2, YamlParser.YamlTokenType.NewLine), // TiresAero:
                new(41,2, YamlParser.YamlTokenType.Whitespace),
                new(43,12, YamlParser.YamlTokenType.Literal),
                new(55,1,YamlParser.YamlTokenType.KeySeparator),
                new(56,2, YamlParser.YamlTokenType.NewLine), // TireCompound:
                new(58,3, YamlParser.YamlTokenType.Whitespace),
                new(61,12, YamlParser.YamlTokenType.Literal),
                new(73,1,YamlParser.YamlTokenType.KeySeparator),
                new(74,1,YamlParser.YamlTokenType.Whitespace),
                new(75,4, YamlParser.YamlTokenType.Literal),
                new(79,2, YamlParser.YamlTokenType.NewLine), // TireCompound: Soft
                new(81,2, YamlParser.YamlTokenType.Whitespace),
                new(83,13, YamlParser.YamlTokenType.Literal),
                new(96,1,YamlParser.YamlTokenType.KeySeparator),
                new(97,2, YamlParser.YamlTokenType.NewLine), // LeftFrontTire:
                new(99,3,YamlParser.YamlTokenType.Whitespace),
                new(102,16, YamlParser.YamlTokenType.Literal),
                new(118,1,YamlParser.YamlTokenType.KeySeparator),
                new(119,1, YamlParser.YamlTokenType.Whitespace),
                new(120,5, YamlParser.YamlTokenType.Literal),
                new(125,1,YamlParser.YamlTokenType.Whitespace),
                new(126,3,YamlParser.YamlTokenType.Literal),
                new(129,2,YamlParser.YamlTokenType.NewLine), // StartingPressure: 151.7 kPa
                new(131,3,YamlParser.YamlTokenType.Whitespace),
                new(134,15, YamlParser.YamlTokenType.Literal),
                new(149,1,YamlParser.YamlTokenType.KeySeparator),
                new(150,1, YamlParser.YamlTokenType.Whitespace),
                new(151,4, YamlParser.YamlTokenType.Literal),
                new(155,1,YamlParser.YamlTokenType.Whitespace),
                new(156,3,YamlParser.YamlTokenType.Literal),
                new(159,2,YamlParser.YamlTokenType.NewLine), // LastHotPressure: 22.8 psi
                new(161,3,YamlParser.YamlTokenType.Whitespace),
                new(164,12, YamlParser.YamlTokenType.Literal),
                new(176,1,YamlParser.YamlTokenType.KeySeparator),
                new(177,1, YamlParser.YamlTokenType.Whitespace),
                new(178,5, YamlParser.YamlTokenType.Literal),
                new(183,1,YamlParser.YamlTokenType.Whitespace),
                new(184,5, YamlParser.YamlTokenType.Literal),
                new(189,1,YamlParser.YamlTokenType.Whitespace),
                new(190,4, YamlParser.YamlTokenType.Literal),
                new(194,2,YamlParser.YamlTokenType.NewLine), // LastTempsOMI: 115C, 121C, 127C
                new(196,3,YamlParser.YamlTokenType.Whitespace),
                new(199,14, YamlParser.YamlTokenType.Literal),
                new(213,1,YamlParser.YamlTokenType.KeySeparator),
                new(214,1, YamlParser.YamlTokenType.Whitespace),
                new(215,4, YamlParser.YamlTokenType.Literal),
                new(219,1,YamlParser.YamlTokenType.Whitespace),
                new(223,4, YamlParser.YamlTokenType.Literal),
                new(224,1,YamlParser.YamlTokenType.Whitespace),
                new(225,3, YamlParser.YamlTokenType.Literal),
                new(228,2,YamlParser.YamlTokenType.NewLine), // TreadRemaining: 91%, 86%, 86%
                new(230,1, YamlParser.YamlTokenType.Whitespace),
                new(231,7, YamlParser.YamlTokenType.Literal),
                new(238,1,YamlParser.YamlTokenType.KeySeparator),
                new(239,2, YamlParser.YamlTokenType.NewLine), // Chassis:
                new(241,2, YamlParser.YamlTokenType.Whitespace),
                new(243,9, YamlParser.YamlTokenType.Literal),
                new(252,1,YamlParser.YamlTokenType.KeySeparator),
                new(253,2, YamlParser.YamlTokenType.NewLine), // LeftFront:
                new(255,3, YamlParser.YamlTokenType.Whitespace),
                new(258,12, YamlParser.YamlTokenType.Literal),
                new(270,1,YamlParser.YamlTokenType.KeySeparator),
                new(271,1,YamlParser.YamlTokenType.Whitespace),
                new(272,4, YamlParser.YamlTokenType.Literal),
                new(276,1,YamlParser.YamlTokenType.Whitespace),
                new(277,1,YamlParser.YamlTokenType.Literal),
                new(278,2, YamlParser.YamlTokenType.NewLine), // CornerWeight: 1964 N
                new(280,3, YamlParser.YamlTokenType.Whitespace),
                new(283,6, YamlParser.YamlTokenType.Literal),
                new(289,1,YamlParser.YamlTokenType.KeySeparator),
                new(290,1,YamlParser.YamlTokenType.Whitespace),
                new(291,5, YamlParser.YamlTokenType.Literal),
                new(297,1,YamlParser.YamlTokenType.Whitespace),
                new(298,3,YamlParser.YamlTokenType.Literal),
                new(301,2, YamlParser.YamlTokenType.NewLine), // Camber: -3.48 deg
                new(303,1, YamlParser.YamlTokenType.Whitespace),
                new(304,10, YamlParser.YamlTokenType.Literal),
                new(314,1,YamlParser.YamlTokenType.KeySeparator),
                new(315,2, YamlParser.YamlTokenType.NewLine), // DriveBrake:
                new(317,2, YamlParser.YamlTokenType.Whitespace),
                new(319,17, YamlParser.YamlTokenType.Literal),
                new(336,1,YamlParser.YamlTokenType.KeySeparator),
                new(337,2, YamlParser.YamlTokenType.NewLine), // BrakeSystemConfig:
                new(339,3, YamlParser.YamlTokenType.Whitespace),
                new(342,13, YamlParser.YamlTokenType.Literal),
                new(355,1,YamlParser.YamlTokenType.KeySeparator),
                new(356,1,YamlParser.YamlTokenType.Whitespace),
                new(357,5, YamlParser.YamlTokenType.Literal),
                new(362,1,YamlParser.YamlTokenType.Whitespace),
                new(363,6,YamlParser.YamlTokenType.Literal),
                new(369,2, YamlParser.YamlTokenType.NewLine), // BaseBrakeBias: 57.0% (BBAL)
                new(371,3, YamlParser.YamlTokenType.Whitespace),
                new(374,13, YamlParser.YamlTokenType.Literal),
                new(387,1,YamlParser.YamlTokenType.KeySeparator),
                new(388,1,YamlParser.YamlTokenType.Whitespace),
                new(391,3, YamlParser.YamlTokenType.Literal),
                new(394,1,YamlParser.YamlTokenType.Whitespace),
                new(395,9,YamlParser.YamlTokenType.Literal),
                new(404,2, YamlParser.YamlTokenType.NewLine), // FineBrakeBias: 0.0 (BB+/BB-)
            }
        };
    }
}
