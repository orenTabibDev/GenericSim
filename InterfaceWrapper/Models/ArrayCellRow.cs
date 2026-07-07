using System.ComponentModel;

namespace InterfaceWrapper.Models
{
    /// <summary>
    /// One editable array cell shown in the UI: a single sub-field of a single array element.
    /// The <see cref="Value"/> is editable so the developer can update each cell of the array.
    /// </summary>
    public sealed class ArrayCellRow : INotifyPropertyChanged
    {
        private string _value = "0";

        /// <summary>The element index inside the array (0-based).</summary>
        public int Index { get; set; }

        /// <summary>The resolved field path for this cell, e.g. DT_SET_DISC.discretes[3].discreteId.</summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>The UI friendly type name, e.g. UINT8.</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>The absolute byte offset of this cell inside the interface buffer.</summary>
        public int Offset { get; set; }

        /// <summary>The editable value of this cell.</summary>
        public string Value
        {
            get => _value;
            set { _value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
