using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using InterfaceWrapper.Models;
using InterfaceWrapper.Services;
using Microsoft.Win32;

namespace InterfaceWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CFileParser _parser = new();
        private ParseResult? _parseResult;
        private string? _messagesFolder;
        private string? _solutionRoot;
        private string? _lastOutputDirectory;
        // The editable cells of the array currently shown in the Arrays panel.
        private readonly ObservableCollection<ArrayCellRow> _arrayCells = new();

        public MainWindow()
        {
            InitializeComponent();
            ArrayCellsGrid.ItemsSource = _arrayCells;
        }

        private void LoadCFilesButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select the Messages folder that contains the cvi.c / cvp.c files"
            };

            if (dialog.ShowDialog(this) != true)
                return;

            _messagesFolder = dialog.FolderName;
            FolderPathRun.Text = _messagesFolder;
            ClearLog();
            _solutionRoot = null;
            GenerateUiButton.IsEnabled = false;

            try
            {
                _parseResult = _parser.Parse(_messagesFolder);

                foreach (var line in _parseResult.Log)
                    AppendLog(line);

                // Left list shows the message names; selecting one shows all its fields.
                MessageListBox.ItemsSource = _parseResult.Messages;
                if (_parseResult.Messages.Count > 0)
                    MessageListBox.SelectedIndex = 0;
                GenerateButton.IsEnabled = _parseResult.Messages.Count > 0;
                StatusText.Text = $"Loaded {_parseResult.Messages.Count} message(s) from '{Path.GetFileName(_messagesFolder)}'.";
            }
            catch (Exception ex)
            {
                _parseResult = null;
                MessageListBox.ItemsSource = null;
                FieldsGrid.ItemsSource = null;
                FieldsHeaderText.Text = "Fields (select a message)";
                MessageSummaryText.Text = "Message Id: -   Length: -   Directions: -";
                ResetArraysPanel();
                GenerateButton.IsEnabled = false;
                GenerateUiButton.IsEnabled = false;
                AppendLog("ERROR: " + ex.Message);
                StatusText.Text = "Failed to load C files.";
                MessageBox.Show(this, ex.Message, "Load C files", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_parseResult is null || _messagesFolder is null)
                return;

            var dialog = new OpenFolderDialog
            {
                Title = $"Select the output folder for the '{WrapperGenerator.ProjectName}' project"
            };

            if (dialog.ShowDialog(this) != true)
                return;

            try
            {
                var generator = new WrapperGenerator(_parseResult, dialog.FolderName);
                IReadOnlyList<string> written = generator.Generate(_messagesFolder);

                _solutionRoot = dialog.FolderName;
                _lastOutputDirectory = Path.Combine(dialog.FolderName, WrapperGenerator.ProjectName);
                OpenOutputButton.IsEnabled = true;
                GenerateUiButton.IsEnabled = true;

                AppendLog(string.Empty);
                AppendLog($"Generated '{WrapperGenerator.ProjectName}' project ({written.Count} file(s)):");
                foreach (var file in written)
                    AppendLog("  " + file);

                StatusText.Text = $"Generated '{WrapperGenerator.ProjectName}' at '{_lastOutputDirectory}'.";
                MessageBox.Show(this,
                    $"Successfully generated the '{WrapperGenerator.ProjectName}' project.\n\nUse 'Generate UI' to add the GenericSim simulator to the same solution.",
                    "Generate Wrapper", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                StatusText.Text = "Generation failed.";
                MessageBox.Show(this, ex.Message, "Generate Wrapper", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateUiButton_Click(object sender, RoutedEventArgs e)
        {
            if (_parseResult is null || _solutionRoot is null)
            {
                MessageBox.Show(this, "Generate the wrapper project first.", "Generate UI",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var source = UseSourceCheck.IsChecked == true ? SourceNameBox.Text : "MBT";
                var destination = UseDestinationCheck.IsChecked == true ? DestinationNameBox.Text : "NIRON";
                var simGenerator = new GenericSimGenerator(_parseResult, source, destination);
                IReadOnlyList<string> written = simGenerator.Generate(_solutionRoot);

                var slnPath = new SolutionGenerator().Generate(_solutionRoot);

                _lastOutputDirectory = Path.Combine(_solutionRoot, GenericSimGenerator.ProjectName);
                OpenOutputButton.IsEnabled = true;

                AppendLog(string.Empty);
                AppendLog($"Side names: source '{source}', destination '{destination}'.");
                AppendLog($"Generated '{GenericSimGenerator.ProjectName}' project ({written.Count} file(s)):");
                foreach (var file in written)
                    AppendLog("  " + file);
                AppendLog("  " + slnPath);

                StatusText.Text = $"Generated '{GenericSimGenerator.ProjectName}' and solution at '{_solutionRoot}'.";
                MessageBox.Show(this,
                    $"Successfully generated the '{GenericSimGenerator.ProjectName}' simulator and the solution file.\n\n" +
                    $"Build order: build '{WrapperGenerator.ProjectName}' (x64) first, then run '{GenericSimGenerator.ProjectName}'.",
                    "Generate UI", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                AppendLog("ERROR: " + ex.Message);
                StatusText.Text = "UI generation failed.";
                MessageBox.Show(this, ex.Message, "Generate UI", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>Shows all the fields of the message selected in the left list.</summary>
        private void MessageListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MessageListBox.SelectedItem is not MessageDefinition msg)
            {
                FieldsGrid.ItemsSource = null;
                FieldsHeaderText.Text = "Fields (select a message)";
                MessageSummaryText.Text = "Message Id: -   Length: -   Directions: -";
                ResetArraysPanel();
                return;
            }

            FieldsGrid.ItemsSource = msg.Fields;
            FieldsHeaderText.Text = $"Fields of {msg.MessageName} ({msg.Fields.Count})";
            MessageSummaryText.Text =
                $"Message Id: {msg.MessageIdDisplay}   Length: {msg.Length} bytes   Directions: {msg.Directions}   " +
                $"Physical Type: {msg.PhysicalType}   Global Variable: {msg.GlobalVariable}";

            // Fill the Arrays panel with the message's array fields (if any).
            ArraysCombo.ItemsSource = msg.Arrays;
            ArraysHeaderText.Text = msg.HasArrays ? $"Arrays ({msg.Arrays.Count})" : "Arrays (none)";
            if (msg.HasArrays)
                ArraysCombo.SelectedIndex = 0;
            else
                ResetArraysPanel(keepCombo: true);
        }

        /// <summary>Clears the Arrays panel to its empty state.</summary>
        private void ResetArraysPanel(bool keepCombo = false)
        {
            if (!keepCombo)
                ArraysCombo.ItemsSource = null;
            _arrayCells.Clear();
            ArrayCountBox.Text = "0";
            ArrayInfoText.Text = "(select an array)";
            if (!keepCombo)
                ArraysHeaderText.Text = "Arrays";
        }

        /// <summary>When an array is chosen, default the element count to its count field / max.</summary>
        private void ArraysCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ArraysCombo.SelectedItem is not MessageArray array)
            {
                _arrayCells.Clear();
                return;
            }

            var countHint = array.CountField is null ? "" : $", count field: {array.CountField}";
            ArrayInfoText.Text =
                $"base {array.BaseOffset}, stride {array.Stride} B, max {array.MaxCount}, {array.Elements.Count} sub-field(s){countHint}";

            // Start with a single element so the developer can grow it as needed.
            ArrayCountBox.Text = Math.Min(1, array.MaxCount).ToString();
            BuildArrayCells(array, 1);
        }

        private void ApplyArrayCount_Click(object sender, RoutedEventArgs e)
        {
            if (ArraysCombo.SelectedItem is not MessageArray array)
                return;
            if (!int.TryParse(ArrayCountBox.Text, out var count) || count < 0)
            {
                MessageBox.Show(this, "Enter a valid non-negative element count.", "Arrays",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (count > array.MaxCount)
            {
                count = array.MaxCount;
                ArrayCountBox.Text = count.ToString();
                AppendLog($"{array.Name}: clamped element count to the maximum ({array.MaxCount}).");
            }
            BuildArrayCells(array, count);
        }

        /// <summary>Builds one editable row per (element index x sub-field) of the array.</summary>
        private void BuildArrayCells(MessageArray array, int count)
        {
            _arrayCells.Clear();
            for (var i = 0; i < count; i++)
            {
                foreach (var element in array.Elements)
                {
                    _arrayCells.Add(new ArrayCellRow
                    {
                        Index = i,
                        Offset = array.BaseOffset + i * array.Stride + element.RelativeOffset,
                        Field = element.Field.Replace($"[{array.IndexVar}]", $"[{i}]", StringComparison.Ordinal),
                        Type = element.Type,
                        Value = "0"
                    });
                }
            }
            ArrayInfoText.Text = $"{count} element(s) \u00D7 {array.Elements.Count} sub-field(s) = {_arrayCells.Count} cell(s)";
        }

        /// <summary>Restricts the element-count box to digits.</summary>
        private void ArrayCountBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void OpenOutputButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_lastOutputDirectory) || !Directory.Exists(_lastOutputDirectory))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = _lastOutputDirectory,
                UseShellExecute = true
            });
        }

        private void AppendLog(string message)
        {
            LogTextBox.AppendText(message + Environment.NewLine);
            LogTextBox.ScrollToEnd();
        }

        private void ClearLog() => LogTextBox.Clear();
    }
}