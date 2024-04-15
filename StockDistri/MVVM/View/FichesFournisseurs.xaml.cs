using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockDistri.MVVM.View
{
    public partial class FichesFournisseurs : Page
    {
        private DatabaseConnection databaseConnection;

        public FichesFournisseurs()
        {
            InitializeComponent();
            databaseConnection = new DatabaseConnection();

            txtCodFou.TextChanged += TxtCodFou_TextChanged;
            txtCodFou.KeyDown += TxtCodFou_KeyDown;
        }

        private void TxtCodFou_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Limiter la saisie à 6 caractères
            if (txtCodFou.Text.Length > 6)
                txtCodFou.Text = txtCodFou.Text.Substring(0, 6);

            // Activer la saisie dans le champ "Libellé" si la longueur du code article est de 6
            txtNameFou.IsEnabled = (txtCodFou.Text.Length == 6);
        }

        private void TxtCodFou_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Si la longueur du texte est inférieure à 6, remplir automatiquement avec des zéros
                if (txtCodFou.Text.Length < 6)
                {
                    string zeros = new string('0', 6 - txtCodFou.Text.Length);
                    txtCodFou.Text = zeros + txtCodFou.Text;
                    txtCodFou.CaretIndex = txtCodFou.Text.Length; // Déplacer le curseur à la fin
                }
            }
        }

        private void CreateFournisseurs_Click(object sender, RoutedEventArgs e)
        {
            CreateValidate.Visibility = Visibility.Visible;
            txtNameFou.IsEnabled = true;
        }

        private void Valider_Click(object sender, RoutedEventArgs e)
        {
            string codeFournisseur = txtCodFou.Text;
            string libelle = txtNameFou.Text;

            // Vérifier si les champs sont remplis
            if (string.IsNullOrEmpty(txtCodFou.Text) || string.IsNullOrEmpty(txtNameFou.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CodeFournisseurExisteDeja(codeFournisseur))
            {
                MessageBox.Show("Le code fournisseur existe déjà. Veuillez en choisir un autre.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Vérifier si la longueur du nom du fournisseur est inférieure à 4 ou supérieure à 13 caractères
            if (libelle.Length < 4 || libelle.Length > 40)
            {
                MessageBox.Show("Le nom du fournisseur doit comporter entre 4 et 40 caractères.");
                return;
            }

            string query = $"INSERT INTO stockbase.fournisseurs (CODFOU, LIBFOU, DATCRE, DATMAJ)" +
                           $"VALUES ('{codeFournisseur}', '{libelle}', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)";

            try
            {
                databaseConnection.ExecuteQuery(query);
                // Afficher un message de succès
                MessageBox.Show("Le fournisseur a été ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                // Réinitialiser les champs après l'ajout du fournisseur
                txtCodFou.Text = string.Empty;
                txtNameFou.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite lors de l'ajout du fournisseur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CodeFournisseurExisteDeja(string codeFournisseur)
        {
            // Check if the codfou already exists in the database
            string query = $"SELECT COUNT(*) FROM stockbase.fournisseurs WHERE CODFOU = '{codeFournisseur}'";
            DataTable result = databaseConnection.ExecuteSelectQuery(query);

            if (result != null && result.Rows.Count > 0)
            {
                // If the count is greater than 0, codfou is taken
                return Convert.ToInt32(result.Rows[0][0]) > 0;
            }
            return false; // codfou is not taken
        }

        public void CloseFicheFournisseurs_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}