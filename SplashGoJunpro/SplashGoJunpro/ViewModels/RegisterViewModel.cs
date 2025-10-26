using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SplashGoJunpro.Commands;
using SplashGoJunpro.Models;

namespace SplashGoJunpro.ViewModels
{
    /// <summary>
    /// ViewModel for RegisterWindow
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        private string _email;
        private string _password;
        private bool _rememberMe;
        private bool _isEmailFocused;
        private bool _isPasswordFocused;

        #region Properties

        /// <summary>
        /// Email address input
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    Debug.WriteLine($"Email changed: {value}");
                }
            }
        }

        /// <summary>
        /// Password input
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    Debug.WriteLine($"Password changed: {(string.IsNullOrEmpty(value) ? "empty" : "has value")}");
                }
            }
        }

        /// <summary>
        /// Remember me checkbox state
        /// </summary>
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        /// <summary>
        /// Email placeholder visibility
        /// </summary>
        public bool IsEmailFocused
        {
            get => _isEmailFocused;
            set
            {
                if (SetProperty(ref _isEmailFocused, value))
                {
                    Debug.WriteLine($"Email focus: {value}");
                }
            }
        }

        /// <summary>
        /// Password placeholder visibility
        /// </summary>
        public bool IsPasswordFocused
        {
            get => _isPasswordFocused;
            set
            {
                if (SetProperty(ref _isPasswordFocused, value))
                {
                    Debug.WriteLine($"Password focus: {value}");
                }
            }
        }

        #endregion

        #region Commands

        public ICommand SignUpCommand { get; }
        public ICommand GoogleSignInCommand { get; }
        public ICommand SignInCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Event untuk navigasi ke LoginWindow
        /// </summary>
        public event EventHandler NavigateToLogin;

        /// <summary>
        /// Event untuk menutup RegisterWindow setelah registrasi berhasil
        /// </summary>
        public event EventHandler RegistrationSuccess;

        #endregion

        #region Constructor

        public RegisterViewModel()
        {
            Debug.WriteLine("RegisterViewModel initialized");

            // Initialize commands
            SignUpCommand = new RelayCommand(ExecuteSignUp);
            GoogleSignInCommand = new RelayCommand(ExecuteGoogleSignIn);
            SignInCommand = new RelayCommand(ExecuteSignIn);
            CloseCommand = new RelayCommand(ExecuteClose);

            Debug.WriteLine("All commands initialized");
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Executes Sign Up logic
        /// </summary>
        private void ExecuteSignUp(object parameter)
        {
            Debug.WriteLine("ExecuteSignUp called");

            string email = Email?.Trim();
            string password = Password;

            // Validate empty fields
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter your password.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate email format
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate password strength
            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Replace with actual registration service
            // For now, simulate successful registration
            try
            {
                // Create new user
                var newUser = new User
                {
                    Email = email,
                    Password = password // Note: In production, hash the password!
                };

                MessageBox.Show($"Registration successful!\n\nWelcome to SplashGo!\nYou can now sign in with your email: {email}",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // TODO: Save user to database
                Debug.WriteLine($"User registered: {email}");

                // Trigger navigation to login
                RegistrationSuccess?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Registration error: {ex}");
            }
        }

        /// <summary>
        /// Executes Google Sign In logic
        /// </summary>
        private void ExecuteGoogleSignIn(object parameter)
        {
            MessageBox.Show("Google Sign-In feature will be implemented soon.\n\n" +
                "This will allow you to sign up using your Google account.",
                "Google Sign-In",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // TODO: Implement OAuth2 Google Sign-In
        }

        /// <summary>
        /// Executes Sign In navigation (redirect to login)
        /// </summary>
        private void ExecuteSignIn(object parameter)
        {
            Debug.WriteLine("Navigating to LoginWindow");
            NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        private void ExecuteClose(object parameter)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates email format
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

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

        #endregion
    }
}
