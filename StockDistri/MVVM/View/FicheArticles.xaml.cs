using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockDistri.MVVM.View
{
    public partial class FicheArticles : Page
    {
        private DatabaseConnection databaseConnection;

        public FicheArticles()
        {
            InitializeComponent();
            databaseConnection = new DatabaseConnection();

            txtCodArt.TextChanged += TxtCodArt_TextChanged;
            txtCodArt.KeyDown += TxtCodArt_KeyDown;
        }

        private void TxtCodArt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Limiter la saisie à 8 caractères
            if (txtCodArt.Text.Length > 8)
                txtCodArt.Text = txtCodArt.Text.Substring(0, 8);

            // Activer la saisie dans le champ "Libellé" si la longueur du code article est de 8
            txtLibelle.IsEnabled = (txtCodArt.Text.Length == 8);
        }

        private void TxtCodArt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Si la longueur du texte est inférieure à 8, remplir automatiquement avec des zéros
                if (txtCodArt.Text.Length < 8)
                {
                    string zeros = new string('0', 8 - txtCodArt.Text.Length);
                    txtCodArt.Text = zeros + txtCodArt.Text;
                    txtCodArt.CaretIndex = txtCodArt.Text.Length; // Déplacer le curseur à la fin
                }
            }
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            string codeArticle = txtCodArt.Text;
            string libelle = txtLibelle.Text;

            // Vérifier si les champs sont remplis
            if (string.IsNullOrEmpty(txtCodArt.Text) || string.IsNullOrEmpty(txtLibelle.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CodeArticleExisteDeja(codeArticle))
            {
                MessageBox.Show("Le code article existe déjà. Veuillez en choisir un autre.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string query = $"INSERT INTO articles (CODART, LIBART, DATCRE, DATMAJ)" +
                           $"VALUES ('{codeArticle}', '{libelle}', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

            try
            {
                databaseConnection.ExecuteQuery(query);
                // Afficher un message de succès
                MessageBox.Show("L'article a été ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Réinitialiser les champs après l'ajout de l'article
                txtCodArt.Text = string.Empty;
                txtLibelle.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de l'ajout de l'article : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CodeArticleExisteDeja(string codeArticle)
        {
            // Check if the username already exists in the database
            string query = $"SELECT COUNT(*) FROM articles WHERE CODART = '{codeArticle}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // If the count is greater than 0, username is taken
                return Convert.ToInt32(result.Rows[0][0]) > 0;
            }
            return false; // Username is not taken
        }

        public void CloseFicheArticles_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}