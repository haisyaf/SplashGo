using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using SplashGoJunpro.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace SplashGoJunpro.ViewModels
{
    public class UserProfileViewModel : INotifyPropertyChanged
    {
        private string _displayName;
        private string _phoneNumber;
        private string _email;
        private string _userInitials;
        private bool _isEditPopupOpen;

        // Temporary values for editing
        private string _tempDisplayName;
        private string _tempPhoneNumber;
        private string _tempEmail;

        public UserProfileViewModel()
        {
            _isEditPopupOpen = false;
        }

        #region Properties

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
                UpdateUserInitials();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string UserInitials
        {
            get => _userInitials;
            set
            {
                _userInitials = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditPopupOpen
        {
            get => _isEditPopupOpen;
            set
            {
                _isEditPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public string TempDisplayName
        {
            get => _tempDisplayName;
            set
            {
                _tempDisplayName = value;
                OnPropertyChanged();
            }
        }

        public string TempPhoneNumber
        {
            get => _tempPhoneNumber;
            set
            {
                _tempPhoneNumber = value;
                OnPropertyChanged();
            }
        }

        public string TempEmail
        {
            get => _tempEmail;
            set
            {
                _tempEmail = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load user profile dari database
        /// </summary>
        public async Task LoadUserProfile()
        {
            try
            {
                // Check if user is logged in
                if (!SessionManager.IsLoggedIn)
                {
                    MessageBox.Show(
                        "You need to log in to view your profile.",
                        "Login Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // TODO: Query dari database untuk mengambil user profile
                var db = new NeonDb();
                string sql = "SELECT display_name, phonenumber, email FROM users WHERE userid = @UserId";
                var parameters = new Dictionary<string, object>
                 {
                     { "@UserId", SessionManager.CurrentUserId }
                 };
                var rows = await db.QueryAsync(sql, parameters);

                if (rows.Count > 0)
                {
                    var row = rows[0];
                    DisplayName = row["display_name"]?.ToString();
                    PhoneNumber = row["phonenumber"]?.ToString();
                    Email = row["email"]?.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load profile: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Update user initials dari display name
        /// </summary>
        private void UpdateUserInitials()
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                UserInitials = "U";
                return;
            }

            var words = DisplayName.Trim().Split(' ');
            if (words.Length >= 2)
            {
                UserInitials = $"{words[0][0]}{words[1][0]}".ToUpper();
            }
            else
            {
                UserInitials = words[0].Substring(0, Math.Min(2, words[0].Length)).ToUpper();
            }
        }

        /// <summary>
        /// Open edit profile popup
        /// </summary>
        public void OpenEditPopup()
        {
            // Copy current values to temp values
            TempDisplayName = DisplayName;
            TempPhoneNumber = PhoneNumber;
            TempEmail = Email;

            IsEditPopupOpen = true;
        }

        /// <summary>
        /// Close edit profile popup
        /// </summary>
        public void CloseEditPopup()
        {
            IsEditPopupOpen = false;
        }

        /// <summary>
        /// Save profile changes
        /// </summary>
        public async Task SaveProfile(string displayName, string phoneNumber, string email)
        {
            try
            {
                // Check if user is logged in
                if (!SessionManager.IsLoggedIn)
                {
                    MessageBox.Show(
                        "You need to log in to update your profile.",
                        "Login Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                var db = new NeonDb();

                // Check if email is already taken by another user
                string checkEmailSql = "SELECT userid FROM users WHERE email = @Email AND userid != @UserId";
                var checkParams = new Dictionary<string, object>
                 {
                     { "@Email", email },
                     { "@UserId", SessionManager.CurrentUserId }
                 };
                var emailRows = await db.QueryAsync(checkEmailSql, checkParams);

                if (emailRows.Count > 0)
                {
                    MessageBox.Show(
                        "This email is already registered to another account.",
                        "Validation Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Update user profile in database
                string updateSql = @"
                     UPDATE users 
                     SET display_name = @DisplayName, 
                         phonenumber = @PhoneNumber, 
                         email = @Email
                     WHERE userid = @UserId";

                var updateParams = new Dictionary<string, object>
                 {
                     { "@DisplayName", displayName },
                     { "@PhoneNumber", phoneNumber },
                     { "@Email", email },
                     { "@UserId", SessionManager.CurrentUserId }
                 };

                await db.ExecuteAsync(updateSql, updateParams);

                // Update local properties
                DisplayName = displayName;
                PhoneNumber = phoneNumber;
                Email = email;

                // Update SessionManager
                if (SessionManager.CurrentUserEmail != null)
                {
                    SessionManager.CurrentUserEmail = email;
                }

                MessageBox.Show(
                    "Profile updated successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                CloseEditPopup();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to update profile: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}