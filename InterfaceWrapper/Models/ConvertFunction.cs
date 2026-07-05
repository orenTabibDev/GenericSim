namespace InterfaceWrapper.Models
{
    /// <summary>
    /// Direction of a generated convert function.
    /// </summary>
    public enum ConvertDirection
    {
        /// <summary>Convert from the ethernet buffer to the physical structure (prefix "cvp.c", suffix _CONVERT_TO_PH).</summary>
        ToPhysical,

        /// <summary>Convert from the physical structure to the ethernet buffer (prefix "cvi.c", suffix _CONVERT_TO_IN).</summary>
        ToInterface
    }

    /// <summary>
    /// Represents a single convert function discovered in a "cvi.c" or "cvp.c" file.
    /// e.g. <c>AdiInt STATUS_MESSAGE_CONVERT_TO_PH(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)</c>
    /// </summary>
    public sealed class ConvertFunction
    {
        /// <summary>The logical message name, e.g. STATUS_MESSAGE.</summary>
        public string MessageName { get; set; } = string.Empty;

        /// <summary>The full C function name, e.g. STATUS_MESSAGE_CONVERT_TO_PH.</summary>
        public string FunctionName { get; set; } = string.Empty;

        /// <summary>The physical structure type, e.g. PHS_STATUSMESSAGE.</summary>
        public string PhysicalType { get; set; } = string.Empty;

        /// <summary>The C return type of the function (AdiInt or void).</summary>
        public string ReturnType { get; set; } = string.Empty;

        /// <summary>The static physical variable used by the dispatcher, e.g. Phs_statusmessage.</summary>
        public string GlobalVariable { get; set; } = string.Empty;

        /// <summary>The direction of conversion.</summary>
        public ConvertDirection Direction { get; set; }

        /// <summary>The source file the function was parsed from.</summary>
        public string SourceFile { get; set; } = string.Empty;
    }
}
