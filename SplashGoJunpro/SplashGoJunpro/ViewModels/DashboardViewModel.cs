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
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Destination> _destinations;
        private ObservableCollection<Destination> _filteredDestinations;
        private string _selectedCategory;
        private string _beachSearchText;
        private string _activitySearchText;
        private string _selectedSortOption;
        private string _priceFrom;
        private string _priceTo;
        private bool _isPricePopupOpen;

        public DashboardViewModel()
        {
            _ = LoadDestinations();
            //Destinations = new ObservableCollection<Destination>();
            //FilteredDestinations = new ObservableCollection<Destination>(Destinations);

            SelectedCategory = "All";
            SelectedSortOption = "Lowest Price";

            BeachSearchText = string.Empty;
            ActivitySearchText = string.Empty;
            PriceFrom = "";
            PriceTo = "";
            IsPricePopupOpen = false;

            // Initialize commands
            SelectCategoryCommand = new RelayCommand(SelectCategory);
            ToggleBookmarkCommand = new RelayCommand(ToggleBookmark);
            SearchCommand = new RelayCommand(Search);
            SortCommand = new RelayCommand(Sort);
            FilterPriceCommand = new RelayCommand(FilterPrice);
            ApplyPriceFilterCommand = new RelayCommand(ApplyPriceFilter);

            // Navigation commands
            DashboardCommand = new RelayCommand(_ => { /* Already on dashboard */ });
            HistoryCommand = new RelayCommand(_ => ExecuteProtectedNavigation("History"));
            SavedCommand = new RelayCommand(_ => ExecuteProtectedNavigation("Saved"));
            AddBusinessCommand = new RelayCommand(_ => ExecuteProtectedNavigation("Add Business"));
            ProfileCommand = new RelayCommand(_ => ExecuteProtectedNavigation("Profile"));
            LogoutCommand = new RelayCommand(_ => ExecuteLogout());
        }

        #region Properties

        public ObservableCollection<Destination> Destinations
        {
            get => _destinations;
            set
            {
                _destinations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Destination> FilteredDestinations
        {
            get => _filteredDestinations;
            set
            {
                _filteredDestinations = value;
                OnPropertyChanged();
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                FilterByCategory();
            }
        }

        public string BeachSearchText
        {
            get => _beachSearchText;
            set
            {
                _beachSearchText = value;
                OnPropertyChanged();
                FilterByCategory();
            }
        }

        public string ActivitySearchText
        {
            get => _activitySearchText;
            set
            {
                _activitySearchText = value;
                OnPropertyChanged();
                FilterByCategory();
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    OnPropertyChanged();

                    Sort(null);
                }
            }
        }

        public string PriceFrom
        {
            get => _priceFrom;
            set
            {
                _priceFrom = value;
                OnPropertyChanged();
            }
        }

        public string PriceTo
        {
            get => _priceTo;
            set
            {
                _priceTo = value;
                OnPropertyChanged();
            }
        }

        public bool IsPricePopupOpen
        {
            get => _isPricePopupOpen;
            set
            {
                _isPricePopupOpen = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand SelectCategoryCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand FilterPriceCommand { get; }
        public ICommand ApplyPriceFilterCommand { get; }

        // Navigation Commands
        public ICommand DashboardCommand { get; }
        public ICommand HistoryCommand { get; }
        public ICommand SavedCommand { get; }
        public ICommand AddBusinessCommand { get; }
        public ICommand ProfileCommand { get; }
        public ICommand LogoutCommand { get; }

        #endregion

        #region Events

        public event EventHandler NavigateToLogin;
        public event EventHandler NavigateToProfile;
        public event EventHandler NavigateToHistory;
        public event EventHandler NavigateToSaved;
        public event EventHandler NavigateToAddBusiness;

        #endregion

        #region Methods
        private void ExecuteProtectedNavigation(string featureName)
        {
            // If NOT logged in → redirect to Login
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show(
                    "You need to log in to access this feature.",
                    "Login Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                NavigateToLogin?.Invoke(this, EventArgs.Empty);
                return;
            }

            // If logged in → show placeholder message
            MessageBox.Show(
                $"{featureName} feature is not available yet. Coming soon!",
                "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private async void ExecuteLogout()
        {
            // Check if user is logged in
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show("You are not logged in.", "Logout Failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Remove login token from database
            try
            {
                var sql = "UPDATE users SET login_token = NULL WHERE email = @Email";

                await new NeonDb().ExecuteAsync(sql, new Dictionary<string, object>
                {
                    { "@Email", SessionManager.CurrentUserEmail }
                });
            }
            catch
            {
                MessageBox.Show("Logout failed (DB error).", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Clear session
            SessionManager.IsLoggedIn = false;
            SessionManager.CurrentUserEmail = null;
            SessionManager.LoginToken = null;

            MessageBox.Show("You have been logged out.", "Logout",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate to Login window
            //NavigateToLogin?.Invoke(this, EventArgs.Empty);
        }

        private async Task LoadDestinations()
        {
            try
            {
                var db = new NeonDb();

                string sql = @"
                    SELECT destinationid, name, location, description, price, category, image_link
                    FROM destinations
                    ORDER BY destinationid;
                ";

                var rows = await db.QueryAsync(sql, new Dictionary<string, object>());

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
                        ImagePath = row["image_link"].ToString()
                    });
                }

                Destinations = list;
                FilteredDestinations = new ObservableCollection<Destination>(list);

                // Apply initial filters & sorting
                FilterByCategory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load destinations: {ex.Message}");
            }
            return; // Explicit return to satisfy all code paths for Task-returning method
        }

        

        private void SelectCategory(object parameter)
        {
            if (parameter is string category)
            {
                // Toggle logic: jika kategori yang sama diklik lagi, set ke "All"
                if (SelectedCategory == category)
                {
                    SelectedCategory = "All";
                }
                else
                {
                    SelectedCategory = category;
                }
            }
        }

        private void FilterByCategory()
        {
            if (Destinations == null || Destinations.Count == 0)
                return;

            var filtered = Destinations.AsEnumerable();

            // Filter by category
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            {
                filtered = filtered.Where(d => d.Category == SelectedCategory);
            }

            // Apply beach search filter
            if (!string.IsNullOrWhiteSpace(BeachSearchText) &&
                BeachSearchText != "Search for beaches...")
            {
                filtered = filtered.Where(d =>
                    d.Location.IndexOf(BeachSearchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Apply activity search filter
            if (!string.IsNullOrWhiteSpace(ActivitySearchText) &&
                ActivitySearchText != "Search for activities...")
            {
                filtered = filtered.Where(d =>
                    d.Name.IndexOf(ActivitySearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    d.Description.IndexOf(ActivitySearchText, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Apply price range filter
            if (decimal.TryParse(PriceFrom, out decimal priceFrom) && priceFrom > 0)
            {
                filtered = filtered.Where(d => d.Price >= priceFrom);
            }

            if (decimal.TryParse(PriceTo, out decimal priceTo) && priceTo > 0)
            {
                filtered = filtered.Where(d => d.Price <= priceTo);
            }

            FilteredDestinations = new ObservableCollection<Destination>(filtered);
            Sort(null); // Apply current sort
        }

        private void ToggleBookmark(object parameter)
        {
            // If NOT logged in → show login message
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show(
                    "You need to log in to use bookmarks.",
                    "Login Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                NavigateToLogin?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Logged in → feature not ready
            MessageBox.Show(
                "Bookmark feature is not available yet. Coming soon!",
                "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // OPTIONAL: Toggle UI bookmark (if you want the heart to change)
            if (parameter is Destination destination)
            {
                destination.IsBookmarked = !destination.IsBookmarked;
            }
        }


        private void Search(object parameter)
        {
            FilterByCategory(); // Re-apply all filters including search
        }

        private void FilterPrice(object parameter)
        {
            FilterByCategory(); // Re-apply all filters including price
        }

        private void ApplyPriceFilter(object parameter)
        {
            FilterByCategory(); // Re-apply all filters including price
            IsPricePopupOpen = false; // Close the popup
        }

        private void Sort(object parameter)
        {
            if (FilteredDestinations == null || FilteredDestinations.Count == 0)
                return;

            var sorted = FilteredDestinations.ToList();

            switch (SelectedSortOption)
            {
                case "Lowest Price":
                    sorted = sorted.OrderBy(d => d.Price).ToList();
                    break;
                case "Highest Price":
                    sorted = sorted.OrderByDescending(d => d.Price).ToList();
                    break;
                //case "Rating":
                //    // Assuming you'll add Rating property later
                //    // sorted = sorted.OrderByDescending(d => d.Rating).ToList();
                //    sorted = sorted.OrderBy(d => d.Name).ToList();
                //    break;
                default:
                    sorted = sorted.OrderBy(d => d.Price).ToList();
                    break;
            }

            FilteredDestinations = new ObservableCollection<Destination>(sorted);
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

    #region RelayCommand Implementation

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    #endregion
}

//private void LoadDestinations()
//{
//    // Load data dummy - nanti bisa diganti dengan data dari database
//    Destinations = new ObservableCollection<Destination>
//    {
//        new Destination
//        {
//            DestinationId = 1,
//            Name = "Beach Paradise Resort",
//            Location = "Kuta Beach",
//            Description = "Beautiful hotel with ocean view",
//            Price = 1200000,
//            Category = "Hotels",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 2,
//            Name = "Ocean View Hotel",
//            Location = "Seminyak",
//            Description = "Luxury beachfront hotel",
//            Price = 950000,
//            Category = "Hotels",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 3,
//            Name = "Sunset Beach Club",
//            Location = "Canggu",
//            Description = "Premium beach club experience",
//            Price = 750000,
//            Category = "Beachclubs",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 4,
//            Name = "Island Explorer Tour",
//            Location = "Nusa Penida",
//            Description = "Full day island tour",
//            Price = 850000,
//            Category = "Tours",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 5,
//            Name = "Spa Bali Wellness",
//            Location = "Ubud",
//            Description = "Traditional Balinese spa",
//            Price = 650000,
//            Category = "Spas",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 6,
//            Name = "Seafood Restaurant Jimbaran",
//            Location = "Jimbaran Bay",
//            Description = "Fresh seafood by the beach",
//            Price = 450000,
//            Category = "Culinaries",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 7,
//            Name = "Water Sports Center",
//            Location = "Tanjung Benoa",
//            Description = "Various water activities",
//            Price = 550000,
//            Category = "Recreational",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 8,
//            Name = "Bali Artisan Market",
//            Location = "Ubud",
//            Description = "Traditional crafts and souvenirs",
//            Price = 200000,
//            Category = "Souvenirs",
//            ImagePath = "/Images/hotel1.jpg"
//        },
//        new Destination
//        {
//            DestinationId = 9,
//            Name = "Grand Bali Hotel",
//            Location = "Sanur",
//            Description = "Comfortable stay near beach",
//            Price = 780000,
//            Category = "Hotels",
//            ImagePath = "/Images/hotel1.jpg"
//        }
//    };
//}