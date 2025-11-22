using System.Windows;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow
    /// </summary>
    public partial class LoginWindow : Window
    {
        private LoginViewModel ViewModel => DataContext as LoginViewModel;

        public LoginWindow()
        {
            // Explicitly qualify InitializeComponent to resolve ambiguity
            // If there are multiple partial classes for LoginWindow, ensure only one contains InitializeComponent
            this.InitializeComponent();
            DataContext = new LoginViewModel();

            // Subscribe to navigation event
            if (ViewModel != null)
            {
                ViewModel.NavigateToRegister += OnNavigateToRegister;
                ViewModel.NavigateToMain += OnNavigateToMain; // Changed from NavigateToDashboard
            }
        }

        /// <summary>
        /// Event handler untuk navigasi ke RegisterWindow
        /// </summary>
        private void OnNavigateToRegister(object sender, System.EventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Event handler untuk navigasi ke MainWindow
        /// </summary>
        private void OnNavigateToMain(object sender, System.EventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
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
                ViewModel.NavigateToRegister -= OnNavigateToRegister;
                ViewModel.NavigateToMain -= OnNavigateToMain; // Changed from NavigateToDashboard
            }
            base.OnClosed(e);
        }
    }
}