using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace EngineeringProjectApp
{
    /// <summary>
    /// Logika interakcji dla klasy Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {

        private string hand;
        private bool returningFlag;
        private int amountOfBirds;
        private int amountOfButterflies;
        private string difficultyLevel;
        private int velocity;
      //  private Database databaseObject;

        public Menu()
        {
            InitializeComponent();
            this.hand = "";
            this.returningFlag = true;
            this.amountOfBirds = 0;
            this.amountOfButterflies = 0;
            this.difficultyLevel = "";
            this.velocity = 0;
            //DatabaseController();
        }


        private void DatabaseController()
        {
           // databaseObject = new Database();

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
