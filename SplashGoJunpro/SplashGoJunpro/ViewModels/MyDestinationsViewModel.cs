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
using System.Windows.Input;

namespace SplashGoJunpro.ViewModels
{
    public class MyDestinationsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DestinationWithStats> _destinations;
        private DestinationWithStats _selectedDestination;
        private bool _isLoading;
        private string _emptyMessage;
        private DestinationAnalytics _analytics;
        private bool _isAddPopupOpen;
        private NewDestinationModel _newDestination;

        public MyDestinationsViewModel()
        {
            _destinations = new ObservableCollection<DestinationWithStats>();
            _analytics = new DestinationAnalytics();
            _emptyMessage = "Loading destinations...";
            _isLoading = true;
            _isAddPopupOpen = false;
            _newDestination = new NewDestinationModel();

            // Initialize commands
            AddCommand = new RelayCommand(_ => OpenAddPopup());
            SaveAddCommand = new RelayCommand(async _ => await SaveNewDestination());
            CancelAddCommand = new RelayCommand(_ => CloseAddPopup());
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

        public bool IsAddPopupOpen
        {
            get => _isAddPopupOpen;
            set
            {
                _isAddPopupOpen = value;
                OnPropertyChanged();
            }
        }

        public NewDestinationModel NewDestination
        {
            get => _newDestination;
            set
            {
                _newDestination = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand AddCommand { get; }
        public ICommand SaveAddCommand { get; }
        public ICommand CancelAddCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Open add destination popup
        /// </summary>
        private void OpenAddPopup()
        {
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show(
                    "Please login first to add destination",
                    "Login Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            NewDestination = new NewDestinationModel();
            IsAddPopupOpen = true;
        }

        /// <summary>
        /// Close add destination popup
        /// </summary>
        private void CloseAddPopup()
        {
            IsAddPopupOpen = false;
            NewDestination = new NewDestinationModel();
        }

        /// <summary>
        /// Save new destination to database
        /// </summary>
        private async Task SaveNewDestination()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(NewDestination.Name))
                {
                    MessageBox.Show("Please enter destination name", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDestination.Location))
                {
                    MessageBox.Show("Please enter location", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDestination.Price) || !decimal.TryParse(NewDestination.Price, out decimal price))
                {
                    MessageBox.Show("Please enter valid price", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDestination.Description))
                {
                    MessageBox.Show("Please enter description", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDestination.Quota) || !int.TryParse(NewDestination.Quota, out int quota))
                {
                    MessageBox.Show("Please enter valid quota", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewDestination.Category))
                {
                    MessageBox.Show("Please enter category", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsLoading = true;

                // Process offer (comma separated to JSON array)
                string offerJson = "[]";
                if (!string.IsNullOrWhiteSpace(NewDestination.OfferText))
                {
                    var offers = NewDestination.OfferText
                        .Split(',')
                        .Select(o => o.Trim())
                        .Where(o => !string.IsNullOrEmpty(o))
                        .Select(o => $"\"{o}\"")
                        .ToList();
                    offerJson = $"[{string.Join(",", offers)}]";
                }

                // Insert to database
                var db = new NeonDb();
                string sql = @"
                    INSERT INTO destinations (name, location, price, description, quota, category, offer, owner_id, image_link)
                    VALUES (@Name, @Location, @Price, @Description, @Quota, @Category, @Offer::jsonb, @OwnerId, @ImageLink)
                    RETURNING destinationid";

                var imageLink = "/Images/hotel1.jpg"; // Placeholder for image link

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", NewDestination.Name },
                    { "@Location", NewDestination.Location },
                    { "@Price", price },
                    { "@Description", NewDestination.Description },
                    { "@Quota", quota },
                    { "@Category", NewDestination.Category },
                    { "@Offer", offerJson },
                    { "@OwnerId", SessionManager.CurrentUserId },
                    { "@ImageLink",imageLink}
                };

                var result = await db.QueryAsync(sql, parameters);

                var destinationId = result != null && result.Count > 0
                    ? Convert.ToInt32(result[0]["destinationid"])
                    : (int?)null;

                if (destinationId != null)
                {
                    string insertScheduleSql = @"
                        INSERT INTO destination_schedules 
                        (destination_id, day_of_week, open_time, close_time)
                        VALUES (@DestinationId, @DayOfWeek, @OpenTime, @CloseTime);";

                    TimeSpan defaultOpen = new TimeSpan(8, 0, 0);
                    TimeSpan defaultClose = new TimeSpan(19, 0, 0);

                    for (int day = 0; day <= 6; day++)
                    {
                        var scheduleParams = new Dictionary<string, object>
                        {
                            { "@DestinationId", destinationId },
                            { "@DayOfWeek", day },
                            { "@OpenTime", defaultOpen },
                            { "@CloseTime", defaultClose }
                        };

                        await db.ExecuteAsync(insertScheduleSql, scheduleParams);  // FIXED
                    }
                }


                if (result != null && result.Count > 0)
                {
                    MessageBox.Show(
                        "Destination added successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Close popup and refresh list
                    CloseAddPopup();
                    await LoadMyDestinations();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add destination",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error adding destination: {ex.Message}",
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

                // Query dari database
                var db = new NeonDb();
                string sql = @"
            SELECT 
                d.destinationid,
                d.name,
                d.location,
                d.price,
                d.description,
                d.quota as available,
                d.category,
                d.image_link,
                COUNT(DISTINCT b.bookingid) as total_bookings,
                COALESCE(SUM(CASE WHEN b.status = 'Paid' THEN b.totalprice ELSE 0 END), 0) as total_revenue,
                COALESCE(SUM(CASE WHEN b.status = 'Paid' THEN b.amount ELSE 0 END), 0) as total_visitors
                FROM destinations d
                LEFT JOIN bookings b ON d.destinationid = b.destinationid
                WHERE d.owner_id = @OwnerId
                GROUP BY d.destinationid, d.name, d.location, d.price, d.description, 
                         d.quota, d.category, d.image_link
                ORDER BY d.name";

                var parameters = new Dictionary<string, object>
                {
                    { "@OwnerId", SessionManager.CurrentUserId }
                };

                var rows = await db.QueryAsync(sql, parameters);
                MessageBox.Show($"Rows returned: {rows?.Count ?? 0}");


                var destinations = new ObservableCollection<DestinationWithStats>();

                if (rows != null && rows.Count > 0)
                {
                    foreach (var row in rows)
                    {
                        var destination = new DestinationWithStats
                        {
                            DestinationId = Convert.ToInt32(row["destinationid"]),
                            Name = row["name"]?.ToString() ?? "",
                            Location = row["location"]?.ToString() ?? "",
                            Price = Convert.ToDecimal(row["price"]),
                            Description = row["description"]?.ToString() ?? "",
                            Available = Convert.ToInt32(row["available"]),
                            Category = row["category"]?.ToString() ?? "",
                            ImageLink = row["image_link"]?.ToString() ?? "",
                            TotalBookings = Convert.ToInt32(row["total_bookings"]),
                            TotalRevenue = Convert.ToDecimal(row["total_revenue"]),
                            TotalVisitors = Convert.ToInt32(row["total_visitors"]),
                            AverageRating = 0, // Removed reviews
                            TotalReviews = 0   // Removed reviews
                        };

                        // Generate monthly data
                        destination.MonthlyData = GenerateMonthlyData(
                            destination.TotalBookings,
                            destination.TotalRevenue
                        );

                        destinations.Add(destination);
                    }
                }

                Destinations = destinations;
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
    /// Model for new destination form
    /// </summary>
    public class NewDestinationModel : INotifyPropertyChanged
    {
        private string _name;
        private string _location;
        private string _price;
        private string _description;
        private string _quota;
        private string _category;
        private string _offerText;

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

        public string Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string Quota
        {
            get => _quota;
            set { _quota = value; OnPropertyChanged(); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(); }
        }

        public string OfferText
        {
            get => _offerText;
            set { _offerText = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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