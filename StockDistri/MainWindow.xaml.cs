using StockDistri.MVVM.View;
using System;
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

        private void ParametreUsers_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ParametreGeneraux(mainFrame.NavigationService));
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
