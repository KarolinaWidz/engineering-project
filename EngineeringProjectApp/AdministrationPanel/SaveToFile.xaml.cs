using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace EngineeringProjectApp
{
    /// <summary>
    /// Logika interakcji dla klasy SaveToFile.xaml
    /// </summary>
    public partial class SaveToFile : Window
    {
        private List<Model.ResultModel> dataToExport;
        private string selectedPath;
        public SaveToFile(List<Model.ResultModel> dataToExport)
        {
            InitializeComponent();
            this.dataToExport = dataToExport;
            selectedPath = "";
        }

        private void BtnPath_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            var folderResult = folderDialog.ShowDialog();
            if (folderResult.HasValue && folderResult.Value)
            {
                Path.Text = folderDialog.SelectedPath;
            }
        }

        private void BtnName_Click(object sender, RoutedEventArgs e)
        {

            selectedPath = Path.Text;
            if (ValidArgument(selectedPath, Name.Text))
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(selectedPath + "/" + Name.Text + ".csv", false, new UTF8Encoding(true)))
                    {
                        CsvWriter writer = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);
                        writer.WriteRecords(dataToExport);
                    }
                    MessageBox.Show("Zapisywanie przeszło pomyślnie", "Info");
                    Close();
                }
                catch(IOException)
                {
                    MessageBox.Show("Nie można zapisać pliku. Upewnij się, że podana lokalizacja istnieje!", "Błąd!");
                }
            }
        }

        private bool ValidArgument(string path, string name)
        {
            if (path.Trim() == "" || name.Trim() == "")
            {
                MessageBox.Show("Podaną błędną ścieżkę/nazwę pliku", "Błąd!");
                return false;
            }
            return true;
        }

    }
}
