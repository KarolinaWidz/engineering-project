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
        bool returningFlag;
        int amountOfBirds;
        int amountOfButterflies;
        string difficultyLevel;
        int velocity;
        public Menu()
        {
            InitializeComponent();
            this.hand = "";
            this.returningFlag = true;
            this.amountOfBirds = 0;
            this.amountOfButterflies = 0;
            this.difficultyLevel = "";
            this.velocity = 0;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.hand = HandComboBox.Text;
            this.returningFlag = ReturningCheckBox.IsChecked == true;
            this.amountOfBirds = int.Parse(AmountOfBirdsBox.Text);
            this.amountOfButterflies = int.Parse(AmountOfButterfliesBox.Text);
            this.difficultyLevel = DificultyLevelComboBox.Text;
            this.velocity = int.Parse(VelocityBox.Text);


            if (ValidArgument(amountOfBirds, amountOfButterflies)){
                MainWindow mainWindow = new MainWindow(this.amountOfBirds, this.amountOfButterflies, this.hand, this.returningFlag, this.difficultyLevel, this.velocity);
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }
            
        }

        private bool ValidArgument(int amountOfBirds, int amountOfButterflies) {
            if (amountOfButterflies + amountOfBirds == 0) {
                MessageBox.Show("Sumaryczna liczba zwierzątek musi być różna od zera", "Błąd!");
                return false;
            }
            return true;
        }
    }
}
