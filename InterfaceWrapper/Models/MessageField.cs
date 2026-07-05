using System;

namespace InterfaceWrapper.Models
{
    /// <summary>
    /// A single scalar field discovered inside a <c>*_CONVERT_TO_IN</c> function body,
    /// e.g. <c>set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));</c>.
    /// Only constant-offset scalar fields are captured (array elements are skipped).
    /// </summary>
    public sealed class MessageField
    {
        /// <summary>The byte offset of the field inside the interface buffer.</summary>
        public int Offset { get; set; }

        /// <summary>The physical field path, e.g. HDR_IO2LCU.msgId.</summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>The UI friendly type name, e.g. UINT16, UINT32, FLOAT64.</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>The size of the field in bytes.</summary>
        public int Size { get; set; }

        /// <summary>True when this field is a CRC tail field (name ends with ".crc").</summary>
        public bool IsCrc => Field.EndsWith(".crc", StringComparison.OrdinalIgnoreCase);

        /// <summary>True when this field is a floating point value.</summary>
        public bool IsFloat => Type is "FLOAT32" or "FLOAT64";
    }
}
