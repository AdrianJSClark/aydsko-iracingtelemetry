// Auto generated code from Aydsko.iRacingTelemetry
// Changes will be overwritten
using System;
using Aydsko.iRacingTelemetry;
using Aydsko.iRacingTelemetry.Generators.Mapper;

namespace Aydsko.iRacingTelemetry.UnitTests;

public sealed class MapperTestClassAydskoMapper : IAydskoMapper<MapperTestClass>
{
    private VariableHeader? SessionTimeHeader { get; }
    private VariableHeader? SessionStateHeader { get; }

    public MapperTestClassAydskoMapper(IEnumerable<VariableHeader> variableHeaders)
    {
        SessionTimeHeader = variableHeaders.FirstOrDefault(x => new string(x.Name) == "SessionTime");
        SessionStateHeader = variableHeaders.FirstOrDefault(x => new string(x.Name) == "SessionState");
    }

    public MapperTestClass MapFromBuffer(Span<byte> buffer)
    {
        var target = new MapperTestClass();
        target.SessionTime = SessionTimeHeader is null ? default : VariableBufferAccess.GetTypedValue<System.TimeSpan>(buffer, SessionTimeHeader.Value);
        target.SessionState = SessionStateHeader is null ? default : VariableBufferAccess.GetTypedValue<Aydsko.iRacingTelemetry.SessionState>(buffer, SessionStateHeader.Value);
        return target;
    }
}
