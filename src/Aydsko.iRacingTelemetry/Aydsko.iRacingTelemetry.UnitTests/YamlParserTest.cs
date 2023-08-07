namespace Aydsko.iRacingTelemetry.UnitTests;

public class YamlParserTest
{
    private string _sessionInfoString;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var testFilePath = Path.Combine(Environment.CurrentDirectory, "SessionInfoString2023-08-07 123410.txt");
        _sessionInfoString = File.ReadAllText(testFilePath);
    }
    [Test]
    public void HelloWorldCheckOfYamlParser()
    {
        var sessionProperties = YamlParser.Parse("hello: world");

        Assert.That(sessionProperties, Is.Not.Null);
        Assert.That(sessionProperties, Contains.Key("hello").WithValue("world"));
    }

    [Test]
    public void CheckYamlParser()
    {
        var sessionProperties = YamlParser.Parse(_sessionInfoString);

        Assert.That(sessionProperties, Is.Not.Null);
        Assert.That(sessionProperties, Contains.Key("WeekendInfo:TrackName").WithValue("silverstone 2019 gp"));
        Assert.That(sessionProperties, Contains.Key("WeekendInfo:TrackID").WithValue("341"));
        Assert.That(sessionProperties, Contains.Key("CarSetup:TiresAero:LeftFrontTire:StartingPressure").WithValue("151.7 kPa"));

        /*
 TrackName: silverstone 2019 gp
 TrackID: 341
CarSetup:
 UpdateCount: 8
 TiresAero:
  TireCompound:
   TireCompound: Soft
  LeftFrontTire:
   StartingPressure: 151.7 kPa
 Drivers:
 - CarIdx: 0
   UserName: Adrian Clark
RadioInfo:
 SelectedRadioNum: 0
 Radios:
 - RadioNum: 0
   HopCount: 2
   NumFrequencies: 7
   TunedToFrequencyNum: 0
   ScanningIsOn: 1
   Frequencies:
   - FrequencyNum: 0
     FrequencyName: "@ALLTEAMS"
         */
    }
}
