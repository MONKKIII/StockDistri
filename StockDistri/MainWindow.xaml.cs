using StockDistri.MVVM.View;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using static StockDistri.MVVM.View.UserLogIn;

namespace StockDistri
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Username { get; set; }

        private NavigationService navigationService;

        public MainWindow()
        {
            InitializeComponent();

            mainFrame.Navigate(new UserLogIn(mainFrame.NavigationService));

            StateChanged += MainWindow_StateChanged;

            Username = DataHolder.Username;

            // Gérer l'événement Loaded pour la mise à jour de l'interface utilisateur
            Loaded += MainWindow_Loaded;

            this.WindowState = WindowState.Normal;
        }

        // Constructeur avec paramètre pour le nom d'utilisateur
        public MainWindow(string username) : this()
        {
            Username = username;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            // Vérifiez si la fenêtre est en mode maximisé
            if (WindowState == WindowState.Maximized)
            {
                // Si la fenêtre est maximisée, définissez une marge supérieure pour éviter que les contrôles ne soient cachés
                BorderMainState.Margin = new Thickness(5, 5, 5, 0);
                BorderNavigation.Margin = new Thickness(6, 0, 5, 0);
            }
            else
            {
                // Si la fenêtre n'est pas maximisée, supprimez la marge supérieure
                BorderMainState.Margin = new Thickness(0);
                BorderNavigation.Margin = new Thickness(0);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Mettez à jour le contenu du Label avec le nom de l'utilisateur une fois que la fenêtre est chargée
            UserLabel.Content = $"Utilisateur : {DataHolder.Username}";

            // Update the visibility of BorderNavigation based on user login status
            BorderNavigation.Visibility = string.IsNullOrEmpty(DataHolder.Username) ? Visibility.Collapsed : Visibility.Visible;
        }

        // Méthode de détermination de l'état de connexion de l'utilisateur (à adapter selon votre logique)
        private bool UserIsLoggedIn()
        {
            // Vous devez implémenter la logique de vérification de l'état de connexion ici
            // Par exemple, vous pourriez vérifier si le nom d'utilisateur est défini, etc.
            return !string.IsNullOrEmpty(Username);
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ReturnMenu_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new MainMenu(mainFrame.NavigationService));
        }

        private void UserMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)userMenu.SelectedItem;
            if (selectedItem != null)
            {
                string pageName = selectedItem.Tag.ToString();
                switch (pageName)
                {
                    case "LogOut":
                        // Effectuer les actions de déconnexion nécessaires ici, telles que vider les informations d'identification de l'utilisateur, réinitialiser l'état de connexion, etc.
                        // Par exemple :
                        DataHolder.Username = null;
                        UpdateUsernameLabel(); // Mettre à jour l'état de connexion dans l'interface utilisateur
                        mainFrame.Navigate(new UserLogIn(mainFrame.NavigationService)); // Naviguer vers la page de connexion
                        break;
                    case "ListUsers":
                        mainFrame.Navigate(new ParametreUsers(mainFrame.NavigationService));
                        break;
                        // Ajoutez des cas pour d'autres pages si nécessaire
                }
            }

            ComboBox comboBox = sender as ComboBox;
            comboBox.SelectedItem = null; // Réinitialiser la sélection de la ComboBox
        }

        private void UserMenu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ouvrir la fenêtre PasswordWindow pour demander le mot de passe
            PasswordWindow passwordWindow = new PasswordWindow();
            // Abonnez-vous à l'événement PasswordConfirmed
            passwordWindow.PasswordConfirmed += PasswordWindow_PasswordConfirmed;
            // Ouvrir la fenêtre modale de manière modale
            bool? result = passwordWindow.ShowDialog();

            // Empêcher la propagation de l'événement pour éviter le comportement par défaut du ComboBox
            e.Handled = true;
        }

        private void PasswordWindow_PasswordConfirmed(bool isPasswordCorrect)
        {
            if (isPasswordCorrect)
            {
                // Le mot de passe est correct, continuer avec l'exécution du programme
                // Par exemple, naviguer vers une page spécifique
                mainFrame.Navigate(new ParametreUsers(navigationService));
            }
            else
            {
                // Le mot de passe est incorrect, afficher un message d'erreur
                MessageBox.Show("Mot de passe incorrect.");
            }
        }

        public void UpdateUsernameLabel()
        {
            // Mettez à jour le contenu du Label avec le nom de l'utilisateur
            UserLabel.Content = $"Utilisateur : {DataHolder.Username}";

            // Update the visibility of BorderNavigation based on user login status
            BorderNavigation.Visibility = string.IsNullOrEmpty(DataHolder.Username) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
