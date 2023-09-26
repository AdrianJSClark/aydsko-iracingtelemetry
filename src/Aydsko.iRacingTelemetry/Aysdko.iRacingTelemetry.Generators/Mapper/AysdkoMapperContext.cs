using Microsoft.CodeAnalysis;

namespace Aysdko.iRacingTelemetry.Generators.Mapper;
internal sealed class AysdkoMapperContext
{
    public string TargetNamespace { get; }
    public string TargetClassName { get; }
    public IReadOnlyCollection<(string mapName, IPropertySymbol property)> MappedProperties { get; }

    public AysdkoMapperContext(string targetNamespace, string targetClassName, IReadOnlyCollection<(string name, IPropertySymbol property)> mappedProperties)
    {
        TargetClassName = targetClassName;
        TargetNamespace = targetNamespace;
        MappedProperties = mappedProperties;
    }
}
