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
                new(0, 5, YamlParser.YamlTokenType.Key),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 5, YamlParser.YamlTokenType.Value),
            }
        };
        yield return new("hello: world")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Key),
                new(5, 1, YamlParser.YamlTokenType.KeySeparator),
                new(6, 1, YamlParser.YamlTokenType.Whitespace),
                new(7, 5, YamlParser.YamlTokenType.Value),
            }
        };
        yield return new("hello: 'world'")
        {
            ExpectedResult = new YamlParser.YamlToken[]
            {
                new(0, 5, YamlParser.YamlTokenType.Key),
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
                new(0, 5, YamlParser.YamlTokenType.Key),
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
                new(0, 9, YamlParser.YamlTokenType.Key),
                new(9, 1, YamlParser.YamlTokenType.KeySeparator),
                new(10, 2, YamlParser.YamlTokenType.NewLine),
                new(12, 1, YamlParser.YamlTokenType.Whitespace),
                new(13, 4, YamlParser.YamlTokenType.Key),
                new(17,1, YamlParser.YamlTokenType.KeySeparator),
                new(18, 1, YamlParser.YamlTokenType.Whitespace),
                new(19,6, YamlParser.YamlTokenType.Value),
                new(25, 2, YamlParser.YamlTokenType.NewLine),
                new(27, 1, YamlParser.YamlTokenType.Whitespace),
                new(28, 4, YamlParser.YamlTokenType.Key),
                new(32,1, YamlParser.YamlTokenType.KeySeparator),
                new(33, 1, YamlParser.YamlTokenType.Whitespace),
                new(34,6, YamlParser.YamlTokenType.Value),
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
                new(0, 9, YamlParser.YamlTokenType.Key),
                new(9, 1, YamlParser.YamlTokenType.KeySeparator),
                new(10, 2, YamlParser.YamlTokenType.NewLine),
                new(12, 1, YamlParser.YamlTokenType.Whitespace),
                new(13, 4, YamlParser.YamlTokenType.Key),
                new(17,1, YamlParser.YamlTokenType.KeySeparator),
                new(18, 1, YamlParser.YamlTokenType.Whitespace),
                new(19, 1, YamlParser.YamlTokenType.StartQuote),
                new(20, 6, YamlParser.YamlTokenType.QuotedString),
                new(26, 1, YamlParser.YamlTokenType.EndQuote),
                new(27, 2, YamlParser.YamlTokenType.NewLine),
                new(29, 1, YamlParser.YamlTokenType.Whitespace),
                new(30, 4, YamlParser.YamlTokenType.Key),
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
                new(0, 4, YamlParser.YamlTokenType.Key),
                new(4, 1, YamlParser.YamlTokenType.KeySeparator),
                new(5, 2, YamlParser.YamlTokenType.NewLine),
                new(7, 1, YamlParser.YamlTokenType.Whitespace),
                new(8, 1, YamlParser.YamlTokenType.ListBullet),
                new(9,1, YamlParser.YamlTokenType.Whitespace),
                new(10,6,YamlParser.YamlTokenType.Value),
                new(16,2, YamlParser.YamlTokenType.NewLine),
                new(18,1, YamlParser.YamlTokenType.Whitespace),
                new(19, 1, YamlParser.YamlTokenType.ListBullet),
                new(20,1, YamlParser.YamlTokenType.Whitespace),
                new(21,6,YamlParser.YamlTokenType.Value),
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
                new(0, 8, YamlParser.YamlTokenType.Key),
                new(8, 1, YamlParser.YamlTokenType.KeySeparator),
                new(9, 2, YamlParser.YamlTokenType.NewLine), // CarSetup:
                new(11,1, YamlParser.YamlTokenType.Whitespace),
                new(12,11, YamlParser.YamlTokenType.Key),
                new(23,1, YamlParser.YamlTokenType.KeySeparator),
                new(24,1, YamlParser.YamlTokenType.Whitespace),
                new(25,1,YamlParser.YamlTokenType.Value),
                new(26,2, YamlParser.YamlTokenType.NewLine), // UpdateCount: 8
                new(28,1, YamlParser.YamlTokenType.Whitespace),
                new(29,9, YamlParser.YamlTokenType.Key),
                new(38,1,YamlParser.YamlTokenType.KeySeparator),
                new(39,2, YamlParser.YamlTokenType.NewLine), // TiresAero:
                new(41,2, YamlParser.YamlTokenType.Whitespace),
                new(43,12, YamlParser.YamlTokenType.Key),
                new(55,1,YamlParser.YamlTokenType.KeySeparator),
                new(56,2, YamlParser.YamlTokenType.NewLine), // TireCompound:
                new(58,3, YamlParser.YamlTokenType.Whitespace),
                new(61,12, YamlParser.YamlTokenType.Key),
                new(73,1,YamlParser.YamlTokenType.KeySeparator),
                new(74,1,YamlParser.YamlTokenType.Whitespace),
                new(75,4, YamlParser.YamlTokenType.Value),
                new(79,2, YamlParser.YamlTokenType.NewLine), // TireCompound: Soft
                new(81,2, YamlParser.YamlTokenType.Whitespace),
                new(83,13, YamlParser.YamlTokenType.Key),
                new(96,1,YamlParser.YamlTokenType.KeySeparator),
                new(97,2, YamlParser.YamlTokenType.NewLine), // LeftFrontTire:
                new(99,3,YamlParser.YamlTokenType.Whitespace),
                new(102,16, YamlParser.YamlTokenType.Key),
                new(118,1,YamlParser.YamlTokenType.KeySeparator),
                new(119,1, YamlParser.YamlTokenType.Whitespace),
                new(120,9, YamlParser.YamlTokenType.Value),
                new(129,2,YamlParser.YamlTokenType.NewLine), // StartingPressure: 151.7 kPa
                new(131,3,YamlParser.YamlTokenType.Whitespace),
                new(134,15, YamlParser.YamlTokenType.Key),
                new(149,1,YamlParser.YamlTokenType.KeySeparator),
                new(150,1, YamlParser.YamlTokenType.Whitespace),
                new(151,8, YamlParser.YamlTokenType.Value),
                new(159,2,YamlParser.YamlTokenType.NewLine), // LastHotPressure: 22.8 psi
                new(161,3,YamlParser.YamlTokenType.Whitespace),
                new(164,12, YamlParser.YamlTokenType.Key),
                new(176,1,YamlParser.YamlTokenType.KeySeparator),
                new(177,1, YamlParser.YamlTokenType.Whitespace),
                new(178,16, YamlParser.YamlTokenType.Value),
                new(194,2,YamlParser.YamlTokenType.NewLine), // LastTempsOMI: 115C, 121C, 127C
                new(196,3,YamlParser.YamlTokenType.Whitespace),
                new(199,14, YamlParser.YamlTokenType.Key),
                new(213,1,YamlParser.YamlTokenType.KeySeparator),
                new(214,1, YamlParser.YamlTokenType.Whitespace),
                new(215,13, YamlParser.YamlTokenType.Value),
                new(228,2,YamlParser.YamlTokenType.NewLine), // TreadRemaining: 91%, 86%, 86%
                new(230,1, YamlParser.YamlTokenType.Whitespace),
                new(231,7, YamlParser.YamlTokenType.Key),
                new(238,1,YamlParser.YamlTokenType.KeySeparator),
                new(239,2, YamlParser.YamlTokenType.NewLine), // Chassis:
                new(241,2, YamlParser.YamlTokenType.Whitespace),
                new(243,9, YamlParser.YamlTokenType.Key),
                new(252,1,YamlParser.YamlTokenType.KeySeparator),
                new(253,2, YamlParser.YamlTokenType.NewLine), // LeftFront:
                new(255,3, YamlParser.YamlTokenType.Whitespace),
                new(258,12, YamlParser.YamlTokenType.Key),
                new(270,1,YamlParser.YamlTokenType.KeySeparator),
                new(271,1,YamlParser.YamlTokenType.Whitespace),
                new(272,6, YamlParser.YamlTokenType.Value),
                new(278,2, YamlParser.YamlTokenType.NewLine), // CornerWeight: 1964 N
                new(280,3, YamlParser.YamlTokenType.Whitespace),
                new(283,6, YamlParser.YamlTokenType.Key),
                new(289,1,YamlParser.YamlTokenType.KeySeparator),
                new(290,1,YamlParser.YamlTokenType.Whitespace),
                new(291,9, YamlParser.YamlTokenType.Value),
                new(300,2, YamlParser.YamlTokenType.NewLine), // Camber: -3.48 deg
                new(302,1, YamlParser.YamlTokenType.Whitespace),
                new(303,10, YamlParser.YamlTokenType.Key),
                new(313,1,YamlParser.YamlTokenType.KeySeparator),
                new(314,2, YamlParser.YamlTokenType.NewLine), // DriveBrake:
                new(316,2, YamlParser.YamlTokenType.Whitespace),
                new(318,17, YamlParser.YamlTokenType.Key),
                new(335,1,YamlParser.YamlTokenType.KeySeparator),
                new(336,2, YamlParser.YamlTokenType.NewLine), // BrakeSystemConfig:
                new(338,3, YamlParser.YamlTokenType.Whitespace),
                new(341,13, YamlParser.YamlTokenType.Key),
                new(354,1,YamlParser.YamlTokenType.KeySeparator),
                new(355,1,YamlParser.YamlTokenType.Whitespace),
                new(356,12, YamlParser.YamlTokenType.Value),
                new(368,2, YamlParser.YamlTokenType.NewLine), // BaseBrakeBias: 57.0% (BBAL)
                new(370,3, YamlParser.YamlTokenType.Whitespace),
                new(373,13, YamlParser.YamlTokenType.Key),
                new(386,1,YamlParser.YamlTokenType.KeySeparator),
                new(387,1,YamlParser.YamlTokenType.Whitespace),
                new(388,13, YamlParser.YamlTokenType.Value), // FineBrakeBias: 0.0 (BB+/BB-)
            }
        };
    }
}
