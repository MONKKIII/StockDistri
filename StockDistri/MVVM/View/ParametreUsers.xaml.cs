using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static StockDistri.MVVM.View.UserLogIn;

namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour ParametreUsers.xaml
    /// </summary>
    /// 
    public class User
    {
        // Modèle de données pour un utilisateur
        public string Username { get; set; } // Nom d'utilisateur
        public string Password { get; set; } // Mot de passe
        public string Role { get; set; } // Rôle
        public DateTime DatCre { get; set; } // Date de création
        public DateTime DatMaj { get; set; } // Date de mise à jour
    }

    public partial class ParametreUsers : Page
    {
        private DatabaseConnection databaseConnection; // Connexion à la base de données
        private NavigationService navigationService; // Service de navigation
        private User selectedUser; // Utilisateur sélectionné pour la modification

        // Constructeur
        public ParametreUsers(NavigationService navigationService)
        {
            InitializeComponent();
            databaseConnection = new DatabaseConnection(); // Initialisation de la connexion à la base de données
            this.navigationService = navigationService;

            // Définition du contexte de liaison des données à une instance de la classe User
            DataContext = new User();

            // Chargement des données utilisateur lors de l'initialisation ou de l'affichage de ParametreUsers
            LoadUserData();

            // Attacher un gestionnaire d'événements de changement de sélection à la ListView
            userListView.SelectionChanged += UserListView_SelectionChanged;
        }

        // Méthode pour charger les données utilisateur depuis la base de données
        private void LoadUserData()
        {
            string query = $"SELECT * FROM stockbase.utilisateurs"; // Requête SQL pour récupérer les utilisateurs
            DataTable result = databaseConnection.ExecuteSelectQuery(query); // Exécution de la requête

            if (result != null && result.Rows.Count > 0)
            {
                // Effacer les éléments existants dans la ListView
                userListView.Items.Clear();

                // Remplir la ListView avec les données du DataTable
                foreach (DataRow row in result.Rows)
                {
                    // Créer un objet User à partir des données de la ligne
                    User user = new User
                    {
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        Role = row["Role"].ToString(),
                        DatCre = row["DatCre"] != DBNull.Value ? Convert.ToDateTime(row["DatCre"]) : DateTime.MinValue,
                        DatMaj = row["DatMaj"] != DBNull.Value ? Convert.ToDateTime(row["DatMaj"]) : DateTime.MinValue
                    };

                    // Ajouter l'utilisateur à la ListView
                    userListView.Items.Add(user);
                }

                // Attacher un menu contextuel à chaque élément de la ListView
                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem();
                deleteMenuItem.Header = "Supprimer l'utilisateur";
                deleteMenuItem.Click += DeleteMenuItem_Click;
                contextMenu.Items.Add(deleteMenuItem);
                userListView.ContextMenu = contextMenu;
            }
            else
            {
                MessageBox.Show("Aucune donnée trouvée.");
            }
        }

        // Méthode appelée lors du clic sur le bouton "Enregistrer"
        private void ParaSaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Ajouter du code ici pour enregistrer les paramètres
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string selectedRole = ((ComboBoxItem)roleComboBox.SelectedItem).Tag.ToString();

            // Vérifier si le nom d'utilisateur contient des caractères invalides
            if (ContainsInvalidCharacters(username))
            {
                MessageBox.Show("Le nom d'utilisateur contient des caractères spéciaux. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Vérifier si la longueur du nom d'utilisateur est inférieure à 4 ou supérieure à 13 caractères
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

            // Vérifier si le nom d'utilisateur existe déjà
            if (IsUsernameTaken(username))
            {
                MessageBox.Show("Nom d'utilisateur déjà utilisé. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Hasher le mot de passe en utilisant un algorithme de hachage sécurisé (par exemple, SHA256)
            string hashedPassword = HashPassword(password);

            // Construire la requête SQL avec le mot de passe hashé
            string query = $"INSERT INTO stockbase.utilisateurs (username, password, role, DATCRE, DATMAJ)" +
                            $"VALUES ('{username}', '{hashedPassword}', {selectedRole}, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

            // Exécuter la requête SQL
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            // Recharger les données utilisateur après l'enregistrement
            LoadUserData();

            MessageBox.Show("Utilisateur ajouté!");

            // Réinitialiser les zones de texte après l'ajout de l'utilisateur
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
            roleComboBox.SelectedIndex = 0; // Réinitialiser la sélection du rôle

        }

        // Méthode pour vérifier si le nom d'utilisateur contient des caractères invalides
        private bool ContainsInvalidCharacters(string username)
        {
            // Définir une liste de caractères invalides
            char[] invalidCharacters = { '*', '$', '#', '@', '!', '/' }; // Ajouter plus de caractères si nécessaire

            // Vérifier si le nom d'utilisateur contient des caractères invalides
            return username.IndexOfAny(invalidCharacters) != -1;
        }

        // Méthode pour vérifier si le nom d'utilisateur est déjà pris
        private bool IsUsernameTaken(string username)
        {
            // Vérifier si le nom d'utilisateur existe déjà dans la base de données
            string query = $"SELECT COUNT(*) FROM stockbase.utilisateurs WHERE username = '{username}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // Si le nombre est supérieur à 0, le nom d'utilisateur est pris
                return Convert.ToInt32(result.Rows[0][0]) > 0;
            }
            return false; // Le nom d'utilisateur n'est pas pris
        }

        // Méthode pour hacher le mot de passe en utilisant SHA256
        public string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Convertir le mot de passe en octets
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir les octets hashés en une chaîne hexadécimale
                return BitConverter.ToString(hashedBytes).Replace("-", "");
            }
        }

        // Méthode appelée lors du clic sur l'élément de menu "Supprimer l'utilisateur"
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier si un seul élément est sélectionné
            if (userListView.SelectedItem != null)
            {
                // Demander confirmation
                MessageBoxResult confirmationResult = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet utilisateur?", "Confirmation", MessageBoxButton.YesNo);

                if (confirmationResult == MessageBoxResult.Yes)
                {
                    User selectedUser = (User)userListView.SelectedItem;
                    string query = $"DELETE FROM stockbase.utilisateurs WHERE Username = '{selectedUser.Username}'";

                    // Exécuter la requête SQL pour supprimer l'utilisateur sélectionné
                    DataTable deletionResult = databaseConnection.ExecuteSelectQuery(query);

                    // Recharger les données utilisateur après la suppression
                    LoadUserData();

                    MessageBox.Show("Utilisateur supprimé!");
                }
            }
            else
            {
                MessageBox.Show("Veuillez choisir un utilisateur à supprimer.");
            }
        }

        // Méthode appelée lors du changement de sélection dans la ListView
        private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtenir l'utilisateur sélectionné
            selectedUser = (User)userListView.SelectedItem;

            // Définir l'utilisateur sélectionné comme DataContext pour permettre la liaison de données dans XAML
            DataContext = selectedUser;

            // Activer les contrôles d'édition (par exemple, zones de texte) pour l'utilisateur sélectionné
            txtUsername.IsEnabled = true;
            txtPassword.IsEnabled = true;
        }

        // Méthode appelée lors du clic sur le bouton "Valider les modifications"
        private void ValidateModify_Click(object sender, RoutedEventArgs e)
        {
            // Assurer qu'un utilisateur est sélectionné pour l'édition
            if (selectedUser == null)
            {
                MessageBox.Show("Veuillez sélectionner un utilisateur à modifier.");
                return;
            }

            string newUsername = txtUsername.Text;
            string password = txtPassword.Password;
            string selectedRole = ((ComboBoxItem)roleComboBox.SelectedItem).Tag.ToString();

            // Vérifier si le nouveau nom d'utilisateur contient des caractères invalides ou est trop court/long
            if (ContainsInvalidCharacters(newUsername) || newUsername.Length < 4 || newUsername.Length > 13)
            {
                MessageBox.Show("Le nom d'utilisateur doit comporter entre 4 et 13 caractères et ne doit pas contenir de caractères spéciaux.");
                return;
            }

            // Vérifier si le nouveau nom d'utilisateur est déjà pris
            if (IsUsernameTaken(newUsername))
            {
                MessageBox.Show("Nom d'utilisateur déjà utilisé. Choisissez un autre nom d'utilisateur.");
                return;
            }

            // Mettre à jour le nom d'utilisateur dans la base de données
            string query = $"UPDATE stockbase.utilisateurs SET username = '{newUsername}', " +
                           $"password = '{password}', " +
                           $"role = {selectedRole}," +
                           $"DATMAJ = CURRENT_TIMESTAMP WHERE Username = '{selectedUser.Username}'";

            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            // Recharger les données utilisateur après l'enregistrement
            LoadUserData();

            MessageBox.Show("Nom d'utilisateur modifié!");
        }
    }
}
