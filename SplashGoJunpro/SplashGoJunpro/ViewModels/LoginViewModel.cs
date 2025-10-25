using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SplashGoJunpro.Commands;
using SplashGoJunpro.Models;

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

        #region Constructor

        public LoginViewModel()
        {
        Debug.WriteLine("LoginViewModel initialized");
            
    // Initialize commands
     SignInCommand = new RelayCommand(ExecuteSignIn); // ? Removed CanExecute - always enabled
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
        private void ExecuteSignIn(object parameter)
        {
 Debug.WriteLine("ExecuteSignIn called");
     
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

            // TODO: Replace with actual authentication service
        // For now, hardcoded validation
     if (email == "admin@splashgo.com" && password == "admin123")
       {
    MessageBox.Show("Login successful!", "Success",
 MessageBoxButton.OK, MessageBoxImage.Information);

   // TODO: Navigate to MainWindow
                // Application.Current.MainWindow can be used here
   // or use a navigation service
 }
  else
            {
MessageBox.Show("Invalid email or password.", "Login Failed",
   MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Executes Forgot Password logic
        /// </summary>
        private void ExecuteForgotPassword(object parameter)
        {
    MessageBox.Show("Forgot password feature will be implemented soon.\n\n" +
      "You will receive a password reset link to your email.",
          "Forgot Password",
        MessageBoxButton.OK,
         MessageBoxImage.Information);

          // TODO: Implement forgot password functionality
     }

        /// <summary>
        /// Executes Google Sign In logic
     /// </summary>
        private void ExecuteGoogleSignIn(object parameter)
      {
   MessageBox.Show("Google Sign-In feature will be implemented soon.\n\n" +
     "This will allow you to sign in using your Google account.",
      "Google Sign-In",
      MessageBoxButton.OK,
        MessageBoxImage.Information);

            // TODO: Implement OAuth2 Google Sign-In
        }

    /// <summary>
        /// Executes Sign Up navigation
        /// </summary>
        private void ExecuteSignUp(object parameter)
        {
    MessageBox.Show("Registration feature will be implemented soon.\n\n" +
 "You will be able to create a new account.",
         "Sign Up",
      MessageBoxButton.OK,
         MessageBoxImage.Information);

       // TODO: Navigate to SignUpWindow
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
