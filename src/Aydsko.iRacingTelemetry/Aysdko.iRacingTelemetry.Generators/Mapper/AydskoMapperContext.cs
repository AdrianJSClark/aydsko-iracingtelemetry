using Microsoft.CodeAnalysis;

namespace Aydsko.iRacingTelemetry.Generators.Mapper;

internal sealed class AydskoMapperContext
{
    public string TargetNamespace { get; }
    public string TargetClassName { get; }
    public IReadOnlyCollection<(string mapName, IPropertySymbol property)> MappedProperties { get; }

    public AydskoMapperContext(string targetNamespace,
                               string targetClassName,
                               IReadOnlyCollection<(string name, IPropertySymbol property)> mappedProperties)
    {
        TargetClassName = targetClassName;
        TargetNamespace = targetNamespace;
        MappedProperties = mappedProperties;
    }
}
