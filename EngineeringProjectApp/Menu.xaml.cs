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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EngineeringProjectApp
{
    /// <summary>
    /// Logika interakcji dla klasy Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        string hand;
        public Menu()
        {
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.hand = HandComboBox.Text;
            if (DificultyLevelComboBox.Text== "Łatwy" && ReturningCheckBox.IsChecked == true) {
                MainWindow mainWindow = new MainWindow(int.Parse(AmountOfBirdsBox.Text), int.Parse(AmountOfButterfliesBox.Text),this.hand);
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }
        }
    }
}
