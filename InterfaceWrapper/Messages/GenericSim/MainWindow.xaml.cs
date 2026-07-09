using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WrapperFromCToSharp;

namespace GenericSim
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<FieldRow> _fields = new();
        private readonly ObservableCollection<HistoryRow> _history = new();
        private readonly ObservableCollection<MonitorRow> _monitor = new();
        private readonly ObservableCollection<ReceivedField> _receivedFields = new();
        // Periodic sends run on PrecisionTimer threads (1 ms resolution, drift-free),
        // never on the UI thread, so a 10 ms cadence stays accurate while the UI works.
        private readonly Dictionary<string, PrecisionTimer> _periodicTimers = new();
        private readonly ObservableCollection<ScenarioStepRow> _scenarioSteps = new();
        private readonly List<DispatcherTimer> _scenarioTimers = new();
        private readonly List<PrecisionTimer> _scenarioPrecisionTimers = new();
        // Armed Response steps: checked against every received message while the scenario runs.
        private readonly List<ResponseRule> _activeResponses = new();
        private bool _loadingResponseUi;
        // The native wrapper writes into static physical structures; serialize access across threads.
        private static readonly object _nativeLock = new();
        private readonly ObservableCollection<FieldRow> _stepFields = new();
        private ScenarioStepRow? _editingStep;
        private string? _scenarioFolder;
        private volatile bool _scenarioRunning;
        private Point _dragStartPoint;
        private readonly MessageRecorder _recorder = new();
        private List<RecordedMessage> _loadedRecords = new();
        private readonly List<TimelineEvent> _timelineEvents = new();
        private readonly Dictionary<string, int> _messageCounts = new();
        private double _graphZoom = 1.0;
        private bool _graphPaused;
        // Set when traffic arrives; the graph is redrawn by _graphRefreshTimer instead of on every packet.
        private bool _timelineDirty;
        // Coalesces bursts of traffic into a few UI repaints per second to keep the UI responsive.
        private readonly DispatcherTimer _graphRefreshTimer = new() { Interval = TimeSpan.FromMilliseconds(300) };
        private readonly Dictionary<string, MessageStat> _messageStats = new();
        private DateTime _lastStatsTick = DateTime.Now;
        // Statistics refresh is intentionally separate from message receive/send cadence.
        private readonly DispatcherTimer _statsTimer = new() { Interval = TimeSpan.FromSeconds(1) };
        private readonly DateTime _simStartTime = DateTime.Now;
        private double _peakMsgRate;
        private double _peakByteRate;
        private long _totalProcessingTicks;
        private long _processedMessages;
        private long _timeoutCount;
        private long _retransmissionCount;
        private TimeSpan _lastCpuTime = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;
        private readonly List<TimelineEvent> _recordGraphEvents = new();
        private double _recordGraphZoom = 1.0;
        // Flow tab: visual designer nodes / connections and interaction state.
        private readonly List<FlowNodeVisual> _flowNodes = new();
        private readonly List<FlowConnection> _flowConnections = new();
        private FlowNodeVisual? _selectedFlowNode;
        private FlowNodeVisual? _movingFlowNode;
        private FlowNodeVisual? _flowConnectStart;
        private Point _flowMoveOffset;
        private bool _loadingFlowProps;
        private int _flowNodeSeq;
        private double _flowZoom = 1.0;
        // Flow tab: editable fields of the selected Message node (shown in Step Properties).
        private readonly ObservableCollection<FieldRow> _flowFields = new();
        // Flow tab: the bottom Scenario Steps table rows.
        private readonly ObservableCollection<FlowStepRow> _flowStepRows = new();
        // Flow tab: execution state (the send order follows the connection arrows).
        private FlowNodeVisual? _runningFlowNode;
        private readonly Dictionary<FlowNodeVisual, int> _flowOrder = new();
        private volatile bool _flowRunning;
        private System.Threading.CancellationTokenSource? _flowRunCts;
        private TaskCompletionSource<byte[]?>? _flowWaitTcs;
        private string? _flowWaitMessage;
        private ITransport? _transport;
        private int _sequence;

        public MainWindow()
        {
            InitializeComponent();
            FieldsGrid.ItemsSource = _fields;
            HistoryGrid.ItemsSource = _history;
            MonitorGrid.ItemsSource = _monitor;
            ReceivedFieldsGrid.ItemsSource = _receivedFields;
            ScenarioStepsList.ItemsSource = _scenarioSteps;
            StepFieldsGrid.ItemsSource = _stepFields;
            FlowFieldsGrid.ItemsSource = _flowFields;
            FlowStepsGrid.ItemsSource = _flowStepRows;
            foreach (var m in MessageCatalog.Messages)
            {
                MessageList.Items.Add(m.Name);
                AllMessagesList.Items.Add(m.Name);
                AddGraphFilter(m.Name);
                ResponseTriggerCombo.Items.Add(m.Name);
                ResponseSendCombo.Items.Add(m.Name);
                FlowMessagesList.Items.Add(m.Name);
                FlowWaitMessageCombo.Items.Add(m.Name);
            }
            if (MessageList.Items.Count > 0)
                MessageList.SelectedIndex = 0;
            foreach (var port in System.IO.Ports.SerialPort.GetPortNames())
                ComPortBox.Items.Add(port);
            if (ComPortBox.Items.Count > 0) ComPortBox.SelectedIndex = 0;
            // Repaint the timeline when the Graph tab becomes visible.
            TimelineCanvas.IsVisibleChanged += (_, _) => { if (TimelineCanvas.IsVisible) RedrawTimeline(); };
            RecordGraphCanvas.IsVisibleChanged += (_, _) => { if (RecordGraphCanvas.IsVisible) RedrawRecordTimeline(); };
            // Coalesced repaint: heavy traffic only marks the timeline dirty and this timer
            // repaints at most ~3 times per second, so bursts of messages cannot freeze the UI.
            _graphRefreshTimer.Tick += (_, _) =>
            {
                if (_timelineDirty && TimelineCanvas.IsVisible && !_graphPaused) RedrawTimeline();
            };
            _graphRefreshTimer.Start();
            // Per-message statistics (rates recomputed every second).
            _statsTimer.Tick += (_, _) => UpdateStatistics();
            _statsTimer.Start();
            // 1 ms Windows timer resolution so 10 ms periodic sends fire on time.
            HighResClock.Enable();
            Closed += (_, _) =>
            {
                foreach (var timer in _periodicTimers.Values) timer.Dispose();
                _periodicTimers.Clear();
                StopScenarioTimers();
                StopTransport();
                HighResClock.Disable();
            };
            Log("Application ready. Simulating MBT side.");
        }

        private MessageInfo? Current =>
            MessageList.SelectedItem is string name ? MessageCatalog.ByName(name) : null;

        private void MessageList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var info = Current;
            _fields.Clear();
            if (info is null) return;
            SelectedMessageHeader.Text = info.Name;
            MessageInfoText.Text = $"Message Id: {info.MessageId}   Length: {info.Length} bytes";
            // Merge the scalar fields and the array rows ordered by offset, so each array
            // appears as a nested (collapsible) table at its correct offset position.
            var arrays = new Queue<FieldRow>(info.Arrays.OrderBy(a => a.BaseOffset).Select(a => new FieldRow(a, Math.Min(1, a.MaxCount))));
            foreach (var f in info.Fields)
            {
                while (arrays.Count > 0 && arrays.Peek().Offset <= f.Offset)
                    _fields.Add(arrays.Dequeue());
                _fields.Add(new FieldRow(f) { Value = f.DefaultValue });
            }
            while (arrays.Count > 0)
                _fields.Add(arrays.Dequeue());
            RefreshHex();
        }

        // ---- Arrays: nested per-cell editing inside the fields grid ----
        /// <summary>Clicking anywhere on an array row opens / closes its nested cells table.
        /// Attached as PreviewMouseLeftButtonUp on the whole grid so it always fires, and the
        /// row container's DetailsVisibility is set directly (a local value the DataGrid must
        /// honor; a RowStyle trigger can be overridden by the grid's own coercion).
        /// Clicks inside the expanded details are ignored so editing never collapses it.</summary>
        private void ArrayRow_ToggleOnClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;
            while (element is not null && element is not System.Windows.Controls.DataGridRow)
            {
                if (element is System.Windows.Controls.Primitives.DataGridDetailsPresenter)
                    return; // click inside the nested cells table
                element = element is System.Windows.Media.Visual || element is System.Windows.Media.Media3D.Visual3D
                    ? System.Windows.Media.VisualTreeHelper.GetParent(element)
                    : LogicalTreeHelper.GetParent(element);
            }
            if (element is System.Windows.Controls.DataGridRow { DataContext: FieldRow { IsArray: true } row } container)
            {
                row.IsExpanded = !row.IsExpanded;
                container.DetailsVisibility = row.IsExpanded ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>Restores an array row's open state whenever its container is (re)created
        /// (initial load, scrolling / virtualization, ItemsSource refresh).</summary>
        private void ArrayRow_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            e.Row.DetailsVisibility = e.Row.DataContext is FieldRow { IsArray: true, IsExpanded: true }
                ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Apply button inside an array row's nested table: rebuilds the cells to
        /// the requested element count and syncs the array's count field.</summary>
        private void ApplyArrayCount_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not FieldRow { ArrayDef: { } array } row) return;
            var applied = row.RebuildCells(int.TryParse(row.CountText, out var n) ? n : 0);
            // Reflect the element count in the array's count field so the receiver knows the length.
            if (array.CountField is not null)
                foreach (var f in _fields)
                    if (!f.IsArray && f.Field == array.CountField) f.Value = applied.ToString();
            RefreshHex();
        }

        /// <summary>Independent copy of all array cells (from the nested tables) for a background send.</summary>
        private List<ArrayCellRow> SnapshotArrayCells()
        {
            var list = new List<ArrayCellRow>();
            foreach (var row in _fields)
                if (row.IsArray)
                    foreach (var c in row.Cells)
                        list.Add(new ArrayCellRow { Index = c.Index, Offset = c.Offset, Field = c.Field, Type = c.Type, Value = c.Value });
            return list;
        }

        // ---- Buffer building via the native wrapper ----
        private byte[] BuildBuffer(MessageInfo info, IReadOnlyList<FieldRow> fields) =>
            BuildBufferCore(info, fields, SnapshotArrayCells(), _sequence, AutoSequenceCheck.IsChecked == true,
                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);

        /// <summary>Thread-safe buffer build (no UI access) usable from PrecisionTimer threads.
        /// Native convert calls are serialized because the wrapper uses static structures.</summary>
        private static byte[] BuildBufferCore(MessageInfo info, IReadOnlyList<FieldRow> fields, IReadOnlyList<ArrayCellRow>? arrayCells, int seq, bool autoSeq, bool autoTs, bool autoCrc)
        {
            var buffer = new byte[Math.Max(info.Length, 1)];
            IntPtr phys = info.GetPhysical();

            if (autoSeq) SetFieldBySuffix(fields, "SeqNum", seq);
            if (autoTs) SetFieldBySuffix(fields, "timestamp", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);

            // Establish the base layout (arrays, fixed structure) through the native
            // convert function, then overlay the edited scalar values onto the buffer.
            lock (_nativeLock)
            {
                if (phys != IntPtr.Zero)
                    info.ConvertToInterface(buffer, phys);
            }

            foreach (var row in fields)
                row.WriteToBuffer(buffer);

            // Overlay the editable array cells after the scalar fields, before the CRC.
            if (arrayCells is not null)
                foreach (var cell in arrayCells)
                    cell.WriteToBuffer(buffer);

            if (autoCrc)
                ApplyCrc(info, buffer);

            return buffer;
        }

        /// <summary>Writes a simple additive checksum into the message CRC field, when present.</summary>
        private static void ApplyCrc(MessageInfo info, byte[] buffer)
        {
            var crc = info.Fields.FirstOrDefault(f => f.Field.EndsWith(".crc", StringComparison.OrdinalIgnoreCase));
            if (crc is null || crc.Offset + 4 > buffer.Length) return;
            uint sum = 0;
            for (int i = 0; i < crc.Offset; i++) sum += buffer[i];
            Array.Copy(BitConverter.GetBytes(sum), 0, buffer, crc.Offset, 4);
        }

        private static void SetFieldBySuffix(IReadOnlyList<FieldRow> fields, string suffix, double value)
        {
            foreach (var row in fields)
                if (row.Field.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                    row.Value = value.ToString(CultureInfo.InvariantCulture);
        }

        // ---- Sending ----
        private void SendSelected_Click(object sender, RoutedEventArgs e) => SendCurrent(false);

        private void SendCurrent(bool periodic)
        {
            var info = Current;
            if (info is null) return;
            SendMessage(info, _fields.ToList(), periodic);
        }

        /// <summary>UI-thread send used by buttons and scenario starts; snapshots the auto
        /// flags then delegates to the thread-safe core.</summary>
        private void SendMessage(MessageInfo info, IReadOnlyList<FieldRow> fields, bool periodic)
        {
            if (_transport is null) { Log("Transport is not started."); return; }
            // Include the edited array cells only when sending the message shown in the editor.
            var arrayCells = ReferenceEquals(info, Current) ? SnapshotArrayCells() : null;
            SendMessageCore(info, fields, arrayCells, periodic, AutoSequenceCheck.IsChecked == true,
                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);
        }

        /// <summary>Builds and sends on the calling thread (safe for PrecisionTimer threads).
        /// UI bookkeeping is queued with InvokeAsync so it never delays the 10 ms cadence.</summary>
        private void SendMessageCore(MessageInfo info, IReadOnlyList<FieldRow> fields, IReadOnlyList<ArrayCellRow>? arrayCells, bool periodic, bool autoSeq, bool autoTs, bool autoCrc)
        {
            var transport = _transport;
            if (transport is null) return;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                var seq = Interlocked.Increment(ref _sequence) - 1;
                var buffer = BuildBufferCore(info, fields, arrayCells, seq, autoSeq, autoTs, autoCrc);
                transport.Send(buffer);
                _recorder.Record(true, info.MessageId, buffer);
                sw.Stop();
                Interlocked.Add(ref _totalProcessingTicks, sw.ElapsedTicks);
                Interlocked.Increment(ref _processedMessages);
                var time = DateTime.Now;
                Dispatcher.InvokeAsync(() =>
                {
                    RecordTimelineEvent(true, info.Name, "SENT", periodic ? "Periodic" : "One time", buffer);
                    UpdateStat(info.Name, buffer.Length, outgoing: true);
                    if (periodic) _retransmissionCount++;
                    _history.Insert(0, new HistoryRow
                    {
                        Time = time.ToString("HH:mm:ss.fff"),
                        Direction = "TX", Message = info.Name, Seq = seq,
                        Bytes = buffer.Length, Periodic = periodic ? "Yes" : "No"
                    });
                    if (_history.Count > 500) _history.RemoveAt(_history.Count - 1);
                    // Per-send log and HEX refresh are skipped for periodic sends so fast rates stay smooth.
                    if (!periodic)
                    {
                        Log($"{info.Name}: sent {buffer.Length} bytes (seq {seq}).");
                        if (ReferenceEquals(info, Current)) RefreshHex();
                    }
                });
            }
            catch (Exception ex) { Dispatcher.InvokeAsync(() => Log("Send error: " + ex.Message)); }
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            _history.Clear();
            Log("Send history cleared.");
        }

        // ---- Periodic (PrecisionTimer threads: accurate down to a few ms, e.g. 10 ms) ----
        private void StartPeriodic_Click(object sender, RoutedEventArgs e)
        {
            var info = Current;
            if (info is null) return;
            StopPeriodic(info.Name);
            var interval = double.TryParse(IntervalBox.Text, out var ms) ? Math.Max(1, ms) : 1000;
            var howMany = int.TryParse(HowManyBox.Text, out var n) ? n : 0;

            // Snapshot the fields and auto flags so the timer thread never touches the UI;
            // the message keeps sending independently of the current selection.
            var snapshot = SnapshotFields(info);
            var arraySnapshot = SnapshotArrayCells();
            bool autoSeq = AutoSequenceCheck.IsChecked == true;
            bool autoTs = AutoTimestampCheck.IsChecked == true;
            bool autoCrc = AutoCrcCheck.IsChecked == true;
            var sent = 0;
            var timer = new PrecisionTimer(interval, () =>
            {
                var i = Interlocked.Increment(ref sent);
                if (howMany > 0 && i > howMany) return; // count reached, disposal is on its way
                SendMessageCore(info, snapshot, arraySnapshot, true, autoSeq, autoTs, autoCrc);
                if (howMany > 0 && i == howMany)
                    Dispatcher.InvokeAsync(() => StopPeriodic(info.Name));
            });
            _periodicTimers[info.Name] = timer;
            Log($"{info.Name}: precision periodic started ({interval} ms, {(howMany == 0 ? "unlimited" : howMany.ToString())}).");
        }

        /// <summary>Creates an independent copy of the message's current field values
        /// (keyed by field path because the grid also contains nested array rows).</summary>
        private List<FieldRow> SnapshotFields(MessageInfo info)
        {
            var current = new Dictionary<string, string>();
            foreach (var row in _fields)
                if (!row.IsArray) current[row.Field] = row.Value;
            var list = new List<FieldRow>(info.Fields.Length);
            foreach (var f in info.Fields)
            {
                var row = new FieldRow(f);
                row.Value = current.TryGetValue(f.Field, out var v) ? v : f.DefaultValue;
                list.Add(row);
            }
            return list;
        }

        private void StopPeriodic_Click(object sender, RoutedEventArgs e)
        {
            if (Current is { } info) StopPeriodic(info.Name);
        }

        private void StopAllPeriodic_Click(object sender, RoutedEventArgs e)
        {
            foreach (var name in _periodicTimers.Keys.ToList()) StopPeriodic(name);
            Log("All periodic timers stopped.");
        }

        private void StopPeriodic(string name)
        {
            if (_periodicTimers.TryGetValue(name, out var timer))
            {
                timer.Dispose(); // stops the dedicated timer thread
                _periodicTimers.Remove(name);
                Log($"{name}: periodic stopped.");
            }
        }

        // ---- Transport lifecycle (UDP / TCP client / TCP server / RS232) ----
        private void TransportCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TcpModePanel is null || SerialPanel is null || NetworkPanel is null) return;
            var choice = (TransportCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "UDP";
            TcpModePanel.Visibility = choice == "TCP" ? Visibility.Visible : Visibility.Collapsed;
            SerialPanel.Visibility = choice == "RS232" ? Visibility.Visible : Visibility.Collapsed;
            NetworkPanel.Visibility = choice == "RS232" ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>Creates the transport selected in the combo boxes.</summary>
        private ITransport CreateTransport()
        {
            var choice = (TransportCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "UDP";
            switch (choice)
            {
                case "TCP":
                    var mode = (TcpModeCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Client";
                    return mode == "Server"
                        ? new TcpServerTransport(int.Parse(LocalPortBox.Text))
                        : new TcpClientTransport(RemoteIpBox.Text, int.Parse(RemotePortBox.Text));
                case "RS232":
                    var parity = ParseParity((ParityBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString());
                    var stopBits = ParseStopBits((StopBitsBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString());
                    return new SerialTransport(ComPortBox.Text, int.Parse(BaudRateBox.Text), parity, stopBits);
                default:
                    return new UdpTransport(int.Parse(LocalPortBox.Text), RemoteIpBox.Text, int.Parse(RemotePortBox.Text));
            }
        }

        private static System.IO.Ports.Parity ParseParity(string? text) => text switch
        {
            "Odd" => System.IO.Ports.Parity.Odd,
            "Even" => System.IO.Ports.Parity.Even,
            "Mark" => System.IO.Ports.Parity.Mark,
            "Space" => System.IO.Ports.Parity.Space,
            _ => System.IO.Ports.Parity.None
        };

        private static System.IO.Ports.StopBits ParseStopBits(string? text) => text switch
        {
            "1.5" => System.IO.Ports.StopBits.OnePointFive,
            "2" => System.IO.Ports.StopBits.Two,
            _ => System.IO.Ports.StopBits.One
        };

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopTransport();
                _transport = CreateTransport();
                // Never block the transport thread on the UI: record on the background thread
                // and queue the UI update, so 10 ms receive rates stay accurate.
                _transport.DataReceived += OnDataReceivedBackground;
                _transport.StatusChanged += message => Dispatcher.Invoke(() =>
                {
                    if (message.Contains("timeout", StringComparison.OrdinalIgnoreCase)) _timeoutCount++;
                    Log(message);
                });
                _transport.Start();
                Log($"Started: {_transport.Description}.");
            }
            catch (Exception ex)
            {
                Log("Start error: " + ex.Message);
                StopTransport();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopTransport();
            Log("Transport stopped.");
        }

        private void StopTransport()
        {
            _transport?.Dispose();
            _transport = null;
        }

        /// <summary>Runs on the transport thread: records with a precise timestamp and
        /// queues the UI update without blocking the receive loop.</summary>
        private void OnDataReceivedBackground(byte[] data, string from)
        {
            _recorder.Record(false, data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : -1, data);
            Dispatcher.InvokeAsync(() => OnReceived(data, from));
        }

        private void OnReceived(byte[] data, string from)
        {
            // Keep this handler light: it runs for every received packet (already recorded).
            // Heavy UI work (graph repaint, statistics refresh) is throttled elsewhere.
            var sw = System.Diagnostics.Stopwatch.StartNew();
            int msgId = data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : -1;
            var info = MessageCatalog.ById(msgId);
            var name = info?.Name ?? $"Unknown (0x{msgId:X})";
            RecordTimelineEvent(false, name, "RECEIVED", $"{data.Length} bytes from {from}", data);
            UpdateStat(name, data.Length, outgoing: false, error: data.Length < 2, dropped: info is null && data.Length >= 2, crcError: HasCrcError(info, data));
            // Scenario Response steps may auto-send a reply when this message matches their rule.
            CheckScenarioResponses(info, name, data);
            // Flow tab: release a Condition node that is waiting for this message.
            CheckFlowWait(name, data);
            _monitor.Insert(0, new MonitorRow
            {
                Time = DateTime.Now.ToString("HH:mm:ss.fff"), From = from, Message = name, Bytes = data.Length
            });
            if (_monitor.Count > 500) _monitor.RemoveAt(_monitor.Count - 1);
            _receivedFields.Clear();
            // Decode the field grid only when it is visible; at fast rates this is the
            // most expensive part of the receive path.
            if (info is not null && ReceivedFieldsGrid.IsVisible)
            {
                lock (_nativeLock) info.ConvertToPhysical(data, info.GetPhysical());
                foreach (var f in info.Fields)
                    _receivedFields.Add(new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(data) });
            }
            // Per-message RX logging is skipped (the Monitor grid shows it) to keep fast rates smooth.
            sw.Stop();
            Interlocked.Add(ref _totalProcessingTicks, sw.ElapsedTicks);
            Interlocked.Increment(ref _processedMessages);
        }

        /// <summary>Verifies the additive checksum of a received message when it has a CRC field.</summary>
        private static bool HasCrcError(MessageInfo? info, byte[] data)
        {
            var crc = info?.Fields.FirstOrDefault(f => f.Field.EndsWith(".crc", StringComparison.OrdinalIgnoreCase));
            if (crc is null || crc.Offset + 4 > data.Length) return false;
            uint sum = 0;
            for (int i = 0; i < crc.Offset; i++) sum += data[i];
            return BitConverter.ToUInt32(data, crc.Offset) != sum;
        }

        private void ClearMonitor_Click(object sender, RoutedEventArgs e)
        {
            _monitor.Clear();
            _receivedFields.Clear();
            Log("Monitor cleared.");
        }

        // ---- HEX preview ----
        private void RefreshHex_Click(object sender, RoutedEventArgs e) => RefreshHex();

        private void RefreshHex()
        {
            var info = Current;
            if (info is null) { HexPreviewBox.Text = string.Empty; return; }
            var buffer = BuildBuffer(info, _fields.ToList());
            var sb = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2")).Append(' ');
                if ((i + 1) % 16 == 0) sb.AppendLine();
            }
            HexPreviewBox.Text = sb.ToString();
        }

        // ---- JSON import/export ----
        private void ExportJson_Click(object sender, RoutedEventArgs e)
        {
            var info = Current;
            if (info is null) return;
            var dlg = new Microsoft.Win32.SaveFileDialog { Filter = "JSON|*.json", FileName = info.Name + ".json" };
            if (dlg.ShowDialog() != true) return;
            var map = _fields.Where(f => !f.IsArray).ToDictionary(f => f.Field, f => f.Value);
            File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true }));
            Log($"Export_{info.Name}: written to {dlg.FileName}.");
        }

        private void ImportJson_Click(object sender, RoutedEventArgs e)
        {
            if (Current is null) return;
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "JSON|*.json" };
            if (dlg.ShowDialog() != true) return;
            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(dlg.FileName));
            if (map is null) return;
            foreach (var row in _fields)
                if (map.TryGetValue(row.Field, out var v)) row.Value = v;
            RefreshHex();
            Log($"Imported field values from {dlg.FileName}.");
        }

        private void ClearTrafficLog_Click(object sender, RoutedEventArgs e)
        {
            TrafficLogBox.Clear();
        }

        // ---- Look & Feel: runtime theme switching ----
        private void ThemeCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var theme = (ThemeCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Dark Blue";
            ApplyTheme(theme);
        }

        /// <summary>Swaps the theme brushes; every control bound with DynamicResource restyles instantly.</summary>
        private void ApplyTheme(string theme)
        {
            // window, panel, foreground, dim, header, border, row, altRow, accent, tabBg, tabFg, gridHeaderFg, graphRx
            var palette = theme switch
            {
                "Midnight Black" => new[] { "#000000", "#0D0D0D", "#E0E0E0", "#8A8A8A", "#9CDCFE", "#2B2B2B", "#111111", "#181818", "#0A84FF", "#9CDCFE", "#000000", "#000000", "#FFFFFF" },
                "Light" => new[] { "#F5F7FA", "#FFFFFF", "#1B1F24", "#5A6B7C", "#0B5CAD", "#C9D4DE", "#EDF2F7", "#F7FAFC", "#1F6FEB", "#D7E3EF", "#1B1F24", "#1B1F24", "#39424E" },
                "Military Green" => new[] { "#101810", "#0B140B", "#DDE8DD", "#7C907C", "#9CCC9C", "#233423", "#152015", "#111A11", "#2E7D32", "#9CCC9C", "#000000", "#000000", "#F0F5F0" },
                _ => new[] { "#0E1621", "#0B1220", "#E6EDF3", "#7A8CA0", "#8FB4D9", "#22303C", "#132030", "#0F1A28", "#1F6FEB", "#8FB4D9", "#000000", "#000000", "#FFFFFF" }
            };
            var keys = new[]
            {
                "Theme.WindowBackground", "Theme.PanelBackground", "Theme.Foreground", "Theme.DimForeground",
                "Theme.HeaderForeground", "Theme.Border", "Theme.RowBackground", "Theme.AltRowBackground",
                "Theme.Accent", "Theme.TabBackground", "Theme.TabForeground", "Theme.GridHeaderForeground", "Theme.GraphRx"
            };
            for (int i = 0; i < keys.Length; i++)
            {
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(palette[i]);
                Resources[keys[i]] = new System.Windows.Media.SolidColorBrush(color);
            }
            if (RecordGraphFieldsGrid is null) return; // still initializing from XAML
            RedrawTimeline();
            RedrawRecordTimeline();
            Log($"Theme changed to '{theme}'.");
        }

        // ---- Scenario tab: drag & drop steps, per-step async scheduling, folder save/load ----
        private void AllMessagesList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void AllMessagesList_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var pos = e.GetPosition(null);
            if (Math.Abs(pos.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance) return;
            if (AllMessagesList.SelectedItem is not string name) return;
            DragDrop.DoDragDrop(AllMessagesList, name, DragDropEffects.Copy);
        }

        private void AllMessagesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AllMessagesList.SelectedItem is string name) AddScenarioStep(name);
        }

        private void ScenarioSteps_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.StringFormat) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void ScenarioSteps_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.StringFormat) is not string name) return;
            if (name == SleepDragToken) AddSleepStep();
            else if (name == ResponseDragToken) AddResponseStep();
            else AddScenarioStep(name);
        }

        // ---- Sleep operation: drag "Sleep" (with its ms) into the scenario ----
        private const string SleepDragToken = "::SLEEP::";

        private void SleepItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            if (e.ClickCount == 2) AddSleepStep();
        }

        private void SleepItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var pos = e.GetPosition(null);
            if (Math.Abs(pos.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance) return;
            DragDrop.DoDragDrop(SleepDragItem, SleepDragToken, DragDropEffects.Copy);
        }

        /// <summary>Adds a Sleep step with the duration from the ms box; when the running
        /// scenario reaches it, every later step is delayed by that duration.</summary>
        private void AddSleepStep()
        {
            var ms = int.TryParse(SleepMsBox.Text, out var v) ? Math.Max(1, v) : 1000;
            _scenarioSteps.Add(new ScenarioStepRow
            {
                Message = "SLEEP", IsSleep = true, TimeMs = ms,
                PeriodicIntervalMs = 0, Periodic = false, MaxMessages = 0
            });
            RenumberSteps();
            Log($"Sleep step added ({ms} ms).");
        }

        // ---- Response operation: when a message is received and a chosen field equals
        // a value, automatically send a configured message ----
        private const string ResponseDragToken = "::RESPONSE::";

        private void ResponseItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            if (e.ClickCount == 2) AddResponseStep();
        }

        private void ResponseItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var pos = e.GetPosition(null);
            if (Math.Abs(pos.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(pos.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance) return;
            DragDrop.DoDragDrop(ResponseDragItem, ResponseDragToken, DragDropEffects.Copy);
        }

        /// <summary>Adds a Response step; configure it in the right panel: 1) the message to
        /// receive, 2) the field and expected value, 3) the message to send when it matches.</summary>
        private void AddResponseStep()
        {
            var first = MessageCatalog.Messages.FirstOrDefault();
            if (first is null) return;
            var step = new ScenarioStepRow
            {
                Message = first.Name, IsResponse = true,
                ResponseTrigger = first.Name,
                ResponseField = first.Fields.FirstOrDefault()?.Field ?? string.Empty,
                ResponseValue = "0",
                FieldValues = first.Fields.ToDictionary(f => f.Field, f => f.DefaultValue),
                TimeMs = 0, PeriodicIntervalMs = 0, Periodic = false, MaxMessages = 0
            };
            _scenarioSteps.Add(step);
            RenumberSteps();
            ScenarioStepsList.SelectedItem = step;
            Log("Response step added: choose the received message, its field, the expected value and the message to send.");
        }

        /// <summary>Loads the Response editor controls from the selected step.</summary>
        private void ShowResponseEditor(ScenarioStepRow step)
        {
            _loadingResponseUi = true;
            ResponseEditPanel.Visibility = Visibility.Visible;
            ResponseTriggerCombo.SelectedItem = step.ResponseTrigger;
            PopulateResponseFields(step.ResponseTrigger);
            ResponseFieldCombo.SelectedItem = string.IsNullOrEmpty(step.ResponseField) ? null : step.ResponseField;
            ResponseValueBox.Text = step.ResponseValue;
            ResponseSendCombo.SelectedItem = step.Message;
            _loadingResponseUi = false;
        }

        private void PopulateResponseFields(string messageName)
        {
            ResponseFieldCombo.Items.Clear();
            var info = MessageCatalog.ByName(messageName);
            if (info is null) return;
            foreach (var f in info.Fields) ResponseFieldCombo.Items.Add(f.Field);
        }

        private void ResponseTriggerCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingResponseUi || _editingStep is not { IsResponse: true } step) return;
            step.ResponseTrigger = ResponseTriggerCombo.SelectedItem as string ?? string.Empty;
            PopulateResponseFields(step.ResponseTrigger);
            if (ResponseFieldCombo.Items.Count > 0) ResponseFieldCombo.SelectedIndex = 0;
        }

        private void ResponseFieldCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingResponseUi || _editingStep is not { IsResponse: true } step) return;
            step.ResponseField = ResponseFieldCombo.SelectedItem as string ?? string.Empty;
        }

        private void ResponseValueBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingResponseUi || _editingStep is not { IsResponse: true } step) return;
            step.ResponseValue = ResponseValueBox.Text;
        }

        private void ResponseSendCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingResponseUi || _editingStep is not { IsResponse: true } step) return;
            var name = ResponseSendCombo.SelectedItem as string ?? string.Empty;
            if (name.Length == 0 || step.Message == name) return;
            step.Message = name;
            var info = MessageCatalog.ByName(name);
            step.FieldValues = info?.Fields.ToDictionary(f => f.Field, f => f.DefaultValue);
            LoadStepFields(step);
        }

        /// <summary>Registers a Response step as an active rule for the running scenario.</summary>
        private void ArmResponse(ScenarioStepRow step)
        {
            var send = MessageCatalog.ByName(step.Message);
            var trigger = MessageCatalog.ByName(step.ResponseTrigger);
            if (send is null || trigger is null || string.IsNullOrEmpty(step.ResponseField))
            {
                Log($"Step {step.Index}: response not armed (choose the received message, field and message to send).");
                return;
            }
            _activeResponses.Add(new ResponseRule
            {
                TriggerName = trigger.Name,
                Field = step.ResponseField,
                Value = step.ResponseValue,
                Send = send,
                SendFields = BuildStepFields(send, step),
                SendArrayCells = BuildStepArrayCells(send, step),
                StepIndex = step.Index
            });
            Log($"Step {step.Index}: response armed - when {trigger.Name}.{step.ResponseField} == {step.ResponseValue}, send {send.Name}.");
        }

        /// <summary>Checks a received message against the armed Response rules and sends
        /// the configured reply when the chosen field equals the expected value.</summary>
        private void CheckScenarioResponses(MessageInfo? info, string name, byte[] data)
        {
            if (!_scenarioRunning || _activeResponses.Count == 0 || info is null) return;
            foreach (var rule in _activeResponses)
            {
                if (rule.TriggerName != name) continue;
                var field = info.Fields.FirstOrDefault(f => f.Field == rule.Field);
                if (field is null) continue;
                var actual = field.Read(data);
                if (!ResponseValuesEqual(actual, rule.Value)) continue;
                Log($"Step {rule.StepIndex}: response triggered ({name}.{rule.Field} == {rule.Value}) -> sending {rule.Send.Name}.");
                SendMessageCore(rule.Send, rule.SendFields, rule.SendArrayCells, false,
                    AutoSequenceCheck.IsChecked == true, AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);
            }
        }

        /// <summary>Compares field values as text first, then numerically.</summary>
        private static bool ResponseValuesEqual(string actual, string expected)
        {
            if (string.Equals(actual.Trim(), expected.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
            return double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out var a)
                && double.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out var b)
                && Math.Abs(a - b) < 1e-9;
        }

        /// <summary>Adds a message to the scenario; the same message can be added any number of times.</summary>
        private void AddScenarioStep(string messageName)
        {
            var info = MessageCatalog.ByName(messageName);
            if (info is null) return;
            var values = info.Fields.ToDictionary(f => f.Field, f => f.DefaultValue);
            if (ReferenceEquals(info, Current))
            {
                // Carry over the Simulator editor values, including array cells and counts.
                foreach (var row in _fields)
                {
                    if (!row.IsArray) { values[row.Field] = row.Value; continue; }
                    values[row.Field + "#count"] = row.CountText;
                    foreach (var cell in row.Cells) values[cell.Field] = cell.Value;
                }
            }
            _scenarioSteps.Add(new ScenarioStepRow
            {
                Message = messageName, TimeMs = 0, PeriodicIntervalMs = 1000,
                Periodic = false, MaxMessages = 0, FieldValues = values
            });
            RenumberSteps();
        }

        private void RenumberSteps()
        {
            for (int i = 0; i < _scenarioSteps.Count; i++) _scenarioSteps[i].Index = i;
        }

        /// <summary>Shows the clicked step's editor: field values for a message step, the
        /// sleep summary for a Sleep step, or the Response rule editor for a Response step.</summary>
        private void ScenarioStepsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _stepFields.Clear();
            _editingStep = ScenarioStepsList.SelectedItem as ScenarioStepRow;
            ResponseEditPanel.Visibility = Visibility.Collapsed;
            if (_editingStep is null)
            {
                StepFieldsHeader.Text = "Step Fields (select a step)";
                return;
            }
            if (_editingStep.IsSleep)
            {
                StepFieldsHeader.Text = $"Step {_editingStep.Index}: \u23F1 Sleep {_editingStep.TimeMs} ms";
                return;
            }
            if (_editingStep.IsResponse)
            {
                StepFieldsHeader.Text = $"Step {_editingStep.Index}: \u21C4 Response";
                ShowResponseEditor(_editingStep);
            }
            else
            {
                StepFieldsHeader.Text = $"Step {_editingStep.Index}: {_editingStep.Message}";
            }
            LoadStepFields(_editingStep);
        }

        /// <summary>Fills the Step Fields grid with the values the step sends: scalar
        /// fields and collapsible array rows merged by offset (like the Simulator tab);
        /// edits sync straight into the step so Run and Save use what is shown.</summary>
        private void LoadStepFields(ScenarioStepRow step)
        {
            _stepFields.Clear();
            var info = MessageCatalog.ByName(step.Message);
            if (info is null) { StepFieldsHeader.Text = "Step Fields (unknown message)"; return; }
            step.FieldValues ??= info.Fields.ToDictionary(f => f.Field, f => f.DefaultValue);
            var arrays = new Queue<FieldRow>(info.Arrays.OrderBy(a => a.BaseOffset)
                .Select(a => CreateStepArrayRow(a, step)));
            foreach (var f in info.Fields)
            {
                while (arrays.Count > 0 && arrays.Peek().Offset <= f.Offset)
                    _stepFields.Add(arrays.Dequeue());
                var row = new FieldRow(f)
                {
                    Value = step.FieldValues.TryGetValue(f.Field, out var v) ? v : f.DefaultValue
                };
                row.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(FieldRow.Value) && step.FieldValues is not null)
                        step.FieldValues[row.Field] = row.Value;
                };
                _stepFields.Add(row);
            }
            while (arrays.Count > 0)
                _stepFields.Add(arrays.Dequeue());
        }

        /// <summary>Creates an array row for the step editor: restores the saved element
        /// count and cell values and syncs every edit back into the step.</summary>
        private static FieldRow CreateStepArrayRow(ArrayInfo array, ScenarioStepRow step)
        {
            var count = Math.Min(1, array.MaxCount);
            if (step.FieldValues is not null &&
                step.FieldValues.TryGetValue(array.Name + "#count", out var saved) &&
                int.TryParse(saved, out var n))
                count = n;
            var row = new FieldRow(array, count);
            WireStepArrayCells(row, step);
            return row;
        }

        /// <summary>Restores the saved cell values of an array row and syncs edits into the step.</summary>
        private static void WireStepArrayCells(FieldRow row, ScenarioStepRow step)
        {
            step.FieldValues ??= new Dictionary<string, string>();
            step.FieldValues[row.Field + "#count"] = row.CountText;
            foreach (var cell in row.Cells)
            {
                if (step.FieldValues.TryGetValue(cell.Field, out var v)) cell.Value = v;
                cell.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(ArrayCellRow.Value) && step.FieldValues is not null)
                        step.FieldValues[cell.Field] = cell.Value;
                };
            }
        }

        /// <summary>Apply button inside a step array row's nested table: rebuilds the cells
        /// to the requested count, restores saved values and updates the count field.</summary>
        private void ApplyStepArrayCount_Click(object sender, RoutedEventArgs e)
        {
            if (_editingStep is not { } step) return;
            if ((sender as FrameworkElement)?.DataContext is not FieldRow { ArrayDef: { } array } row) return;
            var applied = row.RebuildCells(int.TryParse(row.CountText, out var n) ? n : 0);
            WireStepArrayCells(row, step);
            // Reflect the element count in the array's count field so the receiver knows the length.
            if (array.CountField is not null)
                foreach (var f in _stepFields)
                    if (!f.IsArray && f.Field == array.CountField) f.Value = applied.ToString();
        }


        private void RemoveStep_Click(object sender, RoutedEventArgs e)
        {
            if (ScenarioStepsList.SelectedItem is ScenarioStepRow row)
            {
                _scenarioSteps.Remove(row);
                RenumberSteps();
            }
        }

        private void MoveStepUp_Click(object sender, RoutedEventArgs e) => MoveStep(-1);

        private void MoveStepDown_Click(object sender, RoutedEventArgs e) => MoveStep(1);

        private void MoveStep(int delta)
        {
            if (ScenarioStepsList.SelectedItem is not ScenarioStepRow row) return;
            var index = _scenarioSteps.IndexOf(row);
            var target = index + delta;
            if (index < 0 || target < 0 || target >= _scenarioSteps.Count) return;
            _scenarioSteps.Move(index, target);
            RenumberSteps();
            ScenarioStepsList.SelectedItem = row;
        }

        private void ClearSteps_Click(object sender, RoutedEventArgs e)
        {
            StopScenarioTimers();
            _scenarioSteps.Clear();
        }

        private void NewScenario_Click(object sender, RoutedEventArgs e)
        {
            StopScenarioTimers();
            _scenarioSteps.Clear();
            _scenarioFolder = null;
            ScenarioNameText.Text = "(unsaved scenario)";
            Log("New scenario created.");
        }

        private void SaveScenario_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select the scenario folder (Configuration.xml + message JSON files)" };
            if (_scenarioFolder is not null) dlg.InitialDirectory = _scenarioFolder;
            if (dlg.ShowDialog(this) != true) return;
            try
            {
                ScenarioStore.Save(dlg.FolderName, _scenarioSteps.ToList());
                _scenarioFolder = dlg.FolderName;
                ScenarioNameText.Text = Path.GetFileName(dlg.FolderName);
                Log($"Scenario saved to {dlg.FolderName} ({_scenarioSteps.Count} step(s)).");
            }
            catch (Exception ex) { Log("Scenario save error: " + ex.Message); }
        }

        private void OpenScenario_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select a scenario folder containing Configuration.xml" };
            if (dlg.ShowDialog(this) != true) return;
            try
            {
                StopScenarioTimers();
                _scenarioSteps.Clear();
                foreach (var step in ScenarioStore.Load(dlg.FolderName)) _scenarioSteps.Add(step);
                RenumberSteps();
                _scenarioFolder = dlg.FolderName;
                ScenarioNameText.Text = Path.GetFileName(dlg.FolderName);
                Log($"Scenario loaded from {dlg.FolderName} ({_scenarioSteps.Count} step(s)).");
            }
            catch (Exception ex) { Log("Scenario open error: " + ex.Message); }
        }

        // ---- Scenario execution: every step is scheduled independently (asynchronously);
        // a Sleep step pauses the scenario timeline, delaying every step after it;
        // a Response step arms an auto-reply that fires when its message arrives. ----
        private void RunScenario_Click(object sender, RoutedEventArgs e)
        {
            if (_scenarioRunning) { Log("Scenario is already running."); return; }
            if (_transport is null) { Log("Start the transport before running a scenario."); return; }
            if (_scenarioSteps.Count == 0) { Log("The scenario has no steps."); return; }
            _scenarioRunning = true;
            _activeResponses.Clear();
            long sleepOffsetMs = 0;
            foreach (var step in _scenarioSteps)
            {
                if (step.IsSleep)
                {
                    ScheduleSleepLog(step, sleepOffsetMs);
                    sleepOffsetMs += Math.Max(0, step.TimeMs);
                    continue;
                }
                if (step.IsResponse)
                {
                    ArmResponse(step);
                    continue;
                }
                ScheduleStep(step, sleepOffsetMs);
            }
            Log($"Scenario started ({_scenarioSteps.Count} step(s), {_activeResponses.Count} response rule(s)).");
        }

        /// <summary>Logs the moment the running scenario reaches a Sleep step.</summary>
        private void ScheduleSleepLog(ScenarioStepRow step, long startOffsetMs)
        {
            var sleepMs = Math.Max(0, step.TimeMs);
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Math.Max(1, startOffsetMs)) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                _scenarioTimers.Remove(timer);
                if (!_scenarioRunning) return;
                Log($"Step {step.Index}: \u23F1 sleeping {sleepMs} ms (later steps are delayed).");
            };
            _scenarioTimers.Add(timer);
            timer.Start();
        }

        /// <summary>Arms one step: a DispatcherTimer waits its start time, then the
        /// periodic repeats run on a PrecisionTimer for millisecond-accurate cadence.</summary>
        private void ScheduleStep(ScenarioStepRow step, long extraDelayMs = 0)
        {
            if (step.IsSleep || step.IsResponse) return; // handled in RunScenario_Click
            var info = MessageCatalog.ByName(step.Message);
            if (info is null) { Log($"Step {step.Index}: unknown message '{step.Message}'."); return; }
            var fields = BuildStepFields(info, step);
            var arrayCells = BuildStepArrayCells(info, step);
            // The start delay includes the accumulated Sleep steps that precede this step.
            var startTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Math.Max(1, step.TimeMs + extraDelayMs)) };
            startTimer.Tick += (_, _) =>
            {
                startTimer.Stop();
                _scenarioTimers.Remove(startTimer);
                if (!_scenarioRunning) return;
                SendMessageCore(info, fields, arrayCells, step.Periodic, AutoSequenceCheck.IsChecked == true, AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);
                if (!step.Periodic || step.PeriodicIntervalMs <= 0 || step.MaxMessages == 1) return;
                // Snapshot the auto flags on the UI thread; the repeats run off the UI thread.
                bool autoSeq = AutoSequenceCheck.IsChecked == true;
                bool autoTs = AutoTimestampCheck.IsChecked == true;
                bool autoCrc = AutoCrcCheck.IsChecked == true;
                var sent = 1;
                PrecisionTimer? periodicTimer = null;
                periodicTimer = new PrecisionTimer(Math.Max(1, step.PeriodicIntervalMs), () =>
                {
                    if (!_scenarioRunning) return;
                    var i = Interlocked.Increment(ref sent);
                    if (step.MaxMessages > 0 && i > step.MaxMessages) return;
                    SendMessageCore(info, fields, arrayCells, true, autoSeq, autoTs, autoCrc);
                    if (step.MaxMessages > 0 && i == step.MaxMessages)
                    {
                        periodicTimer!.Dispose();
                        lock (_scenarioPrecisionTimers) _scenarioPrecisionTimers.Remove(periodicTimer!);
                        Dispatcher.InvokeAsync(() => Log($"Step {step.Index} ({step.Message}): periodic completed ({i} sent)."));
                    }
                });
                lock (_scenarioPrecisionTimers) _scenarioPrecisionTimers.Add(periodicTimer);
            };
            _scenarioTimers.Add(startTimer);
            startTimer.Start();
        }

        /// <summary>Builds the field rows for a step from its saved JSON values over the defaults.</summary>
        private static List<FieldRow> BuildStepFields(MessageInfo info, ScenarioStepRow step)
        {
            var list = new List<FieldRow>(info.Fields.Length);
            foreach (var f in info.Fields)
            {
                var row = new FieldRow(f) { Value = f.DefaultValue };
                if (step.FieldValues is not null && step.FieldValues.TryGetValue(f.Field, out var v)) row.Value = v;
                list.Add(row);
            }
            return list;
        }

        /// <summary>Builds the array cells for a step from its saved values, so the
        /// scenario sends the array data typed in the step editor.</summary>
        private static List<ArrayCellRow> BuildStepArrayCells(MessageInfo info, ScenarioStepRow step)
        {
            var cells = new List<ArrayCellRow>();
            if (step.FieldValues is null) return cells;
            foreach (var array in info.Arrays)
            {
                var count = 0;
                if (step.FieldValues.TryGetValue(array.Name + "#count", out var saved))
                    _ = int.TryParse(saved, out count);
                count = Math.Clamp(count, 0, array.MaxCount);
                for (int i = 0; i < count; i++)
                    foreach (var el in array.Elements)
                    {
                        var field = el.Field.Replace($"[{array.IndexVar}]", $"[{i}]", StringComparison.Ordinal);
                        cells.Add(new ArrayCellRow
                        {
                            Index = i,
                            Offset = array.BaseOffset + i * array.Stride + el.RelativeOffset,
                            Field = field,
                            Type = el.Type,
                            Value = step.FieldValues.TryGetValue(field, out var v) ? v : "0"
                        });
                    }
            }
            return cells;
        }

        private void StopScenario_Click(object sender, RoutedEventArgs e)
        {
            StopScenarioTimers();
            Log("Scenario stopped.");
        }

        private void StopScenarioTimers()
        {
            _scenarioRunning = false;
            foreach (var timer in _scenarioTimers) timer.Stop();
            _scenarioTimers.Clear();
            _activeResponses.Clear();
            lock (_scenarioPrecisionTimers)
            {
                foreach (var timer in _scenarioPrecisionTimers) timer.Dispose();
                _scenarioPrecisionTimers.Clear();
            }
        }

        private void ClearScenarioLog_Click(object sender, RoutedEventArgs e)
        {
            ScenarioLogBox.Clear();
        }

        // ---- Flow tab: visual scenario designer (drag & drop nodes on a canvas) ----
        private const string FlowConditionToken = "::FLOWCOND::";
        private const string FlowDelayToken = "::FLOWDELAY::";
        private const string FlowLoopToken = "::FLOWLOOP::";

        /// <summary>Remembers the drag start so a palette item only starts a drag after a threshold.</summary>
        private void FlowPaletteItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void FlowMessages_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!FlowDragStarted(e)) return;
            if (FlowMessagesList.SelectedItem is not string name) return;
            DragDrop.DoDragDrop(FlowMessagesList, "MSG:" + name, DragDropEffects.Copy);
        }

        private void FlowItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!FlowDragStarted(e)) return;
            if ((sender as FrameworkElement)?.Tag is string token)
                DragDrop.DoDragDrop((DependencyObject)sender, token, DragDropEffects.Copy);
        }

        private bool FlowDragStarted(MouseEventArgs e)
        {
            var pos = e.GetPosition(null);
            return Math.Abs(pos.X - _dragStartPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(pos.Y - _dragStartPoint.Y) >= SystemParameters.MinimumVerticalDragDistance;
        }

        private void FlowCanvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.StringFormat) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>Drops a palette item onto the canvas as a new node at the cursor.</summary>
        private void FlowCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.StringFormat) is not string payload) return;
            var p = e.GetPosition(FlowCanvas);
            if (payload.StartsWith("MSG:", StringComparison.Ordinal))
                AddFlowNode(FlowNodeKind.Message, payload.Substring(4), p.X - 70, p.Y - 26);
            else if (payload == FlowConditionToken)
                AddFlowNode(FlowNodeKind.Condition, "Condition", p.X - 80, p.Y - 30);
            else if (payload == FlowDelayToken)
                AddFlowNode(FlowNodeKind.Delay, "Delay", p.X - 60, p.Y - 24);
            else if (payload == FlowLoopToken)
                AddFlowNode(FlowNodeKind.Loop, "Loop", p.X - 60, p.Y - 24);
        }

        /// <summary>Creates a flow node, positions it and redraws the canvas.</summary>
        private void AddFlowNode(FlowNodeKind kind, string title, double left, double top)
        {
            var node = new FlowNodeVisual
            {
                Kind = kind,
                Title = title,
                Left = Math.Max(0, left),
                Top = Math.Max(0, top),
                WaitMessage = kind == FlowNodeKind.Condition ? title : string.Empty,
                Id = ++_flowNodeSeq
            };
            if (kind == FlowNodeKind.Condition) node.WaitMessage = MessageCatalog.Messages.FirstOrDefault()?.Name ?? string.Empty;
            _flowNodes.Add(node);
            RedrawFlow();
            SelectFlowNode(node);
            Log($"Flow: added {kind} node.");
        }

        /// <summary>Rebuilds every connection line and node shape on the canvas.</summary>
        private void RedrawFlow()
        {
            if (FlowCanvas is null) return;
            ComputeFlowOrder();
            FlowCanvas.Children.Clear();
            // Connections first so the shapes are drawn on top of the lines.
            foreach (var c in _flowConnections)
                DrawFlowConnection(c);
            foreach (var node in _flowNodes)
                DrawFlowNode(node);
            RefreshFlowSteps();
        }

        /// <summary>Draws a connection as an arrow from the source to the target node,
        /// so the arrow direction shows the order in which messages are sent.</summary>
        private void DrawFlowConnection(FlowConnection c)
        {
            var brush = c.Branch switch
            {
                FlowBranch.True => FlowRgb(0x23, 0x86, 0x36),
                FlowBranch.False => FlowRgb(0xDA, 0x36, 0x33),
                _ => FlowBrush("Theme.HeaderForeground", 0x8F, 0xB4, 0xD9)
            };
            double x1 = c.From.CenterX, y1 = c.From.Bottom, x2 = c.To.CenterX, y2 = c.To.Top;
            FlowCanvas.Children.Add(new System.Windows.Shapes.Line
            {
                X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = brush, StrokeThickness = 2
            });
            // Arrowhead at the target end.
            double dx = x2 - x1, dy = y2 - y1;
            double len = Math.Max(1.0, Math.Sqrt(dx * dx + dy * dy));
            double ux = dx / len, uy = dy / len;
            const double head = 12, halfW = 6;
            double baseX = x2 - ux * head, baseY = y2 - uy * head;
            FlowCanvas.Children.Add(new System.Windows.Shapes.Polygon
            {
                Fill = brush,
                Points = new System.Windows.Media.PointCollection
                {
                    new Point(x2, y2),
                    new Point(baseX - uy * halfW, baseY + ux * halfW),
                    new Point(baseX + uy * halfW, baseY - ux * halfW)
                }
            });
            // Label True / False branches at the midpoint of the line.
            if (c.Branch != FlowBranch.Normal)
            {
                var tag = new System.Windows.Controls.TextBlock
                {
                    Text = c.Branch == FlowBranch.True ? "True" : "False",
                    Foreground = brush, FontWeight = FontWeights.Bold, FontSize = 11, IsHitTestVisible = false
                };
                System.Windows.Controls.Canvas.SetLeft(tag, (x1 + x2) / 2 + 4);
                System.Windows.Controls.Canvas.SetTop(tag, (y1 + y2) / 2 - 8);
                FlowCanvas.Children.Add(tag);
            }
        }

        /// <summary>Draws one node: a Message is a rectangle, a Condition is a rhombus,
        /// Delay and Loop are rounded blocks. Each shape carries the node for hit-testing.</summary>
        private void DrawFlowNode(FlowNodeVisual node)
        {
            var fill = node.Kind switch
            {
                FlowNodeKind.Message => FlowRgb(0x16, 0x2C, 0x50),
                FlowNodeKind.Condition => FlowRgb(0xC8, 0x86, 0x2B),
                FlowNodeKind.Delay => FlowRgb(0x8A, 0x6D, 0x1B),
                _ => FlowRgb(0x6E, 0x40, 0xC9)
            };
            var stroke = node == _runningFlowNode ? FlowRgb(0x23, 0x86, 0x36)
                : node == _selectedFlowNode ? FlowRgb(0x1F, 0x6F, 0xEB)
                : FlowRgb(0x22, 0x30, 0x3C);
            var strokeWidth = node == _runningFlowNode ? 3.5 : node == _selectedFlowNode ? 3.0 : 1.5;

            System.Windows.Shapes.Shape shape;
            if (node.Kind == FlowNodeKind.Condition)
            {
                // Rhombus (diamond): the four corners sit at the midpoints of each side.
                var poly = new System.Windows.Shapes.Polygon { Fill = fill, Stroke = stroke, StrokeThickness = strokeWidth };
                poly.Points = new System.Windows.Media.PointCollection
                {
                    new Point(node.Width / 2, 0), new Point(node.Width, node.Height / 2),
                    new Point(node.Width / 2, node.Height), new Point(0, node.Height / 2)
                };
                shape = poly;
            }
            else
            {
                shape = new System.Windows.Shapes.Rectangle
                {
                    Width = node.Width, Height = node.Height, Fill = fill, Stroke = stroke, StrokeThickness = strokeWidth,
                    RadiusX = node.Kind == FlowNodeKind.Message ? 4 : 10, RadiusY = node.Kind == FlowNodeKind.Message ? 4 : 10
                };
            }
            shape.Tag = node;
            shape.Cursor = Cursors.SizeAll;
            shape.MouseLeftButtonDown += FlowNode_MouseLeftButtonDown;
            shape.MouseRightButtonUp += FlowNode_MouseRightButtonUp;
            System.Windows.Controls.Canvas.SetLeft(shape, node.Left);
            System.Windows.Controls.Canvas.SetTop(shape, node.Top);
            FlowCanvas.Children.Add(shape);

            // Node label (title + a short subtitle) centered on the shape.
            var text = new System.Windows.Controls.TextBlock
            {
                Text = node.Caption, Foreground = System.Windows.Media.Brushes.White,
                FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center,
                Width = node.Width, IsHitTestVisible = false, TextWrapping = TextWrapping.Wrap
            };
            System.Windows.Controls.Canvas.SetLeft(text, node.Left);
            System.Windows.Controls.Canvas.SetTop(text, node.Top + node.Height / 2 - 16);
            FlowCanvas.Children.Add(text);

            // Order badge: the node's position in the send order (set by the arrows).
            if (_flowOrder.TryGetValue(node, out var order))
            {
                var badge = new System.Windows.Shapes.Ellipse
                {
                    Width = 22, Height = 22, Fill = FlowRgb(0x1F, 0x6F, 0xEB),
                    Stroke = System.Windows.Media.Brushes.White, StrokeThickness = 1, IsHitTestVisible = false
                };
                System.Windows.Controls.Canvas.SetLeft(badge, node.Left - 8);
                System.Windows.Controls.Canvas.SetTop(badge, node.Top - 8);
                FlowCanvas.Children.Add(badge);
                var badgeText = new System.Windows.Controls.TextBlock
                {
                    Text = order.ToString(), Foreground = System.Windows.Media.Brushes.White,
                    FontWeight = FontWeights.Bold, FontSize = 11, Width = 22,
                    TextAlignment = TextAlignment.Center, IsHitTestVisible = false
                };
                System.Windows.Controls.Canvas.SetLeft(badgeText, node.Left - 8);
                System.Windows.Controls.Canvas.SetTop(badgeText, node.Top - 6);
                FlowCanvas.Children.Add(badgeText);
            }
        }

        private static System.Windows.Media.SolidColorBrush FlowRgb(byte r, byte g, byte b) =>
            new(System.Windows.Media.Color.FromRgb(r, g, b));

        private System.Windows.Media.Brush FlowBrush(string themeKey, byte r, byte g, byte b) =>
            TryFindResource(themeKey) as System.Windows.Media.Brush ?? FlowRgb(r, g, b);

        /// <summary>Starts moving a node and selects it.</summary>
        private void FlowNode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is not FlowNodeVisual node) return;
            SelectFlowNode(node);
            _movingFlowNode = node;
            var p = e.GetPosition(FlowCanvas);
            _flowMoveOffset = new Point(p.X - node.Left, p.Y - node.Top);
            FlowCanvas.CaptureMouse();
            e.Handled = true;
        }

        /// <summary>Right-click connects nodes: first click picks the source, second the target.</summary>
        private void FlowNode_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is not FlowNodeVisual node) return;
            if (_flowConnectStart is null)
            {
                _flowConnectStart = node;
                Log($"Flow: connect from '{node.Title}' - right-click the target node.");
            }
            else if (!ReferenceEquals(_flowConnectStart, node))
            {
                // A Condition's outgoing links are its True then False branches; others are Normal.
                var branch = FlowBranch.Normal;
                if (_flowConnectStart.Kind == FlowNodeKind.Condition)
                    branch = BranchTarget(_flowConnectStart, FlowBranch.True) is null ? FlowBranch.True : FlowBranch.False;
                _flowConnections.RemoveAll(c => ReferenceEquals(c.From, _flowConnectStart) && c.Branch == branch);
                _flowConnections.Add(new FlowConnection { From = _flowConnectStart, To = node, Branch = branch });
                Log($"Flow: linked '{_flowConnectStart.Title}' -> '{node.Title}' ({branch}).");
                _flowConnectStart = null;
                RedrawFlow();
            }
            e.Handled = true;
        }

        private void FlowCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_movingFlowNode is null || e.LeftButton != MouseButtonState.Pressed) return;
            var p = e.GetPosition(FlowCanvas);
            _movingFlowNode.Left = Math.Max(0, p.X - _flowMoveOffset.X);
            _movingFlowNode.Top = Math.Max(0, p.Y - _flowMoveOffset.Y);
            RedrawFlow();
        }

        private void FlowCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _movingFlowNode = null;
            FlowCanvas.ReleaseMouseCapture();
        }

        /// <summary>Clicking the empty canvas clears the selection and any pending connection.</summary>
        private void FlowCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, FlowCanvas))
            {
                _flowConnectStart = null;
                SelectFlowNode(null);
            }
        }

        /// <summary>Selects a node and loads it into the Step Properties panel.</summary>
        private void SelectFlowNode(FlowNodeVisual? node)
        {
            _selectedFlowNode = node;
            _loadingFlowProps = true;
            FlowMessagePanel.Visibility = Visibility.Collapsed;
            FlowConditionPanel.Visibility = Visibility.Collapsed;
            FlowDelayPanel.Visibility = Visibility.Collapsed;
            FlowLoopPanel.Visibility = Visibility.Collapsed;
            _flowFields.Clear();
            if (node is null)
            {
                FlowPropsHeader.Text = "(no node selected)";
                FlowDescriptionBox.Text = string.Empty;
                _loadingFlowProps = false;
                RedrawFlow();
                return;
            }
            FlowPropsHeader.Text = node.Kind.ToString();
            FlowDescriptionBox.Text = node.Description;
            switch (node.Kind)
            {
                case FlowNodeKind.Message:
                    FlowMessagePanel.Visibility = Visibility.Visible;
                    var info = MessageCatalog.ByName(node.Title);
                    FlowMessageInfoText.Text = info is null
                        ? node.Title
                        : $"{info.Name}\nId: {info.MessageId}   Length: {info.Length} bytes";
                    LoadFlowFields(node);
                    break;
                case FlowNodeKind.Condition:
                    FlowConditionPanel.Visibility = Visibility.Visible;
                    FlowWaitMessageCombo.SelectedItem = node.WaitMessage;
                    FlowTimeoutBox.Text = node.TimeoutMs.ToString();
                    FlowIgnoreTimeoutCheck.IsChecked = node.IgnoreTimeout;
                    PopulateConditionFields(node);
                    SelectConditionOperator(node.ConditionOperator);
                    FlowCondValueBox.Text = node.ConditionValue;
                    PopulateConditionSendCombos(node);
                    PopulateBranchCombos(node);
                    break;
                case FlowNodeKind.Delay:
                    FlowDelayPanel.Visibility = Visibility.Visible;
                    FlowDelayBox.Text = node.DelayMs.ToString();
                    break;
                case FlowNodeKind.Loop:
                    FlowLoopPanel.Visibility = Visibility.Visible;
                    FlowLoopCountBox.Text = node.LoopCount.ToString();
                    PopulateLoopSendCombo(node);
                    break;
            }
            _loadingFlowProps = false;
            RedrawFlow();
        }

        private void FlowWaitMessageCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            node.WaitMessage = FlowWaitMessageCombo.SelectedItem as string ?? string.Empty;
            // The condition field list depends on the awaited message; reset it.
            node.ConditionField = string.Empty;
            PopulateConditionFields(node);
            RedrawFlow();
        }

        private void FlowTimeoutBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            if (int.TryParse(FlowTimeoutBox.Text, out var ms)) { node.TimeoutMs = ms; RedrawFlow(); }
        }

        private void FlowDelayBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Delay } node) return;
            if (int.TryParse(FlowDelayBox.Text, out var ms)) { node.DelayMs = ms; RedrawFlow(); }
        }

        private void FlowLoopCountBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Loop } node) return;
            if (int.TryParse(FlowLoopCountBox.Text, out var n)) { node.LoopCount = n; RedrawFlow(); }
        }

        private void FlowDescriptionBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is null) return;
            _selectedFlowNode.Description = FlowDescriptionBox.Text;
        }

        /// <summary>Deletes the selected node and any connections that touch it.</summary>
        private void FlowDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFlowNode is null) return;
            _flowConnections.RemoveAll(c => ReferenceEquals(c.From, _selectedFlowNode) || ReferenceEquals(c.To, _selectedFlowNode));
            _flowNodes.Remove(_selectedFlowNode);
            Log($"Flow: deleted {_selectedFlowNode.Kind} node.");
            SelectFlowNode(null);
        }

        // ---- Flow: edit all fields of the selected Message node in the Step Properties panel ----
        /// <summary>Loads all the fields (scalars + collapsible array rows) of a Message node
        /// into the FlowFieldsGrid; edits sync into the node so Run sends the shown values.</summary>
        private void LoadFlowFields(FlowNodeVisual node)
        {
            _flowFields.Clear();
            var info = MessageCatalog.ByName(node.Title);
            if (info is null) return;
            node.FieldValues ??= info.Fields.ToDictionary(f => f.Field, f => f.DefaultValue);
            var arrays = new Queue<FieldRow>(info.Arrays.OrderBy(a => a.BaseOffset)
                .Select(a => CreateFlowArrayRow(a, node)));
            foreach (var f in info.Fields)
            {
                while (arrays.Count > 0 && arrays.Peek().Offset <= f.Offset)
                    _flowFields.Add(arrays.Dequeue());
                var row = new FieldRow(f)
                {
                    Value = node.FieldValues.TryGetValue(f.Field, out var v) ? v : f.DefaultValue
                };
                row.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(FieldRow.Value) && node.FieldValues is not null)
                        node.FieldValues[row.Field] = row.Value;
                };
                _flowFields.Add(row);
            }
            while (arrays.Count > 0)
                _flowFields.Add(arrays.Dequeue());
        }

        /// <summary>Creates an array row for a Message node: restores the saved element
        /// count and cell values and syncs edits back into the node.</summary>
        private static FieldRow CreateFlowArrayRow(ArrayInfo array, FlowNodeVisual node)
        {
            var count = Math.Min(1, array.MaxCount);
            if (node.FieldValues is not null &&
                node.FieldValues.TryGetValue(array.Name + "#count", out var saved) &&
                int.TryParse(saved, out var n))
                count = n;
            var row = new FieldRow(array, count);
            WireFlowArrayCells(row, node);
            return row;
        }

        /// <summary>Restores the saved cell values of an array row and syncs edits into the node.</summary>
        private static void WireFlowArrayCells(FieldRow row, FlowNodeVisual node)
        {
            node.FieldValues ??= new Dictionary<string, string>();
            node.FieldValues[row.Field + "#count"] = row.CountText;
            foreach (var cell in row.Cells)
            {
                if (node.FieldValues.TryGetValue(cell.Field, out var v)) cell.Value = v;
                cell.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName == nameof(ArrayCellRow.Value) && node.FieldValues is not null)
                        node.FieldValues[cell.Field] = cell.Value;
                };
            }
        }

        /// <summary>Apply button inside a Message node's array row: rebuilds the cells to the
        /// requested count, restores saved values and updates the array's count field.</summary>
        private void ApplyFlowArrayCount_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFlowNode is not { Kind: FlowNodeKind.Message } node) return;
            if ((sender as FrameworkElement)?.DataContext is not FieldRow { ArrayDef: { } array } row) return;
            var applied = row.RebuildCells(int.TryParse(row.CountText, out var n) ? n : 0);
            WireFlowArrayCells(row, node);
            if (array.CountField is not null)
                foreach (var f in _flowFields)
                    if (!f.IsArray && f.Field == array.CountField) f.Value = applied.ToString();
        }

        /// <summary>Builds the field rows a Message node sends from its saved values over the defaults.</summary>
        private static List<FieldRow> BuildFlowFields(MessageInfo info, FlowNodeVisual node)
        {
            var list = new List<FieldRow>(info.Fields.Length);
            foreach (var f in info.Fields)
            {
                var row = new FieldRow(f) { Value = f.DefaultValue };
                if (node.FieldValues is not null && node.FieldValues.TryGetValue(f.Field, out var v)) row.Value = v;
                list.Add(row);
            }
            return list;
        }

        /// <summary>Builds the array cells a Message node sends from its saved values.</summary>
        private static List<ArrayCellRow> BuildFlowArrayCells(MessageInfo info, FlowNodeVisual node)
        {
            var cells = new List<ArrayCellRow>();
            if (node.FieldValues is null) return cells;
            foreach (var array in info.Arrays)
            {
                var count = 0;
                if (node.FieldValues.TryGetValue(array.Name + "#count", out var saved))
                    _ = int.TryParse(saved, out count);
                count = Math.Clamp(count, 0, array.MaxCount);
                for (int i = 0; i < count; i++)
                    foreach (var el in array.Elements)
                    {
                        var field = el.Field.Replace($"[{array.IndexVar}]", $"[{i}]", StringComparison.Ordinal);
                        cells.Add(new ArrayCellRow
                        {
                            Index = i,
                            Offset = array.BaseOffset + i * array.Stride + el.RelativeOffset,
                            Field = field,
                            Type = el.Type,
                            Value = node.FieldValues.TryGetValue(field, out var v) ? v : "0"
                        });
                    }
            }
            return cells;
        }

        // ---- Flow branching (Condition True/False), zoom and the steps table ----
        /// <summary>The node a given branch points to from this node.</summary>
        private FlowNodeVisual? BranchTarget(FlowNodeVisual node, FlowBranch branch) =>
            _flowConnections.FirstOrDefault(c => ReferenceEquals(c.From, node) && c.Branch == branch)?.To;

        /// <summary>Sets (or clears) the target of a branch from a node, replacing any existing one.</summary>
        private void SetBranchTarget(FlowNodeVisual from, FlowBranch branch, FlowNodeVisual? to)
        {
            _flowConnections.RemoveAll(c => ReferenceEquals(c.From, from) && c.Branch == branch);
            if (to is not null && !ReferenceEquals(to, from))
                _flowConnections.Add(new FlowConnection { From = from, To = to, Branch = branch });
            RedrawFlow();
        }

        /// <summary>Fills the On True / On False combos of a Condition node with the other nodes.</summary>
        private void PopulateBranchCombos(FlowNodeVisual node)
        {
            var choices = new List<FlowNodeVisual?> { null };
            choices.AddRange(_flowNodes.Where(n => !ReferenceEquals(n, node)));
            FlowOnTrueCombo.ItemsSource = choices;
            FlowOnFalseCombo.ItemsSource = new List<FlowNodeVisual?>(choices);
            FlowOnTrueCombo.SelectedItem = BranchTarget(node, FlowBranch.True);
            FlowOnFalseCombo.SelectedItem = BranchTarget(node, FlowBranch.False);
        }

        private void FlowOnTrueCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            SetBranchTarget(node, FlowBranch.True, FlowOnTrueCombo.SelectedItem as FlowNodeVisual);
        }

        private void FlowOnFalseCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            SetBranchTarget(node, FlowBranch.False, FlowOnFalseCombo.SelectedItem as FlowNodeVisual);
        }

        private void FlowIgnoreTimeout_Click(object sender, RoutedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            node.IgnoreTimeout = FlowIgnoreTimeoutCheck.IsChecked == true;
        }

        // ---- Flow Condition: optional logical test on a field of the awaited message ----
        private const string NoConditionField = "(no field - just wait)";

        /// <summary>Fills the condition field combo with the fields of the awaited message.</summary>
        private void PopulateConditionFields(FlowNodeVisual node)
        {
            FlowCondFieldCombo.Items.Clear();
            FlowCondFieldCombo.Items.Add(NoConditionField);
            var info = MessageCatalog.ByName(node.WaitMessage);
            if (info is not null)
                foreach (var f in info.Fields) FlowCondFieldCombo.Items.Add(f.Field);
            FlowCondFieldCombo.SelectedItem = string.IsNullOrEmpty(node.ConditionField) ? NoConditionField : node.ConditionField;
            if (FlowCondFieldCombo.SelectedItem is null) FlowCondFieldCombo.SelectedIndex = 0;
        }

        /// <summary>Selects the operator combo item matching the node's operator.</summary>
        private void SelectConditionOperator(string op)
        {
            foreach (System.Windows.Controls.ComboBoxItem item in FlowCondOperatorCombo.Items)
                if ((item.Content as string) == op) { FlowCondOperatorCombo.SelectedItem = item; return; }
            FlowCondOperatorCombo.SelectedIndex = 0;
        }

        private void FlowCondFieldCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            var field = FlowCondFieldCombo.SelectedItem as string ?? string.Empty;
            node.ConditionField = field == NoConditionField ? string.Empty : field;
            RedrawFlow();
        }

        private void FlowCondOperatorCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            node.ConditionOperator = (FlowCondOperatorCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? ">=";
            RedrawFlow();
        }

        private void FlowCondValueBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            node.ConditionValue = FlowCondValueBox.Text;
            RedrawFlow();
        }

        /// <summary>Evaluates the optional field condition of a Condition node against a
        /// received message; true when no field is chosen (just waiting for the message).</summary>
        private bool EvaluateFieldCondition(FlowNodeVisual node, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(node.ConditionField)) return true;
            var info = MessageCatalog.ByName(node.WaitMessage);
            var field = info?.Fields.FirstOrDefault(f => f.Field == node.ConditionField);
            if (field is null) return false;
            return CompareFlowValues(field.Read(data), node.ConditionOperator, node.ConditionValue);
        }

        /// <summary>Compares two field values numerically when possible, else as text.</summary>
        private static bool CompareFlowValues(string actual, string op, string expected)
        {
            if (double.TryParse(actual, NumberStyles.Float, CultureInfo.InvariantCulture, out var a) &&
                double.TryParse(expected, NumberStyles.Float, CultureInfo.InvariantCulture, out var b))
            {
                return op switch
                {
                    ">" => a > b,
                    ">=" => a >= b,
                    "<" => a < b,
                    "<=" => a <= b,
                    "==" => Math.Abs(a - b) < 1e-9,
                    "!=" => Math.Abs(a - b) >= 1e-9,
                    _ => false
                };
            }
            var cmp = string.Compare(actual?.Trim(), expected?.Trim(), StringComparison.OrdinalIgnoreCase);
            return op switch
            {
                "==" => cmp == 0,
                "!=" => cmp != 0,
                ">" => cmp > 0,
                ">=" => cmp >= 0,
                "<" => cmp < 0,
                "<=" => cmp <= 0,
                _ => false
            };
        }

        // ---- Flow Condition: send a message depending on the True / False result ----
        private const string NoSendMessage = "(none)";

        /// <summary>Fills the On True / On False "Send Message" combos with all messages.</summary>
        private void PopulateConditionSendCombos(FlowNodeVisual node)
        {
            FlowTrueSendCombo.Items.Clear();
            FlowFalseSendCombo.Items.Clear();
            FlowTrueSendCombo.Items.Add(NoSendMessage);
            FlowFalseSendCombo.Items.Add(NoSendMessage);
            foreach (var m in MessageCatalog.Messages)
            {
                FlowTrueSendCombo.Items.Add(m.Name);
                FlowFalseSendCombo.Items.Add(m.Name);
            }
            FlowTrueSendCombo.SelectedItem = string.IsNullOrEmpty(node.TrueSendMessage) ? NoSendMessage : node.TrueSendMessage;
            FlowFalseSendCombo.SelectedItem = string.IsNullOrEmpty(node.FalseSendMessage) ? NoSendMessage : node.FalseSendMessage;
            if (FlowTrueSendCombo.SelectedItem is null) FlowTrueSendCombo.SelectedIndex = 0;
            if (FlowFalseSendCombo.SelectedItem is null) FlowFalseSendCombo.SelectedIndex = 0;
        }

        private void FlowTrueSendCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            var name = FlowTrueSendCombo.SelectedItem as string ?? string.Empty;
            node.TrueSendMessage = name == NoSendMessage ? string.Empty : name;
        }

        private void FlowFalseSendCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Condition } node) return;
            var name = FlowFalseSendCombo.SelectedItem as string ?? string.Empty;
            node.FalseSendMessage = name == NoSendMessage ? string.Empty : name;
        }

        /// <summary>Sends a catalog message by name with its default field values (used by
        /// Condition and Loop nodes to emit a message on their result / each iteration).</summary>
        private void SendFlowMessageByName(string messageName, string result)
        {
            if (string.IsNullOrWhiteSpace(messageName)) return;
            var info = MessageCatalog.ByName(messageName);
            if (info is null) { Log($"Flow: unknown message '{messageName}' ({result})."); return; }
            var fields = info.Fields.Select(f => new FieldRow(f) { Value = f.DefaultValue }).ToList();
            SendMessageCore(info, fields, null, false, AutoSequenceCheck.IsChecked == true,
                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);
            Log($"Flow: {result} -> sent {info.Name}.");
        }

        /// <summary>Fills the Loop node's "Send Message" combo with all messages.</summary>
        private void PopulateLoopSendCombo(FlowNodeVisual node)
        {
            FlowLoopSendCombo.Items.Clear();
            FlowLoopSendCombo.Items.Add(NoSendMessage);
            foreach (var m in MessageCatalog.Messages) FlowLoopSendCombo.Items.Add(m.Name);
            FlowLoopSendCombo.SelectedItem = string.IsNullOrEmpty(node.LoopSendMessage) ? NoSendMessage : node.LoopSendMessage;
            if (FlowLoopSendCombo.SelectedItem is null) FlowLoopSendCombo.SelectedIndex = 0;
        }

        private void FlowLoopSendCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_loadingFlowProps || _selectedFlowNode is not { Kind: FlowNodeKind.Loop } node) return;
            var name = FlowLoopSendCombo.SelectedItem as string ?? string.Empty;
            node.LoopSendMessage = name == NoSendMessage ? string.Empty : name;
            RedrawFlow();
        }

        private void FlowZoomIn_Click(object sender, RoutedEventArgs e) => SetFlowZoom(_flowZoom * 1.25);
        private void FlowZoomOut_Click(object sender, RoutedEventArgs e) => SetFlowZoom(_flowZoom / 1.25);
        private void FlowZoomReset_Click(object sender, RoutedEventArgs e) => SetFlowZoom(1.0);

        /// <summary>Scales the whole designer canvas (with the scrollbars adapting).</summary>
        private void SetFlowZoom(double zoom)
        {
            _flowZoom = Math.Clamp(zoom, 0.25, 4.0);
            if (FlowZoomTransform is not null) { FlowZoomTransform.ScaleX = _flowZoom; FlowZoomTransform.ScaleY = _flowZoom; }
            if (FlowZoomText is not null) FlowZoomText.Text = $"{_flowZoom * 100:0}%";
        }

        /// <summary>Rebuilds the bottom Scenario Steps table from the current nodes and links.</summary>
        private void RefreshFlowSteps()
        {
            _flowStepRows.Clear();
            foreach (var node in _flowNodes)
            {
                var next = node.Kind == FlowNodeKind.Condition
                    ? $"True -> {BranchTarget(node, FlowBranch.True)?.Label ?? "(end)"};  False -> {BranchTarget(node, FlowBranch.False)?.Label ?? "(end)"}"
                    : NextFlowNode(node)?.Label ?? "(end)";
                _flowStepRows.Add(new FlowStepRow
                {
                    Index = _flowOrder.TryGetValue(node, out var order) ? order : 0,
                    Type = node.Kind.ToString(),
                    Message = node.Kind == FlowNodeKind.Message ? node.Title : "-",
                    Details = node.Caption.Replace("\n", "  "),
                    Next = next
                });
            }
        }

        // ---- Flow execution: play the nodes in the order set by the arrows ----
        /// <summary>The next node the arrows point to from this one (prefers the Normal
        /// link, then the True branch, so linear steps and conditions both flow forward).</summary>
        private FlowNodeVisual? NextFlowNode(FlowNodeVisual node) =>
            (BranchTarget(node, FlowBranch.Normal)
             ?? BranchTarget(node, FlowBranch.True)
             ?? _flowConnections.FirstOrDefault(c => ReferenceEquals(c.From, node))?.To);

        /// <summary>The start node is the one no arrow points to (else the first node).</summary>
        private FlowNodeVisual? FindFlowStart()
        {
            var targets = _flowConnections.Select(c => c.To).ToHashSet();
            return _flowNodes.FirstOrDefault(n => !targets.Contains(n)) ?? _flowNodes.FirstOrDefault();
        }

        /// <summary>Numbers the nodes by following the arrows from the start, so each node
        /// can show its position in the send order.</summary>
        private void ComputeFlowOrder()
        {
            _flowOrder.Clear();
            var node = FindFlowStart();
            var index = 1;
            while (node is not null && !_flowOrder.ContainsKey(node))
            {
                _flowOrder[node] = index++;
                node = NextFlowNode(node);
            }
        }

        /// <summary>Run Scenario: executes the nodes following the arrows. A Message is sent,
        /// a Delay waits, a Condition waits for its message (with timeout) and a Loop repeats
        /// back to its target node the configured number of times.</summary>
        private async void RunFlow_Click(object sender, RoutedEventArgs e)
        {
            if (_flowRunning) { Log("Flow is already running."); return; }
            if (_transport is null) { Log("Start the transport before running the flow."); return; }
            if (_flowNodes.Count == 0) { Log("The flow has no nodes."); return; }
            _flowRunning = true;
            _flowRunCts = new System.Threading.CancellationTokenSource();
            try { await RunFlowAsync(_flowRunCts.Token); }
            catch (OperationCanceledException) { Log("Flow: stopped."); }
            catch (Exception ex) { Log("Flow error: " + ex.Message); }
            finally { _flowRunning = false; _runningFlowNode = null; RedrawFlow(); }
        }

        private void StopFlow_Click(object sender, RoutedEventArgs e)
        {
            if (!_flowRunning) return;
            _flowRunCts?.Cancel();
            Log("Flow: stop requested.");
        }

        /// <summary>Walks the flow from the start node following the connection arrows.</summary>
        private async Task RunFlowAsync(System.Threading.CancellationToken token)
        {
            var node = FindFlowStart();
            if (node is null) { Log("Flow: no start node."); return; }
            var loopRemaining = new Dictionary<FlowNodeVisual, int>();
            var steps = 0;
            Log("Flow: run started.");
            while (node is not null && !token.IsCancellationRequested)
            {
                if (steps++ > 10000) { Log("Flow: step limit reached, stopping."); break; }
                _runningFlowNode = node;
                RedrawFlow();
                switch (node.Kind)
                {
                    case FlowNodeKind.Message:
                        var info = MessageCatalog.ByName(node.Title);
                        if (info is null) { Log($"Flow: unknown message '{node.Title}'."); }
                        else
                        {
                            var fields = BuildFlowFields(info, node);
                            var arrayCells = BuildFlowArrayCells(info, node);
                            SendMessageCore(info, fields, arrayCells, false, AutoSequenceCheck.IsChecked == true,
                                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);
                            Log($"Flow: sent {info.Name}.");
                        }
                        break;
                    case FlowNodeKind.Delay:
                        Log($"Flow: delay {node.DelayMs} ms.");
                        await Task.Delay(Math.Max(1, node.DelayMs), token);
                        break;
                    case FlowNodeKind.Condition:
                        var condText = string.IsNullOrWhiteSpace(node.ConditionField)
                            ? string.Empty
                            : $" [{node.ConditionField} {node.ConditionOperator} {node.ConditionValue}]";
                        Log($"Flow: waiting for {node.WaitMessage}{condText} (timeout {node.TimeoutMs} ms).");
                        var received = await WaitForFlowMessageAsync(node.WaitMessage, node.TimeoutMs, token);
                        if (token.IsCancellationRequested) break;
                        bool conditionPass;
                        if (received is null)
                        {
                            conditionPass = node.IgnoreTimeout;
                            Log(conditionPass
                                ? $"Flow: timeout on {node.WaitMessage} but Ignore Timeout is set -> True branch."
                                : $"Flow: timeout waiting for {node.WaitMessage} -> False branch.");
                        }
                        else
                        {
                            conditionPass = EvaluateFieldCondition(node, received);
                            Log(conditionPass
                                ? $"Flow: {node.WaitMessage} received and condition met -> True branch."
                                : $"Flow: {node.WaitMessage} received but condition not met -> False branch.");
                        }
                        // Send the message configured for this result (if any).
                        SendFlowMessageByName(conditionPass ? node.TrueSendMessage : node.FalseSendMessage, conditionPass ? "condition True" : "condition False");
                        node = BranchTarget(node, conditionPass ? FlowBranch.True : FlowBranch.False);
                        continue;
                    case FlowNodeKind.Loop:
                        if (!loopRemaining.TryGetValue(node, out var rem)) rem = Math.Max(1, node.LoopCount);
                        // Send the loop's message on each pass (when one is selected).
                        SendFlowMessageByName(node.LoopSendMessage, $"loop {node.LoopCount - rem + 1}/{node.LoopCount}");
                        var back = NextFlowNode(node);
                        if (rem > 1)
                        {
                            loopRemaining[node] = rem - 1;
                            Log($"Flow: loop iteration done ({rem - 1} more).");
                            node = back ?? node;
                            continue;
                        }
                        loopRemaining.Remove(node);
                        Log("Flow: loop finished.");
                        node = null;
                        continue;
                }
                node = NextFlowNode(node);
            }
            Log("Flow: run finished.");
        }

        /// <summary>Completes when a message with the given name is received, or on timeout.</summary>
        private async Task<byte[]?> WaitForFlowMessageAsync(string message, int timeoutMs, System.Threading.CancellationToken token)
        {
            var tcs = new TaskCompletionSource<byte[]?>(TaskCreationOptions.RunContinuationsAsynchronously);
            _flowWaitMessage = message;
            _flowWaitTcs = tcs;
            using var timeoutCts = System.Threading.CancellationTokenSource.CreateLinkedTokenSource(token);
            timeoutCts.CancelAfter(Math.Max(1, timeoutMs));
            using (timeoutCts.Token.Register(() => tcs.TrySetResult(null)))
            {
                try { return await tcs.Task; }
                finally { _flowWaitTcs = null; _flowWaitMessage = null; }
            }
        }

        /// <summary>Releases a Condition node that is waiting for the received message,
        /// handing it the raw bytes so its field condition can be evaluated.</summary>
        private void CheckFlowWait(string name, byte[] data)
        {
            if (_flowWaitTcs is not null && string.Equals(_flowWaitMessage, name, StringComparison.Ordinal))
                _flowWaitTcs.TrySetResult(data);
        }

        private void FlowClear_Click(object sender, RoutedEventArgs e)
        {
            _flowRunCts?.Cancel();
            _flowNodes.Clear();
            _flowConnections.Clear();
            _flowConnectStart = null;
            _runningFlowNode = null;
            _flowNodeSeq = 0;
            SelectFlowNode(null);
            Log("Flow: cleared the designer.");
        }

        // ---- Recording: capture all TX/RX messages to a binary file, browse them back ----
        private void Record_Click(object sender, RoutedEventArgs e)
        {
            if (_recorder.IsRecording)
            {
                var path = _recorder.Stop();
                RecordButton.Content = "\u25CF Record";
                RecordButton.Background = System.Windows.Media.Brushes.MediumPurple;
                Log($"Recording stopped: {_recorder.RecordedCount} message(s) written to {path}.");
                return;
            }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Recording|*.bin",
                FileName = $"Recording_{DateTime.Now:yyyyMMdd_HHmmss}.bin"
            };
            if (dlg.ShowDialog() != true) return;
            try
            {
                _recorder.Start(dlg.FileName);
                RecordButton.Content = "\u25A0 Stop Rec";
                RecordButton.Background = System.Windows.Media.Brushes.Firebrick;
                Log($"Recording started: {dlg.FileName}.");
            }
            catch (Exception ex) { Log("Record error: " + ex.Message); }
        }

        private void OpenRecording_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Recording|*.bin|All files|*.*" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                _loadedRecords = RecordingFile.Load(dlg.FileName);
                RecordFileText.Text = Path.GetFileName(dlg.FileName);
                RecordSummaryText.Text = $"{_loadedRecords.Count} message(s)";

                // Left side: one entry per message type found in the file.
                RecordTypesList.Items.Clear();
                foreach (var group in _loadedRecords
                    .GroupBy(r => r.MessageId)
                    .OrderBy(g => g.Key))
                {
                    var name = MessageCatalog.ById(group.Key)?.Name ?? $"Unknown (0x{group.Key:X})";
                    RecordTypesList.Items.Add($"{name} [{group.Count()}]");
                }
                RecordedMessagesGrid.ItemsSource = null;
                if (RecordTypesList.Items.Count > 0) RecordTypesList.SelectedIndex = 0;
                Log($"Recording loaded: {dlg.FileName} ({_loadedRecords.Count} message(s)).");
            }
            catch (Exception ex) { Log("Open recording error: " + ex.Message); }
        }

        /// <summary>Shows every recorded message of the type selected on the left.</summary>
        private void RecordTypesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecordTypesList.SelectedItem is not string entry) return;
            var name = entry[..entry.LastIndexOf(" [", StringComparison.Ordinal)];
            var info = MessageCatalog.ByName(name);
            var rows = new List<RecordedMessageRow>();
            var ordinal = 1;
            foreach (var record in _loadedRecords)
            {
                var recordName = MessageCatalog.ById(record.MessageId)?.Name ?? $"Unknown (0x{record.MessageId:X})";
                if (info is null ? recordName != name : record.MessageId != info.MessageId) continue;
                rows.Add(new RecordedMessageRow
                {
                    Ordinal = ordinal++,
                    Time = record.Timestamp.ToString("HH:mm:ss.fff"),
                    Direction = record.IsOutgoing ? "TX (MBT \u2192 NIRON)" : "RX (NIRON \u2192 MBT)",
                    Message = recordName,
                    MessageId = record.MessageId,
                    Bytes = record.Data.Length,
                    Hex = Convert.ToHexString(record.Data),
                    Data = record.Data
                });
            }
            RecordedMessagesGrid.ItemsSource = rows;
            RecordFieldsGrid.ItemsSource = null;
            RecordFieldsHeader.Text = "Message Fields (select a row)";
        }

        /// <summary>Decodes and shows every field of the recorded message clicked in the table.</summary>
        private void RecordedMessagesGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecordedMessagesGrid.SelectedItem is not RecordedMessageRow row)
            {
                RecordFieldsHeader.Text = "Message Fields (select a row)";
                RecordFieldsGrid.ItemsSource = null;
                return;
            }
            var info = MessageCatalog.ById(row.MessageId);
            if (info is null)
            {
                RecordFieldsHeader.Text = $"Message Fields (unknown message 0x{row.MessageId:X})";
                RecordFieldsGrid.ItemsSource = null;
                return;
            }
            RecordFieldsHeader.Text = $"{row.Message}  #{row.Ordinal}  {row.Time}";
            RecordFieldsGrid.ItemsSource = info.Fields
                .Select(f => new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(row.Data) })
                .ToList();
        }

        /// <summary>Exports every recorded message of the selected type to CSV:
        /// one column per message field, one row per recorded message.</summary>
        private void ExportRecordType_Click(object sender, RoutedEventArgs e)
        {
            if (RecordTypesList.SelectedItem is not string entry) { Log("Select a message type to export."); return; }
            var name = entry[..entry.LastIndexOf(" [", StringComparison.Ordinal)];
            var info = MessageCatalog.ByName(name);
            if (info is null) { Log($"Cannot export '{name}': unknown message type."); return; }
            var records = _loadedRecords.Where(r => r.MessageId == info.MessageId).ToList();
            if (records.Count == 0) { Log($"No recorded messages of type '{name}'."); return; }

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "CSV|*.csv",
                FileName = $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (dlg.ShowDialog() != true) return;
            try
            {
                var sb = new StringBuilder();
                // Header: the fields of the message.
                sb.Append("#,Time,Direction");
                foreach (var f in info.Fields) sb.Append(',').Append(Csv(f.Field));
                sb.AppendLine();

                // One row per recorded message of this type, values decoded from the raw bytes.
                var ordinal = 1;
                foreach (var record in records)
                {
                    sb.Append(ordinal++).Append(',');
                    sb.Append(record.Timestamp.ToString("HH:mm:ss.fff")).Append(',');
                    sb.Append(record.IsOutgoing ? "TX" : "RX");
                    foreach (var f in info.Fields) sb.Append(',').Append(Csv(f.Read(record.Data)));
                    sb.AppendLine();
                }
                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                Log($"Export_{name}: {records.Count} message(s) written to {dlg.FileName}.");
            }
            catch (Exception ex) { Log("Export type error: " + ex.Message); }
        }

        /// <summary>Escapes one CSV cell.</summary>
        private static string Csv(string value) =>
            value.Contains(',') || value.Contains('"') || value.Contains('\n')
                ? "\"" + value.Replace("\"", "\"\"") + "\""
                : value;

        // ---- Graph tab: live message timeline (TX lane / RX lane) ----
        private void AddGraphFilter(string messageName)
        {
            var check = new System.Windows.Controls.CheckBox
            {
                Content = messageName, Tag = messageName, IsChecked = true, Margin = new Thickness(2)
            };
            check.SetResourceReference(System.Windows.Controls.CheckBox.ForegroundProperty, "Theme.Foreground");
            check.Checked += (_, _) => RedrawTimeline();
            check.Unchecked += (_, _) => RedrawTimeline();
            GraphFilterPanel.Children.Add(check);
        }

        /// <summary>Hard cap so endless traffic cannot exhaust memory or slow the repaint.</summary>
        private const int MaxTimelineEvents = 5000;

        /// <summary>Stores one timeline point and marks the graph dirty; the refresh timer
        /// repaints it (throttled) so heavy traffic cannot freeze the UI.</summary>
        private void RecordTimelineEvent(bool outgoing, string message, string status, string details, byte[] data)
        {
            _timelineEvents.Add(new TimelineEvent
            {
                Timestamp = DateTime.Now, IsOutgoing = outgoing,
                Message = message, Status = status, Details = details, Data = data
            });
            _messageCounts[message] = _messageCounts.TryGetValue(message, out var count) ? count + 1 : 1;
            // Do not call RedrawTimeline here. At 10 ms traffic rates that would force
            // a full canvas rebuild for every packet. The timer above coalesces repaint work.
            // Drop the oldest chunk when over the cap (chunked so the cost is amortized).
            if (_timelineEvents.Count > MaxTimelineEvents)
                _timelineEvents.RemoveRange(0, 1000);
            _timelineDirty = true;
        }

        /// <summary>Pause freezes the timeline display; events keep accumulating in the background.</summary>
        private void GraphPause_Click(object sender, RoutedEventArgs e)
        {
            _graphPaused = !_graphPaused;
            GraphPauseButton.Content = _graphPaused ? "\u25B6 Resume" : "\u23F8 Pause";
            GraphPauseButton.Background = _graphPaused
                ? System.Windows.Media.Brushes.Firebrick
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0x88, 0x00));
            if (!_graphPaused) RedrawTimeline();
            Log(_graphPaused ? "Graph paused (events keep accumulating)." : "Graph resumed.");
        }

        private void GraphZoomIn_Click(object sender, RoutedEventArgs e) => SetGraphZoom(_graphZoom * 1.25);

        private void GraphZoomOut_Click(object sender, RoutedEventArgs e) => SetGraphZoom(_graphZoom / 1.25);

        private void GraphFit_Click(object sender, RoutedEventArgs e) => SetGraphZoom(1.0);

        private void SetGraphZoom(double zoom)
        {
            _graphZoom = Math.Clamp(zoom, 0.25, 20.0);
            GraphZoomText.Text = $"{_graphZoom * 100:0}%";
            RedrawTimeline();
        }

        /// <summary>Ctrl + mouse wheel zooms the timeline in/out around the cursor position.</summary>
        private void GraphScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            var mouseX = e.GetPosition(GraphScroll).X;
            var contentX = GraphScroll.HorizontalOffset + mouseX;
            var oldZoom = _graphZoom;
            SetGraphZoom(e.Delta > 0 ? _graphZoom * 1.25 : _graphZoom / 1.25);
            GraphScroll.ScrollToHorizontalOffset(Math.Max(0, contentX * (_graphZoom / oldZoom) - mouseX));
            e.Handled = true;
        }

        private void GraphOptions_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RedrawTimeline();

        private void ClearTimeline_Click(object sender, RoutedEventArgs e)
        {
            _timelineEvents.Clear();
            _messageCounts.Clear();
            RedrawTimeline();
            GraphFieldsGrid.ItemsSource = null;
            GraphFieldsHeader.Text = "Message Fields (click a point or row)";
            Log("Timeline cleared.");
        }

        /// <summary>Shows every field of the timeline event selected in the details table.</summary>
        private void TimelineDetailsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TimelineDetailsGrid.SelectedItem is TimelineEvent ev) ShowTimelineEventFields(ev);
        }

        /// <summary>Decodes all the fields of one timeline event into the Message Fields panel.</summary>
        private void ShowTimelineEventFields(TimelineEvent ev)
        {
            var info = MessageCatalog.ByName(ev.Message);
            if (info is null || ev.Data.Length == 0)
            {
                GraphFieldsHeader.Text = $"Message Fields ({ev.Message}: no decode available)";
                GraphFieldsGrid.ItemsSource = null;
                return;
            }
            GraphFieldsHeader.Text = $"{ev.Message}  {ev.Time}  ({(ev.IsOutgoing ? "TX" : "RX")})";
            GraphFieldsGrid.ItemsSource = info.Fields
                .Select(f => new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(ev.Data) })
                .ToList();
        }

        /// <summary>The events that pass the message and direction filters, in time order.</summary>
        private List<TimelineEvent> FilteredTimelineEvents()
        {
            var enabled = GraphFilterPanel.Children.OfType<System.Windows.Controls.CheckBox>()
                .Where(c => c.IsChecked == true)
                .Select(c => c.Tag?.ToString())
                .ToHashSet();
            var direction = (GraphDirectionCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "All";
            var known = MessageCatalog.Messages.Select(m => m.Name).ToHashSet();
            return _timelineEvents
                .Where(ev => enabled.Contains(ev.Message) || !known.Contains(ev.Message))
                .Where(ev => direction == "All" ||
                             (direction == "MBT \u2192 NIRON" && ev.IsOutgoing) ||
                             (direction == "NIRON \u2192 MBT" && !ev.IsOutgoing))
                .OrderBy(ev => ev.Timestamp)
                .ToList();
        }

        /// <summary>Clicking a timeline point selects and scrolls to its row in the Details
        /// table; the selection change also decodes the message fields.</summary>
        private void FocusTimelineEvent(TimelineEvent ev)
        {
            TimelineDetailsGrid.SelectedItem = ev;
            TimelineDetailsGrid.ScrollIntoView(ev);
            TimelineDetailsGrid.Focus();
            ShowTimelineEventFields(ev);
        }

        /// <summary>Draws the Graph tab timeline from the live events.</summary>
        private void RedrawTimeline()
        {
            if (TimelineCanvas is null) return;
            _timelineDirty = false;
            UpdateGraphFilterCounts();
            var events = FilteredTimelineEvents();
            TimelineDetailsGrid.ItemsSource = events;
            DrawTimelineCore(TimelineCanvas, GraphScroll, events, _graphZoom, FocusTimelineEvent);
        }

        /// <summary>Shows the number of sent/received messages next to each message filter.</summary>
        private void UpdateGraphFilterCounts()
        {
            foreach (var check in GraphFilterPanel.Children.OfType<System.Windows.Controls.CheckBox>())
            {
                var name = check.Tag?.ToString() ?? string.Empty;
                check.Content = _messageCounts.TryGetValue(name, out var count) && count > 0
                    ? $"{name} [{count}]"
                    : name;
            }
        }

        /// <summary>Shared two-lane timeline renderer (TX on top, RX below) with time axis and
        /// labels, used by both the Graph and the Record Graph tabs.</summary>
        private static void DrawTimelineCore(System.Windows.Controls.Canvas canvas, System.Windows.Controls.ScrollViewer scroll,
            List<TimelineEvent> events, double zoom, Action<TimelineEvent> onDotClick)
        {
            canvas.Children.Clear();
            if (events.Count == 0) { canvas.Width = double.NaN; return; }

            // Rendering caps keep the canvas responsive under heavy traffic: only the newest
            // events are drawn (the Details table still lists all of them) and per-event
            // labels are skipped for dense views.
            const int maxRendered = 400;
            var total = events.Count;
            if (events.Count > maxRendered)
                events = events.GetRange(events.Count - maxRendered, maxRendered);
            var drawLabels = events.Count <= 150;

            var start = events[0].Timestamp;
            var end = events[^1].Timestamp;
            var span = Math.Max((end - start).TotalMilliseconds, 1);

            const double leftPad = 70, rightPad = 40;
            var viewWidth = Math.Max(scroll.ViewportWidth - 4, 900);
            var width = Math.Max(viewWidth, viewWidth * zoom);
            canvas.Width = width + leftPad + rightPad;

            var height = canvas.Height;
            double txLane = height * 0.30, rxLane = height * 0.62, axisY = height - 46;
            // My side (MBT, TX) is always green; the other brushes follow the active theme.
            var txBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x23, 0x86, 0x36));
            var rxBrush = canvas.TryFindResource("Theme.GraphRx") as System.Windows.Media.Brush
                ?? System.Windows.Media.Brushes.White;
            var laneBrush = canvas.TryFindResource("Theme.Border") as System.Windows.Media.Brush
                ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x22, 0x30, 0x3C));
            var textBrush = canvas.TryFindResource("Theme.Foreground") as System.Windows.Media.Brush
                ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE6, 0xED, 0xF3));
            var dimBrush = canvas.TryFindResource("Theme.DimForeground") as System.Windows.Media.Brush
                ?? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0x7A, 0x8C, 0xA0));

            if (total > events.Count)
            {
                var note = new System.Windows.Controls.TextBlock
                {
                    Text = $"Showing newest {events.Count} of {total} events - the Details table lists all",
                    Foreground = dimBrush, FontSize = 10, FontStyle = FontStyles.Italic
                };
                System.Windows.Controls.Canvas.SetLeft(note, leftPad + 4);
                System.Windows.Controls.Canvas.SetTop(note, 4);
                canvas.Children.Add(note);
            }

            // Lane base lines + labels.
            foreach (var (y, label) in new[] { (txLane, "MBT \u2192 NIRON"), (rxLane, "NIRON \u2192 MBT") })
            {
                var line = new System.Windows.Shapes.Line
                {
                    X1 = leftPad, X2 = leftPad + width, Y1 = y, Y2 = y,
                    Stroke = laneBrush, StrokeThickness = 2
                };
                canvas.Children.Add(line);
                var text = new System.Windows.Controls.TextBlock
                {
                    Text = label, Foreground = dimBrush, FontSize = 10, FontWeight = FontWeights.Bold
                };
                System.Windows.Controls.Canvas.SetLeft(text, 2);
                System.Windows.Controls.Canvas.SetTop(text, y - 8);
                canvas.Children.Add(text);
            }

            // Time axis with ~8 ticks.
            var axis = new System.Windows.Shapes.Line
            {
                X1 = leftPad, X2 = leftPad + width, Y1 = axisY, Y2 = axisY,
                Stroke = laneBrush, StrokeThickness = 1
            };
            canvas.Children.Add(axis);
            for (int t = 0; t <= 8; t++)
            {
                var x = leftPad + width * t / 8.0;
                var time = start.AddMilliseconds(span * t / 8.0);
                var tick = new System.Windows.Shapes.Line
                {
                    X1 = x, X2 = x, Y1 = axisY - 4, Y2 = axisY + 4,
                    Stroke = dimBrush, StrokeThickness = 1
                };
                canvas.Children.Add(tick);
                var tickText = new System.Windows.Controls.TextBlock
                {
                    Text = time.ToString("HH:mm:ss.fff"), Foreground = dimBrush, FontSize = 9
                };
                System.Windows.Controls.Canvas.SetLeft(tickText, x - 34);
                System.Windows.Controls.Canvas.SetTop(tickText, axisY + 8);
                canvas.Children.Add(tickText);
            }
            var axisLabel = new System.Windows.Controls.TextBlock
            {
                Text = "Time (hh:mm:ss.mmm)", Foreground = dimBrush, FontSize = 10
            };
            System.Windows.Controls.Canvas.SetLeft(axisLabel, leftPad + width / 2 - 60);
            System.Windows.Controls.Canvas.SetTop(axisLabel, axisY + 24);
            canvas.Children.Add(axisLabel);

            // One marker per event: dot on its lane, dashed drop line and a label.
            bool flip = false;
            foreach (var ev in events)
            {
                var x = leftPad + width * (ev.Timestamp - start).TotalMilliseconds / span;
                var laneY = ev.IsOutgoing ? txLane : rxLane;
                var brush = ev.IsOutgoing ? txBrush : rxBrush;

                var drop = new System.Windows.Shapes.Line
                {
                    X1 = x, X2 = x, Y1 = laneY, Y2 = axisY,
                    Stroke = brush, StrokeThickness = 1,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection { 3, 3 }, Opacity = 0.5
                };
                canvas.Children.Add(drop);

                var dot = new System.Windows.Shapes.Ellipse { Width = 10, Height = 10, Fill = brush, Cursor = Cursors.Hand };
                System.Windows.Controls.Canvas.SetLeft(dot, x - 5);
                System.Windows.Controls.Canvas.SetTop(dot, laneY - 5);
                dot.ToolTip = $"{ev.Message}\n{ev.Time}\n{ev.Direction}\n{ev.Details}\nClick to focus it in Details and show all fields";
                dot.MouseLeftButtonDown += (_, _) => onDotClick(ev);
                canvas.Children.Add(dot);

                // Labels are skipped in dense views (tooltips still show the details).
                if (!drawLabels) continue;

                // Alternate label rows above/below the lane to reduce overlap.
                var labelY = ev.IsOutgoing ? laneY - (flip ? 46 : 30) : laneY + (flip ? 26 : 10);
                flip = !flip;
                var label = new System.Windows.Controls.TextBlock
                {
                    Text = $"{ev.Message}\n{ev.Time}", Foreground = textBrush,
                    FontSize = 9, TextAlignment = TextAlignment.Center
                };
                System.Windows.Controls.Canvas.SetLeft(label, x - 40);
                System.Windows.Controls.Canvas.SetTop(label, labelY);
                canvas.Children.Add(label);
            }
        }

        // ---- Record Graph tab: timeline view of a recorded .bin file ----
        private void OpenRecordGraph_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = "Recording|*.bin|All files|*.*" };
            if (dlg.ShowDialog() != true) return;
            try
            {
                var records = RecordingFile.Load(dlg.FileName);
                _recordGraphEvents.Clear();
                foreach (var record in records)
                {
                    var name = MessageCatalog.ById(record.MessageId)?.Name ?? $"Unknown (0x{record.MessageId:X})";
                    _recordGraphEvents.Add(new TimelineEvent
                    {
                        Timestamp = record.Timestamp, IsOutgoing = record.IsOutgoing,
                        Message = name, Status = record.IsOutgoing ? "SENT" : "RECEIVED",
                        Details = $"{record.Data.Length} bytes (recorded)", Data = record.Data
                    });
                }
                RecordGraphFileText.Text = Path.GetFileName(dlg.FileName);
                RecordGraphSummaryText.Text = $"{_recordGraphEvents.Count} message(s)";

                // One filter checkbox per message type found in the file, with its count.
                RecordGraphFilterPanel.Children.Clear();
                foreach (var group in _recordGraphEvents.GroupBy(ev => ev.Message).OrderBy(g => g.Key))
                {
                    var check = new System.Windows.Controls.CheckBox
                    {
                        Content = $"{group.Key} [{group.Count()}]", Tag = group.Key,
                        IsChecked = true, Margin = new Thickness(2)
                    };
                    check.SetResourceReference(System.Windows.Controls.CheckBox.ForegroundProperty, "Theme.Foreground");
                    check.Checked += (_, _) => RedrawRecordTimeline();
                    check.Unchecked += (_, _) => RedrawRecordTimeline();
                    RecordGraphFilterPanel.Children.Add(check);
                }
                RecordGraphFieldsGrid.ItemsSource = null;
                RecordGraphFieldsHeader.Text = "Message Fields (click a point or row)";
                RedrawRecordTimeline();
                Log($"Record graph loaded: {dlg.FileName} ({_recordGraphEvents.Count} message(s)).");
            }
            catch (Exception ex) { Log("Open record graph error: " + ex.Message); }
        }

        private void RecordGraphZoomIn_Click(object sender, RoutedEventArgs e) => SetRecordGraphZoom(_recordGraphZoom * 1.25);

        private void RecordGraphZoomOut_Click(object sender, RoutedEventArgs e) => SetRecordGraphZoom(_recordGraphZoom / 1.25);

        private void RecordGraphFit_Click(object sender, RoutedEventArgs e) => SetRecordGraphZoom(1.0);

        private void SetRecordGraphZoom(double zoom)
        {
            _recordGraphZoom = Math.Clamp(zoom, 0.25, 20.0);
            RecordGraphZoomText.Text = $"{_recordGraphZoom * 100:0}%";
            RedrawRecordTimeline();
        }

        /// <summary>Ctrl + mouse wheel zooms the recorded timeline around the cursor position.</summary>
        private void RecordGraphScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            var mouseX = e.GetPosition(RecordGraphScroll).X;
            var contentX = RecordGraphScroll.HorizontalOffset + mouseX;
            var oldZoom = _recordGraphZoom;
            SetRecordGraphZoom(e.Delta > 0 ? _recordGraphZoom * 1.25 : _recordGraphZoom / 1.25);
            RecordGraphScroll.ScrollToHorizontalOffset(Math.Max(0, contentX * (_recordGraphZoom / oldZoom) - mouseX));
            e.Handled = true;
        }

        private void RecordGraphOptions_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => RedrawRecordTimeline();

        /// <summary>Shows every field of the recorded event selected in the details table.</summary>
        private void RecordGraphDetailsGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RecordGraphDetailsGrid.SelectedItem is TimelineEvent ev) ShowRecordGraphEventFields(ev);
        }

        /// <summary>Decodes all the fields of one recorded event into the Message Fields panel.</summary>
        private void ShowRecordGraphEventFields(TimelineEvent ev)
        {
            var info = MessageCatalog.ByName(ev.Message);
            if (info is null || ev.Data.Length == 0)
            {
                RecordGraphFieldsHeader.Text = $"Message Fields ({ev.Message}: no decode available)";
                RecordGraphFieldsGrid.ItemsSource = null;
                return;
            }
            RecordGraphFieldsHeader.Text = $"{ev.Message}  {ev.Time}  ({(ev.IsOutgoing ? "TX" : "RX")})";
            RecordGraphFieldsGrid.ItemsSource = info.Fields
                .Select(f => new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(ev.Data) })
                .ToList();
        }

        /// <summary>The recorded events that pass the message and direction filters, in time order.</summary>
        private List<TimelineEvent> FilteredRecordGraphEvents()
        {
            var enabled = RecordGraphFilterPanel.Children.OfType<System.Windows.Controls.CheckBox>()
                .Where(c => c.IsChecked == true)
                .Select(c => c.Tag?.ToString())
                .ToHashSet();
            var direction = (RecordGraphDirectionCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "All";
            return _recordGraphEvents
                .Where(ev => enabled.Contains(ev.Message))
                .Where(ev => direction == "All" ||
                             (direction == "MBT \u2192 NIRON" && ev.IsOutgoing) ||
                             (direction == "NIRON \u2192 MBT" && !ev.IsOutgoing))
                .OrderBy(ev => ev.Timestamp)
                .ToList();
        }

        /// <summary>Clicking a recorded timeline point selects and scrolls to its row in the
        /// Details table; the selection change also decodes the message fields.</summary>
        private void FocusRecordGraphEvent(TimelineEvent ev)
        {
            RecordGraphDetailsGrid.SelectedItem = ev;
            RecordGraphDetailsGrid.ScrollIntoView(ev);
            RecordGraphDetailsGrid.Focus();
            ShowRecordGraphEventFields(ev);
        }

        /// <summary>Draws the Record Graph timeline from the loaded recording.</summary>
        private void RedrawRecordTimeline()
        {
            if (RecordGraphCanvas is null) return;
            var events = FilteredRecordGraphEvents();
            RecordGraphDetailsGrid.ItemsSource = events;
            DrawTimelineCore(RecordGraphCanvas, RecordGraphScroll, events, _recordGraphZoom, FocusRecordGraphEvent);
        }

        // ---- Statistics tab: per-message counters and per-second rates ----
        private void UpdateStat(string message, int bytes, bool outgoing, bool error = false, bool dropped = false, bool crcError = false)
        {
            if (!_messageStats.TryGetValue(message, out var stat))
                _messageStats[message] = stat = new MessageStat { Name = message };
            if (outgoing) { stat.Sent++; stat.BytesSent += bytes; stat.LastSent = DateTime.Now; }
            else { stat.Received++; stat.BytesReceived += bytes; stat.LastReceived = DateTime.Now; }
            if (error) stat.Errors++;
            if (dropped) stat.Dropped++;
            if (crcError) stat.CrcErrors++;
            stat.TotalBytes += bytes;
            stat.WindowCount++;
            stat.WindowBytes += bytes;
            if (bytes < stat.MinSize) stat.MinSize = bytes;
            if (bytes > stat.MaxSize) stat.MaxSize = bytes;
        }

        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            _messageStats.Clear();
            _peakMsgRate = 0;
            _peakByteRate = 0;
            _totalProcessingTicks = 0;
            _processedMessages = 0;
            _timeoutCount = 0;
            _retransmissionCount = 0;
            UpdateStatistics();
            Log("Statistics reset.");
        }

        private static DateTime? LastActivity(MessageStat s) =>
            s.LastReceived is null ? s.LastSent
            : s.LastSent is null ? s.LastReceived
            : (s.LastReceived > s.LastSent ? s.LastReceived : s.LastSent);

        /// <summary>Recomputes the per-second rates (1 Hz) and refreshes the Statistics grid.</summary>
        private void UpdateStatistics()
        {
            var now = DateTime.Now;
            var elapsed = Math.Max((now - _lastStatsTick).TotalSeconds, 0.001);
            _lastStatsTick = now;
            foreach (var stat in _messageStats.Values)
            {
                stat.MsgPerSec = stat.WindowCount / elapsed;
                stat.BytesPerSec = stat.WindowBytes / elapsed;
                stat.WindowCount = 0;
                stat.WindowBytes = 0;
            }
            var msgRate = _messageStats.Values.Sum(s => s.MsgPerSec);
            var byteRate = _messageStats.Values.Sum(s => s.BytesPerSec);
            if (msgRate > _peakMsgRate) _peakMsgRate = msgRate;
            if (byteRate > _peakByteRate) _peakByteRate = byteRate;

            // CPU usage of this process since the previous tick (kept accurate even when hidden).
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var cpuNow = process.TotalProcessorTime;
            var cpuPercent = (cpuNow - _lastCpuTime).TotalMilliseconds / (elapsed * 1000.0 * Environment.ProcessorCount) * 100.0;
            _lastCpuTime = cpuNow;

            if (!StatsGrid.IsVisible) return;

            StatsGrid.ItemsSource = _messageStats.Values
                .OrderBy(s => s.Name)
                .Select(s => new MessageStatRow
                {
                    Message = s.Name,
                    Received = s.Received,
                    Sent = s.Sent,
                    MsgPerSec = s.MsgPerSec.ToString("0.0"),
                    BytesPerSec = s.BytesPerSec.ToString("0"),
                    TotalBytes = s.TotalBytes,
                    AvgSize = s.Received + s.Sent > 0 ? ((double)s.TotalBytes / (s.Received + s.Sent)).ToString("0.0") : "-",
                    MinSize = s.MinSize == int.MaxValue ? "-" : s.MinSize.ToString(),
                    MaxSize = s.MaxSize == 0 ? "-" : s.MaxSize.ToString(),
                    LastReceived = s.LastReceived?.ToString("HH:mm:ss.fff") ?? "-",
                    LastSent = s.LastSent?.ToString("HH:mm:ss.fff") ?? "-",
                    SinceLast = LastActivity(s) is { } last ? (now - last).TotalSeconds.ToString("0.0") : "-",
                    Errors = s.Errors,
                    Dropped = s.Dropped
                })
                .ToList();

            // Global (whole simulator) statistics.
            var totalSent = _messageStats.Values.Sum(s => s.Sent);
            var totalReceived = _messageStats.Values.Sum(s => s.Received);
            var totalBytesSent = _messageStats.Values.Sum(s => s.BytesSent);
            var totalBytesReceived = _messageStats.Values.Sum(s => s.BytesReceived);
            var errors = _messageStats.Values.Sum(s => s.Errors);
            var crcErrors = _messageStats.Values.Sum(s => s.CrcErrors);
            var dropped = _messageStats.Values.Sum(s => s.Dropped);
            var running = now - _simStartTime;
            var runningText = running.ToString(@"hh\:mm\:ss");
            var runSeconds = Math.Max(running.TotalSeconds, 0.001);
            var lossPercent = totalReceived > 0 ? dropped * 100.0 / totalReceived : 0.0;
            var avgProcessing = _processedMessages > 0
                ? $"{_totalProcessingTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency / _processedMessages:0.000} ms"
                : "-";

            GlobalStatsGrid.ItemsSource = new List<GlobalStatRow>
            {
                new("Total Messages Sent", totalSent.ToString("N0")),
                new("Total Messages Received", totalReceived.ToString("N0")),
                new("Total Bytes Sent", totalBytesSent.ToString("N0")),
                new("Total Bytes Received", totalBytesReceived.ToString("N0")),
                new("Overall Messages/sec", msgRate.ToString("0.0")),
                new("Overall Bytes/sec", byteRate.ToString("N0")),
                new("Average Traffic Rate", $"{(totalSent + totalReceived) / runSeconds:0.0} msg/s, {(totalBytesSent + totalBytesReceived) / runSeconds:N0} B/s"),
                new("Peak Messages/sec", _peakMsgRate.ToString("0.0")),
                new("Peak Bytes/sec", _peakByteRate.ToString("N0")),
                new("Simulator Running Time", runningText),
                new("CPU Usage", $"{Math.Clamp(cpuPercent, 0.0, 100.0):0.0} %"),
                new("Memory Usage", $"{process.WorkingSet64 / (1024.0 * 1024.0):0.0} MB"),
                new("Queue Size (timeline buffer)", _timelineEvents.Count.ToString("N0")),
                new("Connected Interfaces", (_transport?.ConnectionCount ?? 0).ToString()),
                new("Packet Loss", $"{lossPercent:0.0} %"),
                new("Error Count", errors.ToString("N0")),
                new("CRC Errors", crcErrors.ToString("N0")),
                new("Timeout Count", _timeoutCount.ToString("N0")),
                new("Retransmissions (periodic repeats)", _retransmissionCount.ToString("N0")),
                new("Avg Processing Time / Message", avgProcessing)
            };

            StatsSummaryText.Text = $"Total: {totalSent + totalReceived:N0} message(s), {totalBytesSent + totalBytesReceived:N0} bytes   |   " +
                $"Current: {msgRate:0.0} msg/s, {byteRate:N0} B/s   |   Running: {runningText}";
        }

        private void Log(string message)
        {
            var line = $"[{DateTime.Now:HH:mm:ss.fff}] {message}" + Environment.NewLine;
            AppendLog(TrafficLogBox, line);
            AppendLog(ScenarioLogBox, line);
        }

        /// <summary>Appends and trims the log so endless traffic cannot bloat the UI.</summary>
        private static void AppendLog(System.Windows.Controls.TextBox box, string line)
        {
            if (box.Text.Length > 400000) box.Text = box.Text.Substring(box.Text.Length - 200000);
            box.AppendText(line);
            box.ScrollToEnd();
        }
    }
}
