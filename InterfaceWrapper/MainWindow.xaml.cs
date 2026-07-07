using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public MainWindow()
        {
            InitializeComponent();
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
                return;
            }

            FieldsGrid.ItemsSource = msg.Fields;
            FieldsHeaderText.Text = $"Fields of {msg.MessageName} ({msg.Fields.Count})";
            MessageSummaryText.Text =
                $"Message Id: {msg.MessageIdDisplay}   Length: {msg.Length} bytes   Directions: {msg.Directions}   " +
                $"Physical Type: {msg.PhysicalType}   Global Variable: {msg.GlobalVariable}";
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