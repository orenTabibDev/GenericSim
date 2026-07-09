using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace GenericSim
{
    /// <summary>Static description of one scalar field in a message.</summary>
    public sealed class FieldInfo
    {
        public int Offset { get; init; }
        public string Field { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public int Size { get; init; }
        public string DefaultValue { get; init; } = "0";

        /// <summary>Reads this field from a raw little-endian interface buffer.</summary>
        public string Read(byte[] buffer)
        {
            if (Offset + Size > buffer.Length) return "-";
            return Type switch
            {
                "UINT8" => buffer[Offset].ToString(),
                "INT8" => ((sbyte)buffer[Offset]).ToString(),
                "UINT16" => BitConverter.ToUInt16(buffer, Offset).ToString(),
                "INT16" => BitConverter.ToInt16(buffer, Offset).ToString(),
                "UINT32" => BitConverter.ToUInt32(buffer, Offset).ToString(),
                "INT32" => BitConverter.ToInt32(buffer, Offset).ToString(),
                "UINT64" => BitConverter.ToUInt64(buffer, Offset).ToString(),
                "INT64" => BitConverter.ToInt64(buffer, Offset).ToString(),
                "FLOAT32" => BitConverter.ToSingle(buffer, Offset).ToString(CultureInfo.InvariantCulture),
                "FLOAT64" => BitConverter.ToDouble(buffer, Offset).ToString(CultureInfo.InvariantCulture),
                _ => "-"
            };
        }
    }

    /// <summary>Editable row bound to the fields DataGrid. A row is either a scalar field
    /// or an array header whose Cells expand into a nested table at the array's offset.</summary>
    public sealed class FieldRow : INotifyPropertyChanged
    {
        private readonly FieldInfo? _info;
        private readonly ArrayInfo? _array;
        private string _value = "0";
        private bool _isExpanded;
        private string _countText = "1";
        public FieldRow(FieldInfo info) { _info = info; }

        /// <summary>Creates an array header row with the given initial element count.</summary>
        public FieldRow(ArrayInfo array, int initialCount)
        {
            _array = array;
            RebuildCells(initialCount);
        }

        public bool IsArray => _array is not null;
        public ArrayInfo? ArrayDef => _array;
        /// <summary>The editable cells of the nested array table (element x sub-field).</summary>
        public ObservableCollection<ArrayCellRow> Cells { get; } = new();

        public int Offset => _array?.BaseOffset ?? _info!.Offset;
        public string Field => _array?.Name ?? _info!.Field;
        public string Type => _array is null ? _info!.Type : "ARRAY";
        public int Size => _array?.Stride ?? _info!.Size;

        public string Value
        {
            get => _array is null ? _value : $"[{CellElementCount} element(s)] click to open / close the cells";
            set { if (_array is not null) return; _value = value; OnChanged(nameof(Value)); }
        }

        private int CellElementCount => _array is null || _array.Elements.Length == 0 ? 0 : Cells.Count / _array.Elements.Length;

        /// <summary>Expands / collapses the nested cells table under this array row.</summary>
        public bool IsExpanded { get => _isExpanded; set { _isExpanded = value; OnChanged(nameof(IsExpanded)); OnChanged(nameof(ExpanderGlyph)); } }
        public string CountText { get => _countText; set { _countText = value; OnChanged(nameof(CountText)); } }
        public string ExpanderGlyph => _isExpanded ? "\u25BC" : "\u25B6";
        public System.Windows.Visibility ExpanderVisibility => IsArray ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public string ArraySummary => _array is null ? string.Empty
            : $"stride {_array.Stride} B, max {_array.MaxCount}, {_array.Elements.Length} sub-field(s)";

        /// <summary>Rebuilds one cell per (element x sub-field), clamped to the array maximum.</summary>
        public int RebuildCells(int count)
        {
            if (_array is null) return 0;
            count = Math.Clamp(count, 0, _array.MaxCount);
            Cells.Clear();
            for (int i = 0; i < count; i++)
                foreach (var el in _array.Elements)
                    Cells.Add(new ArrayCellRow
                    {
                        Index = i,
                        Offset = _array.BaseOffset + i * _array.Stride + el.RelativeOffset,
                        Field = el.Field.Replace($"[{_array.IndexVar}]", $"[{i}]", StringComparison.Ordinal),
                        Type = el.Type,
                        Value = "0"
                    });
            _countText = count.ToString();
            OnChanged(nameof(CountText));
            OnChanged(nameof(Value));
            return count;
        }

        /// <summary>Writes the edited value(s) straight into the interface buffer image.</summary>
        public void WriteToBuffer(byte[] buffer)
        {
            if (_array is not null)
            {
                // Array header row: write every nested cell at its own offset.
                foreach (var cell in Cells) cell.WriteToBuffer(buffer);
                return;
            }
            if (_info is null || _info.Offset + _info.Size > buffer.Length) return;
            var ci = CultureInfo.InvariantCulture;
            switch (_info.Type)
            {
                case "UINT8": buffer[_info.Offset] = byte.TryParse(_value, out var b) ? b : (byte)0; break;
                case "INT8": buffer[_info.Offset] = unchecked((byte)(sbyte.TryParse(_value, out var sb8) ? sb8 : 0)); break;
                case "UINT16": WriteBytes(buffer, BitConverter.GetBytes(ushort.TryParse(_value, out var u16) ? u16 : (ushort)0)); break;
                case "INT16": WriteBytes(buffer, BitConverter.GetBytes(short.TryParse(_value, out var i16) ? i16 : (short)0)); break;
                case "UINT32": WriteBytes(buffer, BitConverter.GetBytes(uint.TryParse(_value, out var u32) ? u32 : 0u)); break;
                case "INT32": WriteBytes(buffer, BitConverter.GetBytes(int.TryParse(_value, out var i32) ? i32 : 0)); break;
                case "UINT64": WriteBytes(buffer, BitConverter.GetBytes(ulong.TryParse(_value, out var u64) ? u64 : 0ul)); break;
                case "INT64": WriteBytes(buffer, BitConverter.GetBytes(long.TryParse(_value, out var i64) ? i64 : 0L)); break;
                case "FLOAT32": WriteBytes(buffer, BitConverter.GetBytes(float.TryParse(_value, NumberStyles.Float, ci, out var f) ? f : 0f)); break;
                case "FLOAT64": WriteBytes(buffer, BitConverter.GetBytes(double.TryParse(_value, NumberStyles.Float, ci, out var d) ? d : 0d)); break;
            }
        }

        private void WriteBytes(byte[] buffer, byte[] value)
        {
            if (_info is null) return;
            System.Array.Copy(value, 0, buffer, _info.Offset, Math.Min(value.Length, _info.Size));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public sealed class HistoryRow
    {
        public string Time { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Seq { get; set; }
        public int Bytes { get; set; }
        public string Periodic { get; set; } = "No";
    }

    public sealed class MonitorRow
    {
        public string Time { get; set; } = string.Empty;
        public string From { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Bytes { get; set; }
    }

    public sealed class ReceivedField
    {
        public int Offset { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>One point on the Graph tab timeline (a sent or received message).</summary>
    public sealed class TimelineEvent
    {
        public DateTime Timestamp { get; init; }
        public bool IsOutgoing { get; init; }
        public string Message { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string Details { get; init; } = string.Empty;
        /// <summary>The raw message bytes, used to decode the fields when the point is clicked.</summary>
        public byte[] Data { get; init; } = Array.Empty<byte>();
        public string Time => Timestamp.ToString("HH:mm:ss.fff");
        public string Direction => IsOutgoing ? "MBT \u2192 NIRON" : "NIRON \u2192 MBT";
    }

    /// <summary>Running statistics for one message type (Statistics tab).</summary>
    public sealed class MessageStat
    {
        public string Name { get; init; } = string.Empty;
        public long Received;
        public long Sent;
        public long TotalBytes;
        public long BytesSent;
        public long BytesReceived;
        public long CrcErrors;
        public long Errors;
        public long Dropped;
        public int MinSize = int.MaxValue;
        public int MaxSize;
        public DateTime? LastReceived;
        public DateTime? LastSent;
        /// <summary>Messages/bytes since the last statistics tick (for the 1 s rates).</summary>
        public long WindowCount;
        public long WindowBytes;
        public double MsgPerSec;
        public double BytesPerSec;
    }

    /// <summary>Row shown in the Statistics tab grid.</summary>
    public sealed class MessageStatRow
    {
        public string Message { get; set; } = string.Empty;
        public long Received { get; set; }
        public long Sent { get; set; }
        public string MsgPerSec { get; set; } = "0";
        public string BytesPerSec { get; set; } = "0";
        public long TotalBytes { get; set; }
        public string AvgSize { get; set; } = "-";
        public string MinSize { get; set; } = "-";
        public string MaxSize { get; set; } = "-";
        public string LastReceived { get; set; } = "-";
        public string LastSent { get; set; } = "-";
        public string SinceLast { get; set; } = "-";
        public long Errors { get; set; }
        public long Dropped { get; set; }
    }

    /// <summary>One row of the Global Statistics table (Statistics tab).</summary>
    public sealed class GlobalStatRow
    {
        public GlobalStatRow(string statistic, string value) { Statistic = statistic; Value = value; }
        public string Statistic { get; }
        public string Value { get; }
    }

    /// <summary>One sub-field of an array element (Arrays editor).</summary>
    public sealed class ArrayElementInfo
    {
        public int RelativeOffset { get; init; }
        public string Field { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public int Size { get; init; }
    }

    /// <summary>An array field discovered in a convert loop (repeated element block).</summary>
    public sealed class ArrayInfo
    {
        public string Name { get; init; } = string.Empty;
        public int BaseOffset { get; init; }
        public int Stride { get; init; }
        public int MaxCount { get; init; }
        public string IndexVar { get; init; } = "i1";
        public string? CountField { get; init; }
        public ArrayElementInfo[] Elements { get; init; } = Array.Empty<ArrayElementInfo>();
        public string Display => $"{Name}  [max {MaxCount} x {Stride} B]";
    }

    /// <summary>One editable array cell (a single sub-field of a single element).</summary>
    public sealed class ArrayCellRow : INotifyPropertyChanged
    {
        private string _value = "0";
        public int Index { get; set; }
        public string Field { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Offset { get; set; }
        public string Value { get => _value; set { _value = value; OnChanged(nameof(Value)); } }

        /// <summary>Writes this cell's value into the interface buffer at its absolute offset.</summary>
        public void WriteToBuffer(byte[] buffer)
        {
            var ci = CultureInfo.InvariantCulture;
            switch (Type)
            {
                case "UINT8": if (Offset + 1 <= buffer.Length) buffer[Offset] = byte.TryParse(_value, out var b) ? b : (byte)0; break;
                case "INT8": if (Offset + 1 <= buffer.Length) buffer[Offset] = unchecked((byte)(sbyte.TryParse(_value, out var s8) ? s8 : 0)); break;
                case "UINT16": Write(buffer, BitConverter.GetBytes(ushort.TryParse(_value, out var u16) ? u16 : (ushort)0)); break;
                case "INT16": Write(buffer, BitConverter.GetBytes(short.TryParse(_value, out var i16) ? i16 : (short)0)); break;
                case "UINT32": Write(buffer, BitConverter.GetBytes(uint.TryParse(_value, out var u32) ? u32 : 0u)); break;
                case "INT32": Write(buffer, BitConverter.GetBytes(int.TryParse(_value, out var i32) ? i32 : 0)); break;
                case "UINT64": Write(buffer, BitConverter.GetBytes(ulong.TryParse(_value, out var u64) ? u64 : 0ul)); break;
                case "INT64": Write(buffer, BitConverter.GetBytes(long.TryParse(_value, out var i64) ? i64 : 0L)); break;
                case "FLOAT32": Write(buffer, BitConverter.GetBytes(float.TryParse(_value, NumberStyles.Float, ci, out var f) ? f : 0f)); break;
                case "FLOAT64": Write(buffer, BitConverter.GetBytes(double.TryParse(_value, NumberStyles.Float, ci, out var d) ? d : 0d)); break;
            }
        }

        private void Write(byte[] buffer, byte[] value)
        {
            if (Offset + value.Length <= buffer.Length)
                Array.Copy(value, 0, buffer, Offset, value.Length);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    /// <summary>An armed scenario Response rule: when the trigger message is received
    /// and its field equals the expected value, the Send message is sent.</summary>
    public sealed class ResponseRule
    {
        public string TriggerName { get; init; } = string.Empty;
        public string Field { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
        public MessageInfo Send { get; init; } = null!;
        public IReadOnlyList<FieldRow> SendFields { get; init; } = Array.Empty<FieldRow>();
        public IReadOnlyList<ArrayCellRow> SendArrayCells { get; init; } = Array.Empty<ArrayCellRow>();
        public int StepIndex { get; init; }
    }

    /// <summary>The kind of node shown on the Flow designer canvas.</summary>
    public enum FlowNodeKind { Message, Condition, Delay, Loop }

    /// <summary>The branch a connection represents: a plain next link, or the True /
    /// False outcome of a Condition node.</summary>
    public enum FlowBranch { Normal, True, False }

    /// <summary>One node on the Flow designer: a Message (rectangle), a Condition
    /// (rhombus), a Delay or a Loop, with its position, size and edited properties.</summary>
    public sealed class FlowNodeVisual
    {
        public FlowNodeKind Kind { get; init; }
        /// <summary>Stable id used to label the node in combos and the steps table.</summary>
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width => Kind == FlowNodeKind.Condition ? 160 : 140;
        public double Height => Kind == FlowNodeKind.Condition ? 64 : 52;
        public double CenterX => Left + Width / 2;
        public double Bottom => Top + Height;
        // Condition properties.
        public string WaitMessage { get; set; } = string.Empty;
        public int TimeoutMs { get; set; } = 5000;
        /// <summary>When true the condition always takes the True branch (timeout ignored).</summary>
        public bool IgnoreTimeout { get; set; }
        /// <summary>Optional logical test on a field of the awaited message, e.g.
        /// Field1 >= 5. An empty ConditionField means 'just wait for the message'.</summary>
        public string ConditionField { get; set; } = string.Empty;
        public string ConditionOperator { get; set; } = ">=";
        public string ConditionValue { get; set; } = string.Empty;
        /// <summary>Messages to send when the condition is True / False (empty = none).</summary>
        public string TrueSendMessage { get; set; } = string.Empty;
        public string FalseSendMessage { get; set; } = string.Empty;
        // Delay / Loop properties.
        public int DelayMs { get; set; } = 1000;
        public int LoopCount { get; set; } = 3;
        /// <summary>Message the Loop sends on each iteration (empty = none).</summary>
        public string LoopSendMessage { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        /// <summary>The edited field values this Message node sends (field path -> value,
        /// plus "name#count" entries for arrays), like a scenario step.</summary>
        public Dictionary<string, string>? FieldValues { get; set; }

        /// <summary>Display label for combos and the steps table, e.g. "#3 STATUS_MESSAGE".</summary>
        public string Label => Kind == FlowNodeKind.Message ? $"#{Id} {Title}" : $"#{Id} {Kind}";

        /// <summary>The multi-line caption drawn inside the node shape.</summary>
        public string Caption => Kind switch
        {
            FlowNodeKind.Message => Title,
            FlowNodeKind.Condition => string.IsNullOrWhiteSpace(ConditionField)
                ? $"Condition\nWait: {WaitMessage}\nTimeout: {TimeoutMs} ms"
                : $"Condition\nWait: {WaitMessage}\n{ConditionField} {ConditionOperator} {ConditionValue}",
            FlowNodeKind.Delay => $"Delay\n{DelayMs} ms",
            _ => string.IsNullOrWhiteSpace(LoopSendMessage)
                ? $"Loop\nRepeat {LoopCount} times"
                : $"Loop\nRepeat {LoopCount} times\nSend: {LoopSendMessage}"
        };
    }

    /// <summary>A directed connection between two Flow nodes, carrying the branch it
    /// represents (plain next link, or a Condition's True / False outcome).</summary>
    public sealed class FlowConnection
    {
        public FlowNodeVisual From { get; init; } = null!;
        public FlowNodeVisual To { get; init; } = null!;
        public FlowBranch Branch { get; set; } = FlowBranch.Normal;
    }

    /// <summary>One row of the Flow tab's bottom Scenario Steps table.</summary>
    public sealed class FlowStepRow
    {
        public int Index { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string Next { get; set; } = string.Empty;
    }
}
