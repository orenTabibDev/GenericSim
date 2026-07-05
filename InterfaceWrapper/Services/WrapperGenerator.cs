using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InterfaceWrapper.Models;
using InterfaceWrapper.Services;

namespace InterfaceWrapper.Services
{
    /// <summary>
    /// Generates the <c>WrapperFromCToSharp</c> native C++ project that exposes the discovered
    /// convert functions through a flat <c>extern "C"</c> ABI which can be consumed from C#.
    /// </summary>
    public sealed class WrapperGenerator
    {
        public const string ProjectName = "WrapperFromCToSharp";

        private static readonly Guid ProjectGuid = new("D6E2F3A1-4B5C-4D6E-8F90-A1B2C3D4E5F6");

        /// <summary>The project GUID as an uppercase string (no braces), for the .sln file.</summary>
        public static string ProjectGuidString => ProjectGuid.ToString();

        public string OutputDirectory { get; }

        private readonly ParseResult _parseResult;

        public WrapperGenerator(ParseResult parseResult, string outputDirectory)
        {
            _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
            OutputDirectory = outputDirectory ?? throw new ArgumentNullException(nameof(outputDirectory));
        }

        /// <summary>
        /// Generates the whole project. Returns the list of files that were written.
        /// </summary>
        public IReadOnlyList<string> Generate(string sourceMessagesFolder)
        {
            var written = new List<string>();

            var projectDir = Path.Combine(OutputDirectory, ProjectName);
            var messagesDir = Path.Combine(projectDir, "Messages");
            Directory.CreateDirectory(messagesDir);

            var messageFiles = CopyMessageFiles(sourceMessagesFolder, messagesDir);

            written.Add(WriteFile(Path.Combine(projectDir, ProjectName + ".h"), BuildWrapperHeader()));
            written.Add(WriteFile(Path.Combine(projectDir, ProjectName + ".cpp"), BuildWrapperSource()));
            written.Add(WriteFile(Path.Combine(projectDir, ProjectName + ".vcxproj"), BuildVcxproj(messageFiles)));
            written.Add(WriteFile(Path.Combine(projectDir, ProjectName + ".vcxproj.filters"), BuildFilters(messageFiles)));
            written.Add(WriteFile(Path.Combine(projectDir, "WrapperInterop.cs"), BuildCSharpInterop()));
            written.Add(WriteFile(Path.Combine(projectDir, "README.md"), BuildReadme()));

            return written;
        }

        /// <summary>
        /// The base type / helper files that the convert functions depend on. They are always
        /// added to the generated project (from the source folder or the embedded templates)
        /// so the wrapper's types (AdiInt, AdiUInt8, ...) and helpers are always defined.
        /// </summary>
        private static readonly string[] RequiredSupportFiles = { "typelib.h", "convlib.h", "convlib.c" };

        /// <summary>The message C/H files that make up the generated project.</summary>
        private sealed class MessageFileSet
        {
            public List<string> CompiledSources { get; } = new();
            public List<string> Headers { get; } = new();
        }

        /// <summary>
        /// Copies every message C/H file into the generated project so it is self contained and
        /// guarantees the required support files are present. Returns the files to reference.
        /// </summary>
        private MessageFileSet CopyMessageFiles(string sourceMessagesFolder, string messagesDir)
        {
            var allFiles = Directory.GetFiles(sourceMessagesFolder)
                .Where(f =>
                {
                    var ext = Path.GetExtension(f).ToLowerInvariant();
                    return ext is ".c" or ".cpp" or ".h" or ".hpp";
                })
                .ToList();

            foreach (var file in allFiles)
            {
                var name = Path.GetFileName(file);

                // convlib.cpp and convlib.c define the same symbols; we always ship convlib.c
                // (from the source folder or the embedded template), so never compile convlib.cpp.
                if (string.Equals(name, "convlib.cpp", StringComparison.OrdinalIgnoreCase))
                    continue;

                File.Copy(file, Path.Combine(messagesDir, name), overwrite: true);
            }

            EnsureSupportFiles(messagesDir);

            return CollectMessageFiles(messagesDir);
        }

        /// <summary>
        /// Writes any missing support file (typelib.h, convlib.h, convlib.c) from the embedded
        /// templates so the generated project always compiles and links.
        /// </summary>
        private static void EnsureSupportFiles(string messagesDir)
        {
            foreach (var required in RequiredSupportFiles)
            {
                var target = Path.Combine(messagesDir, required);
                if (File.Exists(target))
                    continue;

                WriteEmbeddedTemplate(required, target);
            }
        }

        /// <summary>Extracts an embedded template resource to the target path.</summary>
        private static void WriteEmbeddedTemplate(string fileName, string targetPath)
        {
            var assembly = typeof(WrapperGenerator).Assembly;
            var resourceName = assembly.GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
                throw new InvalidOperationException(
                    $"Embedded template '{fileName}' was not found in the application resources.");

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Unable to open embedded template '{fileName}'.");
            using var target = File.Create(targetPath);
            stream.CopyTo(target);
        }

        /// <summary>Enumerates the generated Messages directory into compiled sources and headers.</summary>
        private static MessageFileSet CollectMessageFiles(string messagesDir)
        {
            var set = new MessageFileSet();

            foreach (var file in Directory.GetFiles(messagesDir))
            {
                var name = Path.GetFileName(file);
                var ext = Path.GetExtension(file).ToLowerInvariant();
                if (ext is ".c" or ".cpp")
                    set.CompiledSources.Add(name);
                else if (ext is ".h" or ".hpp")
                    set.Headers.Add(name);
            }

            set.CompiledSources.Sort(StringComparer.OrdinalIgnoreCase);
            set.Headers.Sort(StringComparer.OrdinalIgnoreCase);
            return set;
        }

        private string PrimaryHeader =>
            _parseResult.SystemPrefix is { Length: > 0 } prefix ? prefix + "prc.h" : "prc.h";

        private string VarsHeader =>
            _parseResult.SystemPrefix is { Length: > 0 } prefix ? prefix + "vare.h" : "vare.h";

        private string BuildWrapperHeader()
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* Auto-generated by InterfaceWrapper. Do not edit by hand. */");
            sb.AppendLine("#ifndef WRAPPER_FROM_C_TO_SHARP_H");
            sb.AppendLine("#define WRAPPER_FROM_C_TO_SHARP_H");
            sb.AppendLine();
            sb.AppendLine("/* Keep the original C types (AdiInt, AdiUInt8, PHS_*) as declared in");
            sb.AppendLine("   typelib.h, convlib.h and the generated type/prototype headers. */");
            sb.AppendLine($"#include \"Messages/{PrimaryHeader}\"");
            sb.AppendLine();
            sb.AppendLine("#ifdef WRAPPERFROMCTOSHARP_EXPORTS");
            sb.AppendLine("#define WRAPPER_API __declspec(dllexport)");
            sb.AppendLine("#else");
            sb.AppendLine("#define WRAPPER_API __declspec(dllimport)");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#ifdef __cplusplus");
            sb.AppendLine("extern \"C\" {");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("/* Generic dispatchers operating on the static physical structures. */");
            sb.AppendLine("WRAPPER_API AdiInt Wrapper_ConvertToPhysical(AdiInt msgId, AdiUInt8* intfPtr);");
            sb.AppendLine("WRAPPER_API void   Wrapper_ConvertToInterface(AdiInt msgId, AdiUInt8* intfPtr);");
            sb.AppendLine();

            foreach (var msg in _parseResult.Messages)
            {
                sb.AppendLine($"/* {msg.MessageName} ({msg.PhysicalType}) */");

                if (!string.IsNullOrEmpty(msg.GlobalVariable))
                    sb.AppendLine($"WRAPPER_API {msg.PhysicalType}* Wrapper_Get_{msg.MessageName}(void);");

                if (msg.HasToPhysical)
                {
                    var ret = ToPhysicalReturnType(msg);
                    sb.AppendLine($"WRAPPER_API {ret} {msg.MessageName}_ConvertToPhysical({msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr);");
                }
                if (msg.HasToInterface)
                {
                    var ret = ToInterfaceReturnType(msg);
                    sb.AppendLine($"WRAPPER_API {ret} {msg.MessageName}_ConvertToInterface({msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr);");
                }

                sb.AppendLine();
            }

            sb.AppendLine("#ifdef __cplusplus");
            sb.AppendLine("}");
            sb.AppendLine("#endif");
            sb.AppendLine();
            sb.AppendLine("#endif /* WRAPPER_FROM_C_TO_SHARP_H */");
            return sb.ToString();
        }

        /// <summary>The original C return type of the convert-to-physical function (defaults to AdiInt).</summary>
        private static string ToPhysicalReturnType(MessageDefinition msg) =>
            string.IsNullOrWhiteSpace(msg.ToPhysical?.ReturnType) ? "AdiInt" : msg.ToPhysical!.ReturnType;

        /// <summary>The original C return type of the convert-to-interface function (defaults to void).</summary>
        private static string ToInterfaceReturnType(MessageDefinition msg) =>
            string.IsNullOrWhiteSpace(msg.ToInterface?.ReturnType) ? "void" : msg.ToInterface!.ReturnType;

        private string BuildWrapperSource()
        {
            var sb = new StringBuilder();
            sb.AppendLine("/* Auto-generated by InterfaceWrapper. Do not edit by hand. */");
            sb.AppendLine("#define WRAPPERFROMCTOSHARP_EXPORTS");
            sb.AppendLine();
            sb.AppendLine($"#include \"{ProjectName}.h\"           /* wrapper API + original C types */");
            sb.AppendLine($"#include \"Messages/{VarsHeader}\" /* static Phs_* physical structures */");
            sb.AppendLine();

            var bus = _parseResult.BusName;

            sb.AppendLine("WRAPPER_API AdiInt Wrapper_ConvertToPhysical(AdiInt msgId, AdiUInt8* intfPtr)");
            sb.AppendLine("{");
            if (!string.IsNullOrEmpty(bus))
                sb.AppendLine($"    return {bus}_ConvertToPhysical(msgId, intfPtr);");
            else
                sb.AppendLine("    (void)msgId; (void)intfPtr; return -1;");
            sb.AppendLine("}");
            sb.AppendLine();

            sb.AppendLine("WRAPPER_API void Wrapper_ConvertToInterface(AdiInt msgId, AdiUInt8* intfPtr)");
            sb.AppendLine("{");
            if (!string.IsNullOrEmpty(bus))
                sb.AppendLine($"    {bus}_ConvertToInterface(msgId, intfPtr);");
            else
                sb.AppendLine("    (void)msgId; (void)intfPtr;");
            sb.AppendLine("}");
            sb.AppendLine();

            foreach (var msg in _parseResult.Messages)
            {
                if (!string.IsNullOrEmpty(msg.GlobalVariable))
                {
                    sb.AppendLine($"WRAPPER_API {msg.PhysicalType}* Wrapper_Get_{msg.MessageName}(void)");
                    sb.AppendLine("{");
                    sb.AppendLine($"    return &{msg.GlobalVariable};");
                    sb.AppendLine("}");
                    sb.AppendLine();
                }

                if (msg.HasToPhysical)
                {
                    var ret = ToPhysicalReturnType(msg);
                    sb.AppendLine($"WRAPPER_API {ret} {msg.MessageName}_ConvertToPhysical({msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr)");
                    sb.AppendLine("{");
                    var call = $"{msg.ToPhysical!.FunctionName}(phsPtr, intfPtr);";
                    sb.AppendLine(ret == "void" ? $"    {call}" : $"    return {call}");
                    sb.AppendLine("}");
                    sb.AppendLine();
                }

                if (msg.HasToInterface)
                {
                    var ret = ToInterfaceReturnType(msg);
                    sb.AppendLine($"WRAPPER_API {ret} {msg.MessageName}_ConvertToInterface({msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr)");
                    sb.AppendLine("{");
                    var call = $"{msg.ToInterface!.FunctionName}(phsPtr, intfPtr);";
                    sb.AppendLine(ret == "void" ? $"    {call}" : $"    return {call}");
                    sb.AppendLine("}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private string BuildVcxproj(MessageFileSet messageFiles)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Project DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            sb.AppendLine("  <ItemGroup Label=\"ProjectConfigurations\">");
            foreach (var (config, platform) in Configurations())
            {
                sb.AppendLine($"    <ProjectConfiguration Include=\"{config}|{platform}\">");
                sb.AppendLine($"      <Configuration>{config}</Configuration>");
                sb.AppendLine($"      <Platform>{platform}</Platform>");
                sb.AppendLine("    </ProjectConfiguration>");
            }
            sb.AppendLine("  </ItemGroup>");

            sb.AppendLine("  <PropertyGroup Label=\"Globals\">");
            sb.AppendLine($"    <ProjectGuid>{{{ProjectGuid}}}</ProjectGuid>");
            sb.AppendLine($"    <RootNamespace>{ProjectName}</RootNamespace>");
            sb.AppendLine("    <WindowsTargetPlatformVersion>10.0</WindowsTargetPlatformVersion>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.Default.props\" />");

            foreach (var (config, platform) in Configurations())
            {
                var isDebug = config == "Debug";
                sb.AppendLine($"  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='{config}|{platform}'\" Label=\"Configuration\">");
                sb.AppendLine("    <ConfigurationType>DynamicLibrary</ConfigurationType>");
                sb.AppendLine($"    <UseDebugLibraries>{(isDebug ? "true" : "false")}</UseDebugLibraries>");
                sb.AppendLine("    <PlatformToolset>v143</PlatformToolset>");
                sb.AppendLine("    <CharacterSet>Unicode</CharacterSet>");
                if (!isDebug)
                    sb.AppendLine("    <WholeProgramOptimization>true</WholeProgramOptimization>");
                sb.AppendLine("  </PropertyGroup>");
            }

            sb.AppendLine("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.props\" />");

            // Deterministic output path so the GenericSim project can locate the DLL:
            // <ProjectDir>\<Platform>\<Configuration>\WrapperFromCToSharp.dll
            foreach (var (config, platform) in Configurations())
            {
                sb.AppendLine($"  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='{config}|{platform}'\">");
                sb.AppendLine($"    <OutDir>$(ProjectDir){platform}\\{config}\\</OutDir>");
                sb.AppendLine($"    <IntDir>$(ProjectDir){platform}\\{config}\\obj\\</IntDir>");
                sb.AppendLine($"    <TargetName>{ProjectName}</TargetName>");
                sb.AppendLine("  </PropertyGroup>");
            }

            foreach (var (config, platform) in Configurations())
            {
                var isDebug = config == "Debug";
                sb.AppendLine($"  <ItemDefinitionGroup Condition=\"'$(Configuration)|$(Platform)'=='{config}|{platform}'\">");
                sb.AppendLine("    <ClCompile>");
                sb.AppendLine("      <AdditionalIncludeDirectories>$(ProjectDir)Messages;%(AdditionalIncludeDirectories)</AdditionalIncludeDirectories>");
                sb.AppendLine($"      <WarningLevel>Level3</WarningLevel>");
                sb.AppendLine($"      <Optimization>{(isDebug ? "Disabled" : "MaxSpeed")}</Optimization>");
                sb.AppendLine("      <PreprocessorDefinitions>WRAPPERFROMCTOSHARP_EXPORTS;_CRT_SECURE_NO_WARNINGS;WIN32;_WINDOWS;_USRDLL;%(PreprocessorDefinitions)</PreprocessorDefinitions>");
                sb.AppendLine("      <CompileAs>Default</CompileAs>");
                sb.AppendLine("    </ClCompile>");
                sb.AppendLine("    <Link>");
                sb.AppendLine("      <SubSystem>Windows</SubSystem>");
                sb.AppendLine($"      <GenerateDebugInformation>{(isDebug ? "true" : "false")}</GenerateDebugInformation>");
                sb.AppendLine("    </Link>");
                sb.AppendLine("  </ItemDefinitionGroup>");
            }

            // Header files.
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ClInclude Include=\"{ProjectName}.h\" />");
            foreach (var header in messageFiles.Headers)
                sb.AppendLine($"    <ClInclude Include=\"Messages\\{header}\" />");
            sb.AppendLine("  </ItemGroup>");

            // Source files.
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ClCompile Include=\"{ProjectName}.cpp\" />");
            foreach (var source in messageFiles.CompiledSources)
                sb.AppendLine($"    <ClCompile Include=\"Messages\\{source}\" />");
            sb.AppendLine("  </ItemGroup>");

            sb.AppendLine("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.targets\" />");
            sb.AppendLine("</Project>");
            return sb.ToString();
        }

        private string BuildFilters(MessageFileSet messageFiles)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine("    <Filter Include=\"Source Files\"><UniqueIdentifier>{4FC737F1-C7A5-4376-A066-2A32D752A2FF}</UniqueIdentifier></Filter>");
            sb.AppendLine("    <Filter Include=\"Header Files\"><UniqueIdentifier>{93995380-89BD-4b04-88EB-625FBE52EBFB}</UniqueIdentifier></Filter>");
            sb.AppendLine("    <Filter Include=\"Messages\"><UniqueIdentifier>{67DA6AB6-F800-4c08-8B7A-83BB121AAD01}</UniqueIdentifier></Filter>");
            sb.AppendLine("  </ItemGroup>");

            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ClInclude Include=\"{ProjectName}.h\"><Filter>Header Files</Filter></ClInclude>");
            foreach (var header in messageFiles.Headers)
                sb.AppendLine($"    <ClInclude Include=\"Messages\\{header}\"><Filter>Messages</Filter></ClInclude>");
            sb.AppendLine("  </ItemGroup>");

            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ClCompile Include=\"{ProjectName}.cpp\"><Filter>Source Files</Filter></ClCompile>");
            foreach (var source in messageFiles.CompiledSources)
                sb.AppendLine($"    <ClCompile Include=\"Messages\\{source}\"><Filter>Messages</Filter></ClCompile>");
            sb.AppendLine("  </ItemGroup>");

            sb.AppendLine("</Project>");
            return sb.ToString();
        }

        private string BuildCSharpInterop()
        {
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by InterfaceWrapper. Do not edit by hand.");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine();
            sb.AppendLine("namespace WrapperFromCToSharp");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// P/Invoke bindings for the native WrapperFromCToSharp.dll.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine("    public static class WrapperInterop");
            sb.AppendLine("    {");
            sb.AppendLine($"        private const string Dll = \"{ProjectName}.dll\";");
            sb.AppendLine();

            // Message id enum.
            var withIds = _parseResult.Messages.Where(m => m.MessageId.HasValue).ToList();
            if (withIds.Count > 0)
            {
                sb.AppendLine("        /// <summary>Numeric message identifiers discovered in the bus dispatcher.</summary>");
                sb.AppendLine("        public enum MessageId");
                sb.AppendLine("        {");
                foreach (var msg in withIds.OrderBy(m => m.MessageId!.Value))
                    sb.AppendLine($"            {msg.MessageName} = {msg.MessageId!.Value},");
                sb.AppendLine("        }");
                sb.AppendLine();
            }

            sb.AppendLine("        // Marshalling map for the original C types:");
            sb.AppendLine("        //   AdiInt / AdiInt32 -> int, AdiUInt8* buffer -> byte[], PHS_* structure pointer -> IntPtr.");
            sb.AppendLine();
            sb.AppendLine("        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]");
            sb.AppendLine("        public static extern int Wrapper_ConvertToPhysical(int msgId, byte[] intfPtr);");
            sb.AppendLine();
            sb.AppendLine("        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]");
            sb.AppendLine("        public static extern void Wrapper_ConvertToInterface(int msgId, byte[] intfPtr);");
            sb.AppendLine();

            foreach (var msg in _parseResult.Messages)
            {
                if (!string.IsNullOrEmpty(msg.GlobalVariable))
                {
                    sb.AppendLine($"        /// <summary>Returns a pointer to the static {msg.PhysicalType} physical structure.</summary>");
                    sb.AppendLine("        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]");
                    sb.AppendLine($"        public static extern IntPtr Wrapper_Get_{msg.MessageName}();");
                    sb.AppendLine();
                }

                if (msg.HasToPhysical)
                {
                    var csRet = ToPhysicalReturnType(msg) == "void" ? "void" : "int";
                    sb.AppendLine($"        /// <summary>{msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr</summary>");
                    sb.AppendLine("        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]");
                    sb.AppendLine($"        public static extern {csRet} {msg.MessageName}_ConvertToPhysical(IntPtr phsPtr, byte[] intfPtr);");
                    sb.AppendLine();
                }

                if (msg.HasToInterface)
                {
                    var csRet = ToInterfaceReturnType(msg) == "void" ? "void" : "int";
                    sb.AppendLine($"        /// <summary>{msg.PhysicalType}* phsPtr, AdiUInt8* intfPtr</summary>");
                    sb.AppendLine("        [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]");
                    sb.AppendLine($"        public static extern {csRet} {msg.MessageName}_ConvertToInterface(IntPtr phsPtr, byte[] intfPtr);");
                    sb.AppendLine();
                }
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string BuildReadme()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {ProjectName}");
            sb.AppendLine();
            sb.AppendLine("This native C++ DLL project was generated automatically by **InterfaceWrapper**.");
            sb.AppendLine("It wraps the generated `cvi.c` / `cvp.c` convert functions behind a flat");
            sb.AppendLine("`extern \"C\"` ABI so they can be called from a C# project.");
            sb.AppendLine();
            sb.AppendLine("## Contents");
            sb.AppendLine();
            sb.AppendLine($"- `{ProjectName}.vcxproj` – native DLL project (build with MSVC / Visual Studio).");
            sb.AppendLine($"- `{ProjectName}.h` / `{ProjectName}.cpp` – exported wrapper functions.");
            sb.AppendLine("- `Messages/` – a self-contained copy of the parsed C source and headers.");
            sb.AppendLine("- `WrapperInterop.cs` – ready-to-use P/Invoke bindings for the C# side.");
            sb.AppendLine();
            sb.AppendLine("## Discovered messages");
            sb.AppendLine();
            sb.AppendLine("| Message | Message Id | Physical Type | Directions |");
            sb.AppendLine("|---------|-----------|---------------|------------|");
            foreach (var msg in _parseResult.Messages)
                sb.AppendLine($"| {msg.MessageName} | {msg.MessageIdDisplay} | {msg.PhysicalType} | {msg.Directions} |");
            sb.AppendLine();
            sb.AppendLine("## Usage from C#");
            sb.AppendLine();
            sb.AppendLine("```csharp");
            sb.AppendLine("// 1. Fill the static physical structure from a received ethernet buffer:");
            sb.AppendLine("WrapperInterop.Wrapper_ConvertToPhysical((int)WrapperInterop.MessageId.STATUS_MESSAGE, rxBuffer);");
            sb.AppendLine("IntPtr phys = WrapperInterop.Wrapper_Get_STATUS_MESSAGE();");
            sb.AppendLine("// Marshal 'phys' to a matching [StructLayout] struct to read the values.");
            sb.AppendLine();
            sb.AppendLine("// 2. Serialize the static physical structure into a buffer to send:");
            sb.AppendLine("WrapperInterop.Wrapper_ConvertToInterface((int)WrapperInterop.MessageId.STATUS_MESSAGE, txBuffer);");
            sb.AppendLine("```");
            return sb.ToString();
        }

        private static IEnumerable<(string Config, string Platform)> Configurations()
        {
            yield return ("Debug", "x64");
            yield return ("Release", "x64");
            yield return ("Debug", "Win32");
            yield return ("Release", "Win32");
        }

        private static string WriteFile(string path, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            return path;
        }
    }
}
