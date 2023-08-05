using System.IO.MemoryMappedFiles;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Aydsko.iRacingTelemetry.UnitTests;

public unsafe class HeaderRead
{
    [Test]
    public void Test()
    {
        using var ibtStream = typeof(HeaderRead).Assembly.GetManifestResourceStream("Aydsko.iRacingTelemetry.UnitTests.mercedesw12_silverstone 2019 gp 2023-08-02 21-44-30.ibt")
                                ?? throw new Exception("Couldn't find \".ibt\" file bytes.");

        using var ibtReader = new BinaryReader(ibtStream);

        Span<byte> headerBuffer = stackalloc byte[sizeof(Header)];
        _ = ibtReader.Read(headerBuffer);
        var header = MemoryMarshal.Read<Header>(headerBuffer);
    }
}
