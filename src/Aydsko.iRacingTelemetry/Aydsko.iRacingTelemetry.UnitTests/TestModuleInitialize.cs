using System.Runtime.CompilerServices;

namespace Aydsko.iRacingTelemetry.UnitTests;

public static class TestModuleInitialize
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyDiffPlex.Initialize();
    }
}
