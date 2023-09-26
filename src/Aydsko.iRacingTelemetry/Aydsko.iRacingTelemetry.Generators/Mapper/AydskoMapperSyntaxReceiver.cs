using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Aydsko.iRacingTelemetry.Generators.Mapper;

internal sealed class AydskoMapperSyntaxReceiver : ISyntaxContextReceiver
{
    private readonly List<AydskoMapperContext> contexts = new();

    public IEnumerable<AydskoMapperContext> Contexts => contexts;

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax cds
            || context.SemanticModel.GetDeclaredSymbol(cds) is not ISymbol declaredClassSymbol
            || declaredClassSymbol.GetAttributes().FirstOrDefault(x => x.AttributeClass?.Name == "AydskoDataAttribute") is not AttributeData dataPackageAttribute)
        {
            return;
        }

        // Get value for auto mapping properties passed as constructor argument
        var autoMapProperties = (bool)(dataPackageAttribute.ConstructorArguments.FirstOrDefault().Value ?? false);

        var mapProperties = cds.ChildNodes()
                               .OfType<PropertyDeclarationSyntax>()
                               .Select(x => context.SemanticModel.GetDeclaredSymbol(x) as IPropertySymbol)
                               .Where(x => x is not null)
                               .Select(x => (name: x!.Name, property: x));

        if (autoMapProperties == false)
        {
            // Only map properties with mapping attribute
            mapProperties = mapProperties.Where(x => x.property.GetAttributes()
                                                               .Any(y => y.AttributeClass?.Name == "AydskoMapAttribute"))
                                         .Select(x => (name: GetMappedName(x.property.GetAttributes().First(y => y.AttributeClass?.Name == "AydskoMapAttribute")) ?? x.name, x.property));
        }

        var targetNameSpace = GetFullNamespace(declaredClassSymbol.ContainingNamespace);
        var mapperContext = new AydskoMapperContext(targetNameSpace,
                                                    declaredClassSymbol.Name,
                                                    mapProperties.ToList());

        contexts.Add(mapperContext);
    }

    private static string GetFullNamespace(INamespaceSymbol symbol)
    {
        var parentNamespace = symbol.ContainingNamespace;
        return string.IsNullOrEmpty(parentNamespace?.Name) ? symbol.Name : $"{GetFullNamespace(parentNamespace!)}.{symbol.Name}";
    }

    private static string? GetMappedName(AttributeData mapAttribute)
    {
        return mapAttribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
    }
}
