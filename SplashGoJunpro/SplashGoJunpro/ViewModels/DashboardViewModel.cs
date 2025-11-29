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

        #endregion

        #region Methods

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
                return;
            }

            // Logged in → feature not ready
            MessageBox.Show(
                "Bookmark feature is not available yet. Coming soon!",
                "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
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