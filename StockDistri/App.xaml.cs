using StockDistri.MVVM.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StockDistri
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        // This method is called when the application starts
        protected override void OnStartup(StartupEventArgs e)
        {
            // Call the base class method first
            base.OnStartup(e);

            // Create an instance of the MainWindow
            MainWindow mainWindow = new MainWindow();

            // Disable interactions with the MainWindow initially
            mainWindow.IsEnabled = false;

            // Create an instance of the UserLogIn window
            UserLogIn loginWindow = new UserLogIn();

            // Show the UserLogIn window
            loginWindow.Show();

            // Bring the UserLogIn window to the front
            loginWindow.Activate();
        }
    }
}
