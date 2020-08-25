using EngineeringProjectApp.Model;
using System.Collections.Generic;
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
        private List<UserModel> users = new List<UserModel>();
        private List<GameModel> games = new List<GameModel>();

        public Menu()
        {
            InitializeComponent();
            hand = "";
            returningFlag = true;
            amountOfBirds = 0;
            amountOfButterflies = 0;
            difficultyLevel = "";
            velocity = 0;
            users = SqliteDataAccess.LoadAllUsers();
            UserList.ItemsSource = users;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hand = HandComboBox.Text;
            returningFlag = ReturningCheckBox.IsChecked == true;
            amountOfBirds = int.Parse(AmountOfBirdsBox.Text);
            amountOfButterflies = int.Parse(AmountOfButterfliesBox.Text);
            difficultyLevel = DificultyLevelComboBox.Text;
            velocity = int.Parse(VelocityBox.Text);

            if (ValidArgument(amountOfBirds, amountOfButterflies) && ValidUser(UserList.SelectedItems.Count)){
                MainWindow mainWindow = new MainWindow(amountOfBirds, amountOfButterflies, hand, returningFlag, difficultyLevel, velocity, (UserModel)UserList.SelectedItems[0]);
                mainWindow.Show();
                Application.Current.Windows[0].Close();
            }            
        }

        private bool ValidArgument(int amountOfBirds, int amountOfButterflies)
        {
            if (amountOfButterflies + amountOfBirds == 0)
            {
                MessageBox.Show("Sumaryczna liczba zwierzątek musi być większa od zera", "Błąd!");
                return false;
            }
            return true;
        }

        private bool ValidUser(int amount)
        {
            if (amount ==0)
            {
                MessageBox.Show("Nie wybrano użytkownika", "Błąd!");
                return false;
            }
            return true;
        }

        private bool CheckNames(string firstName, string lastName)
        {
            if (firstName.Length == 0 || lastName.Length==0)
            {
                MessageBox.Show("Podano błędną nazwę użytkownika", "Błąd!");
                return false;
            }
            return true;
        }



        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CheckNames(FirstNameTextBox.Text, LastNameTextBox.Text))
            {
                UserModel u = new UserModel
                {
                    FirstName = FirstNameTextBox.Text,
                    LastName = LastNameTextBox.Text
                };
                SqliteDataAccess.SaveUser(u);
                users = SqliteDataAccess.LoadAllUsers();
                UserList.ItemsSource = users;
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CheckNames(FirstNameTextBox.Text, LastNameTextBox.Text) && ValidUser((UserList.SelectedItems.Count)))
            {
                UserModel prevUser = (UserModel)UserList.SelectedItems[0];
                UserModel u = new UserModel
                {
                    FirstName = FirstNameTextBox.Text,
                    LastName = LastNameTextBox.Text
                };
                
                SqliteDataAccess.EditUser(u,prevUser.Id);
                users = SqliteDataAccess.LoadAllUsers();
                UserList.ItemsSource = users;
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ValidUser((UserList.SelectedItems.Count)))
            {
                UserModel user = (UserModel)UserList.SelectedItems[0];
                SqliteDataAccess.DeleteUser(user.Id);
                users = SqliteDataAccess.LoadAllUsers();
                UserList.ItemsSource = users;
            }
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (ValidUser((UserList.SelectedItems.Count)))
            {
                UserModel user = (UserModel)UserList.SelectedItems[0];
                Statistics statisticsWindow = new Statistics(user.Id);
                statisticsWindow.Show();
            }
        }
    }

}
