using System.Collections.Generic;

namespace InterfaceWrapper.Models
{
    /// <summary>
    /// One element (sub-field) of an array, captured from a convert loop body such as
    /// <c>DT_SET_DISC.discretes[i1].discreteId</c>. The offset is relative to the first
    /// element (index 0) of the array.
    /// </summary>
    public sealed class MessageArrayElement
    {
        /// <summary>The byte offset of this sub-field inside a single array element.</summary>
        public int RelativeOffset { get; set; }

        /// <summary>The physical field path template, still containing the loop index, e.g. HDR[i1].id.</summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>The UI friendly type name, e.g. UINT8, UINT16, FLOAT32.</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>The size of the sub-field in bytes.</summary>
        public int Size { get; set; }
    }

    /// <summary>
    /// An array field discovered inside a convert loop, e.g.
    /// <c>for (i1 = 0; i1 &lt; 256 &amp;&amp; (i1 &lt; ...discNum); i1++, locationOffset1 += 2)</c>.
    /// Each cell starts at <see cref="BaseOffset"/> + index * <see cref="Stride"/> and contains
    /// every sub-field in <see cref="Elements"/>.
    /// </summary>
    public sealed class MessageArray
    {
        /// <summary>The array path without the index, e.g. DT_SET_DISC.discretes.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The byte offset of the first array element (index 0).</summary>
        public int BaseOffset { get; set; }

        /// <summary>The number of bytes between two consecutive array elements.</summary>
        public int Stride { get; set; }

        /// <summary>The maximum number of elements (the fixed loop bound).</summary>
        public int MaxCount { get; set; }

        /// <summary>The loop index variable, e.g. i1, used to resolve the per-cell field names.</summary>
        public string IndexVar { get; set; } = "i1";

        /// <summary>The physical field that carries the active element count, when present.</summary>
        public string? CountField { get; set; }

        /// <summary>The sub-fields contained in each array element.</summary>
        public List<MessageArrayElement> Elements { get; } = new();

        /// <summary>List display text for the arrays list.</summary>
        public string Display => $"{Name}  [max {MaxCount} \u00D7 {Stride} B]";
    }
}
