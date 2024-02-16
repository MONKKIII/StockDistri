﻿using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour ParametreUsers.xaml
    /// </summary>
    /// 
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Administrator { get; set; }
        public DateTime DatCre { get; set; }
        public DateTime DatMaj { get; set; }
    }

    public partial class ParametreUsers : Page
    {
        private DatabaseConnection databaseConnection;
        private NavigationService navigationService;
        private User selectedUser;

        public ParametreUsers(NavigationService navigationService)
        {
            InitializeComponent();
            databaseConnection = new DatabaseConnection();
            this.navigationService = navigationService;

            // Set DataContext to an instance of your User class
            DataContext = new User();

            // Load user data when the ParametreUsers is initialized or shown
            LoadUserData();

            // Attach a selection changed event handler to the ListView
            userListView.SelectionChanged += UserListView_SelectionChanged;
        }

        private void LoadUserData()
        {
            string query = "SELECT * FROM utilisateurs";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // Clear existing items in the ListView
                userListView.Items.Clear();

                // Populate the ListView with the data from the DataTable
                foreach (DataRow row in result.Rows)
                {
                    User user = new User
                    {
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        Administrator = row["Administrateur"] != DBNull.Value && Convert.ToBoolean(row["Administrateur"]),
                        DatCre = row["DatCre"] != DBNull.Value ? Convert.ToDateTime(row["DatCre"]) : DateTime.MinValue,
                        DatMaj = row["DatMaj"] != DBNull.Value ? Convert.ToDateTime(row["DatMaj"]) : DateTime.MinValue
                    };

                    // Add the user to the ListView
                    userListView.Items.Add(user);
                }

                // Attach a context menu to each item in the ListView
                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem();
                deleteMenuItem.Header = "Delete User";
                deleteMenuItem.Click += DeleteMenuItem_Click;
                contextMenu.Items.Add(deleteMenuItem);
                userListView.ContextMenu = contextMenu;
            }
            else
            {
                MessageBox.Show("Aucune donnée trouvé.");
            }
        }

        private void ParaSaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Add code here to save the settings
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            // Check if the username contains invalid characters
            if (ContainsInvalidCharacters(username))
            {
                MessageBox.Show("Le nom d'utilisateur contient des caractères spéciaux. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Check if the username length is less than 4 or greater than 13 characters
            if (username.Length < 4 || username.Length > 13)
            {
                MessageBox.Show("Le nom d'utilisateur doit comporter entre 4 et 13 caractères.");
                return;
            }

            if (password.Length < 5 || password.Length > 13)
            {
                MessageBox.Show("Le mot de passe doit comporter entre 5 et 13 caractères.");
                return;
            }

            // Check if the username already exists
            if (IsUsernameTaken(username))
            {
                MessageBox.Show("Nom d'utilisateur déjà utilisé. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Hash the password using a secure hashing algorithm (e.g., SHA256)
            string hashedPassword = HashPassword(password);

            // Construct the SQL query with the hashed password
            string query = $"INSERT INTO utilisateurs (username, password, DATCRE, DATMAJ)" +
                            $"VALUES ('{username}', '{hashedPassword}', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

            // Execute the SQL query
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            // Reload user data after saving
            LoadUserData();

            MessageBox.Show("Utilisateur ajouté!");
        }

        private bool ContainsInvalidCharacters(string username)
        {
            // Define a list of invalid characters
            char[] invalidCharacters = { '*', '$', '#', '@', '!', '/' }; // Add more characters as needed

            // Check if the username contains any invalid characters
            return username.IndexOfAny(invalidCharacters) != -1;
        }

        private bool IsUsernameTaken(string username)
        {
            // Check if the username already exists in the database
            string query = $"SELECT COUNT(*) FROM utilisateurs WHERE username = '{username}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // If the count is greater than 0, username is taken
                return Convert.ToInt32(result.Rows[0][0]) > 0;
            }
            return false; // Username is not taken
        }

        // Helper method to hash the password using SHA256
        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Convert the password to bytes
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the hashed bytes to a hexadecimal string
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Check if a single item is selected
            if (userListView.SelectedItem != null)
            {
                // Ask for confirmation
                MessageBoxResult confirmationResult = MessageBox.Show("Êtes vous sûr de vouloir supprimé cet utilisateur?", "Confirmation", MessageBoxButton.YesNo);

                if (confirmationResult == MessageBoxResult.Yes)
                {
                    User selectedUser = (User)userListView.SelectedItem;
                    string query = $"DELETE FROM utilisateurs WHERE Username = '{selectedUser.Username}'";

                    // Execute the SQL query to delete the selected user
                    DataTable deletionResult = databaseConnection.ExecuteSelectQuery(query);

                    // Reload user data after deletion
                    LoadUserData();

                    MessageBox.Show("Utilisateur supprimé!");
                }
            }
            else
            {
                MessageBox.Show("s'il vous plaît, choisissez un utilisateur à supprimé.");
            }
        }

        private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected user
            selectedUser = (User)userListView.SelectedItem;

            // Set the selected user as DataContext to enable data binding in XAML
            DataContext = selectedUser;

            // Enable editing controls (e.g., textboxes) for the selected user
            txtUsername.IsEnabled = true;
            txtPassword.IsEnabled = true;
        }

        private void ValidateModify_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a user is selected for editing
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to edit.");
                return;
            }

            // Update the selected user's username
            string newUsername = txtUsername.Text;

            // Check if the new username contains invalid characters or is too short/long
            if (ContainsInvalidCharacters(newUsername) || newUsername.Length < 4 || newUsername.Length > 13)
            {
                MessageBox.Show("Le nom d'utilisateur doit comporter entre 4 et 13 caractères et ne doit pas contenir de caractères spéciaux.");
                return;
            }

            // Check if the new username is already taken
            if (IsUsernameTaken(newUsername))
            {
                MessageBox.Show("Nom d'utilisateur déjà utilisé. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Update the user's username in the database
            string query = $"UPDATE utilisateurs SET username = '{newUsername}', " +
                           $"DATMAJ = CURRENT_TIMESTAMP WHERE Username = '{selectedUser.Username}'";

            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            // Reload user data after saving
            LoadUserData();

            MessageBox.Show("Nom d'utilisateur modifié!");
        }
    }
}
