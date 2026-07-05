using System.Collections.Generic;

namespace InterfaceWrapper.Models
{
    /// <summary>
    /// Aggregates the convert functions (both directions) discovered for a single message
    /// together with the numeric message id extracted from the bus dispatcher.
    /// </summary>
    public sealed class MessageDefinition
    {
        /// <summary>The logical message name, e.g. STATUS_MESSAGE.</summary>
        public string MessageName { get; set; } = string.Empty;

        /// <summary>The physical structure type, e.g. PHS_STATUSMESSAGE.</summary>
        public string PhysicalType { get; set; } = string.Empty;

        /// <summary>The static physical variable, e.g. Phs_statusmessage.</summary>
        public string GlobalVariable { get; set; } = string.Empty;

        /// <summary>The numeric message id from the dispatcher switch, or null when unknown.</summary>
        public int? MessageId { get; set; }

        /// <summary>The convert-to-physical function, when present.</summary>
        public ConvertFunction? ToPhysical { get; set; }

        /// <summary>The convert-to-interface function, when present.</summary>
        public ConvertFunction? ToInterface { get; set; }

        /// <summary>The scalar fields (offset/type/size) discovered for the message layout.</summary>
        public List<MessageField> Fields { get; } = new();

        /// <summary>The total message length in bytes, derived from the field layout.</summary>
        public int Length { get; set; }

        public bool HasToPhysical => ToPhysical is not null;

        public bool HasToInterface => ToInterface is not null;

        /// <summary>A human readable summary of which directions were found.</summary>
        public string Directions
        {
            get
            {
                var parts = new List<string>();
                if (HasToPhysical) parts.Add("ToPhysical");
                if (HasToInterface) parts.Add("ToInterface");
                return parts.Count == 0 ? "-" : string.Join(" + ", parts);
            }
        }

        /// <summary>Display value for the message id column.</summary>
        public string MessageIdDisplay => MessageId?.ToString() ?? "?";
    }
}
