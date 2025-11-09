using BCrypt.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using SplashGoJunpro.Commands;
using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SplashGoJunpro.ViewModels
{
    /// <summary>
    /// ViewModel for LoginWindow
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private string _email;
        private string _password;
        private bool _rememberMe;
        private bool _isEmailFocused;
        private bool _isPasswordFocused;
        private readonly NeonDb _db;
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

        public ICommand SignInCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand GoogleSignInCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Events

        /// <summary>
        /// Event untuk navigasi ke RegisterWindow
        /// </summary>
        public event EventHandler NavigateToRegister;
        public event EventHandler LoginSuccess;


        #endregion

        #region Constructor

        public LoginViewModel()
        {
            Debug.WriteLine("LoginViewModel initialized");
            _db = new NeonDb();

            // Restore stored credentials
            if (Properties.Settings.Default.RememberMe)
            {
                Email = Properties.Settings.Default.SavedEmail;
                Password = Properties.Settings.Default.SavedPassword; // hashed is fine to auto-fill
                RememberMe = true;
            }

            SignInCommand = new RelayCommand(ExecuteSignIn); 
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPassword);
            GoogleSignInCommand = new RelayCommand(ExecuteGoogleSignIn);
            SignUpCommand = new RelayCommand(ExecuteSignUp);
            CloseCommand = new RelayCommand(ExecuteClose);
  
            Debug.WriteLine("All commands initialized");
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Executes Sign In logic
        /// </summary>
        private async void ExecuteSignIn(object parameter)
        {
            string email = Email?.Trim();
            string password = Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string sql = "SELECT password FROM users WHERE email = @Email LIMIT 1";

                var result = await _db.QueryAsync(sql, new Dictionary<string, object>
                {
                    { "@Email", email }
                });

                if (result.Count == 0)
                {
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Get stored hashed password
                var storedHash = result[0]["password"].ToString();

                // Compare hash with input password
                if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                {
                    // Save login if RememberMe checked
                    if (RememberMe)
                    {
                        Properties.Settings.Default.RememberMe = true;
                        Properties.Settings.Default.SavedEmail = email;
                        Properties.Settings.Default.SavedPassword = password; // or skip storing password entirely
                    }
                    else
                    {
                        Properties.Settings.Default.RememberMe = false;
                        Properties.Settings.Default.SavedEmail = "";
                        Properties.Settings.Default.SavedPassword = "";
                    }

                    Properties.Settings.Default.Save();

                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoginSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Executes Forgot Password logic
        /// </summary>
        private void ExecuteForgotPassword(object parameter)
        {
            MessageBox.Show("Forgot password feature will be implemented soon.\n\n" + "You will receive a password reset link to your email.", "Forgot Password",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

            // TODO: Implement forgot password functionality
        }

        /// <summary>
        /// Executes Google Sign In logic
        /// </summary>
        private async void ExecuteGoogleSignIn(object parameter)
        {
            try
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromFile("Resources/client_secret.json").Secrets,
                    new[] { Oauth2Service.Scope.UserinfoEmail, Oauth2Service.Scope.UserinfoProfile },
                    "user",
                    CancellationToken.None
                );

                var service = new Oauth2Service(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                Userinfo userInfo = await service.Userinfo.Get().ExecuteAsync();

                string email = userInfo.Email;
                string googleId = userInfo.Id;
                string name = userInfo.Name;

                // Check DB
                string sqlCheck = "SELECT * FROM users WHERE email = @Email AND google_id = @GoogleId";
                var user = await _db.QueryAsync(sqlCheck, new Dictionary<string, object>
                {
                    { "@Email", email },
                    { "@GoogleId", googleId },
                });

                // If user does not exist ? Register automatically
                if (user.Count == 0)
                {
                    string sqlInsert = "INSERT INTO users (email, google_id, display_name) VALUES (@Email, @GoogleId, @Name)";
                    await _db.ExecuteAsync(sqlInsert, new Dictionary<string, object>
                    {
                        { "@Email", email },
                        { "@GoogleId", googleId },
                        { "@Name", name }
                    });
                }

                MessageBox.Show($"Welcome {name}! (Google Login successful)", "Success", MessageBoxButton.OK);
                LoginSuccess?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Google login failed: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Executes Sign Up navigation
        /// </summary>
        private void ExecuteSignUp(object parameter)
        {
            Debug.WriteLine("Navigating to RegisterWindow");
            NavigateToRegister?.Invoke(this, EventArgs.Empty);
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
