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
    /// ViewModel for RegisterWindow
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        private readonly NeonDb _db;
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
        public event EventHandler LoginSuccess;

        #endregion

        #region Constructor

        public RegisterViewModel()
        {
            Debug.WriteLine("RegisterViewModel initialized");

            // Initialize commands
            _db = new NeonDb(); // initialize here BEFORE calling any method
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
        private async void ExecuteSignUp(object parameter)
        {
            Debug.WriteLine("ExecuteSignUp called");

            string email = Email?.Trim();
            string password = Password;
            string hashedPassword;


            // Validation
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

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Hash only after we've validated the input
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (ArgumentNullException ane)
            {
                MessageBox.Show("Password cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Hashing error: {ane}");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error while hashing password: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine($"Hashing error: {ex}");
                return;
            }
            
            try
            {
                if (_db == null)
                {
                    throw new InvalidOperationException("_db is not initialized!");
                }

                // Create User model
                var newUser = new User
                {
                    Email = email,
                    Password = hashedPassword // TODO: hash in production (done)
                };

                // Save user to database
                string sql = "INSERT INTO users (email, password) VALUES (@Email, @Password)";
                var parameters = new Dictionary<string, object>
                {
                    { "@Email", newUser.Email },
                    { "@Password", newUser.Password }
                };

                int affected = await _db.ExecuteAsync(sql, parameters);

                if (affected > 0)
                {
                    //  REMEMBER ME LOGIC
                    if (RememberMe) 
                    {
                        Properties.Settings.Default.RememberMe = true;
                        Properties.Settings.Default.SavedEmail = email;
                        Properties.Settings.Default.SavedPassword = password; // optionally remove later (for token-based login)
                    }
                    else
                    {
                        Properties.Settings.Default.RememberMe = false;
                        Properties.Settings.Default.SavedEmail = "";
                        Properties.Settings.Default.SavedPassword = "";
                    }

                    Properties.Settings.Default.Save();

                    MessageBox.Show($"Registration successful!\n\nWelcome to SplashGo!\nYou can now sign in using your account.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    Debug.WriteLine($"User registered: {email}");

                    RegistrationSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    MessageBox.Show("Registration failed: Could not save user to database.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
