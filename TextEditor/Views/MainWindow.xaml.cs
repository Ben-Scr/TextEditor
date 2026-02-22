using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using TextEditor.Resources;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace TextEditor
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BenScr", "TextEditor");

        private string currentFilePath = Path.Combine(MainPath, "Unknown.txt");
        private readonly List<Match> searchMatches = new();
        private int currentSearchIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            SelectFileAtPath(currentFilePath);
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Choose a File",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectFileAtPath(dialog.FileName, true);
            }
        }

        private void SelectFileAtPath(string path, bool readFile = false)
        {
            currentFilePath = path;

            if (readFile)
            {
                FileContentTextBox.Text = File.ReadAllText(currentFilePath, Encoding.UTF8);
                FileSizeText.Content = Utility.ToSI((ulong)new FileInfo(currentFilePath).Length);
            }

            int lettersLength = FileContentTextBox.Text.Length;
            LettersCountText.Content = $"{lettersLength} {(lettersLength == 1 ? "Letter" : "Letters")}";
            SelectedFileName.Content = Path.GetFileName(currentFilePath);

            RecalculateSearchMatches(keepSelection: false);
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save File",
                InitialDirectory = Path.GetDirectoryName(currentFilePath),
                FileName = Path.GetFileName(currentFilePath),
                DefaultExt = Path.GetExtension(currentFilePath),
                OverwritePrompt = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                currentFilePath = saveDialog.FileName;
                File.WriteAllText(currentFilePath, FileContentTextBox.Text);
                SelectFileAtPath(currentFilePath);
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            int lettersLength = FileContentTextBox.Text.Length;
            LettersCountText.Content = $"{lettersLength} {(lettersLength == 1 ? "Letter" : "Letters")}";
            RecalculateSearchMatches();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                OpenSearchPanel();
            }
            else if (e.Key == Key.Escape && SearchPanel.Visibility == Visibility.Visible)
            {
                e.Handled = true;
                CloseSearchPanel();
            }
        }

        private void OpenSearchPanel()
        {
            SearchPanel.Visibility = Visibility.Visible;
            SearchTextBox.Focus();
            SearchTextBox.SelectAll();
            RecalculateSearchMatches();
        }

        private void CloseSearchPanel_Click(object sender, RoutedEventArgs e)
        {
            CloseSearchPanel();
        }

        private void CloseSearchPanel()
        {
            SearchPanel.Visibility = Visibility.Collapsed;
            SearchStatusText.Text = "Enter your search.";
            FileContentTextBox.Focus();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateSearchMatches(keepSelection: false);
        }

        private void SearchOptions_Changed(object sender, RoutedEventArgs e)
        {
            RecalculateSearchMatches(keepSelection: false);
        }

        private void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                FindNextMatch();
            }
        }

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            FindNextMatch();
        }

        private void FindPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (searchMatches.Count == 0)
            {
                RecalculateSearchMatches(keepSelection: false);
                return;
            }

            currentSearchIndex = currentSearchIndex <= 0 ? searchMatches.Count - 1 : currentSearchIndex - 1;
            SelectCurrentMatch();
        }

        private void FindNextMatch()
        {
            if (searchMatches.Count == 0)
            {
                RecalculateSearchMatches(keepSelection: false);
                return;
            }

            currentSearchIndex = (currentSearchIndex + 1) % searchMatches.Count;
            SelectCurrentMatch();
        }

        private void RecalculateSearchMatches(bool keepSelection = true)
        {
            if (SearchPanel.Visibility != Visibility.Visible)
            {
                return;
            }

            string pattern = SearchTextBox.Text;
            searchMatches.Clear();
            currentSearchIndex = -1;

            if (string.IsNullOrWhiteSpace(pattern))
            {
                SearchStatusText.Text = "Enter your search";
                return;
            }

            try
            {
                RegexOptions options = RegexOptions.Multiline;
                if (!MatchCaseCheckBox.IsChecked.GetValueOrDefault())
                {
                    options |= RegexOptions.IgnoreCase;
                }

                MatchCollection matches = Regex.Matches(FileContentTextBox.Text, pattern, options);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        searchMatches.Add(match);
                    }
                }

                if (searchMatches.Count == 0)
                {
                    SearchStatusText.Text = "No results found.";
                    return;
                }

                SearchStatusText.Text = $"{searchMatches.Count} results found.";
                if (keepSelection)
                {
                    FindNextMatch();
                }
            }
            catch (ArgumentException ex)
            {
                SearchStatusText.Text = $"Invalid Regex: {ex.Message}";
            }
        }

        private void SelectCurrentMatch()
        {
            if (currentSearchIndex < 0 || currentSearchIndex >= searchMatches.Count)
            {
                return;
            }

            Match match = searchMatches[currentSearchIndex];
            FileContentTextBox.Focus();
            FileContentTextBox.Select(match.Index, match.Length);
            FileContentTextBox.ScrollToLine(FileContentTextBox.GetLineIndexFromCharacterIndex(match.Index));
            SearchStatusText.Text = $"Treffer {currentSearchIndex + 1} von {searchMatches.Count}";
        }
    }
}