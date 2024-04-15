using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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

        private void FicheFournisseurs_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string buttonText = clickedButton.Content.ToString();

                // Ajoutez une logique pour identifier le bouton cliqué
                // Dans cet exemple, si le bouton "Fiche Fournisseurs" est cliqué, naviguez vers la page FichesFournisseurs
                if (buttonText == "Fiche Fournisseurs")
                {
                    // Utilisez le Frame pour naviguer vers la page FichesFournisseurs
                    NavigationService?.Navigate(new FichesFournisseurs());
                }
                // Ajoutez d'autres conditions pour gérer d'autres boutons si nécessaire
            }
        }
        private void Statistiques_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                string buttonText = clickedButton.Content.ToString();

                // Ajoutez une logique pour identifier le bouton cliqué
                // Dans cet exemple, si le bouton "Statistiques" est cliqué, naviguez vers la page Statistiques
                if (buttonText == "Statistiques")
                {
                    // Utilisez le Frame pour naviguer vers la page Statistiques
                    NavigationService?.Navigate(new Statistiques());
                }
                // Ajoutez d'autres conditions pour gérer d'autres boutons si nécessaire
            }
        }
    }
}
