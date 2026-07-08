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
        private readonly string _sourceName;
        private readonly string _destinationName;

        public GenericSimGenerator(ParseResult parseResult, string sourceName = "MBT", string destinationName = "NIRON")
        {
            _parseResult = parseResult ?? throw new ArgumentNullException(nameof(parseResult));
            _sourceName = SanitizeSide(sourceName, "MBT");
            _destinationName = SanitizeSide(destinationName, "NIRON");
        }

        /// <summary>Keeps only characters that are safe in both XAML and C# string literals.</summary>
        private static string SanitizeSide(string? name, string fallback)
        {
            var cleaned = new string((name ?? string.Empty)
                .Where(c => char.IsLetterOrDigit(c) || c is '_' or '-' or ' ').ToArray()).Trim();
            return cleaned.Length == 0 ? fallback : cleaned;
        }

        /// <summary>Replaces the default side names (MBT / NIRON) with the user provided Source and
        /// Destination names. Temp tokens make swapped names (e.g. source = NIRON) safe.</summary>
        private string ApplySides(string content) => content
            .Replace("MBT", "\uE000")
            .Replace("NIRON", "\uE001")
            .Replace("\uE000", _sourceName)
            .Replace("\uE001", _destinationName);

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
            written.Add(WriteFile(Path.Combine(projectDir, "MainWindow.xaml"), ApplySides(BuildMainWindowXaml())));
            written.Add(WriteFile(Path.Combine(projectDir, "MainWindow.xaml.cs"), ApplySides(BuildMainWindowXamlCs())));
            written.Add(WriteFile(Path.Combine(projectDir, "MessageCatalog.cs"), BuildMessageCatalog()));
            written.Add(WriteFile(Path.Combine(projectDir, "Models.cs"), ApplySides(BuildModels())));
            written.Add(WriteFile(Path.Combine(projectDir, "Scenario.cs"), BuildScenario()));
            written.Add(WriteFile(Path.Combine(projectDir, "Recording.cs"), BuildRecording()));
            written.Add(WriteFile(Path.Combine(projectDir, "Transports.cs"), BuildTransports()));
            written.Add(WriteFile(Path.Combine(projectDir, "PrecisionTimer.cs"), BuildPrecisionTimer()));
            written.Add(WriteFile(Path.Combine(projectDir, "README.md"), ApplySides(BuildReadme())));

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
            sb.AppendLine("        <!-- Theme brushes: swapped at runtime by the Look and Feel selector in the header. -->");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.WindowBackground\" Color=\"#0E1621\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.PanelBackground\" Color=\"#0B1220\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.Foreground\" Color=\"#E6EDF3\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.DimForeground\" Color=\"#7A8CA0\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.HeaderForeground\" Color=\"#8FB4D9\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.Border\" Color=\"#22303C\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.RowBackground\" Color=\"#132030\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.AltRowBackground\" Color=\"#0F1A28\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.Accent\" Color=\"#1F6FEB\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.TabBackground\" Color=\"#8FB4D9\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.TabForeground\" Color=\"#000000\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.GridHeaderForeground\" Color=\"#000000\"/>");
            sb.AppendLine("        <SolidColorBrush x:Key=\"Theme.GraphRx\" Color=\"#FFFFFF\"/>");
            sb.AppendLine("        <Style TargetType=\"TextBlock\"><Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.Foreground}\"/></Style>");
            sb.AppendLine("        <Style TargetType=\"GroupBox\">");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.HeaderForeground}\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"{DynamicResource Theme.Border}\"/>");
            sb.AppendLine("            <Setter Property=\"Margin\" Value=\"6\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"6\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"DataGrid\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"{DynamicResource Theme.WindowBackground}\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.Foreground}\"/>");
            sb.AppendLine("            <Setter Property=\"RowBackground\" Value=\"{DynamicResource Theme.RowBackground}\"/>");
            sb.AppendLine("            <Setter Property=\"AlternatingRowBackground\" Value=\"{DynamicResource Theme.AltRowBackground}\"/>");
            sb.AppendLine("            <Setter Property=\"GridLinesVisibility\" Value=\"Horizontal\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"{DynamicResource Theme.Border}\"/>");
            sb.AppendLine("            <Setter Property=\"HeadersVisibility\" Value=\"Column\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <!-- Column header text follows the theme. -->");
            sb.AppendLine("        <Style TargetType=\"DataGridColumnHeader\">");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.GridHeaderForeground}\"/>");
            sb.AppendLine("            <Setter Property=\"FontWeight\" Value=\"Bold\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"6,4\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"Button\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"{DynamicResource Theme.Accent}\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"White\"/>");
            sb.AppendLine("            <Setter Property=\"BorderThickness\" Value=\"0\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"8,5\"/>");
            sb.AppendLine("            <Setter Property=\"Margin\" Value=\"0,3\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"TextBox\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"{DynamicResource Theme.PanelBackground}\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.Foreground}\"/>");
            sb.AppendLine("            <Setter Property=\"BorderBrush\" Value=\"{DynamicResource Theme.Border}\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("        <Style TargetType=\"TabItem\">");
            sb.AppendLine("            <Setter Property=\"Background\" Value=\"{DynamicResource Theme.TabBackground}\"/>");
            sb.AppendLine("            <Setter Property=\"Foreground\" Value=\"{DynamicResource Theme.TabForeground}\"/>");
            sb.AppendLine("            <Setter Property=\"FontWeight\" Value=\"Bold\"/>");
            sb.AppendLine("            <Setter Property=\"Padding\" Value=\"14,4\"/>");
            sb.AppendLine("        </Style>");
            sb.AppendLine("    </Window.Resources>");
            sb.AppendLine();
            sb.AppendLine("    <Grid Margin=\"10\">");
            sb.AppendLine("        <Grid.RowDefinitions>");
            sb.AppendLine("            <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("            <RowDefinition Height=\"*\"/>");
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
            sb.AppendLine("                <TextBlock Text=\"Look &amp; Feel\" VerticalAlignment=\"Center\" Margin=\"6,0\"/>");
            sb.AppendLine("                <ComboBox x:Name=\"ThemeCombo\" Width=\"120\" VerticalAlignment=\"Center\" SelectedIndex=\"0\"");
            sb.AppendLine("                          SelectionChanged=\"ThemeCombo_SelectionChanged\" ToolTip=\"Change the application look and feel\">");
            sb.AppendLine("                    <ComboBoxItem Content=\"Dark Blue\"/>");
            sb.AppendLine("                    <ComboBoxItem Content=\"Midnight Black\"/>");
            sb.AppendLine("                    <ComboBoxItem Content=\"Light\"/>");
            sb.AppendLine("                    <ComboBoxItem Content=\"Military Green\"/>");
            sb.AppendLine("                </ComboBox>");
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
            sb.AppendLine("                    <TextBlock Text=\"NIRON IP\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
            sb.AppendLine("                    <TextBox x:Name=\"RemoteIpBox\" Text=\"127.0.0.1\" Width=\"100\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                    <TextBlock Text=\"NIRON Port\" VerticalAlignment=\"Center\" Margin=\"12,0,6,0\"/>");
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
            sb.AppendLine("                <Button x:Name=\"RecordButton\" Content=\"\u25CF Record\" Background=\"#6E40C9\" Margin=\"6,0,0,0\" Click=\"Record_Click\"");
            sb.AppendLine("                        ToolTip=\"Record all incoming and outgoing messages to a binary file\"/>");
            sb.AppendLine("            </StackPanel>");
            sb.AppendLine("        </Grid>");
            sb.AppendLine();
            // Tabs: Simulator (original UI) + Scenario (drag & drop sequencer)
            sb.AppendLine("        <TabControl Grid.Row=\"1\" Background=\"#0E1621\" BorderBrush=\"#22303C\">");
            sb.AppendLine("            <TabItem Header=\"Simulator\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"220\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine();
            // Main 3-column area
            sb.AppendLine("        <Grid Grid.Row=\"0\">");
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
            sb.AppendLine("                    <DataGrid x:Name=\"FieldsGrid\" Grid.Row=\"1\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" RowDetailsVisibilityMode=\"Collapsed\"");
            sb.AppendLine("                              PreviewMouseLeftButtonUp=\"ArrayRow_ToggleOnClick\" LoadingRow=\"ArrayRow_LoadingRow\">");
            sb.AppendLine("                        <!-- An array field is a normal row at its real offset; clicking the row opens its");
            sb.AppendLine("                             cells as a nested table right below it (DetailsVisibility is set in code-behind");
            sb.AppendLine("                             because a RowStyle trigger can be overridden by the DataGrid's own coercion). -->");
            sb.AppendLine("                        <DataGrid.Columns>");
            sb.AppendLine("                            <!-- The arrow is a visual indicator; clicking anywhere on the array row");
            sb.AppendLine("                                 toggles the nested cells table (grid PreviewMouseLeftButtonUp). -->");
            sb.AppendLine("                            <DataGridTemplateColumn Width=\"30\" IsReadOnly=\"True\">");
            sb.AppendLine("                                <DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                    <DataTemplate>");
            sb.AppendLine("                                        <Border Background=\"Transparent\" Cursor=\"Hand\" Visibility=\"{Binding ExpanderVisibility}\"");
            sb.AppendLine("                                                ToolTip=\"Click the row to open / close the array cells\">");
            sb.AppendLine("                                            <TextBlock Text=\"{Binding ExpanderGlyph}\" Foreground=\"#8FB4D9\" FontSize=\"12\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                        </Border>");
            sb.AppendLine("                                    </DataTemplate>");
            sb.AppendLine("                                </DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                            </DataGridTemplateColumn>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" IsReadOnly=\"True\" Width=\"70\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" IsReadOnly=\"True\" Width=\"2*\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Type\" Binding=\"{Binding Type}\" IsReadOnly=\"True\" Width=\"90\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Size\" Binding=\"{Binding Size}\" IsReadOnly=\"True\" Width=\"60\"/>");
            sb.AppendLine("                            <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value, UpdateSourceTrigger=PropertyChanged}\" Width=\"*\"/>");
            sb.AppendLine("                        </DataGrid.Columns>");
            sb.AppendLine("                        <DataGrid.RowDetailsTemplate>");
            sb.AppendLine("                            <DataTemplate>");
            sb.AppendLine("                                <Border Background=\"#0B1220\" BorderBrush=\"#22303C\" BorderThickness=\"1\" CornerRadius=\"3\" Margin=\"30,2,8,6\" Padding=\"6\">");
            sb.AppendLine("                                    <StackPanel>");
            sb.AppendLine("                                        <StackPanel Orientation=\"Horizontal\" Margin=\"0,0,0,4\">");
            sb.AppendLine("                                            <TextBlock Text=\"Elements\" VerticalAlignment=\"Center\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                            <TextBox Text=\"{Binding CountText, UpdateSourceTrigger=PropertyChanged}\" Width=\"60\" Margin=\"6,0,0,0\"/>");
            sb.AppendLine("                                            <Button Content=\"Apply\" Width=\"60\" Margin=\"6,0,0,0\" Click=\"ApplyArrayCount_Click\"/>");
            sb.AppendLine("                                            <TextBlock Text=\"{Binding ArraySummary}\" VerticalAlignment=\"Center\" Margin=\"10,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                        </StackPanel>");
            sb.AppendLine("                                        <DataGrid ItemsSource=\"{Binding Cells}\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" MaxHeight=\"180\">");
            sb.AppendLine("                                            <DataGrid.Columns>");
            sb.AppendLine("                                                <DataGridTextColumn Header=\"Idx\" Binding=\"{Binding Index}\" IsReadOnly=\"True\" Width=\"46\"/>");
            sb.AppendLine("                                                <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" IsReadOnly=\"True\" Width=\"60\"/>");
            sb.AppendLine("                                                <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" IsReadOnly=\"True\" Width=\"2*\"/>");
            sb.AppendLine("                                                <DataGridTextColumn Header=\"Type\" Binding=\"{Binding Type}\" IsReadOnly=\"True\" Width=\"70\"/>");
            sb.AppendLine("                                                <!-- Always-active TextBox: a single click puts the caret in the cell so the");
            sb.AppendLine("                                                     value can be typed directly (DataGrid edit-mode inside RowDetails is unreliable). -->");
            sb.AppendLine("                                                <DataGridTemplateColumn Header=\"Value\" Width=\"*\" IsReadOnly=\"True\">");
            sb.AppendLine("                                                    <DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                                        <DataTemplate>");
            sb.AppendLine("                                                            <TextBox Text=\"{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}\" BorderThickness=\"0\"");
            sb.AppendLine("                                                                     Background=\"Transparent\" Foreground=\"#E6EDF3\" Padding=\"2,0\" ToolTip=\"Type the cell value\"/>");
            sb.AppendLine("                                                        </DataTemplate>");
            sb.AppendLine("                                                    </DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                                </DataGridTemplateColumn>");
            sb.AppendLine("                                            </DataGrid.Columns>");
            sb.AppendLine("                                        </DataGrid>");
            sb.AppendLine("                                    </StackPanel>");
            sb.AppendLine("                                </Border>");
            sb.AppendLine("                            </DataTemplate>");
            sb.AppendLine("                        </DataGrid.RowDetailsTemplate>");
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
            sb.AppendLine("        <Grid Grid.Row=\"1\">");
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
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
            AppendScenarioTabXaml(sb);
            AppendRecordTabXaml(sb);
            AppendGraphTabXaml(sb);
            AppendRecordGraphTabXaml(sb);
            AppendStatisticsTabXaml(sb);
            sb.AppendLine("        </TabControl>");
            sb.AppendLine("    </Grid>");
            sb.AppendLine("</Window>");
            return ThemeifyXaml(sb.ToString());
        }

        /// <summary>Rewrites the inline hex colors to theme DynamicResource references so the
        /// Look &amp; Feel selector can restyle the whole window at runtime.</summary>
        private static string ThemeifyXaml(string xaml) => xaml
            .Replace("Background=\"#0E1621\"", "Background=\"{DynamicResource Theme.WindowBackground}\"")
            .Replace("Background=\"#0B1220\"", "Background=\"{DynamicResource Theme.PanelBackground}\"")
            .Replace("Background=\"#1F6FEB\"", "Background=\"{DynamicResource Theme.Accent}\"")
            .Replace("Foreground=\"#E6EDF3\"", "Foreground=\"{DynamicResource Theme.Foreground}\"")
            .Replace("Foreground=\"#7A8CA0\"", "Foreground=\"{DynamicResource Theme.DimForeground}\"")
            .Replace("Foreground=\"#8FB4D9\"", "Foreground=\"{DynamicResource Theme.HeaderForeground}\"")
            .Replace("Foreground=\"#FFFFFF\"", "Foreground=\"{DynamicResource Theme.GraphRx}\"")
            .Replace("BorderBrush=\"#22303C\"", "BorderBrush=\"{DynamicResource Theme.Border}\"");

        /// <summary>
        /// The Scenario tab: an "All Messages" palette on the left, a drag &amp; drop step list
        /// in the middle (each step with its own asynchronous send attributes) and a log below.
        /// </summary>
        private static void AppendScenarioTabXaml(StringBuilder sb)
        {
            sb.AppendLine("            <TabItem Header=\"Scenario\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"170\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine();
            // Scenario toolbar: New / Open / Save on the left, Run / Stop on the right.
            sb.AppendLine("                    <DockPanel Grid.Row=\"0\" Margin=\"6,4\">");
            sb.AppendLine("                        <StackPanel DockPanel.Dock=\"Right\" Orientation=\"Horizontal\">");
            sb.AppendLine("                            <Button Content=\"\u25B6 Run\" Background=\"#238636\" Width=\"80\" Click=\"RunScenario_Click\"/>");
            sb.AppendLine("                            <Button Content=\"\u25A0 Stop\" Background=\"#DA3633\" Width=\"80\" Margin=\"6,3,0,3\" Click=\"StopScenario_Click\"/>");
            sb.AppendLine("                        </StackPanel>");
            sb.AppendLine("                        <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                            <Button Content=\"New Scenario\" Click=\"NewScenario_Click\"/>");
            sb.AppendLine("                            <Button Content=\"Open\" Margin=\"6,3,0,3\" Click=\"OpenScenario_Click\"/>");
            sb.AppendLine("                            <Button Content=\"Save\" Margin=\"6,3,0,3\" Click=\"SaveScenario_Click\"/>");
            sb.AppendLine("                            <TextBlock Text=\"Scenario:\" VerticalAlignment=\"Center\" Margin=\"14,0,4,0\" Foreground=\"#8FB4D9\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"ScenarioNameText\" Text=\"(unsaved scenario)\" VerticalAlignment=\"Center\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                        </StackPanel>");
            sb.AppendLine("                    </DockPanel>");
            sb.AppendLine();
            // Left palette + middle step list.
            sb.AppendLine("                    <Grid Grid.Row=\"1\">");
            sb.AppendLine("                        <Grid.ColumnDefinitions>");
            sb.AppendLine("                            <ColumnDefinition Width=\"280\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"340\"/>");
            sb.AppendLine("                        </Grid.ColumnDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"0\" Header=\"All Messages\">");
            sb.AppendLine("                            <DockPanel>");
            sb.AppendLine("                                <TextBlock DockPanel.Dock=\"Top\" Text=\"Drag a message to the scenario (or double-click)\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                                <StackPanel DockPanel.Dock=\"Bottom\" Margin=\"0,6,0,0\">");
            sb.AppendLine("                                    <TextBlock Text=\"Operations (drag into the scenario, or double-click)\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,4\"/>");
            sb.AppendLine("                                    <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                                        <Border x:Name=\"SleepDragItem\" Background=\"#B08800\" CornerRadius=\"3\" Padding=\"8,4\" Cursor=\"Hand\"");
            sb.AppendLine("                                                ToolTip=\"Drag into the scenario: when the scenario reaches it, every later step is delayed by the given milliseconds\"");
            sb.AppendLine("                                                PreviewMouseLeftButtonDown=\"SleepItem_PreviewMouseLeftButtonDown\"");
            sb.AppendLine("                                                PreviewMouseMove=\"SleepItem_PreviewMouseMove\">");
            sb.AppendLine("                                            <TextBlock Text=\"\u23F1 Sleep\" Foreground=\"White\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                                        </Border>");
            sb.AppendLine("                                        <TextBox x:Name=\"SleepMsBox\" Width=\"70\" Margin=\"8,0,0,0\" Text=\"1000\" VerticalContentAlignment=\"Center\"");
            sb.AppendLine("                                                 ToolTip=\"Sleep duration in milliseconds\"/>");
            sb.AppendLine("                                        <TextBlock Text=\"ms\" VerticalAlignment=\"Center\" Margin=\"4,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                    </StackPanel>");
            sb.AppendLine("                                    <StackPanel Orientation=\"Horizontal\" Margin=\"0,6,0,0\">");
            sb.AppendLine("                                        <Border x:Name=\"ResponseDragItem\" Background=\"#1F8FA8\" CornerRadius=\"3\" Padding=\"8,4\" Cursor=\"Hand\"");
            sb.AppendLine("                                                ToolTip=\"Drag into the scenario: when the chosen message is received and its field equals the value, the configured message is sent\"");
            sb.AppendLine("                                                PreviewMouseLeftButtonDown=\"ResponseItem_PreviewMouseLeftButtonDown\"");
            sb.AppendLine("                                                PreviewMouseMove=\"ResponseItem_PreviewMouseMove\">");
            sb.AppendLine("                                            <TextBlock Text=\"\u21C4 Response\" Foreground=\"White\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                                        </Border>");
            sb.AppendLine("                                        <TextBlock Text=\"receive \u2192 check field \u2192 send\" VerticalAlignment=\"Center\" Margin=\"8,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                    </StackPanel>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                                <ListBox x:Name=\"AllMessagesList\" Background=\"#0B1220\" Foreground=\"#E6EDF3\" BorderBrush=\"#22303C\"");
            sb.AppendLine("                                         PreviewMouseLeftButtonDown=\"AllMessagesList_PreviewMouseLeftButtonDown\"");
            sb.AppendLine("                                         PreviewMouseMove=\"AllMessagesList_PreviewMouseMove\"");
            sb.AppendLine("                                         MouseDoubleClick=\"AllMessagesList_MouseDoubleClick\"/>");
            sb.AppendLine("                            </DockPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"1\" Header=\"Scenario Steps (each step runs by its own attributes)\">");
            sb.AppendLine("                            <DockPanel>");
            sb.AppendLine("                                <TextBlock DockPanel.Dock=\"Top\" Text=\"Drag messages or the Sleep / Response operations here. A Sleep step pauses the scenario; a Response step sends a message when a received message's field matches a value. Click a step to edit it.\" TextWrapping=\"Wrap\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                                <StackPanel DockPanel.Dock=\"Bottom\" Orientation=\"Horizontal\">");
            sb.AppendLine("                                    <Button Content=\"Remove Step\" Background=\"#8B2E1F\" Click=\"RemoveStep_Click\"/>");
            sb.AppendLine("                                    <Button Content=\"\u2191 Move Up\" Margin=\"6,3,0,3\" Click=\"MoveStepUp_Click\"/>");
            sb.AppendLine("                                    <Button Content=\"\u2193 Move Down\" Margin=\"6,3,0,3\" Click=\"MoveStepDown_Click\"/>");
            sb.AppendLine("                                    <Button Content=\"Clear Steps\" Background=\"#8B2E1F\" Margin=\"6,3,0,3\" Click=\"ClearSteps_Click\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                                <DataGrid x:Name=\"ScenarioStepsList\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" AllowDrop=\"True\"");
            sb.AppendLine("                                          Drop=\"ScenarioSteps_Drop\" DragOver=\"ScenarioSteps_DragOver\" Margin=\"0,0,0,6\"");
            sb.AppendLine("                                          SelectionChanged=\"ScenarioStepsList_SelectionChanged\">");
            sb.AppendLine("                                    <DataGrid.Columns>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"#\" Binding=\"{Binding Index}\" IsReadOnly=\"True\" Width=\"40\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" IsReadOnly=\"True\" Width=\"*\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Send Type\" Binding=\"{Binding SendType}\" IsReadOnly=\"True\" Width=\"90\"/>");
            sb.AppendLine("                                        <DataGridCheckBoxColumn Header=\"Periodic\" Binding=\"{Binding Periodic, UpdateSourceTrigger=PropertyChanged}\" Width=\"70\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Time (ms)\" Binding=\"{Binding TimeMs, UpdateSourceTrigger=PropertyChanged}\" Width=\"90\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Interval (ms)\" Binding=\"{Binding PeriodicIntervalMs, UpdateSourceTrigger=PropertyChanged}\" Width=\"100\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Max (0=unlimited)\" Binding=\"{Binding MaxMessages, UpdateSourceTrigger=PropertyChanged}\" Width=\"120\"/>");
            sb.AppendLine("                                    </DataGrid.Columns>");
            sb.AppendLine("                                </DataGrid>");
            sb.AppendLine("                            </DockPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"2\">");
            sb.AppendLine("                            <GroupBox.Header>");
            sb.AppendLine("                                <TextBlock x:Name=\"StepFieldsHeader\" Text=\"Step Fields (select a step)\" Foreground=\"#8FB4D9\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                            </GroupBox.Header>");
            sb.AppendLine("                            <DockPanel>");
            sb.AppendLine("                                <TextBlock DockPanel.Dock=\"Top\" Text=\"Edit the values this step sends (saved with the scenario); click an array row to open its cells and type each value\" TextWrapping=\"Wrap\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                                <StackPanel x:Name=\"ResponseEditPanel\" DockPanel.Dock=\"Top\" Visibility=\"Collapsed\" Margin=\"0,0,0,6\">");
            sb.AppendLine("                                    <TextBlock Text=\"1) When this message is received:\" Foreground=\"#8FB4D9\" Margin=\"0,0,0,2\"/>");
            sb.AppendLine("                                    <ComboBox x:Name=\"ResponseTriggerCombo\" SelectionChanged=\"ResponseTriggerCombo_SelectionChanged\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"2) And this field equals the value:\" Foreground=\"#8FB4D9\" Margin=\"0,6,0,2\"/>");
            sb.AppendLine("                                    <ComboBox x:Name=\"ResponseFieldCombo\" SelectionChanged=\"ResponseFieldCombo_SelectionChanged\"/>");
            sb.AppendLine("                                    <TextBox x:Name=\"ResponseValueBox\" Margin=\"0,4,0,0\" TextChanged=\"ResponseValueBox_TextChanged\" ToolTip=\"Expected value of the chosen field\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"3) Then send this message (edit its values below):\" Foreground=\"#8FB4D9\" Margin=\"0,6,0,2\"/>");
            sb.AppendLine("                                    <ComboBox x:Name=\"ResponseSendCombo\" SelectionChanged=\"ResponseSendCombo_SelectionChanged\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                                <DataGrid x:Name=\"StepFieldsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" RowDetailsVisibilityMode=\"Collapsed\"");
            sb.AppendLine("                                          PreviewMouseLeftButtonUp=\"ArrayRow_ToggleOnClick\" LoadingRow=\"ArrayRow_LoadingRow\">");
            sb.AppendLine("                                    <!-- Array rows open into a nested cells table on row click (same as the");
            sb.AppendLine("                                         Simulator tab; DetailsVisibility is set in code-behind). -->");
            sb.AppendLine("                                    <DataGrid.Columns>");
            sb.AppendLine("                                        <!-- The arrow is a visual indicator; clicking anywhere on the array row");
            sb.AppendLine("                                             toggles the nested cells table (grid PreviewMouseLeftButtonUp). -->");
            sb.AppendLine("                                        <DataGridTemplateColumn Width=\"26\" IsReadOnly=\"True\">");
            sb.AppendLine("                                            <DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                                <DataTemplate>");
            sb.AppendLine("                                                    <Border Background=\"Transparent\" Cursor=\"Hand\" Visibility=\"{Binding ExpanderVisibility}\"");
            sb.AppendLine("                                                            ToolTip=\"Click the row to open / close the array cells\">");
            sb.AppendLine("                                                        <TextBlock Text=\"{Binding ExpanderGlyph}\" Foreground=\"#8FB4D9\" FontSize=\"12\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                                    </Border>");
            sb.AppendLine("                                                </DataTemplate>");
            sb.AppendLine("                                            </DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                        </DataGridTemplateColumn>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" IsReadOnly=\"True\" Width=\"2*\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Type\" Binding=\"{Binding Type}\" IsReadOnly=\"True\" Width=\"70\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value, UpdateSourceTrigger=PropertyChanged}\" Width=\"*\"/>");
            sb.AppendLine("                                    </DataGrid.Columns>");
            sb.AppendLine("                                    <DataGrid.RowDetailsTemplate>");
            sb.AppendLine("                                        <DataTemplate>");
            sb.AppendLine("                                            <Border Background=\"#0B1220\" BorderBrush=\"#22303C\" BorderThickness=\"1\" CornerRadius=\"3\" Margin=\"26,2,8,6\" Padding=\"6\">");
            sb.AppendLine("                                                <StackPanel>");
            sb.AppendLine("                                                    <StackPanel Orientation=\"Horizontal\" Margin=\"0,0,0,4\">");
            sb.AppendLine("                                                        <TextBlock Text=\"Elements\" VerticalAlignment=\"Center\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                                        <TextBox Text=\"{Binding CountText, UpdateSourceTrigger=PropertyChanged}\" Width=\"60\" Margin=\"6,0,0,0\"/>");
            sb.AppendLine("                                                        <Button Content=\"Apply\" Width=\"60\" Margin=\"6,0,0,0\" Click=\"ApplyStepArrayCount_Click\"/>");
            sb.AppendLine("                                                        <TextBlock Text=\"{Binding ArraySummary}\" VerticalAlignment=\"Center\" Margin=\"10,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                                                    </StackPanel>");
            sb.AppendLine("                                                    <DataGrid ItemsSource=\"{Binding Cells}\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" MaxHeight=\"160\">");
            sb.AppendLine("                                                        <DataGrid.Columns>");
            sb.AppendLine("                                                            <DataGridTextColumn Header=\"Idx\" Binding=\"{Binding Index}\" IsReadOnly=\"True\" Width=\"46\"/>");
            sb.AppendLine("                                                            <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" IsReadOnly=\"True\" Width=\"60\"/>");
            sb.AppendLine("                                                            <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" IsReadOnly=\"True\" Width=\"2*\"/>");
            sb.AppendLine("                                                            <DataGridTextColumn Header=\"Type\" Binding=\"{Binding Type}\" IsReadOnly=\"True\" Width=\"70\"/>");
            sb.AppendLine("                                                            <!-- Always-active TextBox: a single click puts the caret in the cell so the");
            sb.AppendLine("                                                                 value can be typed directly (DataGrid edit-mode inside RowDetails is unreliable). -->");
            sb.AppendLine("                                                            <DataGridTemplateColumn Header=\"Value\" Width=\"*\" IsReadOnly=\"True\">");
            sb.AppendLine("                                                                <DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                                                    <DataTemplate>");
            sb.AppendLine("                                                                        <TextBox Text=\"{Binding Value, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}\" BorderThickness=\"0\"");
            sb.AppendLine("                                                                                 Background=\"Transparent\" Foreground=\"#E6EDF3\" Padding=\"2,0\" ToolTip=\"Type the cell value\"/>");
            sb.AppendLine("                                                                    </DataTemplate>");
            sb.AppendLine("                                                                </DataGridTemplateColumn.CellTemplate>");
            sb.AppendLine("                                                            </DataGridTemplateColumn>");
            sb.AppendLine("                                                        </DataGrid.Columns>");
            sb.AppendLine("                                                    </DataGrid>");
            sb.AppendLine("                                                </StackPanel>");
            sb.AppendLine("                                            </Border>");
            sb.AppendLine("                                        </DataTemplate>");
            sb.AppendLine("                                    </DataGrid.RowDetailsTemplate>");
            sb.AppendLine("                                </DataGrid>");
            sb.AppendLine("                            </DockPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine();
            sb.AppendLine("                    <GroupBox Grid.Row=\"2\" Header=\"Scenario Log\">");
            sb.AppendLine("                        <DockPanel>");
            sb.AppendLine("                            <Button DockPanel.Dock=\"Bottom\" Content=\"Clear Log\" Background=\"#8B2E1F\" Click=\"ClearScenarioLog_Click\"/>");
            sb.AppendLine("                            <TextBox x:Name=\"ScenarioLogBox\" IsReadOnly=\"True\" FontFamily=\"Consolas\" FontSize=\"11\"");
            sb.AppendLine("                                     VerticalScrollBarVisibility=\"Auto\" TextWrapping=\"Wrap\"/>");
            sb.AppendLine("                        </DockPanel>");
            sb.AppendLine("                    </GroupBox>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
        }

        /// <summary>
        /// The Record tab: opens a binary recording, lists the message types on the left and
        /// shows every recorded message of the selected type in the middle table.
        /// </summary>
        private static void AppendRecordTabXaml(StringBuilder sb)
        {
            sb.AppendLine("            <TabItem Header=\"Record\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine();
            // Toolbar: open file + status of loaded recording.
            sb.AppendLine("                    <DockPanel Grid.Row=\"0\" Margin=\"6,4\">");
            sb.AppendLine("                        <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                            <Button Content=\"Open Recording (BIN)\" Click=\"OpenRecording_Click\"/>");
            sb.AppendLine("                            <Button Content=\"\u2193 Export Type (CSV)\" Background=\"#1F8FA8\" Margin=\"6,3,0,3\" Click=\"ExportRecordType_Click\"");
            sb.AppendLine("                                    ToolTip=\"Export all recorded messages of the selected type: one column per field, one row per message\"/>");
            sb.AppendLine("                            <TextBlock Text=\"File:\" VerticalAlignment=\"Center\" Margin=\"14,0,4,0\" Foreground=\"#8FB4D9\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"RecordFileText\" Text=\"(no file loaded)\" VerticalAlignment=\"Center\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"RecordSummaryText\" Text=\"\" VerticalAlignment=\"Center\" Margin=\"14,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                        </StackPanel>");
            sb.AppendLine("                    </DockPanel>");
            sb.AppendLine();
            // Left: message types found in the file. Middle: all records of the selected type.
            sb.AppendLine("                    <Grid Grid.Row=\"1\">");
            sb.AppendLine("                        <Grid.ColumnDefinitions>");
            sb.AppendLine("                            <ColumnDefinition Width=\"280\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"340\"/>");
            sb.AppendLine("                        </Grid.ColumnDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"0\" Header=\"Message Types\">");
            sb.AppendLine("                            <DockPanel>");
            sb.AppendLine("                                <TextBlock DockPanel.Dock=\"Top\" Text=\"Click a type to show its recorded messages\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                                <ListBox x:Name=\"RecordTypesList\" Background=\"#0B1220\" Foreground=\"#E6EDF3\" BorderBrush=\"#22303C\"");
            sb.AppendLine("                                         SelectionChanged=\"RecordTypesList_SelectionChanged\"/>");
            sb.AppendLine("                            </DockPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"1\" Header=\"Recorded Messages\">");
            sb.AppendLine("                            <DataGrid x:Name=\"RecordedMessagesGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\"");
            sb.AppendLine("                                      SelectionChanged=\"RecordedMessagesGrid_SelectionChanged\">");
            sb.AppendLine("                                <DataGrid.Columns>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"#\" Binding=\"{Binding Ordinal}\" Width=\"50\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Time\" Binding=\"{Binding Time}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Direction\" Binding=\"{Binding Direction}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Msg Id\" Binding=\"{Binding MessageId}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Bytes\" Binding=\"{Binding Bytes}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Data (HEX)\" Binding=\"{Binding Hex}\" Width=\"*\">");
            sb.AppendLine("                                        <DataGridTextColumn.ElementStyle>");
            sb.AppendLine("                                            <Style TargetType=\"TextBlock\">");
            sb.AppendLine("                                                <Setter Property=\"FontFamily\" Value=\"Consolas\"/>");
            sb.AppendLine("                                                <Setter Property=\"TextWrapping\" Value=\"Wrap\"/>");
            sb.AppendLine("                                            </Style>");
            sb.AppendLine("                                        </DataGridTextColumn.ElementStyle>");
            sb.AppendLine("                                    </DataGridTextColumn>");
            sb.AppendLine("                                </DataGrid.Columns>");
            sb.AppendLine("                            </DataGrid>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"2\">");
            sb.AppendLine("                            <GroupBox.Header>");
            sb.AppendLine("                                <TextBlock x:Name=\"RecordFieldsHeader\" Text=\"Message Fields (select a row)\" Foreground=\"#8FB4D9\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                            </GroupBox.Header>");
            sb.AppendLine("                            <DockPanel>");
            sb.AppendLine("                                <TextBlock DockPanel.Dock=\"Top\" Text=\"Click a recorded message to decode all its fields\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,6\"/>");
            sb.AppendLine("                                <DataGrid x:Name=\"RecordFieldsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\">");
            sb.AppendLine("                                    <DataGrid.Columns>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" Width=\"60\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" Width=\"2*\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value}\" Width=\"*\"/>");
            sb.AppendLine("                                    </DataGrid.Columns>");
            sb.AppendLine("                                </DataGrid>");
            sb.AppendLine("                            </DockPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
        }

        /// <summary>
        /// The Graph tab: a live message timeline with a TX lane (MBT ? NIRON) and an RX lane
        /// (NIRON ? MBT), message filters, zoom / direction view options and a details table.
        /// </summary>
        private static void AppendGraphTabXaml(StringBuilder sb)
        {
            sb.AppendLine("            <TabItem Header=\"Graph\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.ColumnDefinitions>");
            sb.AppendLine("                        <ColumnDefinition Width=\"250\"/>");
            sb.AppendLine("                        <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                    </Grid.ColumnDefinitions>");
            sb.AppendLine();
            // Left: message filters + view options.
            sb.AppendLine("                    <Grid Grid.Column=\"0\">");
            sb.AppendLine("                        <Grid.RowDefinitions>");
            sb.AppendLine("                            <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                            <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        </Grid.RowDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Row=\"0\" Header=\"Messages\">");
            sb.AppendLine("                            <ScrollViewer VerticalScrollBarVisibility=\"Auto\">");
            sb.AppendLine("                                <StackPanel x:Name=\"GraphFilterPanel\"/>");
            sb.AppendLine("                            </ScrollViewer>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Row=\"1\" Header=\"View Options\">");
            sb.AppendLine("                            <StackPanel>");
            sb.AppendLine("                                <TextBlock Text=\"Zoom\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,2\"/>");
            sb.AppendLine("                                <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                                    <Button Content=\"\u2212\" Width=\"32\" Click=\"GraphZoomOut_Click\"/>");
            sb.AppendLine("                                    <TextBlock x:Name=\"GraphZoomText\" Text=\"100%\" Width=\"56\" TextAlignment=\"Center\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                    <Button Content=\"+\" Width=\"32\" Click=\"GraphZoomIn_Click\"/>");
            sb.AppendLine("                                    <Button Content=\"Fit\" Width=\"44\" Margin=\"6,3,0,3\" Click=\"GraphFit_Click\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                                <Button x:Name=\"GraphPauseButton\" Content=\"\u23F8 Pause\" Background=\"#B08800\" Margin=\"0,8,0,0\" Click=\"GraphPause_Click\"");
            sb.AppendLine("                                        ToolTip=\"Freeze the timeline display; events keep accumulating\"/>");
            sb.AppendLine("                                <TextBlock Text=\"Direction Filter\" Foreground=\"#7A8CA0\" Margin=\"2,8,0,2\"/>");
            sb.AppendLine("                                <ComboBox x:Name=\"GraphDirectionCombo\" SelectedIndex=\"0\" SelectionChanged=\"GraphOptions_Changed\">");
            sb.AppendLine("                                    <ComboBoxItem Content=\"All\"/>");
            sb.AppendLine("                                    <ComboBoxItem Content=\"MBT \u2192 NIRON\"/>");
            sb.AppendLine("                                    <ComboBoxItem Content=\"NIRON \u2192 MBT\"/>");
            sb.AppendLine("                                </ComboBox>");
            sb.AppendLine("                                <Button Content=\"Clear Timeline\" Background=\"#8B2E1F\" Margin=\"0,8,0,0\" Click=\"ClearTimeline_Click\"/>");
            sb.AppendLine("                            </StackPanel>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine();
            // Right: timeline canvas + details table.
            sb.AppendLine("                    <Grid Grid.Column=\"1\">");
            sb.AppendLine("                        <Grid.RowDefinitions>");
            sb.AppendLine("                            <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                            <RowDefinition Height=\"220\"/>");
            sb.AppendLine("                        </Grid.RowDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Row=\"0\">");
            sb.AppendLine("                            <GroupBox.Header>");
            sb.AppendLine("                                <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                                    <TextBlock Text=\"Message Timeline\" FontWeight=\"Bold\" Foreground=\"#8FB4D9\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"\u2192 MBT \u2192 NIRON (my side)\" Foreground=\"#238636\" Margin=\"20,0,0,0\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"\u2192 NIRON \u2192 MBT\" Foreground=\"#FFFFFF\" Margin=\"14,0,0,0\"/>");
            sb.AppendLine("                                    <Button Content=\"\u2212\" Width=\"26\" Padding=\"0\" Margin=\"20,0,0,0\" Click=\"GraphZoomOut_Click\" ToolTip=\"Zoom out\"/>");
            sb.AppendLine("                                    <Button Content=\"+\" Width=\"26\" Padding=\"0\" Margin=\"4,0,0,0\" Click=\"GraphZoomIn_Click\" ToolTip=\"Zoom in\"/>");
            sb.AppendLine("                                    <TextBlock Text=\"Ctrl+Wheel zooms\" Foreground=\"#7A8CA0\" Margin=\"8,0,0,0\"/>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                            </GroupBox.Header>");
            sb.AppendLine("                            <ScrollViewer x:Name=\"GraphScroll\" HorizontalScrollBarVisibility=\"Auto\" VerticalScrollBarVisibility=\"Disabled\"");
            sb.AppendLine("                                          PreviewMouseWheel=\"GraphScroll_PreviewMouseWheel\">");
            sb.AppendLine("                                <Canvas x:Name=\"TimelineCanvas\" Background=\"#0B1220\" Height=\"380\" MinWidth=\"900\"/>");
            sb.AppendLine("                            </ScrollViewer>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <Grid Grid.Row=\"1\">");
            sb.AppendLine("                            <Grid.ColumnDefinitions>");
            sb.AppendLine("                                <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                                <ColumnDefinition Width=\"340\"/>");
            sb.AppendLine("                            </Grid.ColumnDefinitions>");
            sb.AppendLine("                            <GroupBox Grid.Column=\"0\" Header=\"Details\">");
            sb.AppendLine("                                <DataGrid x:Name=\"TimelineDetailsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\"");
            sb.AppendLine("                                          SelectionChanged=\"TimelineDetailsGrid_SelectionChanged\">");
            sb.AppendLine("                                    <DataGrid.Columns>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Time\" Binding=\"{Binding Time}\" Width=\"Auto\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Direction\" Binding=\"{Binding Direction}\" Width=\"Auto\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" Width=\"*\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Status\" Binding=\"{Binding Status}\" Width=\"Auto\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Details\" Binding=\"{Binding Details}\" Width=\"*\"/>");
            sb.AppendLine("                                    </DataGrid.Columns>");
            sb.AppendLine("                                </DataGrid>");
            sb.AppendLine("                            </GroupBox>");
            sb.AppendLine("                            <GroupBox Grid.Column=\"1\">");
            sb.AppendLine("                                <GroupBox.Header>");
            sb.AppendLine("                                    <TextBlock x:Name=\"GraphFieldsHeader\" Text=\"Message Fields (click a point or row)\" Foreground=\"#8FB4D9\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                                </GroupBox.Header>");
            sb.AppendLine("                                <DataGrid x:Name=\"GraphFieldsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\">");
            sb.AppendLine("                                    <DataGrid.Columns>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" Width=\"60\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" Width=\"2*\"/>");
            sb.AppendLine("                                        <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value}\" Width=\"*\"/>");
            sb.AppendLine("                                    </DataGrid.Columns>");
            sb.AppendLine("                                </DataGrid>");
            sb.AppendLine("                            </GroupBox>");
            sb.AppendLine("                        </Grid>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
        }

        /// <summary>
        /// The Record Graph tab: opens a recording .bin file and displays it on the same
        /// two-lane timeline as the Graph tab (filters, zoom, details and field decoding).
        /// </summary>
        private static void AppendRecordGraphTabXaml(StringBuilder sb)
        {
            sb.AppendLine("            <TabItem Header=\"Record Graph\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine();
            // Toolbar: open recording + loaded file info.
            sb.AppendLine("                    <DockPanel Grid.Row=\"0\" Margin=\"6,4\">");
            sb.AppendLine("                        <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                            <Button Content=\"Open Recording (BIN)\" Click=\"OpenRecordGraph_Click\"/>");
            sb.AppendLine("                            <TextBlock Text=\"File:\" VerticalAlignment=\"Center\" Margin=\"14,0,4,0\" Foreground=\"#8FB4D9\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"RecordGraphFileText\" Text=\"(no file loaded)\" VerticalAlignment=\"Center\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"RecordGraphSummaryText\" Text=\"\" VerticalAlignment=\"Center\" Margin=\"14,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                        </StackPanel>");
            sb.AppendLine("                    </DockPanel>");
            sb.AppendLine();
            sb.AppendLine("                    <Grid Grid.Row=\"1\">");
            sb.AppendLine("                        <Grid.ColumnDefinitions>");
            sb.AppendLine("                            <ColumnDefinition Width=\"250\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                        </Grid.ColumnDefinitions>");
            sb.AppendLine();
            // Left: message filters + view options.
            sb.AppendLine("                        <Grid Grid.Column=\"0\">");
            sb.AppendLine("                            <Grid.RowDefinitions>");
            sb.AppendLine("                                <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                                <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                            </Grid.RowDefinitions>");
            sb.AppendLine("                            <GroupBox Grid.Row=\"0\" Header=\"Messages\">");
            sb.AppendLine("                                <ScrollViewer VerticalScrollBarVisibility=\"Auto\">");
            sb.AppendLine("                                    <StackPanel x:Name=\"RecordGraphFilterPanel\"/>");
            sb.AppendLine("                                </ScrollViewer>");
            sb.AppendLine("                            </GroupBox>");
            sb.AppendLine("                            <GroupBox Grid.Row=\"1\" Header=\"View Options\">");
            sb.AppendLine("                                <StackPanel>");
            sb.AppendLine("                                    <TextBlock Text=\"Zoom\" Foreground=\"#7A8CA0\" Margin=\"2,0,0,2\"/>");
            sb.AppendLine("                                    <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                                        <Button Content=\"\u2212\" Width=\"32\" Click=\"RecordGraphZoomOut_Click\"/>");
            sb.AppendLine("                                        <TextBlock x:Name=\"RecordGraphZoomText\" Text=\"100%\" Width=\"56\" TextAlignment=\"Center\" VerticalAlignment=\"Center\"/>");
            sb.AppendLine("                                        <Button Content=\"+\" Width=\"32\" Click=\"RecordGraphZoomIn_Click\"/>");
            sb.AppendLine("                                        <Button Content=\"Fit\" Width=\"44\" Margin=\"6,3,0,3\" Click=\"RecordGraphFit_Click\"/>");
            sb.AppendLine("                                    </StackPanel>");
            sb.AppendLine("                                    <TextBlock Text=\"Direction Filter\" Foreground=\"#7A8CA0\" Margin=\"2,8,0,2\"/>");
            sb.AppendLine("                                    <ComboBox x:Name=\"RecordGraphDirectionCombo\" SelectedIndex=\"0\" SelectionChanged=\"RecordGraphOptions_Changed\">");
            sb.AppendLine("                                        <ComboBoxItem Content=\"All\"/>");
            sb.AppendLine("                                        <ComboBoxItem Content=\"MBT \u2192 NIRON\"/>");
            sb.AppendLine("                                        <ComboBoxItem Content=\"NIRON \u2192 MBT\"/>");
            sb.AppendLine("                                    </ComboBox>");
            sb.AppendLine("                                </StackPanel>");
            sb.AppendLine("                            </GroupBox>");
            sb.AppendLine("                        </Grid>");
            sb.AppendLine();
            // Right: timeline canvas + details table + fields panel.
            sb.AppendLine("                        <Grid Grid.Column=\"1\">");
            sb.AppendLine("                            <Grid.RowDefinitions>");
            sb.AppendLine("                                <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                                <RowDefinition Height=\"220\"/>");
            sb.AppendLine("                            </Grid.RowDefinitions>");
            sb.AppendLine("                            <GroupBox Grid.Row=\"0\">");
            sb.AppendLine("                                <GroupBox.Header>");
            sb.AppendLine("                                    <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                                        <TextBlock Text=\"Recorded Message Timeline\" FontWeight=\"Bold\" Foreground=\"#8FB4D9\"/>");
            sb.AppendLine("                                        <TextBlock Text=\"\u2192 MBT \u2192 NIRON (my side)\" Foreground=\"#238636\" Margin=\"20,0,0,0\"/>");
            sb.AppendLine("                                        <TextBlock Text=\"\u2192 NIRON \u2192 MBT\" Foreground=\"#FFFFFF\" Margin=\"14,0,0,0\"/>");
            sb.AppendLine("                                        <Button Content=\"\u2212\" Width=\"26\" Padding=\"0\" Margin=\"20,0,0,0\" Click=\"RecordGraphZoomOut_Click\" ToolTip=\"Zoom out\"/>");
            sb.AppendLine("                                        <Button Content=\"+\" Width=\"26\" Padding=\"0\" Margin=\"4,0,0,0\" Click=\"RecordGraphZoomIn_Click\" ToolTip=\"Zoom in\"/>");
            sb.AppendLine("                                        <TextBlock Text=\"Ctrl+Wheel zooms\" Foreground=\"#7A8CA0\" Margin=\"8,0,0,0\"/>");
            sb.AppendLine("                                    </StackPanel>");
            sb.AppendLine("                                </GroupBox.Header>");
            sb.AppendLine("                                <ScrollViewer x:Name=\"RecordGraphScroll\" HorizontalScrollBarVisibility=\"Auto\" VerticalScrollBarVisibility=\"Disabled\"");
            sb.AppendLine("                                              PreviewMouseWheel=\"RecordGraphScroll_PreviewMouseWheel\">");
            sb.AppendLine("                                    <Canvas x:Name=\"RecordGraphCanvas\" Background=\"#0B1220\" Height=\"380\" MinWidth=\"900\"/>");
            sb.AppendLine("                                </ScrollViewer>");
            sb.AppendLine("                            </GroupBox>");
            sb.AppendLine("                            <Grid Grid.Row=\"1\">");
            sb.AppendLine("                                <Grid.ColumnDefinitions>");
            sb.AppendLine("                                    <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                                    <ColumnDefinition Width=\"340\"/>");
            sb.AppendLine("                                </Grid.ColumnDefinitions>");
            sb.AppendLine("                                <GroupBox Grid.Column=\"0\" Header=\"Details\">");
            sb.AppendLine("                                    <DataGrid x:Name=\"RecordGraphDetailsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\"");
            sb.AppendLine("                                              SelectionChanged=\"RecordGraphDetailsGrid_SelectionChanged\">");
            sb.AppendLine("                                        <DataGrid.Columns>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Time\" Binding=\"{Binding Time}\" Width=\"Auto\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Direction\" Binding=\"{Binding Direction}\" Width=\"Auto\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Message\" Binding=\"{Binding Message}\" Width=\"*\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Status\" Binding=\"{Binding Status}\" Width=\"Auto\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Details\" Binding=\"{Binding Details}\" Width=\"*\"/>");
            sb.AppendLine("                                        </DataGrid.Columns>");
            sb.AppendLine("                                    </DataGrid>");
            sb.AppendLine("                                </GroupBox>");
            sb.AppendLine("                                <GroupBox Grid.Column=\"1\">");
            sb.AppendLine("                                    <GroupBox.Header>");
            sb.AppendLine("                                        <TextBlock x:Name=\"RecordGraphFieldsHeader\" Text=\"Message Fields (click a point or row)\" Foreground=\"#8FB4D9\" FontWeight=\"Bold\"/>");
            sb.AppendLine("                                    </GroupBox.Header>");
            sb.AppendLine("                                    <DataGrid x:Name=\"RecordGraphFieldsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\">");
            sb.AppendLine("                                        <DataGrid.Columns>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Offset\" Binding=\"{Binding Offset}\" Width=\"60\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Field\" Binding=\"{Binding Field}\" Width=\"2*\"/>");
            sb.AppendLine("                                            <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value}\" Width=\"*\"/>");
            sb.AppendLine("                                        </DataGrid.Columns>");
            sb.AppendLine("                                    </DataGrid>");
            sb.AppendLine("                                </GroupBox>");
            sb.AppendLine("                            </Grid>");
            sb.AppendLine("                        </Grid>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
        }

        /// <summary>
        /// The Statistics tab: one row per message with totals, current rates (messages/sec,
        /// bytes/sec), sizes, last activity timestamps and error counters, refreshed every second.
        /// </summary>
        private static void AppendStatisticsTabXaml(StringBuilder sb)
        {
            sb.AppendLine("            <TabItem Header=\"Statistics\">");
            sb.AppendLine("                <Grid>");
            sb.AppendLine("                    <Grid.RowDefinitions>");
            sb.AppendLine("                        <RowDefinition Height=\"Auto\"/>");
            sb.AppendLine("                        <RowDefinition Height=\"*\"/>");
            sb.AppendLine("                    </Grid.RowDefinitions>");
            sb.AppendLine();
            sb.AppendLine("                    <DockPanel Grid.Row=\"0\" Margin=\"6,4\">");
            sb.AppendLine("                        <StackPanel Orientation=\"Horizontal\">");
            sb.AppendLine("                            <Button Content=\"Reset Statistics\" Background=\"#8B2E1F\" Click=\"ResetStats_Click\"/>");
            sb.AppendLine("                            <TextBlock x:Name=\"StatsSummaryText\" Text=\"(no traffic yet)\" VerticalAlignment=\"Center\" Margin=\"14,0,0,0\" Foreground=\"#7A8CA0\"/>");
            sb.AppendLine("                        </StackPanel>");
            sb.AppendLine("                    </DockPanel>");
            sb.AppendLine();
            sb.AppendLine("                    <Grid Grid.Row=\"1\">");
            sb.AppendLine("                        <Grid.ColumnDefinitions>");
            sb.AppendLine("                            <ColumnDefinition Width=\"420\"/>");
            sb.AppendLine("                            <ColumnDefinition Width=\"*\"/>");
            sb.AppendLine("                        </Grid.ColumnDefinitions>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"0\" Header=\"Global Statistics\">");
            sb.AppendLine("                            <DataGrid x:Name=\"GlobalStatsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\">");
            sb.AppendLine("                                <DataGrid.Columns>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Statistic\" Binding=\"{Binding Statistic}\" Width=\"3*\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Value\" Binding=\"{Binding Value}\" Width=\"2*\"/>");
            sb.AppendLine("                                </DataGrid.Columns>");
            sb.AppendLine("                            </DataGrid>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                        <GroupBox Grid.Column=\"1\" Header=\"Per-Message Statistics (refreshed every second)\">");
            sb.AppendLine("                            <DataGrid x:Name=\"StatsGrid\" AutoGenerateColumns=\"False\" CanUserAddRows=\"False\" IsReadOnly=\"True\">");
            sb.AppendLine("                                <DataGrid.Columns>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Message Name\" Binding=\"{Binding Message}\" Width=\"2*\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Received\" Binding=\"{Binding Received}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Sent\" Binding=\"{Binding Sent}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Msg/sec\" Binding=\"{Binding MsgPerSec}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Bytes/sec\" Binding=\"{Binding BytesPerSec}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Total Bytes\" Binding=\"{Binding TotalBytes}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Avg Size\" Binding=\"{Binding AvgSize}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Min Size\" Binding=\"{Binding MinSize}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Max Size\" Binding=\"{Binding MaxSize}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Last RX Time\" Binding=\"{Binding LastReceived}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Last TX Time\" Binding=\"{Binding LastSent}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Since Last (s)\" Binding=\"{Binding SinceLast}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Errors\" Binding=\"{Binding Errors}\" Width=\"Auto\"/>");
            sb.AppendLine("                                    <DataGridTextColumn Header=\"Dropped\" Binding=\"{Binding Dropped}\" Width=\"Auto\"/>");
            sb.AppendLine("                                </DataGrid.Columns>");
            sb.AppendLine("                            </DataGrid>");
            sb.AppendLine("                        </GroupBox>");
            sb.AppendLine("                    </Grid>");
            sb.AppendLine("                </Grid>");
            sb.AppendLine("            </TabItem>");
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
            sb.AppendLine("using System.Windows.Input;");
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
            sb.AppendLine("        // Periodic sends run on PrecisionTimer threads (1 ms resolution, drift-free),");
            sb.AppendLine("        // never on the UI thread, so a 10 ms cadence stays accurate while the UI works.");
            sb.AppendLine("        private readonly Dictionary<string, PrecisionTimer> _periodicTimers = new();");
            sb.AppendLine("        private readonly ObservableCollection<ScenarioStepRow> _scenarioSteps = new();");
            sb.AppendLine("        private readonly List<DispatcherTimer> _scenarioTimers = new();");
            sb.AppendLine("        private readonly List<PrecisionTimer> _scenarioPrecisionTimers = new();");
            sb.AppendLine("        // Armed Response steps: checked against every received message while the scenario runs.");
            sb.AppendLine("        private readonly List<ResponseRule> _activeResponses = new();");
            sb.AppendLine("        private bool _loadingResponseUi;");
            sb.AppendLine("        // The native wrapper writes into static physical structures; serialize access across threads.");
            sb.AppendLine("        private static readonly object _nativeLock = new();");
            sb.AppendLine("        private readonly ObservableCollection<FieldRow> _stepFields = new();");
            sb.AppendLine("        private ScenarioStepRow? _editingStep;");
            sb.AppendLine("        private string? _scenarioFolder;");
            sb.AppendLine("        private volatile bool _scenarioRunning;");
            sb.AppendLine("        private Point _dragStartPoint;");
            sb.AppendLine("        private readonly MessageRecorder _recorder = new();");
            sb.AppendLine("        private List<RecordedMessage> _loadedRecords = new();");
            sb.AppendLine("        private readonly List<TimelineEvent> _timelineEvents = new();");
            sb.AppendLine("        private readonly Dictionary<string, int> _messageCounts = new();");
            sb.AppendLine("        private double _graphZoom = 1.0;");
            sb.AppendLine("        private bool _graphPaused;");
            sb.AppendLine("        // Set when traffic arrives; the graph is redrawn by _graphRefreshTimer instead of on every packet.");
            sb.AppendLine("        private bool _timelineDirty;");
            sb.AppendLine("        // Coalesces bursts of traffic into a few UI repaints per second to keep the UI responsive.");
            sb.AppendLine("        private readonly DispatcherTimer _graphRefreshTimer = new() { Interval = TimeSpan.FromMilliseconds(300) };");
            sb.AppendLine("        private readonly Dictionary<string, MessageStat> _messageStats = new();");
            sb.AppendLine("        private DateTime _lastStatsTick = DateTime.Now;");
            sb.AppendLine("        // Statistics refresh is intentionally separate from message receive/send cadence.");
            sb.AppendLine("        private readonly DispatcherTimer _statsTimer = new() { Interval = TimeSpan.FromSeconds(1) };");
            sb.AppendLine("        private readonly DateTime _simStartTime = DateTime.Now;");
            sb.AppendLine("        private double _peakMsgRate;");
            sb.AppendLine("        private double _peakByteRate;");
            sb.AppendLine("        private long _totalProcessingTicks;");
            sb.AppendLine("        private long _processedMessages;");
            sb.AppendLine("        private long _timeoutCount;");
            sb.AppendLine("        private long _retransmissionCount;");
            sb.AppendLine("        private TimeSpan _lastCpuTime = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;");
            sb.AppendLine("        private readonly List<TimelineEvent> _recordGraphEvents = new();");
            sb.AppendLine("        private double _recordGraphZoom = 1.0;");
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
            sb.AppendLine("            ScenarioStepsList.ItemsSource = _scenarioSteps;");
            sb.AppendLine("            StepFieldsGrid.ItemsSource = _stepFields;");
            sb.AppendLine("            foreach (var m in MessageCatalog.Messages)");
            sb.AppendLine("            {");
            sb.AppendLine("                MessageList.Items.Add(m.Name);");
            sb.AppendLine("                AllMessagesList.Items.Add(m.Name);");
            sb.AppendLine("                AddGraphFilter(m.Name);");
            sb.AppendLine("                ResponseTriggerCombo.Items.Add(m.Name);");
            sb.AppendLine("                ResponseSendCombo.Items.Add(m.Name);");
            sb.AppendLine("            }");
            sb.AppendLine("            if (MessageList.Items.Count > 0)");
            sb.AppendLine("                MessageList.SelectedIndex = 0;");
            sb.AppendLine("            foreach (var port in System.IO.Ports.SerialPort.GetPortNames())");
            sb.AppendLine("                ComPortBox.Items.Add(port);");
            sb.AppendLine("            if (ComPortBox.Items.Count > 0) ComPortBox.SelectedIndex = 0;");
            sb.AppendLine("            // Repaint the timeline when the Graph tab becomes visible.");
            sb.AppendLine("            TimelineCanvas.IsVisibleChanged += (_, _) => { if (TimelineCanvas.IsVisible) RedrawTimeline(); };");
            sb.AppendLine("            RecordGraphCanvas.IsVisibleChanged += (_, _) => { if (RecordGraphCanvas.IsVisible) RedrawRecordTimeline(); };");
            sb.AppendLine("            // Coalesced repaint: heavy traffic only marks the timeline dirty and this timer");
            sb.AppendLine("            // repaints at most ~3 times per second, so bursts of messages cannot freeze the UI.");
            sb.AppendLine("            _graphRefreshTimer.Tick += (_, _) =>");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_timelineDirty && TimelineCanvas.IsVisible && !_graphPaused) RedrawTimeline();");
            sb.AppendLine("            };");
            sb.AppendLine("            _graphRefreshTimer.Start();");
            sb.AppendLine("            // Per-message statistics (rates recomputed every second).");
            sb.AppendLine("            _statsTimer.Tick += (_, _) => UpdateStatistics();");
            sb.AppendLine("            _statsTimer.Start();");
            sb.AppendLine("            // 1 ms Windows timer resolution so 10 ms periodic sends fire on time.");
            sb.AppendLine("            HighResClock.Enable();");
            sb.AppendLine("            Closed += (_, _) =>");
            sb.AppendLine("            {");
            sb.AppendLine("                foreach (var timer in _periodicTimers.Values) timer.Dispose();");
            sb.AppendLine("                _periodicTimers.Clear();");
            sb.AppendLine("                StopScenarioTimers();");
            sb.AppendLine("                StopTransport();");
            sb.AppendLine("                HighResClock.Disable();");
            sb.AppendLine("            };");
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
            sb.AppendLine("            // Merge the scalar fields and the array rows ordered by offset, so each array");
            sb.AppendLine("            // appears as a nested (collapsible) table at its correct offset position.");
            sb.AppendLine("            var arrays = new Queue<FieldRow>(info.Arrays.OrderBy(a => a.BaseOffset).Select(a => new FieldRow(a, Math.Min(1, a.MaxCount))));");
            sb.AppendLine("            foreach (var f in info.Fields)");
            sb.AppendLine("            {");
            sb.AppendLine("                while (arrays.Count > 0 && arrays.Peek().Offset <= f.Offset)");
            sb.AppendLine("                    _fields.Add(arrays.Dequeue());");
            sb.AppendLine("                _fields.Add(new FieldRow(f) { Value = f.DefaultValue });");
            sb.AppendLine("            }");
            sb.AppendLine("            while (arrays.Count > 0)");
            sb.AppendLine("                _fields.Add(arrays.Dequeue());");
            sb.AppendLine("            RefreshHex();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Arrays: nested per-cell editing inside the fields grid ----");
            sb.AppendLine("        /// <summary>Clicking anywhere on an array row opens / closes its nested cells table.");
            sb.AppendLine("        /// Attached as PreviewMouseLeftButtonUp on the whole grid so it always fires, and the");
            sb.AppendLine("        /// row container's DetailsVisibility is set directly (a local value the DataGrid must");
            sb.AppendLine("        /// honor; a RowStyle trigger can be overridden by the grid's own coercion).");
            sb.AppendLine("        /// Clicks inside the expanded details are ignored so editing never collapses it.</summary>");
            sb.AppendLine("        private void ArrayRow_ToggleOnClick(object sender, MouseButtonEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            var element = e.OriginalSource as DependencyObject;");
            sb.AppendLine("            while (element is not null && element is not System.Windows.Controls.DataGridRow)");
            sb.AppendLine("            {");
            sb.AppendLine("                if (element is System.Windows.Controls.Primitives.DataGridDetailsPresenter)");
            sb.AppendLine("                    return; // click inside the nested cells table");
            sb.AppendLine("                element = element is System.Windows.Media.Visual || element is System.Windows.Media.Media3D.Visual3D");
            sb.AppendLine("                    ? System.Windows.Media.VisualTreeHelper.GetParent(element)");
            sb.AppendLine("                    : LogicalTreeHelper.GetParent(element);");
            sb.AppendLine("            }");
            sb.AppendLine("            if (element is System.Windows.Controls.DataGridRow { DataContext: FieldRow { IsArray: true } row } container)");
            sb.AppendLine("            {");
            sb.AppendLine("                row.IsExpanded = !row.IsExpanded;");
            sb.AppendLine("                container.DetailsVisibility = row.IsExpanded ? Visibility.Visible : Visibility.Collapsed;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Restores an array row's open state whenever its container is (re)created");
            sb.AppendLine("        /// (initial load, scrolling / virtualization, ItemsSource refresh).</summary>");
            sb.AppendLine("        private void ArrayRow_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            e.Row.DetailsVisibility = e.Row.DataContext is FieldRow { IsArray: true, IsExpanded: true }");
            sb.AppendLine("                ? Visibility.Visible : Visibility.Collapsed;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Apply button inside an array row's nested table: rebuilds the cells to");
            sb.AppendLine("        /// the requested element count and syncs the array's count field.</summary>");
            sb.AppendLine("        private void ApplyArrayCount_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            if ((sender as FrameworkElement)?.DataContext is not FieldRow { ArrayDef: { } array } row) return;");
            sb.AppendLine("            var applied = row.RebuildCells(int.TryParse(row.CountText, out var n) ? n : 0);");
            sb.AppendLine("            // Reflect the element count in the array's count field so the receiver knows the length.");
            sb.AppendLine("            if (array.CountField is not null)");
            sb.AppendLine("                foreach (var f in _fields)");
            sb.AppendLine("                    if (!f.IsArray && f.Field == array.CountField) f.Value = applied.ToString();");
            sb.AppendLine("            RefreshHex();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Independent copy of all array cells (from the nested tables) for a background send.</summary>");
            sb.AppendLine("        private List<ArrayCellRow> SnapshotArrayCells()");
            sb.AppendLine("        {");
            sb.AppendLine("            var list = new List<ArrayCellRow>();");
            sb.AppendLine("            foreach (var row in _fields)");
            sb.AppendLine("                if (row.IsArray)");
            sb.AppendLine("                    foreach (var c in row.Cells)");
            sb.AppendLine("                        list.Add(new ArrayCellRow { Index = c.Index, Offset = c.Offset, Field = c.Field, Type = c.Type, Value = c.Value });");
            sb.AppendLine("            return list;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Buffer building via the native wrapper ----");
            sb.AppendLine("        private byte[] BuildBuffer(MessageInfo info, IReadOnlyList<FieldRow> fields) =>");
            sb.AppendLine("            BuildBufferCore(info, fields, SnapshotArrayCells(), _sequence, AutoSequenceCheck.IsChecked == true,");
            sb.AppendLine("                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Thread-safe buffer build (no UI access) usable from PrecisionTimer threads.");
            sb.AppendLine("        /// Native convert calls are serialized because the wrapper uses static structures.</summary>");
            sb.AppendLine("        private static byte[] BuildBufferCore(MessageInfo info, IReadOnlyList<FieldRow> fields, IReadOnlyList<ArrayCellRow>? arrayCells, int seq, bool autoSeq, bool autoTs, bool autoCrc)");
            sb.AppendLine("        {");
            sb.AppendLine("            var buffer = new byte[Math.Max(info.Length, 1)];");
            sb.AppendLine("            IntPtr phys = info.GetPhysical();");
            sb.AppendLine();
            sb.AppendLine("            if (autoSeq) SetFieldBySuffix(fields, \"SeqNum\", seq);");
            sb.AppendLine("            if (autoTs) SetFieldBySuffix(fields, \"timestamp\", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0);");
            sb.AppendLine();
            sb.AppendLine("            // Establish the base layout (arrays, fixed structure) through the native");
            sb.AppendLine("            // convert function, then overlay the edited scalar values onto the buffer.");
            sb.AppendLine("            lock (_nativeLock)");
            sb.AppendLine("            {");
            sb.AppendLine("                if (phys != IntPtr.Zero)");
            sb.AppendLine("                    info.ConvertToInterface(buffer, phys);");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            foreach (var row in fields)");
            sb.AppendLine("                row.WriteToBuffer(buffer);");
            sb.AppendLine();
            sb.AppendLine("            // Overlay the editable array cells after the scalar fields, before the CRC.");
            sb.AppendLine("            if (arrayCells is not null)");
            sb.AppendLine("                foreach (var cell in arrayCells)");
            sb.AppendLine("                    cell.WriteToBuffer(buffer);");
            sb.AppendLine();
            sb.AppendLine("            if (autoCrc)");
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
            sb.AppendLine("        /// <summary>UI-thread send used by buttons and scenario starts; snapshots the auto");
            sb.AppendLine("        /// flags then delegates to the thread-safe core.</summary>");
            sb.AppendLine("        private void SendMessage(MessageInfo info, IReadOnlyList<FieldRow> fields, bool periodic)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_transport is null) { Log(\"Transport is not started.\"); return; }");
            sb.AppendLine("            // Include the edited array cells only when sending the message shown in the editor.");
            sb.AppendLine("            var arrayCells = ReferenceEquals(info, Current) ? SnapshotArrayCells() : null;");
            sb.AppendLine("            SendMessageCore(info, fields, arrayCells, periodic, AutoSequenceCheck.IsChecked == true,");
            sb.AppendLine("                AutoTimestampCheck.IsChecked == true, AutoCrcCheck.IsChecked == true);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Builds and sends on the calling thread (safe for PrecisionTimer threads).");
            sb.AppendLine("        /// UI bookkeeping is queued with InvokeAsync so it never delays the 10 ms cadence.</summary>");
            sb.AppendLine("        private void SendMessageCore(MessageInfo info, IReadOnlyList<FieldRow> fields, IReadOnlyList<ArrayCellRow>? arrayCells, bool periodic, bool autoSeq, bool autoTs, bool autoCrc)");
            sb.AppendLine("        {");
            sb.AppendLine("            var transport = _transport;");
            sb.AppendLine("            if (transport is null) return;");
            sb.AppendLine("            var sw = System.Diagnostics.Stopwatch.StartNew();");
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                var seq = Interlocked.Increment(ref _sequence) - 1;");
            sb.AppendLine("                var buffer = BuildBufferCore(info, fields, arrayCells, seq, autoSeq, autoTs, autoCrc);");
            sb.AppendLine("                transport.Send(buffer);");
            sb.AppendLine("                _recorder.Record(true, info.MessageId, buffer);");
            sb.AppendLine("                sw.Stop();");
            sb.AppendLine("                Interlocked.Add(ref _totalProcessingTicks, sw.ElapsedTicks);");
            sb.AppendLine("                Interlocked.Increment(ref _processedMessages);");
            sb.AppendLine("                var time = DateTime.Now;");
            sb.AppendLine("                Dispatcher.InvokeAsync(() =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    RecordTimelineEvent(true, info.Name, \"SENT\", periodic ? \"Periodic\" : \"One time\", buffer);");
            sb.AppendLine("                    UpdateStat(info.Name, buffer.Length, outgoing: true);");
            sb.AppendLine("                    if (periodic) _retransmissionCount++;");
            sb.AppendLine("                    _history.Insert(0, new HistoryRow");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Time = time.ToString(\"HH:mm:ss.fff\"),");
            sb.AppendLine("                        Direction = \"TX\", Message = info.Name, Seq = seq,");
            sb.AppendLine("                        Bytes = buffer.Length, Periodic = periodic ? \"Yes\" : \"No\"");
            sb.AppendLine("                    });");
            sb.AppendLine("                    if (_history.Count > 500) _history.RemoveAt(_history.Count - 1);");
            sb.AppendLine("                    // Per-send log and HEX refresh are skipped for periodic sends so fast rates stay smooth.");
            sb.AppendLine("                    if (!periodic)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Log($\"{info.Name}: sent {buffer.Length} bytes (seq {seq}).\");");
            sb.AppendLine("                        if (ReferenceEquals(info, Current)) RefreshHex();");
            sb.AppendLine("                    }");
            sb.AppendLine("                });");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex) { Dispatcher.InvokeAsync(() => Log(\"Send error: \" + ex.Message)); }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void ClearHistory_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            _history.Clear();");
            sb.AppendLine("            Log(\"Send history cleared.\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        // ---- Periodic (PrecisionTimer threads: accurate down to a few ms, e.g. 10 ms) ----");
            sb.AppendLine("        private void StartPeriodic_Click(object sender, RoutedEventArgs e)");
            sb.AppendLine("        {");
            sb.AppendLine("            var info = Current;");
            sb.AppendLine("            if (info is null) return;");
            sb.AppendLine("            StopPeriodic(info.Name);");
            sb.AppendLine("            var interval = double.TryParse(IntervalBox.Text, out var ms) ? Math.Max(1, ms) : 1000;");
            sb.AppendLine("            var howMany = int.TryParse(HowManyBox.Text, out var n) ? n : 0;");
            sb.AppendLine();
            sb.AppendLine("            // Snapshot the fields and auto flags so the timer thread never touches the UI;");
            sb.AppendLine("            // the message keeps sending independently of the current selection.");
            sb.AppendLine("            var snapshot = SnapshotFields(info);");
            sb.AppendLine("            var arraySnapshot = SnapshotArrayCells();");
            sb.AppendLine("            bool autoSeq = AutoSequenceCheck.IsChecked == true;");
            sb.AppendLine("            bool autoTs = AutoTimestampCheck.IsChecked == true;");
            sb.AppendLine("            bool autoCrc = AutoCrcCheck.IsChecked == true;");
            sb.AppendLine("            var sent = 0;");
            sb.AppendLine("            var timer = new PrecisionTimer(interval, () =>");
            sb.AppendLine("            {");
            sb.AppendLine("                var i = Interlocked.Increment(ref sent);");
            sb.AppendLine("                if (howMany > 0 && i > howMany) return; // count reached, disposal is on its way");
            sb.AppendLine("                SendMessageCore(info, snapshot, arraySnapshot, true, autoSeq, autoTs, autoCrc);");
            sb.AppendLine("                if (howMany > 0 && i == howMany)");
            sb.AppendLine("                    Dispatcher.InvokeAsync(() => StopPeriodic(info.Name));");
            sb.AppendLine("            });");
            sb.AppendLine("            _periodicTimers[info.Name] = timer;");
            sb.AppendLine("            Log($\"{info.Name}: precision periodic started ({interval} ms, {(howMany == 0 ? \"unlimited\" : howMany.ToString())}).\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Creates an independent copy of the message's current field values");
            sb.AppendLine("        /// (keyed by field path because the grid also contains nested array rows).</summary>");
            sb.AppendLine("        private List<FieldRow> SnapshotFields(MessageInfo info)");
            sb.AppendLine("        {");
            sb.AppendLine("            var current = new Dictionary<string, string>();");
            sb.AppendLine("            foreach (var row in _fields)");
            sb.AppendLine("                if (!row.IsArray) current[row.Field] = row.Value;");
            sb.AppendLine("            var list = new List<FieldRow>(info.Fields.Length);");
            sb.AppendLine("            foreach (var f in info.Fields)");
            sb.AppendLine("            {");
            sb.AppendLine("                var row = new FieldRow(f);");
            sb.AppendLine("                row.Value = current.TryGetValue(f.Field, out var v) ? v : f.DefaultValue;");
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
            sb.AppendLine("                timer.Dispose(); // stops the dedicated timer thread");
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
            sb.AppendLine("                // Never block the transport thread on the UI: record on the background thread");
            sb.AppendLine("                // and queue the UI update, so 10 ms receive rates stay accurate.");
            sb.AppendLine("                _transport.DataReceived += OnDataReceivedBackground;");
            sb.AppendLine("                _transport.StatusChanged += message => Dispatcher.Invoke(() =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    if (message.Contains(\"timeout\", StringComparison.OrdinalIgnoreCase)) _timeoutCount++;");
            sb.AppendLine("                    Log(message);");
            sb.AppendLine("                });");
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
            sb.AppendLine("        /// <summary>Runs on the transport thread: records with a precise timestamp and");
            sb.AppendLine("        /// queues the UI update without blocking the receive loop.</summary>");
            sb.AppendLine("        private void OnDataReceivedBackground(byte[] data, string from)");
            sb.AppendLine("        {");
            sb.AppendLine("            _recorder.Record(false, data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : -1, data);");
            sb.AppendLine("            Dispatcher.InvokeAsync(() => OnReceived(data, from));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void OnReceived(byte[] data, string from)");
            sb.AppendLine("        {");
            sb.AppendLine("            // Keep this handler light: it runs for every received packet (already recorded).");
            sb.AppendLine("            // Heavy UI work (graph repaint, statistics refresh) is throttled elsewhere.");
            sb.AppendLine("            var sw = System.Diagnostics.Stopwatch.StartNew();");
            sb.AppendLine("            int msgId = data.Length >= 2 ? BitConverter.ToUInt16(data, 0) : -1;");
            sb.AppendLine("            var info = MessageCatalog.ById(msgId);");
            sb.AppendLine("            var name = info?.Name ?? $\"Unknown (0x{msgId:X})\";");
            sb.AppendLine("            RecordTimelineEvent(false, name, \"RECEIVED\", $\"{data.Length} bytes from {from}\", data);");
            sb.AppendLine("            UpdateStat(name, data.Length, outgoing: false, error: data.Length < 2, dropped: info is null && data.Length >= 2, crcError: HasCrcError(info, data));");
            sb.AppendLine("            // Scenario Response steps may auto-send a reply when this message matches their rule.");
            sb.AppendLine("            CheckScenarioResponses(info, name, data);");
            sb.AppendLine("            _monitor.Insert(0, new MonitorRow");
            sb.AppendLine("            {");
            sb.AppendLine("                Time = DateTime.Now.ToString(\"HH:mm:ss.fff\"), From = from, Message = name, Bytes = data.Length");
            sb.AppendLine("            });");
            sb.AppendLine("            if (_monitor.Count > 500) _monitor.RemoveAt(_monitor.Count - 1);");
            sb.AppendLine("            _receivedFields.Clear();");
            sb.AppendLine("            // Decode the field grid only when it is visible; at fast rates this is the");
            sb.AppendLine("            // most expensive part of the receive path.");
            sb.AppendLine("            if (info is not null && ReceivedFieldsGrid.IsVisible)");
            sb.AppendLine("            {");
            sb.AppendLine("                lock (_nativeLock) info.ConvertToPhysical(data, info.GetPhysical());");
            sb.AppendLine("                foreach (var f in info.Fields)");
            sb.AppendLine("                    _receivedFields.Add(new ReceivedField { Offset = f.Offset, Field = f.Field, Value = f.Read(data) });");
            sb.AppendLine("            }");
            sb.AppendLine("            // Per-message RX logging is skipped (the Monitor grid shows it) to keep fast rates smooth.");
            sb.AppendLine("            sw.Stop();");
            sb.AppendLine("            Interlocked.Add(ref _totalProcessingTicks, sw.ElapsedTicks);");
            sb.AppendLine("            Interlocked.Increment(ref _processedMessages);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Verifies the additive checksum of a received message when it has a CRC field.</summary>");
            sb.AppendLine("        private static bool HasCrcError(MessageInfo? info, byte[] data)");
            sb.AppendLine("        {");
            sb.AppendLine("            var crc = info?.Fields.FirstOrDefault(f => f.Field.EndsWith(\".crc\", StringComparison.OrdinalIgnoreCase));");
            sb.AppendLine("            if (crc is null || crc.Offset + 4 > data.Length) return false;");
            sb.AppendLine("            uint sum = 0;");
            sb.AppendLine("            for (int i = 0; i < crc.Offset; i++) sum += data[i];");
            sb.AppendLine("            return BitConverter.ToUInt32(data, crc.Offset) != sum;");
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
            sb.AppendLine("            var map = _fields.Where(f => !f.IsArray).ToDictionary(f => f.Field, f => f.Value);");
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
            AppendLookAndFeelLogic(sb);
            sb.AppendLine();
            AppendScenarioLogic(sb);
            sb.AppendLine();
            AppendRecordLogic(sb);
            sb.AppendLine();
            AppendGraphLogic(sb);
            sb.AppendLine();
            AppendRecordGraphLogic(sb);
            sb.AppendLine();
            AppendStatisticsLogic(sb);
            sb.AppendLine();
            sb.AppendLine("        private void Log(string message)");
            sb.AppendLine("        {");
            sb.AppendLine("            var line = $\"[{DateTime.Now:HH:mm:ss.fff}] {message}\" + Environment.NewLine;");
            sb.AppendLine("            AppendLog(TrafficLogBox, line);");
            sb.AppendLine("            AppendLog(ScenarioLogBox, line);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Appends and trims the log so endless traffic cannot bloat the UI.</summary>");
            sb.AppendLine("        private static void AppendLog(System.Windows.Controls.TextBox box, string line)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (box.Text.Length > 400000) box.Text = box.Text.Substring(box.Text.Length - 200000);");
            sb.AppendLine("            box.AppendText(line);");
            sb.AppendLine("            box.ScrollToEnd();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>Emits the Look &amp; Feel selector code-behind: four color themes applied
        /// by swapping the theme brushes referenced with DynamicResource.</summary>
        private static void AppendLookAndFeelLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>Emits the Scenario tab code-behind: drag &amp; drop from the palette,
        /// per-step asynchronous scheduling and folder based save/load.</summary>
        private static void AppendScenarioLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>Emits the Record button and Record tab code-behind.</summary>
        private static void AppendRecordLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>Emits the Graph tab code-behind: timeline recording, filtering and rendering.</summary>
        private static void AppendGraphLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>Emits the Record Graph tab code-behind: loads a recording into timeline
        /// events and reuses the shared renderer, filters, zoom and field decoding.</summary>
        private static void AppendRecordGraphLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>Emits the Statistics tab code-behind: per-message counters and 1 Hz rates.</summary>
        private static void AppendStatisticsLogic(StringBuilder sb)
        {
            sb.Append("""
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

            """);
        }

        /// <summary>The binary message recorder, file format and reader for the generated app.</summary>
        private static string BuildRecording() => """
            // Auto-generated by InterfaceWrapper. Do not edit by hand.
            using System;
            using System.Collections.Generic;
            using System.IO;

            namespace GenericSim
            {
                /// <summary>One message captured in a recording.</summary>
                public sealed class RecordedMessage
                {
                    public DateTime Timestamp { get; init; }
                    public bool IsOutgoing { get; init; }
                    public int MessageId { get; init; }
                    public byte[] Data { get; init; } = Array.Empty<byte>();
                }

                /// <summary>Row shown in the Record tab table.</summary>
                public sealed class RecordedMessageRow
                {
                    public int Ordinal { get; set; }
                    public string Time { get; set; } = string.Empty;
                    public string Direction { get; set; } = string.Empty;
                    public string Message { get; set; } = string.Empty;
                    public int MessageId { get; set; }
                    public int Bytes { get; set; }
                    public string Hex { get; set; } = string.Empty;
                    /// <summary>The raw payload, used to decode the fields when the row is clicked.</summary>
                    public byte[] Data { get; set; } = Array.Empty<byte>();
                }

                /// <summary>
                /// Binary recording format:
                ///   header  : magic "GSRC" (4 bytes) + version (int32)
                ///   record  : timestamp ticks (int64), direction (byte: 1=TX 0=RX),
                ///             message id (int32), payload length (int32), payload bytes.
                /// </summary>
                public static class RecordingFile
                {
                    public const string Magic = "GSRC";
                    public const int Version = 1;

                    public static void WriteHeader(BinaryWriter writer)
                    {
                        writer.Write(Magic.ToCharArray());
                        writer.Write(Version);
                    }

                    public static void WriteRecord(BinaryWriter writer, RecordedMessage record)
                    {
                        writer.Write(record.Timestamp.Ticks);
                        writer.Write(record.IsOutgoing ? (byte)1 : (byte)0);
                        writer.Write(record.MessageId);
                        writer.Write(record.Data.Length);
                        writer.Write(record.Data);
                    }

                    public static List<RecordedMessage> Load(string path)
                    {
                        var records = new List<RecordedMessage>();
                        using var reader = new BinaryReader(File.OpenRead(path));

                        var magic = new string(reader.ReadChars(4));
                        if (magic != Magic)
                            throw new InvalidDataException("Not a GenericSim recording file (bad magic).");
                        var version = reader.ReadInt32();
                        if (version != Version)
                            throw new InvalidDataException($"Unsupported recording version {version}.");

                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            var ticks = reader.ReadInt64();
                            var outgoing = reader.ReadByte() == 1;
                            var messageId = reader.ReadInt32();
                            var length = reader.ReadInt32();
                            var data = reader.ReadBytes(length);
                            records.Add(new RecordedMessage
                            {
                                Timestamp = new DateTime(ticks),
                                IsOutgoing = outgoing,
                                MessageId = messageId,
                                Data = data
                            });
                        }
                        return records;
                    }
                }

                /// <summary>Streams incoming and outgoing messages into a recording file.</summary>
                public sealed class MessageRecorder : IDisposable
                {
                    private readonly object _sync = new();
                    private BinaryWriter? _writer;
                    private string? _path;

                    public bool IsRecording { get { lock (_sync) return _writer is not null; } }
                    public int RecordedCount { get; private set; }

                    public void Start(string path)
                    {
                        lock (_sync)
                        {
                            _writer?.Dispose();
                            _writer = new BinaryWriter(File.Create(path));
                            _path = path;
                            RecordedCount = 0;
                            RecordingFile.WriteHeader(_writer);
                            _writer.Flush();
                        }
                    }

                    /// <summary>Appends one message; safe to call from any thread and a no-op when idle.</summary>
                    public void Record(bool outgoing, int messageId, byte[] data)
                    {
                        lock (_sync)
                        {
                            if (_writer is null) return;
                            RecordingFile.WriteRecord(_writer, new RecordedMessage
                            {
                                Timestamp = DateTime.Now,
                                IsOutgoing = outgoing,
                                MessageId = messageId,
                                Data = data
                            });
                            _writer.Flush();
                            RecordedCount++;
                        }
                    }

                    /// <summary>Stops recording and returns the file path.</summary>
                    public string? Stop()
                    {
                        lock (_sync)
                        {
                            _writer?.Dispose();
                            _writer = null;
                            return _path;
                        }
                    }

                    public void Dispose() => Stop();
                }
            }

            """;

        /// <summary>The scenario step model and the Configuration.xml + JSON folder store.</summary>
        private static string BuildScenario() => """
            // Auto-generated by InterfaceWrapper. Do not edit by hand.
            using System;
            using System.Collections.Generic;
            using System.ComponentModel;
            using System.Globalization;
            using System.IO;
            using System.Linq;
            using System.Text.Json;
            using System.Xml.Linq;

            namespace GenericSim
            {
                /// <summary>One scenario step: a message plus its own asynchronous send attributes.</summary>
                public sealed class ScenarioStepRow : INotifyPropertyChanged
                {
                    private int _index;
                    private string _message = string.Empty;
                    private int _timeMs;
                    private int _periodicIntervalMs = 1000;
                    private bool _periodic;
                    private int _maxMessages;
                    private bool _isSleep;
                    private bool _isResponse;
                    private string _responseTrigger = string.Empty;
                    private string _responseField = string.Empty;
                    private string _responseValue = "0";

                    /// <summary>The JSON prefix number; also the send order shown in the grid.</summary>
                    public int Index { get => _index; set { _index = value; OnChanged(nameof(Index)); } }
                    public string Message { get => _message; set { _message = value; OnChanged(nameof(Message)); } }
                    /// <summary>Milliseconds from scenario start until the first send;
                    /// for a Sleep step, the sleep duration.</summary>
                    public int TimeMs { get => _timeMs; set { _timeMs = value; OnChanged(nameof(TimeMs)); } }
                    /// <summary>Interval between periodic sends, in milliseconds.</summary>
                    public int PeriodicIntervalMs { get => _periodicIntervalMs; set { _periodicIntervalMs = value; OnChanged(nameof(PeriodicIntervalMs)); } }
                    public bool Periodic { get => _periodic; set { _periodic = value; OnChanged(nameof(Periodic)); OnChanged(nameof(SendType)); } }
                    /// <summary>Number of periodic sends; 0 sends with no limit.</summary>
                    public int MaxMessages { get => _maxMessages; set { _maxMessages = value; OnChanged(nameof(MaxMessages)); } }
                    /// <summary>True for a Sleep operation step: TimeMs holds the sleep duration and
                    /// every step after it is delayed by that amount when the scenario runs.</summary>
                    public bool IsSleep { get => _isSleep; set { _isSleep = value; OnChanged(nameof(IsSleep)); OnChanged(nameof(SendType)); } }
                    /// <summary>True for a Response operation step: when <see cref="ResponseTrigger"/> is
                    /// received and <see cref="ResponseField"/> equals <see cref="ResponseValue"/>, the
                    /// step's Message is sent with its saved field values.</summary>
                    public bool IsResponse { get => _isResponse; set { _isResponse = value; OnChanged(nameof(IsResponse)); OnChanged(nameof(SendType)); } }
                    /// <summary>The message that must be received to evaluate the response rule.</summary>
                    public string ResponseTrigger { get => _responseTrigger; set { _responseTrigger = value; OnChanged(nameof(ResponseTrigger)); } }
                    /// <summary>The field of the received message that is compared.</summary>
                    public string ResponseField { get => _responseField; set { _responseField = value; OnChanged(nameof(ResponseField)); } }
                    /// <summary>The expected value of the compared field.</summary>
                    public string ResponseValue { get => _responseValue; set { _responseValue = value; OnChanged(nameof(ResponseValue)); } }
                    public string SendType => IsSleep ? "Sleep" : IsResponse ? "Response" : Periodic ? "Periodic" : "One Time";
                    /// <summary>The message field values saved in the step JSON file.</summary>
                    public Dictionary<string, string>? FieldValues { get; set; }

                    public event PropertyChangedEventHandler? PropertyChanged;
                    private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
                }

                /// <summary>
                /// Persists a scenario as a folder: Configuration.xml (the Intervals sections) plus one
                /// "{index}_{message}.json" file per step holding the message field values, e.g.
                /// "0_STATUS_MESSAGE.json, 1_NONCE_MESSAGE.json".
                /// </summary>
                public static class ScenarioStore
                {
                    public const string ConfigurationFileName = "Configuration.xml";

                    public static void Save(string folder, IReadOnlyList<ScenarioStepRow> steps)
                    {
                        Directory.CreateDirectory(folder);

                        // Remove stale step files so the folder always mirrors the current scenario.
                        foreach (var file in Directory.GetFiles(folder, "*.json"))
                        {
                            var name = Path.GetFileNameWithoutExtension(file);
                            var underscore = name.IndexOf('_');
                            if (underscore > 0 && int.TryParse(name[..underscore], out _))
                                File.Delete(file);
                        }

                        var root = new XElement("Intervals");
                        for (int i = 0; i < steps.Count; i++)
                        {
                            var step = steps[i];
                            root.Add(new XElement("section",
                                new XAttribute("index", i),
                                new XAttribute("time", step.TimeMs),
                                new XAttribute("periodicInterval", step.PeriodicIntervalMs),
                                new XAttribute("periodic", step.Periodic ? "true" : "false"),
                                new XAttribute("maxMessages", step.MaxMessages),
                                new XAttribute("sleep", step.IsSleep ? "true" : "false"),
                                new XAttribute("response", step.IsResponse ? "true" : "false"),
                                new XAttribute("responseTrigger", step.ResponseTrigger),
                                new XAttribute("responseField", step.ResponseField),
                                new XAttribute("responseValue", step.ResponseValue)));

                            var json = JsonSerializer.Serialize(
                                step.FieldValues ?? new Dictionary<string, string>(),
                                new JsonSerializerOptions { WriteIndented = true });
                            File.WriteAllText(Path.Combine(folder, $"{i}_{step.Message}.json"), json);
                        }

                        new XDocument(new XDeclaration("1.0", "utf-8", null), root)
                            .Save(Path.Combine(folder, ConfigurationFileName));
                    }

                    public static List<ScenarioStepRow> Load(string folder)
                    {
                        var configPath = Path.Combine(folder, ConfigurationFileName);
                        if (!File.Exists(configPath))
                            throw new FileNotFoundException($"'{ConfigurationFileName}' was not found in {folder}.");

                        var steps = new List<ScenarioStepRow>();
                        var root = XDocument.Load(configPath).Root
                            ?? throw new InvalidDataException("Configuration.xml has no root element.");

                        foreach (var section in root.Elements("section"))
                        {
                            var index = ReadInt(section, "index", steps.Count);
                            var step = new ScenarioStepRow
                            {
                                Index = index,
                                TimeMs = ReadInt(section, "time", 0),
                                PeriodicIntervalMs = ReadInt(section, "periodicInterval", 0),
                                Periodic = string.Equals((string?)section.Attribute("periodic"), "true", StringComparison.OrdinalIgnoreCase),
                                MaxMessages = ReadInt(section, "maxMessages", 0),
                                IsSleep = string.Equals((string?)section.Attribute("sleep"), "true", StringComparison.OrdinalIgnoreCase),
                                IsResponse = string.Equals((string?)section.Attribute("response"), "true", StringComparison.OrdinalIgnoreCase),
                                ResponseTrigger = (string?)section.Attribute("responseTrigger") ?? string.Empty,
                                ResponseField = (string?)section.Attribute("responseField") ?? string.Empty,
                                ResponseValue = (string?)section.Attribute("responseValue") ?? "0"
                            };

                            // The JSON file prefixed with the index carries the message name and its values.
                            var file = Directory.GetFiles(folder, index + "_*.json").FirstOrDefault();
                            if (file is not null)
                            {
                                step.Message = Path.GetFileNameWithoutExtension(file)[(index.ToString().Length + 1)..];
                                step.FieldValues = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));
                            }
                            steps.Add(step);
                        }
                        return steps;
                    }

                    private static int ReadInt(XElement element, string attribute, int fallback) =>
                        int.TryParse((string?)element.Attribute(attribute), NumberStyles.Integer,
                            CultureInfo.InvariantCulture, out var value) ? value : fallback;
                }
            }

            """;

        private static string BuildModels()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Collections.ObjectModel;");
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
            sb.AppendLine("    /// <summary>Editable row bound to the fields DataGrid. A row is either a scalar field");
            sb.AppendLine("    /// or an array header whose Cells expand into a nested table at the array's offset.</summary>");
            sb.AppendLine("    public sealed class FieldRow : INotifyPropertyChanged");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly FieldInfo? _info;");
            sb.AppendLine("        private readonly ArrayInfo? _array;");
            sb.AppendLine("        private string _value = \"0\";");
            sb.AppendLine("        private bool _isExpanded;");
            sb.AppendLine("        private string _countText = \"1\";");
            sb.AppendLine("        public FieldRow(FieldInfo info) { _info = info; }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Creates an array header row with the given initial element count.</summary>");
            sb.AppendLine("        public FieldRow(ArrayInfo array, int initialCount)");
            sb.AppendLine("        {");
            sb.AppendLine("            _array = array;");
            sb.AppendLine("            RebuildCells(initialCount);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public bool IsArray => _array is not null;");
            sb.AppendLine("        public ArrayInfo? ArrayDef => _array;");
            sb.AppendLine("        /// <summary>The editable cells of the nested array table (element x sub-field).</summary>");
            sb.AppendLine("        public ObservableCollection<ArrayCellRow> Cells { get; } = new();");
            sb.AppendLine();
            sb.AppendLine("        public int Offset => _array?.BaseOffset ?? _info!.Offset;");
            sb.AppendLine("        public string Field => _array?.Name ?? _info!.Field;");
            sb.AppendLine("        public string Type => _array is null ? _info!.Type : \"ARRAY\";");
            sb.AppendLine("        public int Size => _array?.Stride ?? _info!.Size;");
            sb.AppendLine();
            sb.AppendLine("        public string Value");
            sb.AppendLine("        {");
            sb.AppendLine("            get => _array is null ? _value : $\"[{CellElementCount} element(s)] click to open / close the cells\";");
            sb.AppendLine("            set { if (_array is not null) return; _value = value; OnChanged(nameof(Value)); }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private int CellElementCount => _array is null || _array.Elements.Length == 0 ? 0 : Cells.Count / _array.Elements.Length;");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Expands / collapses the nested cells table under this array row.</summary>");
            sb.AppendLine("        public bool IsExpanded { get => _isExpanded; set { _isExpanded = value; OnChanged(nameof(IsExpanded)); OnChanged(nameof(ExpanderGlyph)); } }");
            sb.AppendLine("        public string CountText { get => _countText; set { _countText = value; OnChanged(nameof(CountText)); } }");
            sb.AppendLine("        public string ExpanderGlyph => _isExpanded ? \"\\u25BC\" : \"\\u25B6\";");
            sb.AppendLine("        public System.Windows.Visibility ExpanderVisibility => IsArray ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;");
            sb.AppendLine("        public string ArraySummary => _array is null ? string.Empty");
            sb.AppendLine("            : $\"stride {_array.Stride} B, max {_array.MaxCount}, {_array.Elements.Length} sub-field(s)\";");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Rebuilds one cell per (element x sub-field), clamped to the array maximum.</summary>");
            sb.AppendLine("        public int RebuildCells(int count)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_array is null) return 0;");
            sb.AppendLine("            count = Math.Clamp(count, 0, _array.MaxCount);");
            sb.AppendLine("            Cells.Clear();");
            sb.AppendLine("            for (int i = 0; i < count; i++)");
            sb.AppendLine("                foreach (var el in _array.Elements)");
            sb.AppendLine("                    Cells.Add(new ArrayCellRow");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Index = i,");
            sb.AppendLine("                        Offset = _array.BaseOffset + i * _array.Stride + el.RelativeOffset,");
            sb.AppendLine("                        Field = el.Field.Replace($\"[{_array.IndexVar}]\", $\"[{i}]\", StringComparison.Ordinal),");
            sb.AppendLine("                        Type = el.Type,");
            sb.AppendLine("                        Value = \"0\"");
            sb.AppendLine("                    });");
            sb.AppendLine("            _countText = count.ToString();");
            sb.AppendLine("            OnChanged(nameof(CountText));");
            sb.AppendLine("            OnChanged(nameof(Value));");
            sb.AppendLine("            return count;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Writes the edited value(s) straight into the interface buffer image.</summary>");
            sb.AppendLine("        public void WriteToBuffer(byte[] buffer)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (_array is not null)");
            sb.AppendLine("            {");
            sb.AppendLine("                // Array header row: write every nested cell at its own offset.");
            sb.AppendLine("                foreach (var cell in Cells) cell.WriteToBuffer(buffer);");
            sb.AppendLine("                return;");
            sb.AppendLine("            }");
            sb.AppendLine("            if (_info is null || _info.Offset + _info.Size > buffer.Length) return;");
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
            sb.AppendLine("            if (_info is null) return;");
            sb.AppendLine("            System.Array.Copy(value, 0, buffer, _info.Offset, Math.Min(value.Length, _info.Size));");
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
            sb.AppendLine();
            sb.AppendLine("    /// <summary>One point on the Graph tab timeline (a sent or received message).</summary>");
            sb.AppendLine("    public sealed class TimelineEvent");
            sb.AppendLine("    {");
            sb.AppendLine("        public DateTime Timestamp { get; init; }");
            sb.AppendLine("        public bool IsOutgoing { get; init; }");
            sb.AppendLine("        public string Message { get; init; } = string.Empty;");
            sb.AppendLine("        public string Status { get; init; } = string.Empty;");
            sb.AppendLine("        public string Details { get; init; } = string.Empty;");
            sb.AppendLine("        /// <summary>The raw message bytes, used to decode the fields when the point is clicked.</summary>");
            sb.AppendLine("        public byte[] Data { get; init; } = Array.Empty<byte>();");
            sb.AppendLine("        public string Time => Timestamp.ToString(\"HH:mm:ss.fff\");");
            sb.AppendLine("        public string Direction => IsOutgoing ? \"MBT \\u2192 NIRON\" : \"NIRON \\u2192 MBT\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>Running statistics for one message type (Statistics tab).</summary>");
            sb.AppendLine("    public sealed class MessageStat");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Name { get; init; } = string.Empty;");
            sb.AppendLine("        public long Received;");
            sb.AppendLine("        public long Sent;");
            sb.AppendLine("        public long TotalBytes;");
            sb.AppendLine("        public long BytesSent;");
            sb.AppendLine("        public long BytesReceived;");
            sb.AppendLine("        public long CrcErrors;");
            sb.AppendLine("        public long Errors;");
            sb.AppendLine("        public long Dropped;");
            sb.AppendLine("        public int MinSize = int.MaxValue;");
            sb.AppendLine("        public int MaxSize;");
            sb.AppendLine("        public DateTime? LastReceived;");
            sb.AppendLine("        public DateTime? LastSent;");
            sb.AppendLine("        /// <summary>Messages/bytes since the last statistics tick (for the 1 s rates).</summary>");
            sb.AppendLine("        public long WindowCount;");
            sb.AppendLine("        public long WindowBytes;");
            sb.AppendLine("        public double MsgPerSec;");
            sb.AppendLine("        public double BytesPerSec;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>Row shown in the Statistics tab grid.</summary>");
            sb.AppendLine("    public sealed class MessageStatRow");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Message { get; set; } = string.Empty;");
            sb.AppendLine("        public long Received { get; set; }");
            sb.AppendLine("        public long Sent { get; set; }");
            sb.AppendLine("        public string MsgPerSec { get; set; } = \"0\";");
            sb.AppendLine("        public string BytesPerSec { get; set; } = \"0\";");
            sb.AppendLine("        public long TotalBytes { get; set; }");
            sb.AppendLine("        public string AvgSize { get; set; } = \"-\";");
            sb.AppendLine("        public string MinSize { get; set; } = \"-\";");
            sb.AppendLine("        public string MaxSize { get; set; } = \"-\";");
            sb.AppendLine("        public string LastReceived { get; set; } = \"-\";");
            sb.AppendLine("        public string LastSent { get; set; } = \"-\";");
            sb.AppendLine("        public string SinceLast { get; set; } = \"-\";");
            sb.AppendLine("        public long Errors { get; set; }");
            sb.AppendLine("        public long Dropped { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>One row of the Global Statistics table (Statistics tab).</summary>");
            sb.AppendLine("    public sealed class GlobalStatRow");
            sb.AppendLine("    {");
            sb.AppendLine("        public GlobalStatRow(string statistic, string value) { Statistic = statistic; Value = value; }");
            sb.AppendLine("        public string Statistic { get; }");
            sb.AppendLine("        public string Value { get; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>One sub-field of an array element (Arrays editor).</summary>");
            sb.AppendLine("    public sealed class ArrayElementInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public int RelativeOffset { get; init; }");
            sb.AppendLine("        public string Field { get; init; } = string.Empty;");
            sb.AppendLine("        public string Type { get; init; } = string.Empty;");
            sb.AppendLine("        public int Size { get; init; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>An array field discovered in a convert loop (repeated element block).</summary>");
            sb.AppendLine("    public sealed class ArrayInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Name { get; init; } = string.Empty;");
            sb.AppendLine("        public int BaseOffset { get; init; }");
            sb.AppendLine("        public int Stride { get; init; }");
            sb.AppendLine("        public int MaxCount { get; init; }");
            sb.AppendLine("        public string IndexVar { get; init; } = \"i1\";");
            sb.AppendLine("        public string? CountField { get; init; }");
            sb.AppendLine("        public ArrayElementInfo[] Elements { get; init; } = Array.Empty<ArrayElementInfo>();");
            sb.AppendLine("        public string Display => $\"{Name}  [max {MaxCount} x {Stride} B]\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>One editable array cell (a single sub-field of a single element).</summary>");
            sb.AppendLine("    public sealed class ArrayCellRow : INotifyPropertyChanged");
            sb.AppendLine("    {");
            sb.AppendLine("        private string _value = \"0\";");
            sb.AppendLine("        public int Index { get; set; }");
            sb.AppendLine("        public string Field { get; set; } = string.Empty;");
            sb.AppendLine("        public string Type { get; set; } = string.Empty;");
            sb.AppendLine("        public int Offset { get; set; }");
            sb.AppendLine("        public string Value { get => _value; set { _value = value; OnChanged(nameof(Value)); } }");
            sb.AppendLine();
            sb.AppendLine("        /// <summary>Writes this cell's value into the interface buffer at its absolute offset.</summary>");
            sb.AppendLine("        public void WriteToBuffer(byte[] buffer)");
            sb.AppendLine("        {");
            sb.AppendLine("            var ci = CultureInfo.InvariantCulture;");
            sb.AppendLine("            switch (Type)");
            sb.AppendLine("            {");
            sb.AppendLine("                case \"UINT8\": if (Offset + 1 <= buffer.Length) buffer[Offset] = byte.TryParse(_value, out var b) ? b : (byte)0; break;");
            sb.AppendLine("                case \"INT8\": if (Offset + 1 <= buffer.Length) buffer[Offset] = unchecked((byte)(sbyte.TryParse(_value, out var s8) ? s8 : 0)); break;");
            sb.AppendLine("                case \"UINT16\": Write(buffer, BitConverter.GetBytes(ushort.TryParse(_value, out var u16) ? u16 : (ushort)0)); break;");
            sb.AppendLine("                case \"INT16\": Write(buffer, BitConverter.GetBytes(short.TryParse(_value, out var i16) ? i16 : (short)0)); break;");
            sb.AppendLine("                case \"UINT32\": Write(buffer, BitConverter.GetBytes(uint.TryParse(_value, out var u32) ? u32 : 0u)); break;");
            sb.AppendLine("                case \"INT32\": Write(buffer, BitConverter.GetBytes(int.TryParse(_value, out var i32) ? i32 : 0)); break;");
            sb.AppendLine("                case \"UINT64\": Write(buffer, BitConverter.GetBytes(ulong.TryParse(_value, out var u64) ? u64 : 0ul)); break;");
            sb.AppendLine("                case \"INT64\": Write(buffer, BitConverter.GetBytes(long.TryParse(_value, out var i64) ? i64 : 0L)); break;");
            sb.AppendLine("                case \"FLOAT32\": Write(buffer, BitConverter.GetBytes(float.TryParse(_value, NumberStyles.Float, ci, out var f) ? f : 0f)); break;");
            sb.AppendLine("                case \"FLOAT64\": Write(buffer, BitConverter.GetBytes(double.TryParse(_value, NumberStyles.Float, ci, out var d) ? d : 0d)); break;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private void Write(byte[] buffer, byte[] value)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (Offset + value.Length <= buffer.Length)");
            sb.AppendLine("                Array.Copy(value, 0, buffer, Offset, value.Length);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public event PropertyChangedEventHandler? PropertyChanged;");
            sb.AppendLine("        private void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>An armed scenario Response rule: when the trigger message is received");
            sb.AppendLine("    /// and its field equals the expected value, the Send message is sent.</summary>");
            sb.AppendLine("    public sealed class ResponseRule");
            sb.AppendLine("    {");
            sb.AppendLine("        public string TriggerName { get; init; } = string.Empty;");
            sb.AppendLine("        public string Field { get; init; } = string.Empty;");
            sb.AppendLine("        public string Value { get; init; } = string.Empty;");
            sb.AppendLine("        public MessageInfo Send { get; init; } = null!;");
            sb.AppendLine("        public IReadOnlyList<FieldRow> SendFields { get; init; } = Array.Empty<FieldRow>();");
            sb.AppendLine("        public IReadOnlyList<ArrayCellRow> SendArrayCells { get; init; } = Array.Empty<ArrayCellRow>();");
            sb.AppendLine("        public int StepIndex { get; init; }");
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
                    /// <summary>Number of connected peers (TCP server: clients; others: 1 when open).</summary>
                    int ConnectionCount { get; }
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
                    public int ConnectionCount => _udp is null ? 0 : 1;
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
                    private readonly object _writeLock = new();

                    public TcpClientTransport(string remoteIp, int remotePort)
                    {
                        _remoteIp = remoteIp;
                        _remotePort = remotePort;
                    }

                    public string Description => $"TCP client -> {_remoteIp}:{_remotePort}";
                    public int ConnectionCount => _client?.Connected == true ? 1 : 0;
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
                        // Serialize writes: periodic sends may arrive from several timer threads.
                        lock (_writeLock) stream.Write(data, 0, data.Length);
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
                    private readonly object _writeLock = new();
                    private readonly List<TcpClient> _clients = new();
                    private TcpListener? _listener;
                    private CancellationTokenSource? _cts;

                    public TcpServerTransport(int localPort) => _localPort = localPort;

                    public string Description => $"TCP server on port {_localPort}";
                    public int ConnectionCount { get { lock (_sync) return _clients.Count; } }
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
                        // Serialize writes: periodic sends may arrive from several timer threads.
                        lock (_writeLock)
                        {
                            foreach (var client in clients)
                            {
                                try { client.GetStream().Write(data, 0, data.Length); }
                                catch (Exception ex) { StatusChanged?.Invoke("TCP TX error: " + ex.Message); }
                            }
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
                    private readonly object _writeLock = new();

                    public SerialTransport(string portName, int baudRate, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
                    {
                        _portName = portName;
                        _baudRate = baudRate;
                        _parity = parity;
                        _stopBits = stopBits;
                    }

                    public string Description => $"RS232 {_portName} @ {_baudRate} baud, {_parity} parity, {_stopBits} stop bits";
                    public int ConnectionCount => _port?.IsOpen == true ? 1 : 0;
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
                        // Serialize writes: periodic sends may arrive from several timer threads.
                        lock (_writeLock) port.Write(data, 0, data.Length);
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

        /// <summary>The 1 ms clock helper and the drift-free precision timer used for
        /// millisecond-accurate periodic sending (e.g. every 10 ms).</summary>
        private static string BuildPrecisionTimer() => """
            // Auto-generated by InterfaceWrapper. Do not edit by hand.
            using System;
            using System.Diagnostics;
            using System.Runtime.InteropServices;
            using System.Threading;

            namespace GenericSim
            {
                /// <summary>Raises the Windows timer resolution to 1 ms while enabled so short
                /// sleeps (and therefore fast periodic sends) fire on time.</summary>
                public static class HighResClock
                {
                    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")] private static extern uint TimeBeginPeriod(uint ms);
                    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")] private static extern uint TimeEndPeriod(uint ms);
                    private static int _enabled;

                    public static void Enable() { if (Interlocked.Exchange(ref _enabled, 1) == 0) TimeBeginPeriod(1); }
                    public static void Disable() { if (Interlocked.Exchange(ref _enabled, 0) == 1) TimeEndPeriod(1); }
                }

                /// <summary>
                /// Drift-free periodic timer on a dedicated high-priority background thread.
                /// It sleeps for the bulk of each interval and spin-waits the last ~2 ms, giving
                /// millisecond cadence accuracy for intervals as small as a few ms (e.g. 10 ms).
                /// Ticks are scheduled on an absolute timeline (next += interval), so one late
                /// tick does not shift the following ones. The callback runs on the timer
                /// thread and must be thread-safe.
                /// </summary>
                public sealed class PrecisionTimer : IDisposable
                {
                    private readonly double _intervalMs;
                    private readonly Action _tick;
                    private volatile bool _running = true;

                    public PrecisionTimer(double intervalMs, Action tick)
                    {
                        _intervalMs = Math.Max(1.0, intervalMs);
                        _tick = tick ?? throw new ArgumentNullException(nameof(tick));
                        HighResClock.Enable();
                        var thread = new Thread(Loop) { IsBackground = true, Priority = ThreadPriority.Highest, Name = "PrecisionTimer" };
                        thread.Start();
                    }

                    private void Loop()
                    {
                        var clock = Stopwatch.StartNew();
                        double next = _intervalMs;
                        while (_running)
                        {
                            double remaining = next - clock.Elapsed.TotalMilliseconds;
                            if (remaining > 4.0)
                            {
                                // Coarse wait: sleep almost all of the remaining time (1 ms resolution).
                                Thread.Sleep((int)(remaining - 2.0));
                            }
                            else if (remaining > 0.05)
                            {
                                // Fine wait: spin the last ~2 ms for sub-millisecond accuracy.
                                Thread.SpinWait(250);
                            }
                            else
                            {
                                try { _tick(); }
                                catch { /* never break the cadence */ }
                                next += _intervalMs;
                                // After a long stall (breakpoint, suspend) resync instead of bursting.
                                if (next < clock.Elapsed.TotalMilliseconds - _intervalMs)
                                    next = clock.Elapsed.TotalMilliseconds + _intervalMs;
                            }
                        }
                    }

                    public void Dispose() => _running = false;
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
            sb.AppendLine("        public ArrayInfo[] Arrays { get; init; } = Array.Empty<ArrayInfo>();");
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
                    sb.AppendLine("                Fields = Array.Empty<FieldInfo>(),");
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
                    sb.AppendLine("                },");
                }
                if (msg.Arrays.Count == 0)
                {
                    sb.AppendLine("                Arrays = Array.Empty<ArrayInfo>()");
                }
                else
                {
                    sb.AppendLine("                Arrays = new[]");
                    sb.AppendLine("                {");
                    foreach (var arr in msg.Arrays)
                    {
                        var countField = arr.CountField is null ? "null" : $"\"{arr.CountField}\"";
                        sb.AppendLine("                    new ArrayInfo");
                        sb.AppendLine("                    {");
                        sb.AppendLine($"                        Name = \"{arr.Name}\", BaseOffset = {arr.BaseOffset}, Stride = {arr.Stride}, MaxCount = {arr.MaxCount}, IndexVar = \"{arr.IndexVar}\", CountField = {countField},");
                        sb.AppendLine("                        Elements = new[]");
                        sb.AppendLine("                        {");
                        foreach (var el in arr.Elements)
                        {
                            sb.AppendLine($"                            new ArrayElementInfo {{ RelativeOffset = {el.RelativeOffset}, Field = \"{el.Field}\", Type = \"{el.Type}\", Size = {el.Size} }},");
                        }
                        sb.AppendLine("                        }");
                        sb.AppendLine("                    },");
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
            sb.AppendLine("- Array fields (e.g. bit/discrete arrays) appear as rows inside the fields grid");
            sb.AppendLine("  at their real offset; the arrow expander opens a nested cells table under the");
            sb.AppendLine("  row where every element can be edited, and collapses it when not needed. The");
            sb.AppendLine("  element count box sets how many elements are sent (the count field is synced).");
            sb.AppendLine("- Auto sequence / timestamp / CRC toggles.");
            sb.AppendLine("- Live HEX preview built through the native convert functions.");
            sb.AppendLine("- Selectable transport: UDP, TCP (client or server) or RS232 serial.");
            sb.AppendLine("- Transport settings in the top bar (ports, IP, TCP mode, COM port, baud rate, parity, stop bits).");
            sb.AppendLine("- Periodic send (interval + count), send history, NIRON\u2192MBT monitor and traffic log.");
            sb.AppendLine("- Export/Import message field values as JSON.");
            sb.AppendLine("- Scenario tab: drag messages from 'All Messages' into an ordered step list,");
            sb.AppendLine("  set per-step attributes (one time / periodic, start time, interval, max count)");
            sb.AppendLine("  and run all steps in parallel. Step fields support arrays: expand an array");
            sb.AppendLine("  row to type each cell's value (saved with the scenario and sent on run).");
            sb.AppendLine("  A Sleep operation can be dragged from the");
            sb.AppendLine("  Operations panel under 'All Messages' (with a millisecond duration); when the");
            sb.AppendLine("  running scenario reaches the Sleep step it pauses the timeline for that long");
            sb.AppendLine("  (every later step is delayed). A Response operation (drag it the same way)");
            sb.AppendLine("  waits for a chosen message: when it is received and the selected field equals");
            sb.AppendLine("  the entered value, the configured message is sent automatically. A scenario");
            sb.AppendLine("  is stored in its own folder as `Configuration.xml` plus one");
            sb.AppendLine("  `{index}_{message}.json` file per step.");
            sb.AppendLine("- Record button: captures every incoming and outgoing message to a binary `.bin`");
            sb.AppendLine("  file. The Record tab opens such a file, lists the message types on the left,");
            sb.AppendLine("  shows every recorded message of the selected type in the table and decodes");
            sb.AppendLine("  all the fields of the message clicked in the table. 'Export Type (CSV)' writes");
            sb.AppendLine("  the selected type to a CSV file: one column per field, one row per recorded");
            sb.AppendLine("  message (6 received messages of a type produce 6 rows).");
            sb.AppendLine("- Graph tab: live message timeline with a TX lane (MBT \u2192 NIRON) and an RX lane");
            sb.AppendLine("  (NIRON \u2192 MBT), per-message filter checkboxes (each showing the live number of");
            sb.AppendLine("  sent/received messages of that type), zoom in/out (timeline header");
            sb.AppendLine("  buttons, View Options or Ctrl+mouse wheel), direction / pause options");
            sb.AppendLine("  and a details table showing when every message arrived. Clicking a point on the");
            sb.AppendLine("  timeline focuses its row in the details table and decodes all the fields of that");
            sb.AppendLine("  message (clicking a details row decodes it as well). Under heavy traffic the");
            sb.AppendLine("  graph repaints at most ~3 times per second and draws only the newest events,");
            sb.AppendLine("  so the UI stays responsive.");
            sb.AppendLine("- Record Graph tab: opens a recording `.bin` file (captured with the Record");
            sb.AppendLine("  button) and displays it on the same two-lane timeline as the Graph tab, with");
            sb.AppendLine("  per-type filter checkboxes (each showing how many messages of that type are");
            sb.AppendLine("  in the file), zoom / direction options, a details table and");
            sb.AppendLine("  full field decoding; clicking a timeline point focuses its row in the");
            sb.AppendLine("  details table.");
            sb.AppendLine("- Statistics tab: a Global Statistics panel (total messages/bytes sent and");
            sb.AppendLine("  received, current and peak messages/sec and bytes/sec, average traffic rate,");
            sb.AppendLine("  running time, CPU and memory usage, queue size, connected interfaces, packet");
            sb.AppendLine("  loss, error / CRC / timeout / retransmission counters and average processing");
            sb.AppendLine("  time per message) next to a per-message table with totals (received/sent),");
            sb.AppendLine("  the current rates (messages per second and bytes per second), total bytes,");
            sb.AppendLine("  average / minimum / maximum message size, last RX/TX timestamps, seconds");
            sb.AppendLine("  since the last message, receive errors and dropped (unknown id) messages;");
            sb.AppendLine("  refreshed every second, with a totals summary line and a reset button.");
            sb.AppendLine("- Look & Feel selector in the header: Dark Blue (default), Midnight Black,");
            sb.AppendLine("  Light and Military Green themes; the whole UI (panels, tables, graphs)");
            sb.AppendLine("  restyles instantly.");
            sb.AppendLine("- Precision timing: periodic sends (Simulator tab and scenario steps) run on");
            sb.AppendLine("  dedicated high-priority PrecisionTimer threads with 1 ms Windows timer");
            sb.AppendLine("  resolution and drift-free absolute scheduling, so intervals as small as");
            sb.AppendLine("  10 ms are accurate. The receive path records on the transport thread and");
            sb.AppendLine("  never blocks on the UI, so fast incoming rates stay accurate too.");
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
