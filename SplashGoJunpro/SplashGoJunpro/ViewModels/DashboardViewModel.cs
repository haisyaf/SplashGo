using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SplashGoJunpro.Models;

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

        public DashboardViewModel()
        {
            LoadDestinations();
            FilteredDestinations = new ObservableCollection<Destination>(Destinations);
            SelectedCategory = "Hotels";
            SelectedSortOption = "Lowest Price";
            PriceFrom = "";
            PriceTo = "";

            // Initialize commands
            SelectCategoryCommand = new RelayCommand(SelectCategory);
            ToggleBookmarkCommand = new RelayCommand(ToggleBookmark);
            SearchCommand = new RelayCommand(Search);
            SortCommand = new RelayCommand(Sort);
            FilterPriceCommand = new RelayCommand(FilterPrice);
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
            }
        }

        public string BeachSearchText
        {
            get => _beachSearchText;
            set
            {
                _beachSearchText = value;
                OnPropertyChanged();
            }
        }

        public string ActivitySearchText
        {
            get => _activitySearchText;
            set
            {
                _activitySearchText = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                Sort(null);
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

        #endregion

        #region Commands

        public ICommand SelectCategoryCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand FilterPriceCommand { get; }

        #endregion

        #region Methods

        private void LoadDestinations()
        {
            // Load data dummy - nanti bisa diganti dengan data dari database
            Destinations = new ObservableCollection<Destination>
            {
                new Destination
                {
                    DestinationId = 1,
                    Name = "Beach Paradise Resort",
                    Location = "Kuta Beach",
                    Description = "Beautiful hotel with ocean view",
                    Price = 1200000,
                    Category = "Hotels",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 2,
                    Name = "Ocean View Hotel",
                    Location = "Seminyak",
                    Description = "Luxury beachfront hotel",
                    Price = 950000,
                    Category = "Hotels",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 3,
                    Name = "Sunset Beach Club",
                    Location = "Canggu",
                    Description = "Premium beach club experience",
                    Price = 750000,
                    Category = "Beachclubs",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 4,
                    Name = "Island Explorer Tour",
                    Location = "Nusa Penida",
                    Description = "Full day island tour",
                    Price = 850000,
                    Category = "Tours",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 5,
                    Name = "Spa Bali Wellness",
                    Location = "Ubud",
                    Description = "Traditional Balinese spa",
                    Price = 650000,
                    Category = "Spas",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 6,
                    Name = "Seafood Restaurant Jimbaran",
                    Location = "Jimbaran Bay",
                    Description = "Fresh seafood by the beach",
                    Price = 450000,
                    Category = "Culinaries",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 7,
                    Name = "Water Sports Center",
                    Location = "Tanjung Benoa",
                    Description = "Various water activities",
                    Price = 550000,
                    Category = "Recreational",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 8,
                    Name = "Bali Artisan Market",
                    Location = "Ubud",
                    Description = "Traditional crafts and souvenirs",
                    Price = 200000,
                    Category = "Souvenirs",
                    ImagePath = "/Images/hotel1.jpg"
                },
                new Destination
                {
                    DestinationId = 9,
                    Name = "Grand Bali Hotel",
                    Location = "Sanur",
                    Description = "Comfortable stay near beach",
                    Price = 780000,
                    Category = "Hotels",
                    ImagePath = "/Images/hotel1.jpg"
                }
            };
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

                FilterByCategory();
            }
        }

        private void FilterByCategory()
        {
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
            if (parameter is Destination destination)
            {
                destination.IsBookmarked = !destination.IsBookmarked;

                // TODO: Save bookmark status to database
                // Example: _bookmarkService.SaveBookmark(destination.DestinationId, destination.IsBookmarked);
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
                case "Rating":
                    // Assuming you'll add Rating property later
                    // sorted = sorted.OrderByDescending(d => d.Rating).ToList();
                    sorted = sorted.OrderBy(d => d.Name).ToList();
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