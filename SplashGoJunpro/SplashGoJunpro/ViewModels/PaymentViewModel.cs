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
    public class PaymentViewModel : INotifyPropertyChanged
    {
        private Destination _destination;
        private ObservableCollection<AvailableDate> _availableDates;
        private AvailableDate _selectedDate;
        private int _paxCount;
        private decimal _totalPriceValue;

        public PaymentViewModel(Destination destination)
        {
            _destination = destination;
            _paxCount = 2; // Default PAX count
            _availableDates = new ObservableCollection<AvailableDate>();

            CalculateTotalPrice();
        }

        #region Properties

        public Destination Destination
        {
            get => _destination;
            set
            {
                _destination = value;
                OnPropertyChanged();
                CalculateTotalPrice();
            }
        }

        public ObservableCollection<AvailableDate> AvailableDates
        {
            get => _availableDates;
            set
            {
                _availableDates = value;
                OnPropertyChanged();
            }
        }

        public AvailableDate SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }

        public int PaxCount
        {
            get => _paxCount;
            set
            {
                if (value >= 1 && value <= 10) // Limit PAX count
                {
                    _paxCount = value;
                    OnPropertyChanged();
                    CalculateTotalPrice();
                }
            }
        }

        public string PaxPrice
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {_destination?.Price.ToString("N0", culture)}";
            }
        }

        public string TotalPrice
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {TotalPriceValue.ToString("N0", culture)}";
            }
        }

        public decimal TotalPriceValue
        {
            get => _totalPriceValue;
            set
            {
                _totalPriceValue = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load available dates untuk booking
        /// </summary>
        public async Task LoadAvailableDates()
        {
            try
            {
                // TODO: Query dari database untuk mengambil available dates
                // var db = new NeonDb();
                // string sql = "SELECT * FROM available_dates WHERE destination_id = @DestinationId";
                // var rows = await db.QueryAsync(sql, new Dictionary<string, object> { { "@DestinationId", _destination.DestinationId } });

                // For now, generate dummy dates
                var dates = new ObservableCollection<AvailableDate>();
                var today = DateTime.Today;

                for (int i = 0; i < 6; i++)
                {
                    var date = today.AddDays(i);
                    dates.Add(new AvailableDate
                    {
                        Date = date,
                        DateString = $"{date.Day} {date:MMM}",
                        IsAvailable = true
                    });
                }

                AvailableDates = dates;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load available dates: {ex.Message}");
            }
        }

        /// <summary>
        /// Select date untuk booking
        /// </summary>
        public void SelectDate(AvailableDate date)
        {
            if (date.IsAvailable)
            {
                // Unselect previous date
                if (SelectedDate != null)
                {
                    SelectedDate.IsSelected = false;
                }

                // Select new date
                date.IsSelected = true;
                SelectedDate = date;
            }
        }

        /// <summary>
        /// Increase PAX count
        /// </summary>
        public void IncreasePax()
        {
            if (PaxCount < 10)
            {
                PaxCount++;
            }
        }

        /// <summary>
        /// Decrease PAX count
        /// </summary>
        public void DecreasePax()
        {
            if (PaxCount > 1)
            {
                PaxCount--;
            }
        }

        /// <summary>
        /// Calculate total price berdasarkan PAX count
        /// </summary>
        private void CalculateTotalPrice()
        {
            if (_destination != null)
            {
                TotalPriceValue = _destination.Price * _paxCount;
            }
        }

        /// <summary>
        /// Process payment
        /// </summary>
        public async void ProcessPayment(PaymentData paymentData)
        {
            try
            {
                // Check if user is logged in
                if (!SessionManager.IsLoggedIn)
                {
                    MessageBox.Show(
                        "You need to log in to complete payment.",
                        "Login Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // TODO: Save booking to database
                // var db = new NeonDb();
                // string sql = @"
                //     INSERT INTO bookings (user_id, destination_id, full_name, id_number, mobile_number, 
                //                          booking_date, pax_count, total_amount, created_at)
                //     VALUES (@UserId, @DestinationId, @FullName, @IdNumber, @MobileNumber, 
                //            @BookingDate, @PaxCount, @TotalAmount, @CreatedAt)
                // ";
                // var parameters = new Dictionary<string, object>
                // {
                //     { "@UserId", SessionManager.CurrentUser.UserId },
                //     { "@DestinationId", _destination.DestinationId },
                //     { "@FullName", paymentData.FullName },
                //     { "@IdNumber", paymentData.IdNumber },
                //     { "@MobileNumber", paymentData.MobileNumber },
                //     { "@BookingDate", paymentData.SelectedDate.Date },
                //     { "@PaxCount", paymentData.PaxCount },
                //     { "@TotalAmount", paymentData.TotalAmount },
                //     { "@CreatedAt", DateTime.Now }
                // };
                // await db.ExecuteAsync(sql, parameters);

                MessageBox.Show(
                    $"Payment successful!\n\nBooking Details:\n" +
                    $"Destination: {_destination.Name}\n" +
                    $"Date: {paymentData.SelectedDate.Date:dd MMMM yyyy}\n" +
                    $"PAX: {paymentData.PaxCount}\n" +
                    $"Total: Rp {paymentData.TotalAmount:N0}\n\n" +
                    $"Thank you for your booking!",
                    "Payment Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // TODO: Navigate to booking confirmation page or dashboard
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Payment failed: {ex.Message}",
                    "Payment Error",
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

    #region Helper Classes

    /// <summary>
    /// Model untuk available date
    /// </summary>
    public class AvailableDate : INotifyPropertyChanged
    {
        private bool _isSelected;

        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public bool IsAvailable { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Model untuk payment data
    /// </summary>
    public class PaymentData
    {
        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string MobileNumber { get; set; }
        public AvailableDate SelectedDate { get; set; }
        public int PaxCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    #endregion
}