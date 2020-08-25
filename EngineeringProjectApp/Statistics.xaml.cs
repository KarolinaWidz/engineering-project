using EngineeringProjectApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EngineeringProjectApp
{
    /// <summary>
    /// Logika interakcji dla klasy Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        private List<ResultModel> results;
        private int userId;

        public Statistics(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            results = SqliteDataAccess.LoadGamesForUser(DificultyLevelComboBox.Text, userId);
            StatisticsList.ItemsSource = results;
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       
        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            results = SqliteDataAccess.LoadGamesForUser(DificultyLevelComboBox.Text, userId);
            StatisticsList.ItemsSource = results;
        }
    }
}
