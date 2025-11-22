using System;
using System.Windows;
using SplashGoJunpro.Views;
using SplashGoJunpro.Services;

namespace SplashGoJunpro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigate to Dashboard by default
            MainFrame.Navigate(new DashboardPage());
        }

        /// <summary>
        /// Event handler untuk navigasi ke Dashboard
        /// </summary>
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        /// <summary>
        /// Event handler untuk navigasi ke History
        /// </summary>
        private void History_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("History page is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Event handler untuk navigasi ke Saved
        /// </summary>
        private void Saved_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Saved page is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Event handler untuk navigasi ke Add Business
        /// </summary>
        private void AddBusiness_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Business page is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Event handler untuk navigasi ke Profile
        /// </summary>
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Profile page is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Event handler untuk Logout
        /// </summary>
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // Clear session
                SessionManager.IsLoggedIn = false;
                SessionManager.CurrentUserEmail = null;
                SessionManager.LoginToken = null;

                // Navigate to Login
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}