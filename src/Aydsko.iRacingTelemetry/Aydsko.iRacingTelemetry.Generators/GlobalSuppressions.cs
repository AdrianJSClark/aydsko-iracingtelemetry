// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

/*
 * This shouldn't be required but until "How should source generators emit newlines?" (https://github.com/dotnet/roslyn/issues/51437) is resolved
 * or a change is made (see "False positive RS1035 error for Environment.NewLine" https://github.com/dotnet/roslyn-analyzers/issues/6467) to the
 * analyzer, this suppression is required.
 */
[assembly: SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers", Justification = "<Pending>", Scope = "member", Target = "~M:Aydsko.iRacingTelemetry.Generators.Mapper.AydskoMapperGenerator.CreateMapperSource(Microsoft.CodeAnalysis.GeneratorExecutionContext,Aydsko.iRacingTelemetry.Generators.Mapper.AydskoMapperContext)")]
