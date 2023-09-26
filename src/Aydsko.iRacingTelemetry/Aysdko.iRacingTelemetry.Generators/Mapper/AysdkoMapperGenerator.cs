using System.Text;
using Microsoft.CodeAnalysis;

namespace Aysdko.iRacingTelemetry.Generators.Mapper;

[Generator]
internal sealed class AysdkoMapperGenerator : ISourceGenerator
{
    // class and method names from Aysdko.iRacingTelemetry project
    // --> this project cannot be linked to the source generator because it would create a cyclic dependency
    //     therefore the names of classes and methods need to be provided as string constants
    private const string VariableHeaderClass = "VariableHeader";
    private const string VariableHeaderNameProperty = "Name";
    private const string VariableBufferAccessClass = "VariableBufferAccess";
    private const string GetTypedValueMethod = "GetTypedValue";

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is AysdkoMapperSyntaxReceiver syntaxReceiver == false)
        {
            return;
        }

        foreach(var mapperContext in syntaxReceiver.Contexts)
        {
            CreateMapperSource(context, mapperContext);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AysdkoMapperSyntaxReceiver());
    }

    private static void CreateMapperSource(GeneratorExecutionContext context, AysdkoMapperContext mapperContext)
    {
        var sourceBuilder = new StringBuilder();
        var mapperClassName = $"{mapperContext.TargetClassName}_AysdkoMapper";

        // write header, using, namespace and class definition
        sourceBuilder.Append($$"""
            // Auto generated code from Aysdko.iRacingTelemetry
            // Changes will be ovewritten
            using System;
            using Aysdko.iRacingTelemetry;
            using Aysdko.iRacingTelemetry.Generators.Mapper;

            namespace {{mapperContext.TargetNamespace}};

            public sealed class {{mapperClassName}} : {{nameof(IAysdkoMapper<object>)}}<{{mapperContext.TargetClassName}}>
            {
            """);

        // write stored headers for each mapped variable
        foreach(var (_, property) in mapperContext.MappedProperties)
        {
            sourceBuilder.Append($$"""
                private {{VariableHeaderClass}}? {{property.Name}}_Header { get; }

            """);
        }

        // write constructor
        sourceBuilder.Append($$"""
                
                public {{mapperClassName}}(IEnumerable<{{VariableHeaderClass}}> variableHeaders)
                {

            """);
        foreach(var (mapName, property) in mapperContext.MappedProperties)
        {
            sourceBuilder.Append($$"""
                    {{property.Name}}_Header = variableHeaders.FirstOrDefault(x => new string(x.{{VariableHeaderNameProperty}}) == "{{mapName}}");

            """);
        }
        sourceBuilder.Append("""
                }
            """);

        // write map method
        var accessClass = 
        sourceBuilder.Append($$"""
                
                public {{mapperContext.TargetClassName}} {{nameof(IAysdkoMapper<object>.MapFromBuffer)}}(Span<byte> buffer)
                {
                    var target = new {{mapperContext.TargetClassName}}();

            """);
        foreach(var (_, property) in mapperContext.MappedProperties)
        {
            sourceBuilder.Append($$"""
                    target.{{property.Name}} = {{property.Name}}_Header is null ? default : {{VariableBufferAccessClass}}.{{GetTypedValueMethod}}<{{property.Type.ToDisplayString()}}>(buffer, {{property.Name}}_Header.Value);

            """);
        }
        sourceBuilder.Append("""
                    return target;
                }

            """);

        // end of class
        sourceBuilder.AppendLine("}");

        context.AddSource($"{mapperClassName}.generated", sourceBuilder.ToString());
    }
}
