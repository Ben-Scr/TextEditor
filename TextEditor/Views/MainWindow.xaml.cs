using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        public MainWindow()
        {
            InitializeComponent();
            SelectFileAtPath(currentFilePath);
        }

        private void SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Wähle eine Datei im gewünschten Ordner",
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

            if(readFile)
                FileContentTextBox.Text = File.ReadAllText(currentFilePath, Encoding.UTF8);

            int lettersLength = FileContentTextBox.Text.Length;
            LettersCountText.Content = $"{lettersLength} {(lettersLength == 1 ? "Letter" : "Letters")}";

            SelectedFileName.Content = Path.GetFileName(currentFilePath);
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save File",
                InitialDirectory = Path.GetDirectoryName(currentFilePath),
                FileName = Path.GetFileName(currentFilePath),
                DefaultExt =Path.GetExtension(currentFilePath),
                OverwritePrompt = true
            };

            if (saveDialog.ShowDialog() == true)
            {
                currentFilePath = saveDialog.FileName;
                File.WriteAllText(currentFilePath, FileContentTextBox.Text);
                SelectFileAtPath(currentFilePath);
            }
            else
            {
                return;
            }
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            int lettersLength = FileContentTextBox.Text.Length;
            LettersCountText.Content = $"{lettersLength} {(lettersLength == 1 ? "Letter" : "Letters")}";
        }
    }
}