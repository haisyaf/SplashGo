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

            // Update login/logout button based on session status
            UpdateLoginLogoutButton();

            // Navigate to Dashboard by default
            MainFrame.Navigate(new DashboardPage());
        }

        /// <summary>
        /// Update tombol Login/Logout berdasarkan status login
        /// </summary>
        private void UpdateLoginLogoutButton()
        {
            if (SessionManager.IsLoggedIn)
            {
                // Tampilkan sebagai Logout
                LoginLogoutImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("/Images/logout.png", UriKind.Relative));
                LoginLogoutText.Text = "Logout";
            }
            else
            {
                // Tampilkan sebagai Login
                LoginLogoutImage.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new Uri("/Images/login.png", UriKind.Relative));
                LoginLogoutText.Text = "Login";
            }
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
            //MessageBox.Show("History page is under construction.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            MainFrame.Navigate(new TransactionHistoryPage());
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
            MainFrame.Navigate(new MyDestinationsPage());
        }

        /// <summary>
        /// Event handler untuk navigasi ke Profile
        /// </summary>
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UserProfilePage());
        }

        /// <summary>
        /// Event handler untuk Login/Logout
        /// </summary>
        private void LoginLogout_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.IsLoggedIn)
            {
                // Logout logic
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
            else
            {
                // Login logic - navigate to login window
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}