using System;
using System.Windows;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private RegisterViewModel ViewModel => DataContext as RegisterViewModel;

        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel();

            // Subscribe to navigation events
            if (ViewModel != null)
            {
                ViewModel.NavigateToLogin += OnNavigateToLogin;
                ViewModel.RegistrationSuccess += OnRegistrationSuccess;
            }
        }

        /// <summary>
        /// Event handler untuk navigasi ke LoginWindow
        /// </summary>
        private void OnNavigateToLogin(object sender, System.EventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Event handler untuk registrasi berhasil
        /// </summary>
        private void OnRegistrationSuccess(object sender, System.EventArgs e)
        {
            // Navigate to login after successful registration
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Event handler untuk drag window dengan mouse
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Event handler saat EmailTextBox mendapat focus
        /// </summary>
        private void EmailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsEmailFocused = true;
        }

        /// <summary>
        /// Event handler saat EmailTextBox kehilangan focus
        /// </summary>
        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsEmailFocused = false;
        }

        /// <summary>
        /// Event handler saat PasswordBox mendapat focus
        /// </summary>
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsPasswordFocused = true;
        }

        /// <summary>
        /// Event handler saat PasswordBox kehilangan focus
        /// </summary>
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.IsPasswordFocused = false;
        }

        /// <summary>
        /// Clean up event subscriptions
        /// </summary>
        protected override void OnClosed(System.EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.NavigateToLogin -= OnNavigateToLogin;
                ViewModel.RegistrationSuccess -= OnRegistrationSuccess;
            }
            base.OnClosed(e);
        }
    }
}
