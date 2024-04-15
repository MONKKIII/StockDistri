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
using System.Windows.Shapes;

namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour PasswordWindow.xaml
    /// </summary>
    public partial class PasswordWindow : Window
    {
        public event Action<bool> PasswordConfirmed;

        public PasswordWindow()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            string password = PasswordBox.Password;

            // Vérifier si le mot de passe est correct
            if (password == "Admin99!")
            {
                // Mot de passe correct
                PasswordConfirmed?.Invoke(true);
                Close(); // Fermer la fenêtre
            }
            else
            {
                // Mot de passe incorrect
                MessageBox.Show("Mot de passe incorrect.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
