using CsvHelper;
using EngineeringProjectApp.Model;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace EngineeringProjectApp
{

    public partial class Statistics : Window
    {
        private List<ResultModel> results;
        private int userId;

        public Statistics(int userId)
        {
            InitializeComponent();
            this.userId = userId;
            results = SqliteDataAccess.LoadGamesForUser(DificultyLevelComboBox.Text, userId, ReturningCheckBox.IsChecked == true ? 1 : 0);
            StatisticsList.ItemsSource = results;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            results = SqliteDataAccess.LoadGamesForUser(DificultyLevelComboBox.Text, userId, ReturningCheckBox.IsChecked == true ? 1 : 0);
            StatisticsList.ItemsSource = results;
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveToFile saveToFileWindow = new SaveToFile(results);
            saveToFileWindow.Show();
        }
    }
}
