using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using SplashGoJunpro.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SplashGoJunpro.ViewModels
{
    public class BookmarkViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Destination> _bookmarkedDestinations;
        private bool _isLoading;
        private bool _hasBookmarks;

        public BookmarkViewModel()
        {
            _ = LoadBookmarkedDestinations();
            RemoveBookmarkCommand = new RelayCommand(RemoveBookmark);
        }

        #region Properties

        public ObservableCollection<Destination> BookmarkedDestinations
        {
            get => _bookmarkedDestinations;
            set
            {
                if (_bookmarkedDestinations != null)
                    _bookmarkedDestinations.CollectionChanged -= BookmarkedDestinations_CollectionChanged;

                _bookmarkedDestinations = value;
                OnPropertyChanged();
                HasBookmarks = _bookmarkedDestinations != null && _bookmarkedDestinations.Count > 0;

                if (_bookmarkedDestinations != null)
                    _bookmarkedDestinations.CollectionChanged += BookmarkedDestinations_CollectionChanged;
            }
        }

        // Add this event handler to update HasBookmarks when collection changes
        private void BookmarkedDestinations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            HasBookmarks = _bookmarkedDestinations != null && _bookmarkedDestinations.Count > 0;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool HasBookmarks
        {
            get => _hasBookmarks;
            set
            {
                _hasBookmarks = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand RemoveBookmarkCommand { get; }

        #endregion

        #region Methods

        private async Task LoadBookmarkedDestinations()
        {
            IsLoading = true;

            try
            {
                if (!SessionManager.IsLoggedIn)
                {
                    BookmarkedDestinations = new ObservableCollection<Destination>();
                    return;
                }

                var db = new NeonDb();

                string sql = @"
                    SELECT 
                        d.destinationid, 
                        d.name, 
                        d.location, 
                        d.description, 
                        d.price, 
                        d.category, 
                        d.image_link,
                        b.bookmark_id
                    FROM bookmarks b
                    INNER JOIN destinations d ON b.destination_id = d.destinationid
                    WHERE b.user_id = @userId
                    ORDER BY b.bookmark_id DESC;
                ";

                var parameters = new Dictionary<string, object>
                {
                    { "@userId", SessionManager.CurrentUserId }
                };

                var rows = await db.QueryAsync(sql, parameters);

                var list = new ObservableCollection<Destination>();

                foreach (var row in rows)
                {
                    list.Add(new Destination
                    {
                        DestinationId = Convert.ToInt32(row["destinationid"]),
                        Name = row["name"].ToString(),
                        Location = row["location"].ToString(),
                        Description = row["description"].ToString(),
                        Price = Convert.ToDecimal(row["price"]),
                        Category = row["category"].ToString(),
                        ImagePath = row["image_link"].ToString(),
                        BookmarkId = Convert.ToInt32(row["bookmark_id"])
                    });
                }

                BookmarkedDestinations = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load bookmarks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BookmarkedDestinations = new ObservableCollection<Destination>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void RemoveBookmark(object parameter)
        {
            if (parameter is Destination destination)
            {
                var result = MessageBox.Show(
                    $"Remove '{destination.Name}' from bookmarks?",
                    "Remove Bookmark",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var db = new NeonDb();

                        string sql = @"
                            DELETE FROM bookmarks 
                            WHERE user_id = @userId AND destination_id = @destinationId;
                        ";

                        var parameters = new Dictionary<string, object>
                        {
                            { "@userId", SessionManager.CurrentUserId },
                            { "@destinationId", destination.DestinationId }
                        };

                        await db.ExecuteAsync(sql, parameters);

                        // Remove from collection
                        BookmarkedDestinations.Remove(destination);

                        MessageBox.Show(
                            "Bookmark removed successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to remove bookmark: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        public async Task RefreshBookmarks()
        {
            await LoadBookmarkedDestinations();
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