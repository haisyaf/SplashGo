using System.Windows;
using System.Windows.Controls;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Services;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage : Page
    {
        private UserProfileViewModel _viewModel;

        public UserProfilePage()
        {
            InitializeComponent();
            _viewModel = new UserProfileViewModel();
            DataContext = _viewModel;

            // Load user profile
            _ = _viewModel.LoadUserProfile();
        }

        /// <summary>
        /// Event handler untuk edit profile button
        /// </summary>
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.OpenEditPopup();
        }

        /// <summary>
        /// Event handler untuk close popup button
        /// </summary>
        private void CloseEditPopup_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.CloseEditPopup();
        }

        /// <summary>
        /// Event handler untuk save profile button
        /// </summary>
        private async void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields using temp values
            if (string.IsNullOrWhiteSpace(_viewModel.TempDisplayName))
            {
                MessageBox.Show(
                    "Display name is required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.TempPhoneNumber))
            {
                MessageBox.Show(
                    "Phone number is required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.TempEmail))
            {
                MessageBox.Show(
                    "Email is required.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Validate email format
            if (!IsValidEmail(_viewModel.TempEmail))
            {
                MessageBox.Show(
                    "Please enter a valid email address.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Save profile using temp values
            await _viewModel.SaveProfile(
                _viewModel.TempDisplayName,
                _viewModel.TempPhoneNumber,
                _viewModel.TempEmail
            );
        }

        /// <summary>
        /// Validate email format
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}