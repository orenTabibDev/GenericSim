using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using InterfaceWrapper.Models;

namespace InterfaceWrapper.Services
{
    /// <summary>
    /// Generates the <c>GenericSim</c> WPF simulator project which consumes the
    /// <c>WrapperFromCToSharp.dll</c> and provides a professional message simulator UI
    /// (message list, editable field grid, HEX preview, UDP send/receive, history and logs).
    /// </summary>
    public sealed class GenericSimGenerator
    {
        public const string ProjectName = "GenericSim";

        private static readonly Guid ProjectGuid = new("B7C1D2E3-4F56-4A78-9B0C-1D2E3F4A5B6C");

        /// <summary>The project GUID as an uppercase string (no braces), for the .sln file.</summary>
        public static string ProjectGuidString => ProjectGuid.ToString();

        private readonly ParseResult _parseResult;

        public GenericSimGenerator(ParseResult parseResult)
        {
            _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
        }

        /// <summary>
        /// Generates the GenericSim project next to the generated wrapper project.
        /// <paramref name="solutionRoot"/> is the folder that contains the WrapperFromCToSharp folder.
        /// </summary>
        public IReadOnlyList<string> Generate(string solutionRoot)
        {
            var written = new List<string>();
            var projectDir = Path.Combine(solutionRoot, ProjectName);
            Directory.CreateDirectory(projectDir);

            written.Add(WriteFile(Path.Combine(projectDir, ProjectName + ".csproj"), BuildCsproj()));
            written.Add(WriteFile(Path.Combine(projectDir, "App.xaml"), BuildAppXaml()));
            written.Add(WriteFile(Path.Combine(projectDir, "App.xaml.cs"), BuildAppXamlCs()));
            written.Add(WriteFile(Path.Combine(projectDir, "MainWindow.xaml"), BuildMainWindowXaml()));
            written.Add(WriteFile(Path.Combine(projectDir, "MainWindow.xaml.cs"), BuildMainWindowXamlCs()));
            written.Add(WriteFile(Path.Combine(projectDir, "MessageCatalog.cs"), BuildMessageCatalog()));
            written.Add(WriteFile(Path.Combine(projectDir, "Models.cs"), BuildModels()));
            written.Add(WriteFile(Path.Combine(projectDir, "Transports.cs"), BuildTransports()));
            written.Add(WriteFile(Path.Combine(projectDir, "README.md"), BuildReadme()));

            return written;
        }

        private string WrapperRelativePath =>
            $"..\\{WrapperGenerator.ProjectName}\\WrapperInterop.cs";

        private string BuildCsproj()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <OutputType>WinExe</OutputType>");
            sb.AppendLine("    <TargetFramework>net8.0-windows</TargetFramework>");
            sb.AppendLine("    <Nullable>enable</Nullable>");
            sb.AppendLine("    <ImplicitUsings>enable</ImplicitUsings>");
            sb.AppendLine("    <UseWPF>true</UseWPF>");
            sb.AppendLine("    <Platforms>x64</Platforms>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <!-- RS232 serial port support on .NET 8. -->");
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine("    <PackageReference Include=\"System.IO.Ports\" Version=\"8.0.0\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <!-- Reuse the P/Invoke bindings emitted alongside the native wrapper. -->");
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <Compile Include=\"{WrapperRelativePath}\" Link=\"WrapperInterop.cs\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <!-- Copy the native DLL next to the executable once it has been built.");
            sb.AppendLine($"       Build the {WrapperGenerator.ProjectName}.vcxproj (x64) first. -->");
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <None Include=\"..\\{WrapperGenerator.ProjectName}\\x64\\$(Configuration)\\{WrapperGenerator.ProjectName}.dll\" Condition=\"Exists('..\\{WrapperGenerator.ProjectName}\\x64\\$(Configuration)\\{WrapperGenerator.ProjectName}.dll')\">");
            sb.AppendLine("      <Link>WrapperFromCToSharp.dll</Link>");
            sb.AppendLine("      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
            sb.AppendLine("    </None>");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");
            return sb.ToString();
        }

        private static string BuildAppXaml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<Application x:Class=\"GenericSim.App\"");
            sb.AppendLine("             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            sb.AppendLine("             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            sb.AppendLine("             StartupUri=\"MainWindow.xaml\">");
            sb.AppendLine("    <Application.Resources />");
            sb.AppendLine("</Application>");
            return sb.ToString();
        }

        private static string BuildAppXamlCs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System.Windows;");
            sb.AppendLine();
            sb.AppendLine("namespace GenericSim");
            sb.AppendLine("{");
            sb.AppendLine("    public partial class App : Application");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string BuildMainWindowXaml()
        {
            // A dark-themed layout mirroring the reference MBT Side Professional Simulator.
            var sb = new StringBuilder();
            sb.AppendLine("<Window x:Class=\"GenericSim.MainWindow\"");
            sb.AppendLine("        xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
            sb.AppendLine("        xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
            sb.AppendLine("        Title=\"MBT Side Professional Simulator\" Height=\"820\" Width=\"1380\"");
            sb.AppendLine("        Background=\"#0E1621\" Foreground=\"#E6EDF3\">");
            sb.AppendLine("    <Window.Resources>");
            sb.AppendLine("        <Style TargetType=\"TextBlock\"><Setter Property=\"Foreground\" Value=\"#E6EDF3\"/></Style>");
            sb.AppendLine("        <Style TargetType=\"GroupBox\">");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"#8FB4D9\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"#22303C\"/>");
            sb.AppendLine("            <Setter Property=\"Margin\" Value=\"6\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"6\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"DataGrid\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"#0E1621\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"#E6EDF3\"/>");
            sb.AppendLine("            <Setter Property=\"RowBackground\" Value=\"#132030\"/>");
            sb.AppendLine("            <Setter Property=\"AlternatingRowBackground\" Value=\"#0F1A28\"/>");
            sb.AppendLine("            <Setter Property=\"GridLinesVisibility\" Value=\"Horizontal\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"#22303C\"/>");
            sb.AppendLine("            <Setter Property=\"HeadersVisibility\" Value=\"Column\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <!-- Column header text in black for all tables. -->");
            sb.AppendLine("        <Style TargetType=\"DataGridColumnHeader\">");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"Black\"/>");
            sb.AppendLine("            <Setter Property=\"FontWeight\" Value=\"Bold\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"6,4\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"Button\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"#1F6FEB\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"White\"/>");
            sb.AppendLine("            <Setter Property=\"BorderThickness\" Value=\"0\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"8,5\"/>");
            sb.AppendLine("            <Setter Property=\"Margin\" Value=\"0,3\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"TextBox\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"#0B1220\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"#E6EDF3\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"#22303C\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("    </Window.Resources>");
            sb.AppendLine();
            sb.AppendLine("    <Grid Margin=\"10\">");
            sb.AppendLine("        <Grid.RowDefinitions>");
            sb.AppendLine("            <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("            <RowDefinition Height=\"*\"/>");
            sb.AppendLine("            <RowDefinition Height=\"220\"/>");
            sb.AppendLine("        </Grid.RowDefinitions>");
            sb.AppendLine();
            // Header / connection bar
            sb.AppendLine("        <Grid Grid.Row=\"0\" Margin=\"6\">");
            sb.AppendLine("            <Grid.ColumnDefinitions>");
            sb.AppendLine("                <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                <ColumnDefinition Width=\"Auto\"/>");
            sb.AppendLine("            </Grid.ColumnDefinitions>");
            sb.AppendLine("            <TextBlock Text=\"MBT Side Simulator\" FontSize=\"22\" FontWeight=\"Bold\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("            <StackPanel Grid.Column=\"1\" Orientation=\"Horizontal\" VerticalAlignment=\"Center\">");
            sb.AppendLine("                <TextBlock Text=\"Transport\" VerticalAlignment=\"Center\" Margin=\"6,0\"/>");
            sb.AppendLine("                <ComboBox x:Name=\"TransportCombo\" Width=\"75\" VerticalAlignment=\"Center\" SelectedIndex=\"0\"");
            sb.AppendLine("                          SelectionChanged=\"TransportCombo_SelectionChanged\">");
            sb.AppendLine("                    <ComboBoxItem Content=\"UDP\"/>");
            sb.AppendLine("                    <ComboBoxItem Content=\"TCP\"/>");
            sb.AppendLine("                    <ComboBoxItem Content=\"RS232\"/>");
            sb.AppendLine("                </ComboBox>");
            sb.AppendLine("                <StackPanel x:Name=\"TcpModePanel\" Orientation=\"Horizontal\" Visibility=\"Collapsed\">");
            sb.AppendLine("                    <TextBlock Text=\"TCP Mode\" VerticalAlignment=\"Center\" Margin=\"8,0,6,0\"/>");
            sb.AppendLine("                    <ComboBox x:Name=\"TcpModeCombo\" Width=\"75\" VerticalAlignment=\"Center\" SelectedIndex=\"0\">");
            sb.AppendLine("                        <ComboBoxItem Content=\"Client\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"Server\"/>");
            sb.AppendLine("                    </ComboBox>");
            sb.AppendLine("                </StackPanel>");
            sb.AppendLine("                <StackPanel x:Name=\"NetworkPanel\" Orientation=\"Horizontal\">");
            sb.AppendLine("                    <TextBlock Text=\"Local Port\" VerticalAlignment=\"Center\" Margin=\"8,0,6,0\"/>");
            sb.AppendLine("                    <TextBox x:Name=\"LocalPortBox\" Text=\"5001\" Width=\"60\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                    <TextBlock Text=\"MBT IP\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <TextBox x:Name=\"RemoteIpBox\" Text=\"127.0.0.1\" Width=\"100\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                    <TextBlock Text=\"MBT Port\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <TextBox x:Name=\"RemotePortBox\" Text=\"5000\" Width=\"60\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                </StackPanel>");
            sb.AppendLine("                <StackPanel x:Name=\"SerialPanel\" Orientation=\"Horizontal\" Visibility=\"Collapsed\">");
            sb.AppendLine("                    <TextBlock Text=\"COM Port\" VerticalAlignment=\"Center\" Margin=\"8,0,6,0\"/>");
            sb.AppendLine("                    <ComboBox x:Name=\"ComPortBox\" Width=\"80\" IsEditable=\"True\" Text=\"COM1\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                    <TextBlock Text=\"Baud\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <ComboBox x:Name=\"BaudRateBox\" Width=\"80\" IsEditable=\"True\" Text=\"115200\" VerticalAlignment=\"Center\">");
            sb.AppendLine("                        <ComboBoxItem Content=\"4800\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"9600\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"19200\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"38400\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"57600\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"115200\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"230400\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"460800\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"921600\"/>");
            sb.AppendLine("                    </ComboBox>");
            sb.AppendLine("                    <TextBlock Text=\"Parity\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <ComboBox x:Name=\"ParityBox\" Width=\"70\" VerticalAlignment=\"Center\" SelectedIndex=\"0\">");
            sb.AppendLine("                        <ComboBoxItem Content=\"None\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"Odd\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"Even\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"Mark\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"Space\"/>");
            sb.AppendLine("                    </ComboBox>");
            sb.AppendLine("                    <TextBlock Text=\"Stop Bits\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <ComboBox x:Name=\"StopBitsBox\" Width=\"60\" VerticalAlignment=\"Center\" SelectedIndex=\"0\">");
            sb.AppendLine("                        <ComboBoxItem Content=\"1\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"1.5\"/>");
            sb.AppendLine("                        <ComboBoxItem Content=\"2\"/>");
            sb.AppendLine("                    </ComboBox>");
            sb.AppendLine("                </StackPanel>");
            sb.AppendLine("                <Button x:Name=\"StartButton\" Content=\"Start\" Background=\"#238636\" Margin=\"12,0,0,0\" Click=\"Start_Click\"/>");
            sb.AppendLine("                <Button x:Name=\"StopButton\" Content=\"Stop\" Background=\"#DA3633\" Margin=\"6,0,0,0\" Click=\"Stop_Click\"/>");
            sb.AppendLine("            </StackPanel>");
            sb.AppendLine("        </Grid>");
            sb.AppendLine();
            // Main 3-column area
            sb.AppendLine("        <Grid Grid.Row=\"1\">");
            sb.AppendLine("            <Grid.ColumnDefinitions>");
            sb.AppendLine("                <ColumnDefinition Width=\"300\"/>");
            sb.AppendLine("                <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                <ColumnDefinition Width=\"360\"/>");
            sb.AppendLine("            </Grid.ColumnDefinitions>");
            sb.AppendLine();
            // Left: message list + actions
            sb.AppendLine("            <GroupBox Grid.Column=\"0\" Header=\"MBT \u2192 NIRON Messages\">");
            sb.AppendLine("                <DockPanel>");
            sb.AppendLine("                    <TextBlock DockPanel.Dock=\"Top\" Text=\"Select a message to edit its own fields\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                    <StackPanel DockPanel.Dock=\"Bottom\">");
            sb.AppendLine("                        <Button Content=\"Send Selected Message\" Click=\"SendSelected_Click\"/>");
            sb.AppendLine("                        <Button Content=\"Start Periodic Selected\" Background=\"#238636\" Click=\"StartPeriodic_Click\"/>");
            sb.AppendLine("                        <Button Content=\"Stop Periodic Selected\" Background=\"#DA3633\" Click=\"StopPeriodic_Click\"/>");
            sb.AppendLine("                        <Button Content=\"Stop All Periodic\" Background=\"#8B2E1F\" Click=\"StopAllPeriodic_Click\"/>");
            sb.AppendLine("                        <Button Content=\"\u2193 Export Message (JSON)\" Background=\"#1F6FEB\" Click=\"ExportJson_Click\"/>");
            sb.AppendLine("                        <Button Content=\"\u2191 Import Message (JSON)\" Background=\"#1F8FA8\" Click=\"ImportJson_Click\"/>");
            sb.AppendLine("                    </StackPanel>");
            sb.AppendLine("                    <ListBox x:Name=\"MessageList\" Background=\"#0B1220\" Foreground=\"#E6EDF3\" BorderBrush=\"#22303C\"");
            sb.AppendLine("                             SelectionChanged=\"MessageList_SelectionChanged\" Margin=\"0,0,0,6\"/>");
            sb.AppendLine("                </DockPanel>");
            sb.AppendLine("            </GroupBox>");
            sb.AppendLine();
            // Center: fields + periodic + hex
            sb.AppendLine("            <GroupBox Grid.Column=\"1\">");
            sb.AppendLine("                <GroupBox.Header>");
            sb.AppendLine("                    <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                        <TextBlock x:Name=\"SelectedMessageHeader\" Text=\"(no message)\" FontSize=\"16\" FontWeight=\"Bold\" Foreground=\"#E6EDF3\"/>");
            sb.AppendLine("                        <CheckBox x:Name=\"AutoSequenceCheck\" Content=\"Auto sequence\" IsChecked=\"True\" Foreground=\"#E6EDF3\" Margin=\"14,0,0,0\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                        <CheckBox x:Name=\"AutoTimestampCheck\" Content=\"Auto timestamp\" IsChecked=\"True\" Foreground=\"#E6EDF3\" Margin=\"10,0,0,0\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                        <CheckBox x:Name=\"AutoCrcCheck\" Content=\"Auto CRC\" IsChecked=\"True\" Foreground=\"#E6EDF3\" Margin=\"10,0,0,0\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                    </StackPanel>");
            sb.AppendLine("                </GroupBox.Header>");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine("                    <TextBlock x:Name=\"MessageInfoText\" Grid.Row=\"0\" Text=\"Message Id: -   Length: - bytes\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                    <DataGrid x:Name=\"FieldsGrid\" Grid.Row=\"1\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\">");
            sb.AppendLine("                        <DataGrid.Columns>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" IsReadOnly=\"True\" Width=\"70\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" IsReadOnly=\"True\" Width=\"2*\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Type\" Binding=\"{Binding Type}\" IsReadOnly=\"True\" Width=\"90\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Size\" Binding=\"{Binding Size}\" IsReadOnly=\"True\" Width=\"60\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value, UpdateSourceTrigger=PropertyChanged}\" Width=\"*\"/>");
            sb.AppendLine("                        </DataGrid.Columns>");
            sb.AppendLine("                    </DataGrid>");
            sb.AppendLine("                    <Grid Grid.Row=\"2\">");
            sb.AppendLine("                        <Grid.ColumnDefinitions>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                        </Grid.ColumnDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"0\" Header=\"Periodic Send Settings\">");
            sb.AppendLine("                            <StackPanel>");
            sb.AppendLine("                                <CheckBox x:Name=\"EnablePeriodicCheck\" Content=\"Enable periodic for this message\" Foreground=\"#E6EDF3\" Margin=\"0,2,0,6\"/>");
            sb.AppendLine("                                <StackPanel Orientation=\"Horizontal\" Margin=\"0,2\">");
            sb.AppendLine("                                    <TextBlock Text=\"Interval (ms)\" Width=\"90\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                    <TextBox x:Name=\"IntervalBox\" Text=\"1000\" Width=\"120\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                                <StackPanel Orientation=\"Horizontal\" Margin=\"0,2\">");
            sb.AppendLine("                                    <TextBlock Text=\"How many\" Width=\"90\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                    <TextBox x:Name=\"HowManyBox\" Text=\"0\" Width=\"120\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"0 = unlimited\" Foreground=\"#7A8CA0\" VerticalAlignment=\"Center\" Margin=\"8,0,0,0\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                            </StackPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"1\" Header=\"HEX Preview\">");
            sb.AppendLine("                            <TextBox x:Name=\"HexPreviewBox\" IsReadOnly=\"True\" FontFamily=\"Consolas\" Height=\"120\"");
            sb.AppendLine("                                     VerticalScrollBarVisibility=\"Auto\" TextWrapping=\"Wrap\"/>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </GroupBox>");
            sb.AppendLine();
            // Right: monitor + received fields
            sb.AppendLine("            <GroupBox Grid.Column=\"2\" Header=\"Monitor: NIRON \u2192 MBT\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine("                    <DockPanel Grid.Row=\"0\">");
            sb.AppendLine("                        <Button DockPanel.Dock=\"Bottom\" x:Name=\"ClearMonitorButton\" Content=\"Clear Monitor\"");
            sb.AppendLine("                                Background=\"#8B2E1F\" Click=\"ClearMonitor_Click\"/>");
            sb.AppendLine("                        <DataGrid x:Name=\"MonitorGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\">");
            sb.AppendLine("                            <DataGrid.Columns>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Time\" Binding=\"{Binding Time}\" Width=\"Auto\"/>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"From\" Binding=\"{Binding From}\" Width=\"Auto\"/>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" Width=\"*\"/>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Bytes\" Binding=\"{Binding Bytes}\" Width=\"Auto\"/>");
            sb.AppendLine("                            </DataGrid.Columns>");
            sb.AppendLine("                        </DataGrid>");
            sb.AppendLine("                    </DockPanel>");
            sb.AppendLine("                    <GroupBox Grid.Row=\"1\" Header=\"Received Message Fields\">");
            sb.AppendLine("                        <DataGrid x:Name=\"ReceivedFieldsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\">");
            sb.AppendLine("                            <DataGrid.Columns>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" Width=\"Auto\"/>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" Width=\"*\"/>");
            sb.AppendLine("                                <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value}\" Width=\"Auto\"/>");
            sb.AppendLine("                            </DataGrid.Columns>");
            sb.AppendLine("                        </DataGrid>");
            sb.AppendLine("                    </GroupBox>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </GroupBox>");
            sb.AppendLine("        </Grid>");
            sb.AppendLine();
            // Bottom: history + traffic log
            sb.AppendLine("        <Grid Grid.Row=\"2\">");
            sb.AppendLine("            <Grid.ColumnDefinitions>");
            sb.AppendLine("                <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                <ColumnDefinition Width=\"420\"/>");
            sb.AppendLine("            </Grid.ColumnDefinitions>");
            sb.AppendLine("            <GroupBox Grid.Column=\"0\" Header=\"Send History\">");
            sb.AppendLine("                <DockPanel>");
            sb.AppendLine("                    <Button DockPanel.Dock=\"Bottom\" x:Name=\"ClearHistoryButton\" Content=\"Clear History\"");
            sb.AppendLine("                            Background=\"#8B2E1F\" Click=\"ClearHistory_Click\"/>");
            sb.AppendLine("                    <DataGrid x:Name=\"HistoryGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\">");
            sb.AppendLine("                        <DataGrid.Columns>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Time\" Binding=\"{Binding Time}\" Width=\"Auto\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Direction\" Binding=\"{Binding Direction}\" Width=\"Auto\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" Width=\"*\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Seq\" Binding=\"{Binding Seq}\" Width=\"Auto\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Bytes\" Binding=\"{Binding Bytes}\" Width=\"Auto\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Periodic\" Binding=\"{Binding Periodic}\" Width=\"Auto\"/>");
            sb.AppendLine("                        </DataGrid.Columns>");
            sb.AppendLine("                    </DataGrid>");
            sb.AppendLine("                </DockPanel>");
            sb.AppendLine("            </GroupBox>");
            sb.AppendLine("            <GroupBox Grid.Column=\"1\" Header=\"Traffic Log\">");
            sb.AppendLine("                <DockPanel>");
            sb.AppendLine("                    <Button DockPanel.Dock=\"Bottom\" x:Name=\"ClearTrafficLogButton\" Content=\"Clear Traffic Log\"");
            sb.AppendLine("                            Background=\"#8B2E1F\" Click=\"ClearTrafficLog_Click\"/>");
            sb.AppendLine("                    <Button DockPanel.Dock=\"Bottom\" Content=\"Refresh HEX Preview\" Click=\"RefreshHex_Click\"/>");
            sb.AppendLine("                    <TextBox x:Name=\"TrafficLogBox\" IsReadOnly=\"True\" FontFamily=\"Consolas\" FontSize=\"11\"");
            sb.AppendLine("                             VerticalScrollBarVisibility=\"Auto\" TextWrapping=\"Wrap\"/>");
            sb.AppendLine("                </DockPanel>");
            sb.AppendLine("            </GroupBox>");
            sb.AppendLine("        </Grid>");
            sb.AppendLine("    </Grid>");
            sb.AppendLine("</Window>");
            return sb.ToString();
        }

        private string BuildMainWindowXamlCs()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
            sb.AppendLine("using System.Globalization;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Net;");
            sb.AppendLine("using System.Net.Sockets;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Text.Json;");
            sb.AppendLine("using System.Threading;");
            sb.AppendLine("using System.Windows;");
            sb.AppendLine("using System.Windows.Threading;");
            sb.AppendLine("using WrapperFromCToSharp;");
            sb.AppendLine();
            sb.AppendLine("namespace GenericSim");
            sb.AppendLine("{");
            sb.AppendLine("    public partial class MainWindow : Window");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly ObservableCollection<FieldRow> _fields = new();");
            sb.AppendLine("        private readonly ObservableCollection<HistoryRow> _history = new();");
            sb.AppendLine("        private readonly ObservableCollection<MonitorRow> _monitor = new();");
            sb.AppendLine("        private readonly ObservableCollection<ReceivedField> _receivedFields = new();");
            sb.AppendLine("        private readonly Dictionary<string, DispatcherTimer> _periodicTimers = new();");
            sb.AppendLine("        private ITransport? _transport;");
            sb.AppendLine("        private int _sequence;");
            sb.AppendLine();
            sb.AppendLine("        public MainWindow()");
            sb.AppendLine("        {");
            sb.AppendLine("            InitializeComponent();");
            sb.AppendLine("            FieldsGrid.ItemsSource = _fields;");
            sb.AppendLine("            HistoryGrid.ItemsSource = _history;");
            sb.AppendLine("            MonitorGrid.ItemsSource = _monitor;");
            sb.AppendLine("            ReceivedFieldsGrid.ItemsSource = _receivedFields;");
            sb.AppendLine("            foreach (var m in MessageCatalog.Messages)");
            sb.AppendLine("                MessageList.Items.Add(m.Name);");
            sb.AppendLine("            if (MessageList.Items.Count > 0)");
            sb.AppendLine("                MessageList.SelectedIndex = 0;");
            sb.AppendLine("            foreach (var port in System.IO.Ports.SerialPort.GetPortNames())");
            sb.AppendLine("                ComPortBox.Items.Add(port);");
            sb.AppendLine("            if (ComPortBox.Items.Count > 0) ComPortBox.SelectedIndex = 0;");
            sb.AppendLine("            Log(\"Application ready. Simulating MBT side.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private MessageInfo? Current =>");
            sb.AppendLine("            MessageList.SelectedItem is string name ? MessageCatalog.ByName(name) : null;");
            sb.AppendLine();
            sb.AppendLine("        private void MessageList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            _fields.Clear();");
            sb.AppendLine("            if (info is null) return;");
            sb.AppendLine("            SelectedMessageHeader.Text = info.Name;");
            sb.AppendLine("            MessageInfoText.Text = $\"Message Id: {info.MessageId}   Length: {info.Length} bytes\";");
            sb.AppendLine("            foreach (var f in info.Fields)");
            sb.AppendLine("                _fields.Add(new FieldRow(f) { Value = f.DefaultValue });");
            sb.AppendLine("            RefreshHex();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Buffer building via the native wrapper ----");
            sb.AppendLine("        private byte[] BuildBuffer(MessageInfo info, IReadOnlyList<FieldRow> fields)");
            sb.AppendLine("        {");
            sb.AppendLine("            var buffer = new byte[Math.Max(info.Length, 1)];");
            sb.AppendLine("            IntPtr phys = info.GetPhysical();");
            sb.AppendLine();
            sb.AppendLine("            if (AutoSequenceCheck.IsChecked == true) SetFieldBySuffix(fields, \"SeqNum\", _sequence);");
            sb.AppendLine("            if (AutoTimestampCheck.IsChecked == true) SetFieldBySuffix(fields, \"timestamp\", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);");
            sb.AppendLine();
            sb.AppendLine("            // Establish the base layout (arrays, fixed structure) through the native");
            sb.AppendLine("            // convert function, then overlay the edited scalar values onto the buffer.");
            sb.AppendLine("            if (phys != IntPtr.Zero)");
            sb.AppendLine("                info.ConvertToInterface(buffer, phys);");
            sb.AppendLine();
            sb.AppendLine("            foreach (var row in fields)");
            sb.AppendLine("                row.WriteToBuffer(buffer);");
            sb.AppendLine();
            sb.AppendLine("            if (AutoCrcCheck.IsChecked == true)");
            sb.AppendLine("                ApplyCrc(info, buffer);");
            sb.AppendLine();
            sb.AppendLine("            return buffer;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Writes a simple additive checksum into the message CRC field, when present.</summary>");
            sb.AppendLine("        private static void ApplyCrc(MessageInfo info, byte[] buffer)");
            sb.AppendLine("        {");
            sb.AppendLine("            var crc = info.Fields.FirstOrDefault(f => f.Field.EndsWith(\".crc\", StringComparison.OrdinalIgnoreCase));");
            sb.AppendLine("            if (crc is null || crc.Offset + 4 > buffer.Length) return;");
            sb.AppendLine("            uint sum = 0;");
            sb.AppendLine("            for (int i = 0; i < crc.Offset; i++) sum += buffer[i];");
            sb.AppendLine("            Array.Copy(BitConverter.GetBytes(sum), 0, buffer, crc.Offset, 4);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static void SetFieldBySuffix(IReadOnlyList<FieldRow> fields, string suffix, double value)");
            sb.AppendLine("        {");
            sb.AppendLine("            foreach (var row in fields)");
            sb.AppendLine("                if (row.Field.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))");
            sb.AppendLine("                    row.Value = value.ToString(CultureInfo.InvariantCulture);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Sending ----");
            sb.AppendLine("        private void SendSelected_Click(object sender, RoutedEventArgs e) => SendCurrent(false);");
            sb.AppendLine();
            sb.AppendLine("        private void SendCurrent(bool periodic)");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            if (info is null) return;");
            sb.AppendLine("            SendMessage(info, _fields.ToList(), periodic);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Builds and sends a specific message using its own field snapshot, so a");
            sb.AppendLine("        /// periodic send keeps running independently of the current UI selection.</summary>");
            sb.AppendLine("        private void SendMessage(MessageInfo info, IReadOnlyList<FieldRow> fields, bool periodic)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_transport is null) { Log(\"Transport is not started.\"); return; }");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                var buffer = BuildBuffer(info, fields);");
            sb.AppendLine("                var seq = _sequence++;");
            sb.AppendLine("                _transport.Send(buffer);");
            sb.AppendLine("                _history.Insert(0, new HistoryRow");
            sb.AppendLine("                {");
            sb.AppendLine("                    Time = DateTime.Now.ToString(\"HH:mm:ss.fff\"),");
            sb.AppendLine("                    Direction = \"TX\", Message = info.Name, Seq = seq,");
            sb.AppendLine("                    Bytes = buffer.Length, Periodic = periodic ? \"Yes\" : \"No\"");
            sb.AppendLine("                });");
            sb.AppendLine("                Log($\"{info.Name}: sent {buffer.Length} bytes (seq {seq}).\");");
            sb.AppendLine("                if (ReferenceEquals(info, Current)) RefreshHex();");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex) { Log(\"Send error: \" + ex.Message); }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ClearHistory_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            _history.Clear();");
            sb.AppendLine("            Log(\"Send history cleared.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Periodic ----");
            sb.AppendLine("        private void StartPeriodic_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            if (info is null) return;");
            sb.AppendLine("            StopPeriodic(info.Name);");
            sb.AppendLine("            var interval = int.TryParse(IntervalBox.Text, out var ms) ? Math.Max(1, ms) : 1000;");
            sb.AppendLine("            var howMany = int.TryParse(HowManyBox.Text, out var n) ? n : 0;");
            sb.AppendLine();
            sb.AppendLine("            // Snapshot this message's fields so the periodic timer keeps sending it");
            sb.AppendLine("            // independently, even while you select and send other messages in parallel.");
            sb.AppendLine("            var snapshot = SnapshotFields(info);");
            sb.AppendLine("            var sent = 0;");
            sb.AppendLine("            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(interval) };");
            sb.AppendLine("            timer.Tick += (_, _) =>");
            sb.AppendLine("            {");
            sb.AppendLine("                SendMessage(info, snapshot, true);");
            sb.AppendLine("                sent++;");
            sb.AppendLine("                if (howMany > 0 && sent >= howMany) StopPeriodic(info.Name);");
            sb.AppendLine("            };");
            sb.AppendLine("            _periodicTimers[info.Name] = timer;");
            sb.AppendLine("            timer.Start();");
            sb.AppendLine("            Log($\"{info.Name}: periodic started ({interval} ms, {(howMany == 0 ? \"unlimited\" : howMany.ToString())}).\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Creates an independent copy of the message's current field values.</summary>");
            sb.AppendLine("        private List<FieldRow> SnapshotFields(MessageInfo info)");
            sb.AppendLine("        {");
            sb.AppendLine("            var list = new List<FieldRow>(info.Fields.Length);");
            sb.AppendLine("            for (int i = 0; i < info.Fields.Length; i++)");
            sb.AppendLine("            {");
            sb.AppendLine("                var row = new FieldRow(info.Fields[i]);");
            sb.AppendLine("                row.Value = i < _fields.Count ? _fields[i].Value : info.Fields[i].DefaultValue;");
            sb.AppendLine("                list.Add(row);");
            sb.AppendLine("            }");
            sb.AppendLine("            return list;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void StopPeriodic_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (Current is { } info) StopPeriodic(info.Name);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void StopAllPeriodic_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            foreach (var name in _periodicTimers.Keys.ToList()) StopPeriodic(name);");
            sb.AppendLine("            Log(\"All periodic timers stopped.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void StopPeriodic(string name)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_periodicTimers.TryGetValue(name, out var timer))");
            sb.AppendLine("            {");
            sb.AppendLine("                timer.Stop();");
            sb.AppendLine("                _periodicTimers.Remove(name);");
            sb.AppendLine("                Log($\"{name}: periodic stopped.\");");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Transport lifecycle (UDP / TCP client / TCP server / RS232) ----");
            sb.AppendLine("        private void TransportCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (TcpModePanel is null || SerialPanel is null || NetworkPanel is null) return;");
            sb.AppendLine("            var choice = (TransportCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? \"UDP\";");
            sb.AppendLine("            TcpModePanel.Visibility = choice == \"TCP\" ? Visibility.Visible : Visibility.Collapsed;");
            sb.AppendLine("            SerialPanel.Visibility = choice == \"RS232\" ? Visibility.Visible : Visibility.Collapsed;");
            sb.AppendLine("            NetworkPanel.Visibility = choice == \"RS232\" ? Visibility.Collapsed : Visibility.Visible;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Creates the transport selected in the combo boxes.</summary>");
            sb.AppendLine("        private ITransport CreateTransport()");
            sb.AppendLine("        {");
            sb.AppendLine("            var choice = (TransportCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? \"UDP\";");
            sb.AppendLine("            switch (choice)");
            sb.AppendLine("            {");
            sb.AppendLine("                case \"TCP\":");
            sb.AppendLine("                    var mode = (TcpModeCombo.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? \"Client\";");
            sb.AppendLine("                    return mode == \"Server\"");
            sb.AppendLine("                        ? new TcpServerTransport(int.Parse(LocalPortBox.Text))");
            sb.AppendLine("                        : new TcpClientTransport(RemoteIpBox.Text, int.Parse(RemotePortBox.Text));");
            sb.AppendLine("                case \"RS232\":");
            sb.AppendLine("                    var parity = ParseParity((ParityBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString());");
            sb.AppendLine("                    var stopBits = ParseStopBits((StopBitsBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString());");
            sb.AppendLine("                    return new SerialTransport(ComPortBox.Text, int.Parse(BaudRateBox.Text), parity, stopBits);");
            sb.AppendLine("                default:");
            sb.AppendLine("                    return new UdpTransport(int.Parse(LocalPortBox.Text), RemoteIpBox.Text, int.Parse(RemotePortBox.Text));");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static System.IO.Ports.Parity ParseParity(string? text) => text switch");
            sb.AppendLine("        {");
            sb.AppendLine("            \"Odd\" => System.IO.Ports.Parity.Odd,");
            sb.AppendLine("            \"Even\" => System.IO.Ports.Parity.Even,");
            sb.AppendLine("            \"Mark\" => System.IO.Ports.Parity.Mark,");
            sb.AppendLine("            \"Space\" => System.IO.Ports.Parity.Space,");
            sb.AppendLine("            _ => System.IO.Ports.Parity.None");
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        private static System.IO.Ports.StopBits ParseStopBits(string? text) => text switch");
            sb.AppendLine("        {");
            sb.AppendLine("            \"1.5\" => System.IO.Ports.StopBits.OnePointFive,");
            sb.AppendLine("            \"2\" => System.IO.Ports.StopBits.Two,");
            sb.AppendLine("            _ => System.IO.Ports.StopBits.One");
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        private void Start_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                StopTransport();");
            sb.AppendLine("                _transport = CreateTransport();");
            sb.AppendLine("                _transport.DataReceived += (data, from) => Dispatcher.Invoke(() => OnReceived(data, from));");
            sb.AppendLine("                _transport.StatusChanged += message => Dispatcher.Invoke(() => Log(message));");
            sb.AppendLine("                _transport.Start();");
            sb.AppendLine("                Log($\"Started: {_transport.Description}.\");");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                Log(\"Start error: \" + ex.Message);");
            sb.AppendLine("                StopTransport();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Stop_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            StopTransport();");
            sb.AppendLine("            Log(\"Transport stopped.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void StopTransport()");
            sb.AppendLine("        {");
            sb.AppendLine("            _transport?.Dispose();");
            sb.AppendLine("            _transport = null;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void OnReceived(byte[] data, string from)");
            sb.AppendLine("        {");
            sb.AppendLine("            int msgId = data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : -1;");
            sb.AppendLine("            var info = MessageCatalog.ById(msgId);");
            sb.AppendLine("            var name = info?.Name ?? $\"Unknown (0x{msgId:X})\";");
            sb.AppendLine("            _monitor.Insert(0, new MonitorRow");
            sb.AppendLine("            {");
            sb.AppendLine("                Time = DateTime.Now.ToString(\"HH:mm:ss.fff\"), From = from, Message = name, Bytes = data.Length");
            sb.AppendLine("            });");
            sb.AppendLine("            _receivedFields.Clear();");
            sb.AppendLine("            if (info is not null)");
            sb.AppendLine("            {");
            sb.AppendLine("                info.ConvertToPhysical(data, info.GetPhysical());");
            sb.AppendLine("                foreach (var f in info.Fields)");
            sb.AppendLine("                    _receivedFields.Add(new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(data) });");
            sb.AppendLine("            }");
            sb.AppendLine("            Log($\"RX {name} ({data.Length} bytes) from {from}.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ClearMonitor_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            _monitor.Clear();");
            sb.AppendLine("            _receivedFields.Clear();");
            sb.AppendLine("            Log(\"Monitor cleared.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- HEX preview ----");
            sb.AppendLine("        private void RefreshHex_Click(object sender, RoutedEventArgs e) => RefreshHex();");
            sb.AppendLine();
            sb.AppendLine("        private void RefreshHex()");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            if (info is null) { HexPreviewBox.Text = string.Empty; return; }");
            sb.AppendLine("            var buffer = BuildBuffer(info, _fields.ToList());");
            sb.AppendLine("            var sb = new StringBuilder();");
            sb.AppendLine("            for (int i = 0; i < buffer.Length; i++)");
            sb.AppendLine("            {");
            sb.AppendLine("                sb.Append(buffer[i].ToString(\"X2\")).Append(' ');");
            sb.AppendLine("                if ((i + 1) % 16 == 0) sb.AppendLine();");
            sb.AppendLine("            }");
            sb.AppendLine("            HexPreviewBox.Text = sb.ToString();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- JSON import/export ----");
            sb.AppendLine("        private void ExportJson_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            if (info is null) return;");
            sb.AppendLine("            var dlg = new Microsoft.Win32.SaveFileDialog { Filter = \"JSON|*.json\", FileName = info.Name + \".json\" };");
            sb.AppendLine("            if (dlg.ShowDialog() != true) return;");
            sb.AppendLine("            var map = _fields.ToDictionary(f => f.Field, f => f.Value);");
            sb.AppendLine("            File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(map, new JsonSerializerOptions { WriteIndented = true }));");
            sb.AppendLine("            Log($\"Export_{info.Name}: written to {dlg.FileName}.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ImportJson_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (Current is null) return;");
            sb.AppendLine("            var dlg = new Microsoft.Win32.OpenFileDialog { Filter = \"JSON|*.json\" };");
            sb.AppendLine("            if (dlg.ShowDialog() != true) return;");
            sb.AppendLine("            var map = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(dlg.FileName));");
            sb.AppendLine("            if (map is null) return;");
            sb.AppendLine("            foreach (var row in _fields)");
            sb.AppendLine("                if (map.TryGetValue(row.Field, out var v)) row.Value = v;");
            sb.AppendLine("            RefreshHex();");
            sb.AppendLine("            Log($\"Imported field values from {dlg.FileName}.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ClearTrafficLog_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            TrafficLogBox.Clear();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Log(string message)");
            sb.AppendLine("        {");
            sb.AppendLine("            TrafficLogBox.AppendText($\"[{DateTime.Now:HH:mm:ss.fff}] {message}\" + Environment.NewLine);");
            sb.AppendLine("            TrafficLogBox.ScrollToEnd();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string BuildModels()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine("using System.Globalization;");
            sb.AppendLine("using System.Runtime.InteropServices;");
            sb.AppendLine();
            sb.AppendLine("namespace GenericSim");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>Static description of one scalar field in a message.</summary>");
            sb.AppendLine("    public sealed class FieldInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public int Offset { get; init; }");
            sb.AppendLine("        public string Field { get; init; } = string.Empty;");
            sb.AppendLine("        public string Type { get; init; } = string.Empty;");
            sb.AppendLine("        public int Size { get; init; }");
            sb.AppendLine("        public string DefaultValue { get; init; } = \"0\";");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Reads this field from a raw little-endian interface buffer.</summary>");
            sb.AppendLine("        public string Read(byte[] buffer)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (Offset + Size > buffer.Length) return \"-\";");
            sb.AppendLine("            return Type switch");
            sb.AppendLine("            {");
            sb.AppendLine("                \"UINT8\" => buffer[Offset].ToString(),");
            sb.AppendLine("                \"INT8\" => ((sbyte)buffer[Offset]).ToString(),");
            sb.AppendLine("                \"UINT16\" => BitConverter.ToUInt16(buffer, Offset).ToString(),");
            sb.AppendLine("                \"INT16\" => BitConverter.ToInt16(buffer, Offset).ToString(),");
            sb.AppendLine("                \"UINT32\" => BitConverter.ToUInt32(buffer, Offset).ToString(),");
            sb.AppendLine("                \"INT32\" => BitConverter.ToInt32(buffer, Offset).ToString(),");
            sb.AppendLine("                \"UINT64\" => BitConverter.ToUInt64(buffer, Offset).ToString(),");
            sb.AppendLine("                \"INT64\" => BitConverter.ToInt64(buffer, Offset).ToString(),");
            sb.AppendLine("                \"FLOAT32\" => BitConverter.ToSingle(buffer, Offset).ToString(CultureInfo.InvariantCulture),");
            sb.AppendLine("                \"FLOAT64\" => BitConverter.ToDouble(buffer, Offset).ToString(CultureInfo.InvariantCulture),");
            sb.AppendLine("                _ => \"-\"");
            sb.AppendLine("            };");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>Editable row bound to the fields DataGrid.</summary>");
            sb.AppendLine("    public sealed class FieldRow : INotifyPropertyChanged");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly FieldInfo _info;");
            sb.AppendLine("        private string _value = \"0\";");
            sb.AppendLine("        public FieldRow(FieldInfo info) { _info = info; }");
            sb.AppendLine("        public int Offset => _info.Offset;");
            sb.AppendLine("        public string Field => _info.Field;");
            sb.AppendLine("        public string Type => _info.Type;");
            sb.AppendLine("        public int Size => _info.Size;");
            sb.AppendLine("        public string Value { get => _value; set { _value = value; OnChanged(nameof(Value)); } }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Writes the edited value straight into the interface buffer image.</summary>");
            sb.AppendLine("        public void WriteToBuffer(byte[] buffer)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_info.Offset + _info.Size > buffer.Length) return;");
            sb.AppendLine("            var ci = CultureInfo.InvariantCulture;");
            sb.AppendLine("            switch (_info.Type)");
            sb.AppendLine("            {");
            sb.AppendLine("                case \"UINT8\": buffer[_info.Offset] = byte.TryParse(_value, out var b) ? b : (byte)0; break;");
            sb.AppendLine("                case \"INT8\": buffer[_info.Offset] = unchecked((byte)(sbyte.TryParse(_value, out var sb8) ? sb8 : 0)); break;");
            sb.AppendLine("                case \"UINT16\": WriteBytes(buffer, BitConverter.GetBytes(ushort.TryParse(_value, out var u16) ? u16 : (ushort)0)); break;");
            sb.AppendLine("                case \"INT16\": WriteBytes(buffer, BitConverter.GetBytes(short.TryParse(_value, out var i16) ? i16 : (short)0)); break;");
            sb.AppendLine("                case \"UINT32\": WriteBytes(buffer, BitConverter.GetBytes(uint.TryParse(_value, out var u32) ? u32 : 0u)); break;");
            sb.AppendLine("                case \"INT32\": WriteBytes(buffer, BitConverter.GetBytes(int.TryParse(_value, out var i32) ? i32 : 0)); break;");
            sb.AppendLine("                case \"UINT64\": WriteBytes(buffer, BitConverter.GetBytes(ulong.TryParse(_value, out var u64) ? u64 : 0ul)); break;");
            sb.AppendLine("                case \"INT64\": WriteBytes(buffer, BitConverter.GetBytes(long.TryParse(_value, out var i64) ? i64 : 0L)); break;");
            sb.AppendLine("                case \"FLOAT32\": WriteBytes(buffer, BitConverter.GetBytes(float.TryParse(_value, NumberStyles.Float, ci, out var f) ? f : 0f)); break;");
            sb.AppendLine("                case \"FLOAT64\": WriteBytes(buffer, BitConverter.GetBytes(double.TryParse(_value, NumberStyles.Float, ci, out var d) ? d : 0d)); break;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void WriteBytes(byte[] buffer, byte[] value)");
            sb.AppendLine("        {");
            sb.AppendLine("            Array.Copy(value, 0, buffer, _info.Offset, Math.Min(value.Length, _info.Size));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public event PropertyChangedEventHandler? PropertyChanged;");
            sb.AppendLine("        private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public sealed class HistoryRow");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Time { get; set; } = string.Empty;");
            sb.AppendLine("        public string Direction { get; set; } = string.Empty;");
            sb.AppendLine("        public string Message { get; set; } = string.Empty;");
            sb.AppendLine("        public int Seq { get; set; }");
            sb.AppendLine("        public int Bytes { get; set; }");
            sb.AppendLine("        public string Periodic { get; set; } = \"No\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public sealed class MonitorRow");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Time { get; set; } = string.Empty;");
            sb.AppendLine("        public string From { get; set; } = string.Empty;");
            sb.AppendLine("        public string Message { get; set; } = string.Empty;");
            sb.AppendLine("        public int Bytes { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public sealed class ReceivedField");
            sb.AppendLine("    {");
            sb.AppendLine("        public int Offset { get; set; }");
            sb.AppendLine("        public string Field { get; set; } = string.Empty;");
            sb.AppendLine("        public string Value { get; set; } = string.Empty;");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>The transport layer (UDP / TCP client / TCP server / RS232) for the generated app.</summary>
        private static string BuildTransports() => """
            // Auto-generated by InterfaceWrapper. Do not edit by hand.
            using System;
            using System.Collections.Generic;
            using System.IO.Ports;
            using System.Linq;
            using System.Net;
            using System.Net.Sockets;
            using System.Threading;
            using System.Threading.Tasks;

            namespace GenericSim
            {
                /// <summary>Abstraction over the physical link (UDP, TCP client/server or RS232).</summary>
                public interface ITransport : IDisposable
                {
                    string Description { get; }
                    void Start();
                    void Send(byte[] data);
                    event Action<byte[], string>? DataReceived;
                    event Action<string>? StatusChanged;
                }

                public sealed class UdpTransport : ITransport
                {
                    private readonly int _localPort;
                    private readonly string _remoteIp;
                    private readonly int _remotePort;
                    private UdpClient? _udp;
                    private CancellationTokenSource? _cts;

                    public UdpTransport(int localPort, string remoteIp, int remotePort)
                    {
                        _localPort = localPort;
                        _remoteIp = remoteIp;
                        _remotePort = remotePort;
                    }

                    public string Description => $"UDP local {_localPort} -> {_remoteIp}:{_remotePort}";
                    public event Action<byte[], string>? DataReceived;
                    public event Action<string>? StatusChanged;

                    public void Start()
                    {
                        _udp = new UdpClient(_localPort);
                        _cts = new CancellationTokenSource();
                        _ = ReceiveLoopAsync(_udp, _cts.Token);
                    }

                    public void Send(byte[] data)
                    {
                        var udp = _udp ?? throw new InvalidOperationException("UDP is not started.");
                        udp.Send(data, data.Length, _remoteIp, _remotePort);
                    }

                    private async Task ReceiveLoopAsync(UdpClient client, CancellationToken token)
                    {
                        while (!token.IsCancellationRequested)
                        {
                            try
                            {
                                var result = await client.ReceiveAsync(token);
                                DataReceived?.Invoke(result.Buffer, result.RemoteEndPoint.ToString());
                            }
                            catch (OperationCanceledException) { break; }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception ex) { StatusChanged?.Invoke("UDP RX error: " + ex.Message); break; }
                        }
                    }

                    public void Dispose()
                    {
                        _cts?.Cancel();
                        _udp?.Close();
                        _udp?.Dispose();
                        _udp = null;
                    }
                }

                public sealed class TcpClientTransport : ITransport
                {
                    private readonly string _remoteIp;
                    private readonly int _remotePort;
                    private TcpClient? _client;
                    private NetworkStream? _stream;
                    private CancellationTokenSource? _cts;

                    public TcpClientTransport(string remoteIp, int remotePort)
                    {
                        _remoteIp = remoteIp;
                        _remotePort = remotePort;
                    }

                    public string Description => $"TCP client -> {_remoteIp}:{_remotePort}";
                    public event Action<byte[], string>? DataReceived;
                    public event Action<string>? StatusChanged;

                    public void Start()
                    {
                        _client = new TcpClient();
                        _client.Connect(_remoteIp, _remotePort);
                        _stream = _client.GetStream();
                        _cts = new CancellationTokenSource();
                        var from = _client.Client.RemoteEndPoint?.ToString() ?? $"{_remoteIp}:{_remotePort}";
                        _ = ReceiveLoopAsync(_stream, from, _cts.Token);
                    }

                    public void Send(byte[] data)
                    {
                        var stream = _stream ?? throw new InvalidOperationException("TCP is not connected.");
                        stream.Write(data, 0, data.Length);
                    }

                    private async Task ReceiveLoopAsync(NetworkStream stream, string from, CancellationToken token)
                    {
                        var buffer = new byte[65536];
                        while (!token.IsCancellationRequested)
                        {
                            try
                            {
                                int read = await stream.ReadAsync(buffer.AsMemory(), token);
                                if (read <= 0) { StatusChanged?.Invoke("TCP connection closed by remote."); break; }
                                DataReceived?.Invoke(buffer.AsSpan(0, read).ToArray(), from);
                            }
                            catch (OperationCanceledException) { break; }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception ex) { StatusChanged?.Invoke("TCP RX error: " + ex.Message); break; }
                        }
                    }

                    public void Dispose()
                    {
                        _cts?.Cancel();
                        _stream?.Dispose();
                        _client?.Close();
                        _client = null;
                    }
                }

                public sealed class TcpServerTransport : ITransport
                {
                    private readonly int _localPort;
                    private readonly object _sync = new();
                    private readonly List<TcpClient> _clients = new();
                    private TcpListener? _listener;
                    private CancellationTokenSource? _cts;

                    public TcpServerTransport(int localPort) => _localPort = localPort;

                    public string Description => $"TCP server on port {_localPort}";
                    public event Action<byte[], string>? DataReceived;
                    public event Action<string>? StatusChanged;

                    public void Start()
                    {
                        _listener = new TcpListener(IPAddress.Any, _localPort);
                        _listener.Start();
                        _cts = new CancellationTokenSource();
                        _ = AcceptLoopAsync(_listener, _cts.Token);
                    }

                    public void Send(byte[] data)
                    {
                        List<TcpClient> clients;
                        lock (_sync) clients = _clients.ToList();
                        if (clients.Count == 0) throw new InvalidOperationException("No TCP client is connected.");
                        foreach (var client in clients)
                        {
                            try { client.GetStream().Write(data, 0, data.Length); }
                            catch (Exception ex) { StatusChanged?.Invoke("TCP TX error: " + ex.Message); }
                        }
                    }

                    private async Task AcceptLoopAsync(TcpListener listener, CancellationToken token)
                    {
                        while (!token.IsCancellationRequested)
                        {
                            try
                            {
                                var client = await listener.AcceptTcpClientAsync(token);
                                lock (_sync) _clients.Add(client);
                                var from = client.Client.RemoteEndPoint?.ToString() ?? "client";
                                StatusChanged?.Invoke($"TCP client connected: {from}.");
                                _ = ReceiveLoopAsync(client, from, token);
                            }
                            catch (OperationCanceledException) { break; }
                            catch (ObjectDisposedException) { break; }
                            catch (Exception ex) { StatusChanged?.Invoke("TCP accept error: " + ex.Message); break; }
                        }
                    }

                    private async Task ReceiveLoopAsync(TcpClient client, string from, CancellationToken token)
                    {
                        var buffer = new byte[65536];
                        try
                        {
                            var stream = client.GetStream();
                            while (!token.IsCancellationRequested)
                            {
                                int read = await stream.ReadAsync(buffer.AsMemory(), token);
                                if (read <= 0) break;
                                DataReceived?.Invoke(buffer.AsSpan(0, read).ToArray(), from);
                            }
                        }
                        catch (OperationCanceledException) { }
                        catch (ObjectDisposedException) { }
                        catch (Exception ex) { StatusChanged?.Invoke("TCP RX error: " + ex.Message); }
                        lock (_sync) _clients.Remove(client);
                        StatusChanged?.Invoke($"TCP client disconnected: {from}.");
                        client.Close();
                    }

                    public void Dispose()
                    {
                        _cts?.Cancel();
                        _listener?.Stop();
                        lock (_sync)
                        {
                            foreach (var client in _clients) client.Close();
                            _clients.Clear();
                        }
                    }
                }

                public sealed class SerialTransport : ITransport
                {
                    private readonly string _portName;
                    private readonly int _baudRate;
                    private readonly Parity _parity;
                    private readonly StopBits _stopBits;
                    private SerialPort? _port;

                    public SerialTransport(string portName, int baudRate, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
                    {
                        _portName = portName;
                        _baudRate = baudRate;
                        _parity = parity;
                        _stopBits = stopBits;
                    }

                    public string Description => $"RS232 {_portName} @ {_baudRate} baud, {_parity} parity, {_stopBits} stop bits";
                    public event Action<byte[], string>? DataReceived;
                    public event Action<string>? StatusChanged;

                    public void Start()
                    {
                        _port = new SerialPort(_portName, _baudRate, _parity, 8, _stopBits);
                        _port.DataReceived += OnDataReceived;
                        _port.Open();
                    }

                    public void Send(byte[] data)
                    {
                        var port = _port ?? throw new InvalidOperationException("RS232 port is not open.");
                        port.Write(data, 0, data.Length);
                    }

                    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
                    {
                        try
                        {
                            var port = _port;
                            if (port is null) return;
                            int count = port.BytesToRead;
                            if (count <= 0) return;
                            var data = new byte[count];
                            port.Read(data, 0, count);
                            DataReceived?.Invoke(data, _portName);
                        }
                        catch (Exception ex) { StatusChanged?.Invoke("RS232 RX error: " + ex.Message); }
                    }

                    public void Dispose()
                    {
                        var port = _port;
                        _port = null;
                        if (port is not null)
                        {
                            port.DataReceived -= OnDataReceived;
                            try { if (port.IsOpen) port.Close(); } catch { /* port already gone */ }
                            port.Dispose();
                        }
                    }
                }
            }

            """;

        private string BuildMessageCatalog()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using WrapperFromCToSharp;");
            sb.AppendLine();
            sb.AppendLine("namespace GenericSim");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>Static catalog of all messages discovered from the C files.</summary>");
            sb.AppendLine("    public sealed class MessageInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Name { get; init; } = string.Empty;");
            sb.AppendLine("        public int MessageId { get; init; }");
            sb.AppendLine("        public int Length { get; init; }");
            sb.AppendLine("        public FieldInfo[] Fields { get; init; } = Array.Empty<FieldInfo>();");
            sb.AppendLine("        public Func<IntPtr> GetPhysical { get; init; } = () => IntPtr.Zero;");
            sb.AppendLine("        public Action<byte[], IntPtr> ConvertToInterface { get; init; } = (_, _) => { };");
            sb.AppendLine("        public Action<byte[], IntPtr> ConvertToPhysical { get; init; } = (_, _) => { };");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public static class MessageCatalog");
            sb.AppendLine("    {");
            sb.AppendLine("        public static readonly IReadOnlyList<MessageInfo> Messages = BuildAll();");
            sb.AppendLine();
            sb.AppendLine("        public static MessageInfo? ByName(string name) => Messages.FirstOrDefault(m => m.Name == name);");
            sb.AppendLine("        public static MessageInfo? ById(int id) => Messages.FirstOrDefault(m => m.MessageId == id);");
            sb.AppendLine();
            sb.AppendLine("        private static List<MessageInfo> BuildAll()");
            sb.AppendLine("        {");
            sb.AppendLine("            var list = new List<MessageInfo>();");

            foreach (var msg in _parseResult.Messages.OrderBy(m => m.MessageId ?? int.MaxValue))
            {
                var hasGlobal = !string.IsNullOrEmpty(msg.GlobalVariable);
                var getPhys = hasGlobal ? $"() => WrapperInterop.Wrapper_Get_{msg.MessageName}()" : "() => IntPtr.Zero";
                var toIntf = msg.HasToInterface
                    ? $"(buf, phys) => WrapperInterop.{msg.MessageName}_ConvertToInterface(phys, buf)"
                    : "(_, _) => { }";
                var toPhys = msg.HasToPhysical
                    ? $"(buf, phys) => WrapperInterop.{msg.MessageName}_ConvertToPhysical(phys, buf)"
                    : "(_, _) => { }";

                sb.AppendLine("            list.Add(new MessageInfo");
                sb.AppendLine("            {");
                sb.AppendLine($"                Name = \"{msg.MessageName}\",");
                sb.AppendLine($"                MessageId = {msg.MessageId ?? 0},");
                sb.AppendLine($"                Length = {msg.Length},");
                sb.AppendLine($"                GetPhysical = {getPhys},");
                sb.AppendLine($"                ConvertToInterface = {toIntf},");
                sb.AppendLine($"                ConvertToPhysical = {toPhys},");
                if (msg.Fields.Count == 0)
                {
                    sb.AppendLine("                Fields = Array.Empty<FieldInfo>()");
                }
                else
                {
                    sb.AppendLine("                Fields = new[]");
                    sb.AppendLine("                {");
                    foreach (var f in msg.Fields)
                    {
                        var def = DefaultFor(msg, f);
                        sb.AppendLine($"                    new FieldInfo {{ Offset = {f.Offset}, Field = \"{f.Field}\", Type = \"{f.Type}\", Size = {f.Size}, DefaultValue = \"{def}\" }},");
                    }
                    sb.AppendLine("                }");
                }
                sb.AppendLine("            });");
            }

            sb.AppendLine("            return list;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>Default field value: the message id for the msgId field, total length for totalLength, else 0.</summary>
        private static string DefaultFor(MessageDefinition msg, MessageField field)
        {
            if (field.Field.EndsWith(".msgId", StringComparison.OrdinalIgnoreCase))
                return (msg.MessageId ?? 0).ToString(CultureInfo.InvariantCulture);
            if (field.Field.EndsWith(".totalLength", StringComparison.OrdinalIgnoreCase))
                return msg.Length.ToString(CultureInfo.InvariantCulture);
            return "0";
        }

        private string BuildReadme()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# {ProjectName}");
            sb.AppendLine();
            sb.AppendLine("A WPF message simulator (MBT side) generated by **InterfaceWrapper**.");
            sb.AppendLine("It consumes the native `WrapperFromCToSharp.dll` to build and parse messages.");
            sb.AppendLine();
            sb.AppendLine("## Build order");
            sb.AppendLine();
            sb.AppendLine($"1. Build `{WrapperGenerator.ProjectName}.vcxproj` for **x64** (produces the DLL).");
            sb.AppendLine($"2. Build/run `{ProjectName}.csproj` (x64). The DLL is copied next to the executable.");
            sb.AppendLine();
            sb.AppendLine("## Features");
            sb.AppendLine();
            sb.AppendLine("- Message list with per-message editable field grid (Offset/Field/Type/Size/Value).");
            sb.AppendLine("- Auto sequence / timestamp / CRC toggles.");
            sb.AppendLine("- Live HEX preview built through the native convert functions.");
            sb.AppendLine("- Selectable transport: UDP, TCP (client or server) or RS232 serial.");
            sb.AppendLine("- Transport settings in the top bar (ports, IP, TCP mode, COM port, baud rate, parity, stop bits).");
            sb.AppendLine("- Periodic send (interval + count), send history, NIRON\u2192MBT monitor and traffic log.");
            sb.AppendLine("- Export/Import message field values as JSON.");
            return sb.ToString();
        }

        private static string WriteFile(string path, string content)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            return path;
        }
    }
}
