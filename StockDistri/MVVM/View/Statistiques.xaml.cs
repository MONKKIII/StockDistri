using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logique d'interaction pour Statistiques.xaml
    /// </summary>
    /// 
    public class Article
    {
        // Modèle de données pour un utilisateur
        public string CODART { get; set; } // Code Article
        public string LIBART { get; set; } // Libellé
        public string PA { get; set; } // Prix D'Achat
        public string STOCK { get; set; } // Stock
        public DateTime DATCRE { get; set; } // Date de création
        public DateTime DATMAJ { get; set; } // Date de mise à jour
    }

    public partial class Statistiques : Page
    {
        private DatabaseConnection databaseConnection; // Connexion à la base de données
        public Statistiques()
        {
            InitializeComponent();
            databaseConnection = new DatabaseConnection(); // Initialisation de la connexion à la base de données

            // Définition du contexte de liaison des données à une instance de la classe User
            DataContext = new Article();

            // Chargement des données utilisateur lors de l'initialisation ou de l'affichage de ParametreUsers
            LoadArticleData();
        }

        // Méthode pour charger les données utilisateur depuis la base de données
        private void LoadArticleData()
        {
            string query = $"SELECT * FROM stockbase.articles"; // Requête SQL pour récupérer les utilisateurs
            DataTable result = databaseConnection.ExecuteSelectQuery(query); // Exécution de la requête

            if (result != null && result.Rows.Count > 0)
            {
                // Effacer les éléments existants dans la ListView
                ArticleListView.Items.Clear();

                // Remplir la ListView avec les données du DataTable
                foreach (DataRow row in result.Rows)
                {
                    // Créer un objet Article à partir des données de la ligne
                    Article article = new Article
                    {
                        CODART = row["CODART"].ToString(),
                        LIBART = row["LIBART"].ToString(),
                        PA = row["PA"].ToString(),
                        STOCK = row["STOCK"].ToString(),
                        DATCRE = row["DATCRE"] != DBNull.Value ? Convert.ToDateTime(row["DATCRE"]) : DateTime.MinValue,
                        DATMAJ = row["DATMAJ"] != DBNull.Value ? Convert.ToDateTime(row["DATMAJ"]) : DateTime.MinValue
                    };

                    // Ajouter l'article à la ListView
                    ArticleListView.Items.Add(article);
                }
            }
            else
            {
                MessageBox.Show("Aucune donnée trouvée.");
            }
        }
    }
}
