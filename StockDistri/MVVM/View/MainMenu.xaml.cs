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

namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu(NavigationService navigationService)
        {
            InitializeComponent();
        }

        private void FicheArticles_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string buttonText = clickedButton.Content.ToString();

                // Ajoutez une logique pour identifier le bouton cliqué
                // Dans cet exemple, si le bouton "Fiche Articles" est cliqué, naviguez vers la page FicheArticles
                if (buttonText == "Fiche Articles")
                {
                    // Utilisez le Frame pour naviguer vers la page FicheArticles
                    NavigationService?.Navigate(new FicheArticles());
                }
                // Ajoutez d'autres conditions pour gérer d'autres boutons si nécessaire
            }
        }

        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }
    }
}
