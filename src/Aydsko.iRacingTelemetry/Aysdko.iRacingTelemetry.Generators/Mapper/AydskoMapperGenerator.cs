using System.Text;
using Microsoft.CodeAnalysis;

namespace Aydsko.iRacingTelemetry.Generators.Mapper;

[Generator]
internal sealed class AydskoMapperGenerator : ISourceGenerator
{
    // class and method names from Aydsko.iRacingTelemetry project
    // --> this project cannot be linked to the source generator because it would create a cyclic dependency
    //     therefore the names of classes and methods need to be provided as string constants
    private const string VariableHeaderClass = "VariableHeader";
    private const string VariableHeaderNameProperty = "Name";
    private const string VariableBufferAccessClass = "VariableBufferAccess";
    private const string GetTypedValueMethod = "GetTypedValue";

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is AydskoMapperSyntaxReceiver syntaxReceiver == false)
        {
            return;
        }

        foreach (var mapperContext in syntaxReceiver.Contexts)
        {
            CreateMapperSource(context, mapperContext);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AydskoMapperSyntaxReceiver());
    }

    private static void CreateMapperSource(GeneratorExecutionContext context, AydskoMapperContext mapperContext)
    {
        var sourceBuilder = new StringBuilder();
        var mapperClassName = $"{mapperContext.TargetClassName}AydskoMapper";

        // write header, using, namespace and class definition
        _ = sourceBuilder.Append($$"""
            // Auto generated code from Aydsko.iRacingTelemetry
            // Changes will be overwritten
            using System;
            using Aydsko.iRacingTelemetry;
            using Aydsko.iRacingTelemetry.Generators.Mapper;

            namespace {{mapperContext.TargetNamespace}};

            public sealed class {{mapperClassName}} : {{nameof(IAydskoMapper<object>)}}<{{mapperContext.TargetClassName}}>
            {
            """);

        _ = sourceBuilder.AppendLine();

        // Write stored headers for each mapped variable
        var propertyDeclarations = mapperContext.MappedProperties.Select(mappedProperty => $"    private {VariableHeaderClass}? {mappedProperty.property.Name}Header {{ get; }}");
        _ = sourceBuilder.AppendLine(string.Join(Environment.NewLine, propertyDeclarations));

        _ = sourceBuilder.AppendLine();

        // Write constructor
        _ = sourceBuilder.AppendLine($"    public {mapperClassName}(IEnumerable<{VariableHeaderClass}> variableHeaders)");
        _ = sourceBuilder.AppendLine("    {");

        var propertyInitializations = mapperContext.MappedProperties.Select(mappedProperty => $"        {mappedProperty.property.Name}Header = variableHeaders.FirstOrDefault(x => new string(x.{VariableHeaderNameProperty}) == \"{mappedProperty.mapName}\");");
        _ = sourceBuilder.AppendLine(string.Join(Environment.NewLine, propertyInitializations));

        _ = sourceBuilder.AppendLine("    }");

        _ = sourceBuilder.AppendLine();

        // Write map method
        _ = sourceBuilder.AppendLine($"    public {mapperContext.TargetClassName} {nameof(IAydskoMapper<object>.MapFromBuffer)}(Span<byte> buffer)");
        _ = sourceBuilder.AppendLine("    {");
        _ = sourceBuilder.AppendLine($"        var target = new {mapperContext.TargetClassName}();");

        var valueMapLines = mapperContext.MappedProperties.Select(mappedProperty => $"        target.{mappedProperty.property.Name} = {mappedProperty.property.Name}Header is null ? default : {VariableBufferAccessClass}.{GetTypedValueMethod}<{mappedProperty.property.Type.ToDisplayString()}>(buffer, {mappedProperty.property.Name}Header.Value);");
        _ = sourceBuilder.Append(string.Join(Environment.NewLine, valueMapLines));

        _ = sourceBuilder.AppendLine();
        _ = sourceBuilder.AppendLine("        return target;");
        _ = sourceBuilder.AppendLine("    }");

        // End of class
        _ = sourceBuilder.AppendLine("}");

        context.AddSource($"{mapperClassName}.g.cs", sourceBuilder.ToString());
    }
}
