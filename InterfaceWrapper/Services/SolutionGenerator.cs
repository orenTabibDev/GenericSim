using System.IO;
using System.Text;

namespace InterfaceWrapper.Services
{
    /// <summary>
    /// Writes a Visual Studio solution (.sln) that ties the native <c>WrapperFromCToSharp</c>
    /// C++ project together with the <c>GenericSim</c> WPF project, so both live in the same
    /// solution and build in the correct order.
    /// </summary>
    public sealed class SolutionGenerator
    {
        // Visual Studio project type GUIDs.
        private const string CppProjectType = "8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942";
        private const string CSharpSdkProjectType = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";

        /// <summary>
        /// Writes <c>WrapperFromCToSharp.sln</c> into <paramref name="solutionRoot"/> and returns its path.
        /// </summary>
        public string Generate(string solutionRoot)
        {
            var slnPath = Path.Combine(solutionRoot, WrapperGenerator.ProjectName + ".sln");
            var wrapperGuid = WrapperGenerator.ProjectGuidString.ToUpperInvariant();
            var simGuid = GenericSimGenerator.ProjectGuidString.ToUpperInvariant();

            var wrapper = WrapperGenerator.ProjectName;
            var sim = GenericSimGenerator.ProjectName;

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
            sb.AppendLine("# Visual Studio Version 17");
            sb.AppendLine("VisualStudioVersion = 17.0.31903.59");
            sb.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

            sb.AppendLine($"Project(\"{{{CppProjectType}}}\") = \"{wrapper}\", \"{wrapper}\\{wrapper}.vcxproj\", \"{{{wrapperGuid}}}\"");
            sb.AppendLine("EndProject");

            sb.AppendLine($"Project(\"{{{CSharpSdkProjectType}}}\") = \"{sim}\", \"{sim}\\{sim}.csproj\", \"{{{simGuid}}}\"");
            sb.AppendLine("\tProjectSection(ProjectDependencies) = postProject");
            sb.AppendLine($"\t\t{{{wrapperGuid}}} = {{{wrapperGuid}}}");
            sb.AppendLine("\tEndProjectSection");
            sb.AppendLine("EndProject");

            sb.AppendLine("Global");
            sb.AppendLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            sb.AppendLine("\t\tDebug|x64 = Debug|x64");
            sb.AppendLine("\t\tRelease|x64 = Release|x64");
            sb.AppendLine("\tEndGlobalSection");
            sb.AppendLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (var guid in new[] { wrapperGuid, simGuid })
            {
                sb.AppendLine($"\t\t{{{guid}}}.Debug|x64.ActiveCfg = Debug|x64");
                sb.AppendLine($"\t\t{{{guid}}}.Debug|x64.Build.0 = Debug|x64");
                sb.AppendLine($"\t\t{{{guid}}}.Release|x64.ActiveCfg = Release|x64");
                sb.AppendLine($"\t\t{{{guid}}}.Release|x64.Build.0 = Release|x64");
            }
            sb.AppendLine("\tEndGlobalSection");
            sb.AppendLine("\tGlobalSection(SolutionProperties) = preSolution");
            sb.AppendLine("\t\tHideSolutionNode = FALSE");
            sb.AppendLine("\tEndGlobalSection");
            sb.AppendLine("EndGlobal");

            Directory.CreateDirectory(solutionRoot);
            File.WriteAllText(slnPath, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            return slnPath;
        }
    }
}
