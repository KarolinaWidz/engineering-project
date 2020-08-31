using EngineeringProjectApp.Model;
using System.Collections.Generic;
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
    }
}
