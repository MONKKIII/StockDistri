using System;
using System.Windows;
using System.Windows.Input;
namespace StockDistri.MVVM.View
{
    /// <summary>
    /// Logique d'interaction pour UserLogIn.xaml
    /// </summary>
    public partial class UserLogIn : Window
    {
        public UserLogIn()
        {
            InitializeComponent();
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

        private void ValidateLogIn_Click(object sender, RoutedEventArgs e)
        {
            // Add any validation or logic here if needed

            // Close the UserLogIn window
            this.Close();
        }
    }
}
