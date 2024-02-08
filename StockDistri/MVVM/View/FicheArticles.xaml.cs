using System.Windows;
using System.Windows.Controls;
using static StockDistri.MVVM.View.FicheArticles;
namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour FicheArticles.xaml
    /// </summary>
    public partial class FicheArticles : Page
    {
        public FicheArticles()
        {
            InitializeComponent();
        }

        public void CloseFicheArticles_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
