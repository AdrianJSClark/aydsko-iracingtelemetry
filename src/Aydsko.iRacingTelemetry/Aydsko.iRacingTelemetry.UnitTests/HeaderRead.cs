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

        var data = new byte[(Marshal.SizeOf(typeof(irsdk_header)))];
        ibtStream.Read(data, 0, data.Length);

        var headerHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

        try
        {
            var header = Marshal.PtrToStructure<irsdk_header>(headerHandle.AddrOfPinnedObject());
        }
        finally
        {
            headerHandle.Free();
        }
    }
}
