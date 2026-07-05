using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using InterfaceWrapper.Models;

namespace InterfaceWrapper.Services
{
    /// <summary>
    /// The result of parsing a folder of formatted C files.
    /// </summary>
    public sealed class ParseResult
    {
        public List<MessageDefinition> Messages { get; } = new();

        /// <summary>The bus name discovered from the dispatcher, e.g. IOCARD_LCU_PR.</summary>
        public string? BusName { get; set; }

        /// <summary>The system prefix used by the generated files, e.g. IOCARD.</summary>
        public string? SystemPrefix { get; set; }

        /// <summary>The header files (*.h) discovered in the folder, used by the generated project.</summary>
        public List<string> HeaderFiles { get; } = new();

        /// <summary>The implementation files (*.c / *.cpp) discovered in the folder.</summary>
        public List<string> SourceFiles { get; } = new();

        public List<string> Log { get; } = new();
    }

    /// <summary>
    /// Parses the known-formatted C files ("cvi.c" and "cvp.c") to discover the convert
    /// functions and the metadata required to build the C++ wrapper.
    /// </summary>
    public sealed class CFileParser
    {
        // e.g. AdiInt STATUS_MESSAGE_CONVERT_TO_PH(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
        // or   void  STATUS_MESSAGE_CONVERT_TO_IN(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
        private static readonly Regex ConvertFunctionRegex = new(
            @"(?<ret>\w[\w\s\*]*?)\s+(?<name>(?<msg>\w+?)_CONVERT_TO_(?<dir>PH|IN))\s*\(\s*(?<phys>\w+)\s*\*\s*\w+\s*,\s*\w+\s*\*\s*\w+\s*\)",
            RegexOptions.Compiled);

        // e.g. case 102: /* STATUS_MESSAGE */
        //          STATUS_MESSAGE_CONVERT_TO_IN(&Phs_statusmessage, MessageBuffer); break;
        private static readonly Regex DispatcherCaseRegex = new(
            @"case\s+(?<id>\d+)\s*:.*?(?<name>\w+_CONVERT_TO_(?:PH|IN))\s*\(\s*&\s*(?<var>\w+)\s*,",
            RegexOptions.Compiled | RegexOptions.Singleline);

        // e.g. void IOCARD_LCU_PR_ConvertToInterface(AdiInt MsgID, AdiUInt8* MessageBuffer)
        private static readonly Regex BusDispatcherRegex = new(
            @"(?<bus>\w+)_ConvertTo(?:Interface|Physical)\s*\(",
            RegexOptions.Compiled);

        // e.g. set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
        // The offset must be a plain integer immediately closed by ')', which naturally
        // skips array elements written as (intfPtr + 25 + locationOffset1).
        private static readonly Regex SetAlignFieldRegex = new(
            @"set_(?<fn>\w+)_align\s*\(\s*\(?\s*intfPtr\s*\+\s*(?<off>\d+)\s*\)?\s*,\s*\(\s*(?<cast>Adi\w+)\s*\)\s*\(\s*phsPtr->(?<field>[A-Za-z_][\w\.]*)\s*\)",
            RegexOptions.Compiled);

        // e.g. (*(AdiUInt8 *)(intfPtr + 24)) = (AdiUInt8)(phsPtr->DT_SET_DISC.discNum);
        private static readonly Regex DirectByteFieldRegex = new(
            @"\(\s*\*\s*\(\s*(?<bt>Adi\w+)\s*\*\s*\)\s*\(\s*intfPtr\s*\+\s*(?<off>\d+)\s*\)\s*\)\s*=\s*\(\s*Adi\w+\s*\)\s*\(\s*phsPtr->(?<field>[A-Za-z_][\w\.]*)\s*\)",
            RegexOptions.Compiled);

        /// <summary>Maps a <c>set_*_align</c> helper name to a UI type name and byte size.</summary>
        private static readonly Dictionary<string, (string Type, int Size)> AlignTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["ushort"] = ("UINT16", 2),
            ["short"] = ("INT16", 2),
            ["uint"] = ("UINT32", 4),
            ["int"] = ("INT32", 4),
            ["ulong"] = ("UINT32", 4),
            ["long"] = ("INT32", 4),
            ["uint64"] = ("UINT64", 8),
            ["int64"] = ("INT64", 8),
            ["float"] = ("FLOAT32", 4),
            ["double"] = ("FLOAT64", 8),
        };

        /// <summary>Maps a base C type used in a direct byte write to a UI type name and byte size.</summary>
        private static readonly Dictionary<string, (string Type, int Size)> ByteTypeMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AdiUInt8"] = ("UINT8", 1),
            ["AdiInt8"] = ("INT8", 1),
        };

        public ParseResult Parse(string messagesFolder)
        {
            if (string.IsNullOrWhiteSpace(messagesFolder))
                throw new ArgumentException("Messages folder path is required.", nameof(messagesFolder));
            if (!Directory.Exists(messagesFolder))
                throw new DirectoryNotFoundException($"Messages folder not found: {messagesFolder}");

            var result = new ParseResult();

            var cviFiles = Directory.GetFiles(messagesFolder, "*cvi.c", SearchOption.TopDirectoryOnly);
            var cvpFiles = Directory.GetFiles(messagesFolder, "*cvp.c", SearchOption.TopDirectoryOnly);

            result.Log.Add($"Found {cvpFiles.Length} convert-to-physical file(s) (cvp.c).");
            result.Log.Add($"Found {cviFiles.Length} convert-to-interface file(s) (cvi.c).");

            if (cviFiles.Length == 0 && cvpFiles.Length == 0)
                throw new InvalidOperationException(
                    "No 'cvi.c' or 'cvp.c' files were found in the selected folder.");

            // Collect all files that make up the generated project.
            foreach (var header in Directory.GetFiles(messagesFolder, "*.h", SearchOption.TopDirectoryOnly))
                result.HeaderFiles.Add(Path.GetFileName(header));
            foreach (var source in Directory.GetFiles(messagesFolder, "*.c", SearchOption.TopDirectoryOnly))
                result.SourceFiles.Add(Path.GetFileName(source));
            foreach (var source in Directory.GetFiles(messagesFolder, "*.cpp", SearchOption.TopDirectoryOnly))
                result.SourceFiles.Add(Path.GetFileName(source));

            var byMessage = new Dictionary<string, MessageDefinition>(StringComparer.Ordinal);

            ParseConvertFunctions(cvpFiles, ConvertDirection.ToPhysical, byMessage, result);
            ParseConvertFunctions(cviFiles, ConvertDirection.ToInterface, byMessage, result);

            // Extract message ids and the static physical variables from the dispatchers.
            ExtractDispatcherInfo(cvpFiles.Concat(cviFiles), byMessage, result);

            // Extract the field layout (offset/type/size) from the convert-to-interface bodies.
            ExtractMessageFields(cviFiles, byMessage, result);

            result.SystemPrefix ??= DeriveSystemPrefix(cvpFiles, cviFiles);

            result.Messages.AddRange(byMessage.Values.OrderBy(m => m.MessageName, StringComparer.Ordinal));
            result.Log.Add($"Discovered {result.Messages.Count} message(s).");

            return result;
        }

        private static void ParseConvertFunctions(
            IEnumerable<string> files,
            ConvertDirection direction,
            IDictionary<string, MessageDefinition> byMessage,
            ParseResult result)
        {
            var expectedSuffix = direction == ConvertDirection.ToPhysical ? "PH" : "IN";

            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                var count = 0;

                foreach (Match match in ConvertFunctionRegex.Matches(text))
                {
                    if (!string.Equals(match.Groups["dir"].Value, expectedSuffix, StringComparison.Ordinal))
                        continue;

                    var returnType = match.Groups["ret"].Value.Trim();
                    // Skip forward declarations that are actually prototypes inside headers only:
                    // the .c body definitions are what we want. Both parse identically here, so we
                    // rely on de-duplication by function name below.
                    var fn = new ConvertFunction
                    {
                        MessageName = match.Groups["msg"].Value,
                        FunctionName = match.Groups["name"].Value,
                        PhysicalType = match.Groups["phys"].Value,
                        ReturnType = returnType,
                        Direction = direction,
                        SourceFile = Path.GetFileName(file)
                    };

                    var def = GetOrCreate(byMessage, fn.MessageName);
                    def.PhysicalType = fn.PhysicalType;

                    if (direction == ConvertDirection.ToPhysical)
                        def.ToPhysical = fn;
                    else
                        def.ToInterface = fn;

                    count++;
                }

                result.Log.Add($"  {Path.GetFileName(file)}: parsed {count} {direction} function(s).");
            }
        }

        private void ExtractDispatcherInfo(
            IEnumerable<string> files,
            IDictionary<string, MessageDefinition> byMessage,
            ParseResult result)
        {
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);

                if (result.BusName is null)
                {
                    var busMatch = BusDispatcherRegex.Match(text);
                    if (busMatch.Success)
                    {
                        result.BusName = busMatch.Groups["bus"].Value;
                        result.Log.Add($"Bus name: {result.BusName}");
                    }
                }

                foreach (Match match in DispatcherCaseRegex.Matches(text))
                {
                    var functionName = match.Groups["name"].Value;
                    var messageName = functionName
                        .Replace("_CONVERT_TO_PH", string.Empty, StringComparison.Ordinal)
                        .Replace("_CONVERT_TO_IN", string.Empty, StringComparison.Ordinal);

                    if (!byMessage.TryGetValue(messageName, out var def))
                        continue;

                    if (int.TryParse(match.Groups["id"].Value, out var id))
                        def.MessageId ??= id;

                    var variable = match.Groups["var"].Value;
                    if (string.IsNullOrEmpty(def.GlobalVariable))
                        def.GlobalVariable = variable;

                    if (def.ToPhysical is not null && string.IsNullOrEmpty(def.ToPhysical.GlobalVariable))
                        def.ToPhysical.GlobalVariable = variable;
                    if (def.ToInterface is not null && string.IsNullOrEmpty(def.ToInterface.GlobalVariable))
                        def.ToInterface.GlobalVariable = variable;
                }
            }
        }

        private static MessageDefinition GetOrCreate(
            IDictionary<string, MessageDefinition> byMessage, string messageName)
        {
            if (!byMessage.TryGetValue(messageName, out var def))
            {
                def = new MessageDefinition { MessageName = messageName };
                byMessage[messageName] = def;
            }
            return def;
        }

        /// <summary>
        /// Extracts the constant-offset scalar field layout from the "cvi.c" bodies. Array
        /// elements (written with a locationOffset variable) are intentionally skipped so the
        /// generated UI shows the fixed header/tail/scalar fields.
        /// </summary>
        private static void ExtractMessageFields(
            IEnumerable<string> cviFiles,
            IDictionary<string, MessageDefinition> byMessage,
            ParseResult result)
        {
            foreach (var file in cviFiles)
            {
                var text = File.ReadAllText(file);

                foreach (Match fnMatch in ConvertFunctionRegex.Matches(text))
                {
                    if (!string.Equals(fnMatch.Groups["dir"].Value, "IN", StringComparison.Ordinal))
                        continue;

                    var messageName = fnMatch.Groups["msg"].Value;
                    if (!byMessage.TryGetValue(messageName, out var def))
                        continue;

                    var body = ExtractFunctionBody(text, fnMatch.Index + fnMatch.Length);
                    if (body is null)
                        continue;

                    ParseFieldsFromBody(body, def);
                }
            }

            foreach (var def in byMessage.Values)
            {
                if (def.Fields.Count == 0)
                    continue;

                def.Fields.Sort((a, b) => a.Offset.CompareTo(b.Offset));
                var last = def.Fields[^1];
                def.Length = last.Offset + last.Size;
                result.Log.Add($"  {def.MessageName}: {def.Fields.Count} field(s), length {def.Length} bytes.");
            }
        }

        private static void ParseFieldsFromBody(string body, MessageDefinition def)
        {
            var seen = new HashSet<int>();

            foreach (Match m in SetAlignFieldRegex.Matches(body))
            {
                if (!AlignTypeMap.TryGetValue(m.Groups["fn"].Value, out var info))
                    continue;
                AddField(def, seen, int.Parse(m.Groups["off"].Value), m.Groups["field"].Value, info.Type, info.Size);
            }

            foreach (Match m in DirectByteFieldRegex.Matches(body))
            {
                if (!ByteTypeMap.TryGetValue(m.Groups["bt"].Value, out var info))
                    info = ("UINT8", 1);
                AddField(def, seen, int.Parse(m.Groups["off"].Value), m.Groups["field"].Value, info.Type, info.Size);
            }
        }

        private static void AddField(
            MessageDefinition def, HashSet<int> seen, int offset, string field, string type, int size)
        {
            if (!seen.Add(offset))
                return;

            def.Fields.Add(new MessageField
            {
                Offset = offset,
                Field = field,
                Type = type,
                Size = size
            });
        }

        /// <summary>
        /// Returns the text of a function body starting at the opening brace that follows
        /// <paramref name="searchStart"/>, using brace matching to find the end.
        /// </summary>
        private static string? ExtractFunctionBody(string text, int searchStart)
        {
            var open = text.IndexOf('{', searchStart);
            if (open < 0)
                return null;

            var depth = 0;
            for (var i = open; i < text.Length; i++)
            {
                if (text[i] == '{')
                    depth++;
                else if (text[i] == '}')
                {
                    depth--;
                    if (depth == 0)
                        return text.Substring(open, i - open + 1);
                }
            }
            return null;
        }

        /// <summary>
        /// Derives the system prefix (e.g. "IOCARD") from a file name such as "IOCARDcvp.c".
        /// </summary>
        private static string DeriveSystemPrefix(string[] cvpFiles, string[] cviFiles)
        {
            var sample = cvpFiles.FirstOrDefault() ?? cviFiles.FirstOrDefault();
            if (sample is null)
                return "Messages";

            var name = Path.GetFileNameWithoutExtension(sample);
            foreach (var suffix in new[] { "cvp", "cvi" })
            {
                if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    return name[..^suffix.Length];
            }
            return name;
        }
    }
}
