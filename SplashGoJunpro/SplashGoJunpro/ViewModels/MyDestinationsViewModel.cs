using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using SplashGoJunpro.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace SplashGoJunpro.ViewModels
{
    public class MyDestinationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DestinationWithStats> _destinations;
        private DestinationWithStats _selectedDestination;
        private bool _isLoading;
        private string _emptyMessage;
        private DestinationAnalytics _analytics;

        public MyDestinationsViewModel()
        {
            _destinations = new ObservableCollection<DestinationWithStats>();
            _analytics = new DestinationAnalytics();
            _emptyMessage = "Loading destinations...";
            _isLoading = true;
        }

        #region Properties

        public ObservableCollection<DestinationWithStats> Destinations
        {
            get => _destinations;
            set
            {
                _destinations = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDestinations));
            }
        }

        public DestinationWithStats SelectedDestination
        {
            get => _selectedDestination;
            set
            {
                _selectedDestination = value;
                OnPropertyChanged();
            }
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

        public string EmptyMessage
        {
            get => _emptyMessage;
            set
            {
                _emptyMessage = value;
                OnPropertyChanged();
            }
        }

        public bool HasDestinations => _destinations != null && _destinations.Count > 0;

        public DestinationAnalytics Analytics
        {
            get => _analytics;
            set
            {
                _analytics = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load destinations owned by current user
        /// </summary>
        public async Task LoadMyDestinations()
        {
            try
            {
                IsLoading = true;

                if (!SessionManager.IsLoggedIn)
                {
                    EmptyMessage = "Please login to view your destinations";
                    Destinations = new ObservableCollection<DestinationWithStats>();
                    return;
                }

                // TODO: Query dari database
                // var db = new NeonDb();
                // string sql = @"
                //     SELECT 
                //         d.destination_id,
                //         d.name,
                //         d.location,
                //         d.price,
                //         d.description,
                //         d.available,
                //         d.category,
                //         d.image_link,
                //         COUNT(DISTINCT b.booking_id) as total_bookings,
                //         SUM(CASE WHEN b.status = 'Paid' THEN b.total_amount ELSE 0 END) as total_revenue,
                //         SUM(CASE WHEN b.status = 'Paid' THEN b.pax_count ELSE 0 END) as total_visitors,
                //         AVG(CASE WHEN r.rating IS NOT NULL THEN r.rating ELSE 0 END) as avg_rating,
                //         COUNT(DISTINCT r.review_id) as total_reviews
                //     FROM destinations d
                //     LEFT JOIN bookings b ON d.destination_id = b.destination_id
                //     LEFT JOIN reviews r ON d.destination_id = r.destination_id
                //     WHERE d.owner_id = @OwnerId
                //     GROUP BY d.destination_id
                //     ORDER BY d.name
                // ";
                // 
                // var parameters = new Dictionary<string, object>
                // {
                //     { "@OwnerId", SessionManager.CurrentUser.UserId }
                // };
                // 
                // var rows = await db.QueryAsync(sql, parameters);

                await Task.Delay(1000);

                var dummyDestinations = new ObservableCollection<DestinationWithStats>
                {
                    new DestinationWithStats
                    {
                        DestinationId = 1,
                        Name = "Hotel Malioboro Yogyakarta",
                        Location = "Malioboro, Yogyakarta",
                        Price = 500000,
                        Description = "Hotel berbintang di jantung kota Yogyakarta",
                        Available = 25,
                        Category = "Hotel",
                        ImageLink = "/Images/malioboro.jpg",
                        TotalBookings = 156,
                        TotalRevenue = 78000000,
                        TotalVisitors = 312,
                        AverageRating = 4.5,
                        TotalReviews = 89,
                        MonthlyData = GenerateMonthlyData(156, 78000000)
                    },
                    new DestinationWithStats
                    {
                        DestinationId = 2,
                        Name = "Pantai Parangtritis",
                        Location = "Bantul, Yogyakarta",
                        Price = 50000,
                        Description = "Pantai dengan pemandangan sunset yang indah",
                        Available = 100,
                        Category = "Wisata Alam",
                        ImageLink = "/Images/parangtritis.jpg",
                        TotalBookings = 89,
                        TotalRevenue = 13350000,
                        TotalVisitors = 267,
                        AverageRating = 4.2,
                        TotalReviews = 45,
                        MonthlyData = GenerateMonthlyData(89, 13350000)
                    },
                    new DestinationWithStats
                    {
                        DestinationId = 3,
                        Name = "Candi Prambanan",
                        Location = "Sleman, Yogyakarta",
                        Price = 75000,
                        Description = "Kompleks candi Hindu terbesar di Indonesia",
                        Available = 200,
                        Category = "Wisata Budaya",
                        ImageLink = "/Images/prambanan.jpg",
                        TotalBookings = 234,
                        TotalRevenue = 52650000,
                        TotalVisitors = 702,
                        AverageRating = 4.8,
                        TotalReviews = 156,
                        MonthlyData = GenerateMonthlyData(234, 52650000)
                    }
                };

                Destinations = dummyDestinations;
                CalculateAnalytics();

                if (Destinations.Count == 0)
                {
                    EmptyMessage = "You don't have any destinations yet";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load destinations: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                EmptyMessage = "Failed to load destinations";
                Destinations = new ObservableCollection<DestinationWithStats>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Calculate overall analytics from all destinations
        /// </summary>
        private void CalculateAnalytics()
        {
            if (Destinations == null || Destinations.Count == 0)
            {
                Analytics = new DestinationAnalytics();
                return;
            }

            Analytics = new DestinationAnalytics
            {
                TotalDestinations = Destinations.Count,
                TotalBookings = Destinations.Sum(d => d.TotalBookings),
                TotalRevenue = Destinations.Sum(d => d.TotalRevenue),
                TotalVisitors = Destinations.Sum(d => d.TotalVisitors),
                AverageRating = Destinations.Average(d => d.AverageRating),
                TotalReviews = Destinations.Sum(d => d.TotalReviews),
                CategoryDistribution = CalculateCategoryDistribution(),
                TopPerformers = Destinations.OrderByDescending(d => d.TotalRevenue).Take(3).ToList()
            };
        }

        /// <summary>
        /// Calculate category distribution for pie chart
        /// </summary>
        private ObservableCollection<CategoryData> CalculateCategoryDistribution()
        {
            var categoryGroups = Destinations
                .GroupBy(d => d.Category)
                .Select(g => new CategoryData
                {
                    Category = g.Key,
                    Count = g.Count(),
                    TotalRevenue = g.Sum(d => d.TotalRevenue),
                    Percentage = (double)g.Count() / Destinations.Count * 100
                })
                .OrderByDescending(c => c.Count)
                .ToList();

            return new ObservableCollection<CategoryData>(categoryGroups);
        }

        /// <summary>
        /// Generate monthly data for charts (last 6 months)
        /// </summary>
        private ObservableCollection<MonthlyData> GenerateMonthlyData(int totalBookings, decimal totalRevenue)
        {
            var monthlyData = new ObservableCollection<MonthlyData>();
            var random = new Random();

            for (int i = 5; i >= 0; i--)
            {
                var month = DateTime.Now.AddMonths(-i);
                var bookings = (int)(totalBookings * (0.1 + random.NextDouble() * 0.2));
                var revenue = totalRevenue * (decimal)(0.1 + (random.NextDouble() * 0.2));

                monthlyData.Add(new MonthlyData
                {
                    Month = month.ToString("MMM yyyy"),
                    Bookings = bookings,
                    Revenue = revenue,
                    Visitors = bookings * 2 + random.Next(10, 50)
                });
            }

            return monthlyData;
        }

        /// <summary>
        /// Refresh destinations list
        /// </summary>
        public async Task RefreshDestinations()
        {
            await LoadMyDestinations();
        }

        /// <summary>
        /// Filter destinations by category
        /// </summary>
        public async Task FilterByCategory(string category)
        {
            try
            {
                IsLoading = true;
                await Task.Delay(300);

                // TODO: Implement database filtering
                // For now, filter in-memory
                if (string.IsNullOrEmpty(category) || category == "All")
                {
                    await LoadMyDestinations();
                }
                else
                {
                    var filtered = _destinations.Where(d => d.Category == category).ToList();
                    Destinations = new ObservableCollection<DestinationWithStats>(filtered);
                    CalculateAnalytics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to filter destinations: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            finally
            {
                IsLoading = false;
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

    #region Model Classes

    /// <summary>
    /// Destination model with statistics
    /// </summary>
    public class DestinationWithStats : INotifyPropertyChanged
    {
        private int _destinationId;
        private string _name;
        private string _location;
        private decimal _price;
        private string _description;
        private int _available;
        private string _category;
        private string _imageLink;
        private int _totalBookings;
        private decimal _totalRevenue;
        private int _totalVisitors;
        private double _averageRating;
        private int _totalReviews;
        private ObservableCollection<MonthlyData> _monthlyData;

        public int DestinationId
        {
            get => _destinationId;
            set { _destinationId = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PriceFormatted));
            }
        }

        public string PriceFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {Price.ToString("N0", culture)}";
            }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public int Available
        {
            get => _available;
            set { _available = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public string ImageLink
        {
            get => _imageLink;
            set { _imageLink = value; OnPropertyChanged(); }
        }

        public int TotalBookings
        {
            get => _totalBookings;
            set { _totalBookings = value; OnPropertyChanged(); }
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalRevenueFormatted));
            }
        }

        public string TotalRevenueFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {TotalRevenue.ToString("N0", culture)}";
            }
        }

        public int TotalVisitors
        {
            get => _totalVisitors;
            set { _totalVisitors = value; OnPropertyChanged(); }
        }

        public double AverageRating
        {
            get => _averageRating;
            set
            {
                _averageRating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageRatingFormatted));
            }
        }

        public string AverageRatingFormatted => $"{AverageRating:F1} ⭐";

        public int TotalReviews
        {
            get => _totalReviews;
            set { _totalReviews = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MonthlyData> MonthlyData
        {
            get => _monthlyData;
            set { _monthlyData = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Overall analytics data
    /// </summary>
    public class DestinationAnalytics : INotifyPropertyChanged
    {
        private int _totalDestinations;
        private int _totalBookings;
        private decimal _totalRevenue;
        private int _totalVisitors;
        private double _averageRating;
        private int _totalReviews;
        private ObservableCollection<CategoryData> _categoryDistribution;
        private List<DestinationWithStats> _topPerformers;

        public int TotalDestinations
        {
            get => _totalDestinations;
            set { _totalDestinations = value; OnPropertyChanged(); }
        }

        public int TotalBookings
        {
            get => _totalBookings;
            set { _totalBookings = value; OnPropertyChanged(); }
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set
            {
                _totalRevenue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalRevenueFormatted));
            }
        }

        public string TotalRevenueFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {TotalRevenue.ToString("N0", culture)}";
            }
        }

        public int TotalVisitors
        {
            get => _totalVisitors;
            set { _totalVisitors = value; OnPropertyChanged(); }
        }

        public double AverageRating
        {
            get => _averageRating;
            set
            {
                _averageRating = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageRatingFormatted));
            }
        }

        public string AverageRatingFormatted => $"{AverageRating:F1} ⭐";

        public int TotalReviews
        {
            get => _totalReviews;
            set { _totalReviews = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CategoryData> CategoryDistribution
        {
            get => _categoryDistribution;
            set { _categoryDistribution = value; OnPropertyChanged(); }
        }

        public List<DestinationWithStats> TopPerformers
        {
            get => _topPerformers;
            set { _topPerformers = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Monthly statistics data for charts
    /// </summary>
    public class MonthlyData
    {
        public string Month { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public int Visitors { get; set; }

        public string RevenueFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {Revenue.ToString("N0", culture)}";
            }
        }
    }

    /// <summary>
    /// Category distribution data for pie chart
    /// </summary>
    public class CategoryData
    {
        public string Category { get; set; }
        public int Count { get; set; }
        public decimal TotalRevenue { get; set; }
        public double Percentage { get; set; }

        public string PercentageFormatted => $"{Percentage:F1}%";

        public string RevenueFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {TotalRevenue.ToString("N0", culture)}";
            }
        }
    }

    #endregion
}