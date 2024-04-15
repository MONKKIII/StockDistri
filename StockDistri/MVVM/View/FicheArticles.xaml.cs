using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

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

            // Si le texte de txtCodArt est vide, masquez le bouton ModifyArticles, sinon affichez-le
            ModifyArticles.Visibility = string.IsNullOrEmpty(txtCodArt.Text) ? Visibility.Collapsed : Visibility.Visible;
            // Si le texte de txtCodArt est vide, masquez le bouton ModifyArticles, sinon affichez-le
            DeleteArticles.Visibility = string.IsNullOrEmpty(txtCodArt.Text) ? Visibility.Collapsed : Visibility.Visible;
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

                // Fetch and display the article label
                string codeArticle = txtCodArt.Text;
                string query = $"SELECT LIBART, PA, STOCK, DATCRE, DATMAJ FROM stockbase.articles WHERE CODART = '{codeArticle}'";

                try
                {
                    DataTable result = databaseConnection.ExecuteSelectQuery(query);
                    if (result != null && result.Rows.Count > 0)
                    {
                        // Récupérer les valeurs de la base de données
                        string libelle = result.Rows[0]["LIBART"].ToString();
                        string PrixAchat = result.Rows[0]["PA"].ToString();
                        string Stock = result.Rows[0]["STOCK"].ToString();
                        DateTime dateCreation = Convert.ToDateTime(result.Rows[0]["DATCRE"]);
                        DateTime dateMiseAJour = Convert.ToDateTime(result.Rows[0]["DATMAJ"]);

                        // Afficher les valeurs dans les champs correspondants
                        txtLibelle.Text = libelle;
                        txtPA.Text = PrixAchat;
                        txtStock.Text = Stock;

                        // Convertir les dates en chaînes de caractères formatées
                        string dateCreationFormatted = dateCreation.ToString("dd/MM/yyyy HH:mm:ss");
                        string dateMiseAJourFormatted = dateMiseAJour.ToString("dd/MM/yyyy HH:mm:ss");

                        // Afficher les dates dans les champs correspondants
                        txtDateCre.Text = dateCreationFormatted;
                        txtDateMaj.Text = dateMiseAJourFormatted;
                    }
                    else
                    {
                        txtLibelle.Text = ""; // Clear the text if no matching article found
                        txtPA.Text = ""; // Clear the text if no matching article found
                        txtStock.Text = ""; // Clear the text if no matching article found
                        txtDateMaj.Text = ""; // Clear the text if no matching article found
                        txtDateCre.Text = ""; // Clear the text if no matching article found
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la récupération du libellé : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CreateArticles_Click(object sender, RoutedEventArgs e)
        {
            ValiderCreate.Visibility = Visibility.Visible;
            txtLibelle.IsEnabled = true;
            txtPA.IsEnabled = true;
            txtStock.IsEnabled = true;

            txtCodArt.Text = string.Empty;
            txtLibelle.Text = string.Empty;
            txtPA.Text = string.Empty;
            txtStock.Text = string.Empty;
            txtDateMaj.Text = string.Empty;
            txtDateCre.Text = string.Empty;
        }

        private void ValiderCreate_Click(object sender, RoutedEventArgs e)
        {
            string codeArticle = txtCodArt.Text;
            string libelle = txtLibelle.Text;
            string PrixAchat = txtPA.Text;
            string Stock = txtStock.Text;

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

            // Vérifier si la longueur du nom de l'article est inférieure à 4 ou supérieure à 13 caractères
            if (libelle.Length < 5 || libelle.Length > 60)
            {
                MessageBox.Show("Le nom de l'article doit comporter entre 5 et 60 caractères.");
                return;
            }

            string query = $"INSERT INTO stockbase.articles (CODART, LIBART, PA, STOCK, DATCRE, DATMAJ)" +
                           $"VALUES ('{codeArticle}', '{libelle}', '{PrixAchat}', '{Stock}', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

            try
            {
                databaseConnection.ExecuteQuery(query);
                // Afficher un message de succès
                MessageBox.Show("L'article a été ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Réinitialiser les champs après l'ajout de l'article
                txtCodArt.Text = string.Empty;
                txtLibelle.Text = string.Empty;
                txtPA.Text = string.Empty;
                txtStock.Text = string.Empty;
                txtDateMaj.Text = string.Empty;
                txtDateCre.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de l'ajout de l'article : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ValiderCreate.Visibility = Visibility.Collapsed;
            txtLibelle.IsEnabled = false;
            txtPA.IsEnabled = false;
            txtStock.IsEnabled = false;
        }

        private void ValiderModify_Click(object sender, RoutedEventArgs e)
        {
            string codeArticle = txtCodArt.Text;
            string newLibelle = txtLibelle.Text;
            string PrixAchat = txtPA.Text;
            string Stock = txtStock.Text;

            // Vérifiez si les champs sont remplis
            if (string.IsNullOrEmpty(codeArticle) || string.IsNullOrEmpty(newLibelle))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifiez si la longueur du libellé est valide
            if (newLibelle.Length < 5 || newLibelle.Length > 60)
            {
                MessageBox.Show("Le libellé doit comporter entre 5 et 60 caractères.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Construisez la requête SQL pour mettre à jour l'article dans la base de données
            string query = $"UPDATE stockbase.articles SET LIBART = '{newLibelle}', PA = '{PrixAchat}', STOCK = '{Stock}', DATMAJ = CURRENT_TIMESTAMP WHERE CODART = '{codeArticle}'";

            try
            {
                // Exécutez la requête SQL
                databaseConnection.ExecuteQuery(query);

                // Affichez un message de succès
                MessageBox.Show("L'article a été mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Réinitialisez les champs après la mise à jour de l'article
                txtCodArt.Text = string.Empty;
                txtLibelle.Text = string.Empty;
                txtPA.Text = string.Empty;
                txtStock.Text = string.Empty;
                txtDateMaj.Text = string.Empty;
                txtDateCre.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Affichez un message d'erreur en cas d'échec de la mise à jour
                MessageBox.Show($"Une erreur s'est produite lors de la mise à jour de l'article : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ValiderModify.Visibility = Visibility.Collapsed;
            txtLibelle.IsEnabled = false;
            txtPA.IsEnabled = false;
            txtStock.IsEnabled = false;
        }

        private bool CodeArticleExisteDeja(string codeArticle)
        {
            // Check if the username already exists in the database
            string query = $"SELECT COUNT(*) FROM stockbase.articles WHERE CODART = '{codeArticle}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // If the count is greater than 0, username is taken
                return Convert.ToInt32(result.Rows[0][0]) > 0;
            }
            return false; // Username is not taken
        }

        private bool IsArticleSelected()
        {
            // Implement your logic to check if an article is selected
            // For example, you could check if a certain field (like article code) is filled or if a specific item is selected in a list/grid
            return !string.IsNullOrEmpty(txtCodArt.Text);
        }

        private void ModifyArticles_Click(object sender, RoutedEventArgs e)
        {
            // Check if an article is selected
            if (IsArticleSelected())
            {
                // Enable editing mode
                EnableEditMode();
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un article à modifier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnableEditMode()
        {
            // Enable editing mode by showing relevant UI elements
            txtLibelle.IsEnabled = true; // Enable the textbox for modifying the article label
            txtPA.IsEnabled = true; // Enable the textbox for modifying the article price
            txtStock.IsEnabled = true; // Enable the textbox for modifying the article stock
            ValiderModify.Visibility = Visibility.Visible; // Show the "Validate" button for saving modifications
        }

        private void DeleteArticle_Click(object sender, RoutedEventArgs e)
        {
            // Vérifiez d'abord si un article est sélectionné
            if (IsArticleSelected())
            {
                // Demandez confirmation à l'utilisateur avant de supprimer l'article
                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet article ?", "Confirmation de suppression", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Récupérez le code de l'article sélectionné
                    string codeArticle = txtCodArt.Text;

                    // Construisez la requête SQL pour supprimer l'article de la base de données
                    string query = $"DELETE FROM stockbase.articles WHERE CODART = '{codeArticle}'";

                    try
                    {
                        // Exécutez la requête SQL pour supprimer l'article
                        databaseConnection.ExecuteQuery(query);

                        // Affichez un message de succès
                        MessageBox.Show("L'article a été supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Réinitialisez les champs après la suppression de l'article
                        txtCodArt.Text = string.Empty;
                        txtLibelle.Text = string.Empty;
                        txtPA.Text = string.Empty;
                        txtStock.Text = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        // Affichez un message d'erreur en cas d'échec de la suppression
                        MessageBox.Show($"Une erreur s'est produite lors de la suppression de l'article : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un article à supprimer.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CloseFicheArticles_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}