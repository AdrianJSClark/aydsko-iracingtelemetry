// Auto generated code from Aysdko.iRacingTelemetry
// Changes will be ovewritten
using System;
using Aysdko.iRacingTelemetry;
using Aysdko.iRacingTelemetry.Generators.Mapper;

namespace Aydsko.iRacingTelemetry.UnitTests;

public sealed class MapperTestClass_AysdkoMapper : IAysdkoMapper<MapperTestClass>
{    private VariableHeader? SessionTime_Header { get; }
    private VariableHeader? SessionState_Header { get; }
    
    public MapperTestClass_AysdkoMapper(IEnumerable<VariableHeader> variableHeaders)
    {
        SessionTime_Header = variableHeaders.FirstOrDefault(x => new string(x.Name) == "SessionTime");
        SessionState_Header = variableHeaders.FirstOrDefault(x => new string(x.Name) == "SessionState");
    }    
    public MapperTestClass MapFromBuffer(Span<byte> buffer)
    {
        var target = new MapperTestClass();
        target.SessionTime = SessionTime_Header is null ? default : VariableBufferAccess.GetTypedValue<System.TimeSpan>(buffer, SessionTime_Header.Value);
        target.SessionState = SessionState_Header is null ? default : VariableBufferAccess.GetTypedValue<Aydsko.iRacingTelemetry.SessionState>(buffer, SessionState_Header.Value);
        return target;
    }
}
