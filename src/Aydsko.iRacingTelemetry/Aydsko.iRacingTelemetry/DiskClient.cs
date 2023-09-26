using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Aydsko.iRacingTelemetry;

public class DiskClient : IDisposable
{
    private readonly string ibtPath;
    private bool disposedValue;
    private FileStream? _ibtFile;
    private FileHeader _header;
    private DiskSubHeaderInternal _diskSubHeader;
    private DiskSubHeader? _diskSubHeaderValue;
    private List<VariableHeader>? _headerVariables;

    public string? SessionInfoYaml { get; private set; }

    public DiskSubHeader DiskSubHeader
    {
        get
        {
            if (_diskSubHeaderValue is null)
            {
                var epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
                _diskSubHeaderValue = new(epoch.AddSeconds(_diskSubHeader.SessionStartDate),
                                          _diskSubHeader.SessionStartTime,
                                          _diskSubHeader.SessionEndTime,
                                          _diskSubHeader.SessionLapCount,
                                          _diskSubHeader.SessionRecordCount);
            }
            return _diskSubHeaderValue;
        }
    }

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

        _header = await ReadFromStreamAsync<FileHeader>(_ibtFile, cancellationToken).ConfigureAwait(false);
        _diskSubHeader = await ReadFromStreamAsync<DiskSubHeaderInternal>(_ibtFile, cancellationToken).ConfigureAwait(!false);

        if (_header.SessionInfoLen == 0)
        {
#pragma warning disable CA2201 // Do not raise reserved exception types
            throw new Exception("Unsure of file format. Session information length \"0\" in header.");
#pragma warning restore CA2201 // Do not raise reserved exception types
        }

        SessionInfoYaml = await ReadUtf8StringFromStreamAsync(_ibtFile, _header.SessionInfoOffset, _header.SessionInfoLen, cancellationToken).ConfigureAwait(false);

        if (_header.NumberOfVariables > 0)
        {
            _headerVariables = new(_header.NumberOfVariables);
            var headerSize = Marshal.SizeOf(typeof(VariableHeader));

            _ = _ibtFile.Seek(_header.VariableHeaderOffset, SeekOrigin.Begin);

            Memory<byte> headerVarBytes = new byte[_header.NumberOfVariables * headerSize];
            _ = await _ibtFile.ReadAsync(headerVarBytes, cancellationToken).ConfigureAwait(false);

            for (var offset = 0; offset < headerVarBytes.Length; offset += headerSize)
            {
                var headerVar = CreateFromSpan<VariableHeader>(headerVarBytes.Slice(offset, headerSize).Span);
                _headerVariables.Add(headerVar);
            }
        }
    }

    public IEnumerable<VariableHeader> GetHeaderVariables() => _headerVariables ?? Enumerable.Empty<VariableHeader>();

    public async IAsyncEnumerable<Memory<byte>> ReadBufferLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_ibtFile is null)
        {
            throw new InvalidOperationException("Unexpected null value for the \".ibt\" file.");
        }

        _ = _ibtFile.Seek(_header.VariableBuffers[0].BufferOffset, SeekOrigin.Begin);

        Memory<byte> valueBuffer = new byte[_header.BufferLengthBytes];
        while ((await _ibtFile.ReadAsync(valueBuffer, cancellationToken).ConfigureAwait(false)) == _header.BufferLengthBytes)
        {
            yield return valueBuffer;
        }
    }

    public async IAsyncEnumerable<Variable[]> ReadVariableLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_ibtFile is null)
        {
            throw new InvalidOperationException("Unexpected null value for the \".ibt\" file.");
        }

        if (_headerVariables is null or { Count: 0 })
        {
            yield break;
        }

        _ = _ibtFile.Seek(_header.VariableBuffers[0].BufferOffset, SeekOrigin.Begin);

        Memory<byte> valueBuffer = new byte[_header.BufferLengthBytes];
        while ((await _ibtFile.ReadAsync(valueBuffer, cancellationToken).ConfigureAwait(false)) == _header.BufferLengthBytes)
        {
            var line = new List<Variable>(_header.NumberOfVariables);
            foreach (var headerVar in _headerVariables)
            {
                Variable headerVariable = (headerVar.Type, headerVar.Count, CreateString(headerVar.Unit)) switch
                {
                    (0, 1, _) => new Variable<char>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, char>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 1).Span)[0]),
                    (1, 1, _) => new Variable<bool>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, bool>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 1).Span)[0]),
                    (2, 1, "irsdk_TrkSurf") => new Variable<TrackSurface>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (TrackSurface)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, "irsdk_TrkLoc") => new Variable<TrackLocation>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (TrackLocation)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, "irsdk_SessionState") => new Variable<SessionState>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (SessionState)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, "irsdk_PitSvStatus") => new Variable<PitServiceStatus>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (PitServiceStatus)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, "irsdk_CameraState") => new Variable<CameraState>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (CameraState)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, "irsdk_PaceMode") => new Variable<PaceMode>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (PaceMode)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (2, 1, _) => new Variable<int>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (3, 1, "irsdk_Flags") => new Variable<GlobalState>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (GlobalState)MemoryMarshal.Cast<byte, uint>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (3, 1, "irsdk_PitSvFlags") => new Variable<PitService>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (PitService)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (3, 1, "irsdk_EngineWarnings") => new Variable<EngineWarnings>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), (EngineWarnings)MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (3, 1, _) => new Variable<int>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (4, 1, _) => new Variable<float>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, float>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span)[0]),
                    (5, 1, _) => new Variable<double>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, double>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 8).Span)[0]),
                    (0, > 1, _) => new Variable<char[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, char>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 1).Span).ToArray()),
                    (1, > 1, _) => new Variable<bool[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, bool>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 1).Span).ToArray()),
                    (2, > 1, _) => new Variable<int[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span).ToArray()),
                    (3, > 1, _) => new Variable<int[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, int>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span).ToArray()),
                    (4, > 1, _) => new Variable<float[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, float>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 4).Span).ToArray()),
                    (5, > 1, _) => new Variable<double[]>(CreateString(headerVar.Name), CreateString(headerVar.Description), CreateString(headerVar.Unit), MemoryMarshal.Cast<byte, double>(valueBuffer.Slice(headerVar.Offset, headerVar.Count * 8).Span).ToArray()),
                    _ => throw new InvalidDataException("Unexpected header variable type value: " + headerVar.Type)
                };

                line.Add(headerVariable);
            }

            yield return line.ToArray();
        }
    }

    private static string? CreateString(char[] chars)
    {
        if (chars is null or { Length: 0 } || chars[0] == '\0')
        {
            return null;
        }

        var value = new string(chars);
        return value.TrimEnd('\0');
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

    private static T? CreateFromSpan<T>(Span<byte> span)
    {
        var handle = GCHandle.Alloc(span.ToArray(), GCHandleType.Pinned);
        try
        {
            return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }

    private static async Task<string> ReadUtf8StringFromStreamAsync(FileStream stream, long fromOffset, long length, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(nameof(stream));

        if (!stream.CanSeek)
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
