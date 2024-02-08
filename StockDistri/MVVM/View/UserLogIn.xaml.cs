using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;
using System.Windows.Navigation;
using static StockDistri.MVVM.View.ParametreUsers;
using System.Diagnostics;

namespace StockDistri.MVVM.View
{
    public partial class UserLogIn : Page
    {
        private DatabaseConnection databaseConnection;
        private NavigationService navigationService;
        private MainWindow mainWindow; // Add this

        public static bool IsUserLoggedIn { get; private set; }

        public UserLogIn(NavigationService navigationService)
        {
            InitializeComponent();
            this.navigationService = navigationService;
            this.mainWindow = mainWindow; // Initialize the reference to MainWindow
            databaseConnection = new DatabaseConnection();

            // Pass the NavigationService instance to ParametreUsers constructor
            ParametreUsers ParametreUsersInstance = new ParametreUsers(navigationService);
        }

        public string Username { get; internal set; }

        private bool AuthenticateUser(string username, string enteredPassword, ParametreUsers ParametreUsers)
        {
            string query = $"SELECT Password FROM utilisateurs WHERE Username = '{username}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // Retrieve the hashed password from the database
                string storedHashedPassword = result.Rows[0]["Password"].ToString();

                Debug.WriteLine($"Stored Hashed Password: {storedHashedPassword}");

                // Hash the entered password using the same method used during registration
                string enteredHashedPassword = ParametreUsers.HashPassword(enteredPassword);

                Debug.WriteLine($"Entered Hashed Password: {enteredHashedPassword}");

                // Compare the hashed passwords
                return storedHashedPassword == enteredHashedPassword;
            }

            Debug.WriteLine("User not found");
            return false; // User not found
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public static class DataHolder
        {
            public static string Username { get; set; }
        }

        private void ValidateLogIn_Click(object sender, RoutedEventArgs e)
        {
            // Assuming you have a reference to ParametreUsers instance, replace 'ParametreUsersInstance' with the actual instance
            ParametreUsers ParametreUsersInstance = new ParametreUsers(navigationService);

            string username = txtUsername.Text;
            string enteredPassword = txtPassword.Password;

            // Hash the entered password using the same method used during registration
            string enteredHashedPassword = ParametreUsersInstance.HashPassword(enteredPassword);

            if (AuthenticateUser(username, enteredPassword, ParametreUsersInstance))
            {
                // User is authenticated
                DataHolder.Username = username; // Update the username in DataHolder

                // Get the current instance of MainWindow
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

                // Update the username label and navigation visibility in MainWindow
                mainWindow.UpdateUsernameLabel();

                // Navigate to the MainMenu
                NavigationService?.Navigate(new MainMenu(navigationService));
            }
            else
            {
                MessageBox.Show("Nom d'utilisateur ou mot de passe non valide.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
