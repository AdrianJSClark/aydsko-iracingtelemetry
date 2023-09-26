using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aysdko.iRacingTelemetry.Generators.Mapper;
internal sealed class AysdkoMapperSyntaxReceiver : ISyntaxContextReceiver
{
    private readonly List<AysdkoMapperContext> contexts = new();
    public IEnumerable<AysdkoMapperContext> Contexts => contexts;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax cds == false)
        {
            return;
        }

        var declaredClassSymbol = context.SemanticModel.GetDeclaredSymbol(cds);
        if (declaredClassSymbol is null)
        {
            return;
        }

        // check if data package attribute was declared
        var dataPackageAttribute = declaredClassSymbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == nameof(AysdkoDataAttribute));
        if (dataPackageAttribute is null)
        {
            return;
        }

        // get value for auto mapping properties passed as constructor argument
        var autoMapProperties = (bool)(dataPackageAttribute.ConstructorArguments.FirstOrDefault().Value ?? false);

        var mapProperties = cds.ChildNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Select(x => context.SemanticModel.GetDeclaredSymbol(x) as IPropertySymbol)
            .Where(x => x is not null)
            .Select(x => (name: x!.Name, property: x));
        if (autoMapProperties == false)
        {
            // only map properties with mapping attribute
            mapProperties = mapProperties
                .Where(x => x.property.GetAttributes()
                    .Any(y => y.AttributeClass?.Name == nameof(AysdkoMapAttribute)))
                .Select(x => (name: GetMappedName(x.property.GetAttributes().First(y => y.AttributeClass?.Name == nameof(AysdkoMapAttribute))) ?? x.name, x.property));
        }

        var targetNameSpace = GetFullNamespace(declaredClassSymbol.ContainingNamespace);
        var mapperContext = new AysdkoMapperContext(
            targetNameSpace,
            declaredClassSymbol.Name,
            mapProperties.ToList()
        );

        contexts.Add(mapperContext);
    }

    private string GetFullNamespace(INamespaceSymbol symbol)
    {
        var parentNamespace = symbol.ContainingNamespace;
        if (string.IsNullOrEmpty(parentNamespace?.Name))
        {
            return symbol.Name;
        }
        return $"{GetFullNamespace(parentNamespace!)}.{symbol.Name}";
    }

    private static string? GetMappedName(AttributeData mapAttribute)
    {
        return mapAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
    }
}
