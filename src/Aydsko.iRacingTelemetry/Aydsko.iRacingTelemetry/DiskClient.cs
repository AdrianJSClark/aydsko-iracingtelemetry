using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;

namespace Aydsko.iRacingTelemetry;

public class DiskClient : IDisposable
{
    private readonly string ibtPath;
    private bool disposedValue;
    private FileStream? _ibtFile;
    private string? _sessionInfoYaml;
    private List<irsdk_varHeader>? _headerVars;

    public string? SessionInfoYaml { get => _sessionInfoYaml; }

    public static async Task<DiskClient> OpenFileAsync(string ibtPath, CancellationToken cancellationToken = default)
    {
        var client = new DiskClient(ibtPath);
        await client.OpenFileInternalAsync(cancellationToken).ConfigureAwait(false);
        return client;
    }

    private DiskClient(string ibtPath)
    {
        if (string.IsNullOrWhiteSpace(ibtPath))
        {
            throw new ArgumentException($"'{nameof(ibtPath)}' cannot be null or whitespace.", nameof(ibtPath));
        }

        this.ibtPath = ibtPath;
    }

    private async Task OpenFileInternalAsync(CancellationToken cancellationToken)
    {
        _ibtFile = File.OpenRead(ibtPath);

        var header = await ReadFromStreamAsync<irsdk_header>(_ibtFile, cancellationToken).ConfigureAwait(false);

        if (header.sessionInfoLen == 0)
        {
#pragma warning disable CA2201 // Do not raise reserved exception types
            throw new Exception("Unsure of file format. Session information length \"0\" in header.");
#pragma warning restore CA2201 // Do not raise reserved exception types
        }

        _sessionInfoYaml = await ReadUtf8StringFromStreamAsync(_ibtFile, header.sessionInfoOffset, header.sessionInfoLen, cancellationToken).ConfigureAwait(false);

        if (header.numVars > 0)
        {
            _headerVars = new();
            _ibtFile.Seek(header.varHeaderOffset, SeekOrigin.Begin);
            var headerSize = Marshal.SizeOf(typeof(irsdk_varHeader));

            for (long offset = header.varHeaderOffset; offset < header.varHeaderOffset + (header.numVars * headerSize); offset += headerSize)
            {
                var headerVar = await ReadFromStreamAsync<irsdk_varHeader>(_ibtFile, cancellationToken).ConfigureAwait(false);
                _headerVars.Add(headerVar);
            }
        }
    }


    private static async Task<T> ReadFromStreamAsync<T>(FileStream stream, CancellationToken cancellation) where T : struct
    {
        var data = new byte[Marshal.SizeOf(typeof(T))];
        if ((await stream.ReadAsync(data, cancellation).ConfigureAwait(false)) == data.Length)
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

#pragma warning disable CA2201 // Do not raise reserved exception types - I'm lazy.
        throw new Exception("Couldn't read full length of data for object: " + typeof(T).Name);
#pragma warning restore CA2201 // Do not raise reserved exception types
    }

    private static async Task<string> ReadUtf8StringFromStreamAsync(FileStream stream, long fromOffset,  long length, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(nameof(stream));
        
        if(!stream.CanSeek)
        {
            throw new ArgumentException("Must be able to seek the given stream.", nameof(stream));
        }

        if (length == 0)
        {
            return string.Empty;
        }

        var data = new byte[length];
        _ = stream.Seek(fromOffset, SeekOrigin.Begin);
        _ = await stream.ReadAsync(data, cancellation).ConfigureAwait(false);
        return Encoding.UTF8.GetString(data);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _ibtFile?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~DiskClient()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
