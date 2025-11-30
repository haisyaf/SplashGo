using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using SplashGoJunpro.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SplashGoJunpro.ViewModels
{
    public class DestinationDetailViewModel : INotifyPropertyChanged
    {
        private Destination _currentDestination;
        private ObservableCollection<Destination> _similarDestinations;
        public event EventHandler<Destination> NavigateToPayment;

        public DestinationDetailViewModel(Destination destination)
        {
            // Initialize commands
            FindTicketCommand = new RelayCommand(FindTicket);
            ToggleBookmarkCommand = new RelayCommand(ToggleBookmark);
        }

        #region Properties

        public Destination CurrentDestination
        {
            get => _currentDestination;
            set
            {
                _currentDestination = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Destination> SimilarDestinations
        {
            get => _similarDestinations;
            set
            {
                _similarDestinations = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        public ICommand FindTicketCommand { get; }
        public ICommand ToggleBookmarkCommand { get; }

        #endregion

        #region Methods

        public async Task LoadDestinationDetails(int destinationId)
        {
            try
            {
                var db = new NeonDb();

                // Load current destination
                string sql = @"
                    SELECT 
                        d.destinationid,
                        d.name,
                        d.location,
                        d.description,
                        d.price,
                        d.category,
                        d.image_link,
                        d.offer,
                        s.day_of_week,
                        s.open_time,
                        s.close_time,
                        u.display_name AS owner_name
                    FROM destinations d
                    JOIN destination_schedules s 
                        ON d.destinationid = s.destination_id
                    JOIN users u
                        ON d.owner_id = u.userid
                    WHERE d.destinationid = @DestinationId
                    ORDER BY s.day_of_week;
                ";

                var rows = await db.QueryAsync(sql, new Dictionary<string, object>
                {
                    { "@DestinationId", destinationId }
                });

                if (rows.Count > 0)
                {
                    // Build destination (from first row)
                    var first = rows[0];

                    CurrentDestination = new Destination
                    {
                        DestinationId = Convert.ToInt32(first["destinationid"]),
                        Name = first["name"].ToString(),
                        Location = first["location"].ToString(),
                        Description = first["description"].ToString(),
                        Price = Convert.ToDecimal(first["price"]),
                        Category = first["category"].ToString(),
                        ImagePath = first["image_link"].ToString(),
                        Owner = first["owner_name"].ToString(),
                        Schedule = new ObservableCollection<DestinationSchedule>(),
                        Offer = ParseOffer(first["offer"])
                    };

                    //Add all 7 schedule rows
                    foreach (var r in rows)
                    {
                        TimeSpan? openTime = null;
                        TimeSpan? closeTime = null;

                        if (r["open_time"] != null && !string.IsNullOrWhiteSpace(r["open_time"].ToString()))
                        {
                            if (TimeSpan.TryParse(r["open_time"].ToString().Trim(), out var parsedOpen))
                            {
                                openTime = parsedOpen;
                            }
                        }

                        if (r["close_time"] != null && !string.IsNullOrWhiteSpace(r["close_time"].ToString()))
                        {
                            if (TimeSpan.TryParse(r["close_time"].ToString().Trim(), out var parsedClose))
                            {
                                closeTime = parsedClose;
                            }
                        }

                        CurrentDestination.Schedule.Add(new DestinationSchedule
                        {
                            DayOfWeek = Convert.ToInt32(r["day_of_week"]),
                            OpenTime = openTime,
                            CloseTime = closeTime
                        });

                    }
                    // Notify that schedule-related UI must refresh
                    OnPropertyChanged(nameof(CurrentDestination));
                    OnPropertyChanged(nameof(CurrentDestination.TodayScheduleDisplay));



                    // Load similar destinations (same category, different ID)
                    await LoadSimilarDestinations(CurrentDestination.Category, destinationId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load destination details: {ex.Message}");
            }
        }

        private List<string> ParseOffer(object offerObj)
        {
            if (offerObj == null)
                return new List<string>();

            string raw = offerObj.ToString().Trim();

            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            // CASE 1 — JSON array string
            if (raw.StartsWith("["))
            {
                try
                {
                    return JsonSerializer.Deserialize<List<string>>(raw);
                }
                catch
                {
                    // ignore and try CSV fallback
                }
            }

            // CASE 2 — CSV string
            return raw
                .Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();
        }

        private async Task LoadSimilarDestinations(string category, int currentDestinationId)
        {
            try
            {
                var db = new NeonDb();

                string sql = @"
                    SELECT destinationid, name, location, description, price, category, image_link
                    FROM destinations
                    WHERE category = @Category AND destinationid != @CurrentId
                    ORDER BY RANDOM();
                ";

                var rows = await db.QueryAsync(sql, new Dictionary<string, object>
                {
                    { "@Category", category },
                    { "@CurrentId", currentDestinationId }
                });

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
                        // check image link exists, if not, use default image
                        ImagePath = string.IsNullOrEmpty(row["image_link"].ToString()) ? "Images/hotel1.jpg" : row["image_link"].ToString()
                    });
                }

                SimilarDestinations = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load similar destinations: {ex.Message}");
            }
        }

        private void FindTicket(object parameter)
        {
            // Check if user is logged in
            if (!SessionManager.IsLoggedIn)
            {
                MessageBox.Show(
                    "You need to log in to book tickets.",
                    "Login Required",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // to PaymentPage
            NavigateToPayment?.Invoke(this, CurrentDestination);
        }

        private void ToggleBookmark(object parameter)
        {
            // Check if user is logged in
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

            // Feature not ready yet
            MessageBox.Show(
                "Bookmark feature is not available yet. Coming soon!",
                "Coming Soon",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
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