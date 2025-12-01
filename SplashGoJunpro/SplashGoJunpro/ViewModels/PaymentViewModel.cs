using SplashGoJunpro.Data;
using SplashGoJunpro.Models;
using SplashGoJunpro.Services;
using SplashGoJunpro.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

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
            _paxCount = 1; // Default PAX count
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
                // Determine max PAX: minimum of remaining quota
                int maxPax = 10;
                if (SelectedDate != null)
                {
                    maxPax = SelectedDate.RemainingQuota;
                }

                if (value >= 1 && value <= maxPax)
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
                //var schedules = Destination.Schedule;
                var db = new NeonDb();

                for (int i = 0; i < 6; i++) // next 14 days
                {
                    var date = today.AddDays(i);
                    int dow = (int)date.DayOfWeek;

                    // 1. GET SCHEDULE (open/close)
                    string scheduleSql = @"
                        SELECT * FROM destination_schedules
                        WHERE destination_id=@DestId AND day_of_week=@Dow
                    ";

                    var scheduleRows = await db.QueryAsync(
                        scheduleSql,
                        new Dictionary<string, object> { { "@DestId", _destination.DestinationId }, { "@Dow", dow } }
                    );

                    // If you need to map the result to DestinationSchedule, you must do it manually:
                    var schedule = scheduleRows
                        .Select(row => new DestinationSchedule
                        {
                            DestinationId = Convert.ToInt32(row["destination_id"]),
                            DayOfWeek = Convert.ToInt32(row["day_of_week"]),
                            OpenTime = row["open_time"] as TimeSpan?,
                            CloseTime = row["close_time"] as TimeSpan?
                        })
                        .FirstOrDefault();

                    bool isOpen = schedule != null &&
                                  schedule.OpenTime.HasValue &&
                                  schedule.CloseTime.HasValue;

                    // 2. GET TOTAL BOOKED FOR THAT DATE
                    string bookedSql = @"
                        SELECT COALESCE(SUM(pax_count), 0) AS total_booked
                        FROM bookings
                        WHERE destinationid = @DestId 
                          AND date = @Date
                          AND LOWER(status) IN ('pending', 'paid')
                    ";

                    var bookedRows = await db.QueryAsync(
                        bookedSql,
                        new Dictionary<string, object> { { "@DestId", _destination.DestinationId }, { "@Date", date.Date } }
                    );

                    int totalBooked = 0;
                    if (bookedRows.Count > 0 && bookedRows[0].ContainsKey("total_booked"))
                    {
                        totalBooked = Convert.ToInt32(bookedRows[0]["total_booked"]);
                    }

                    int remaining = _destination.DestinationQuota - totalBooked;

                    // 3. DETERMINE UI STATE
                    bool isEnabled = true;
                    string status;

                    if (!isOpen)
                    {
                        status = "Closed";
                        isEnabled = false;
                    }
                    else if (remaining <= 0)
                    {
                        status = "Full";
                        isEnabled = false;
                    }
                    else
                    {
                        status = $"Available ({remaining} left)";
                    }

                    // 4. ADD TO LIST
                    dates.Add(new AvailableDate
                    {
                        Date = date,
                        DateString = date.ToString("dd MMM"),
                        RemainingQuota = remaining,
                        Status = status,
                        IsAvailable = isEnabled
                    });
                }

                //for (int i = 0; i < 6; i++)
                //{
                //    var date = today.AddDays(i);
                //    dates.Add(new AvailableDate
                //    {
                //        Date = date,
                //        DateString = $"{date.Day} {date:MMM}",
                //        IsAvailable = true
                //    });
                //}

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
            int maxPax = 0;
            if (SelectedDate != null)
            {
                maxPax = SelectedDate.RemainingQuota;
            }

            if (PaxCount < maxPax)
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
        public async void ProcessPayment(PaymentData paymentData, NavigationService navService)
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

                // Validate payment data
                if (paymentData?.SelectedDate == null || paymentData.PaxCount <= 0)
                {
                    MessageBox.Show(
                        "Please select a date and ensure PAX count is valid.",
                        "Invalid Data",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // CREATE BOOKING RECORD IN DATABASE
                var db = new NeonDb();
                string insertBookingSql = @"
                    INSERT INTO bookings (userid, destinationid, created_at, amount, totalprice, status, booking_date)
                    VALUES (@UserId, @DestinationId, @CreatedAt, @PaxCount, @TotalAmount, @Status, @BookingDate)
                    RETURNING bookingid;
                ";

                var bookingParameters = new Dictionary<string, object>
                {
                    { "@UserId", SessionManager.CurrentUserId },
                    { "@DestinationId", _destination.DestinationId },
                    { "@BookingDate", paymentData.SelectedDate.Date },
                    { "@PaxCount", paymentData.PaxCount },
                    { "@TotalAmount", paymentData.TotalAmount },
                    { "@Status", "pending" },
                    { "@CreatedAt", DateTime.Now }
                };

                var bookingResult = await db.QueryAsync(insertBookingSql, bookingParameters);
                if (bookingResult.Count == 0)
                {
                    throw new Exception("Failed to create booking record.");
                }

                int bookingId = Convert.ToInt32(bookingResult[0]["bookingid"]);

                string orderCode = $"BOOKING-{bookingId}-{DateTime.Now.Ticks}";

                string updateOrderSql = "UPDATE bookings SET order_code = @OrderCode WHERE bookingid = @BookingId";
                await db.ExecuteAsync(updateOrderSql, new Dictionary<string, object>
                {
                    { "@OrderCode", orderCode },
                    { "@BookingId", bookingId }
                });


                string insertVisitorSql = @"
                    INSERT INTO visitors (booking_id, full_name, id_number, mobile_number)
                    VALUES (@BookingId, @FullName, @IdNumber, @MobileNumber);
                ";  

                var visitorParameters = new Dictionary<string, object>
                {
                    { "@BookingId", bookingId},
                    { "@FullName", paymentData.FullName },
                    { "@IdNumber", paymentData.IdNumber },
                    { "@MobileNumber", paymentData.MobileNumber }
                };

                var rowsAffected = await db.ExecuteAsync(insertVisitorSql, visitorParameters);
                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to create lead visitor record.");
                }


                // PREPARE MIDTRANS PAYMENT REQUEST
                var midtransService = new MidtransService();
                var transactionRequest = new MidtransTransactionRequest
                {
                    transaction_details = new TransactionDetails
                    {
                        order_id = orderCode,
                        gross_amount = (long)paymentData.TotalAmount
                    },
                    customer_details = new CustomerDetails
                    {
                        first_name = paymentData.FullName,
                        phone = paymentData.MobileNumber,
                        id_number = paymentData.IdNumber,
                        email = SessionManager.CurrentUserEmail ?? "customer@splashgo.com"
                    },
                    item_details = new List<ItemDetail>
                    {
                        new ItemDetail
                        {
                            id = _destination.DestinationId.ToString(),
                            name = _destination.Name,
                            quantity = paymentData.PaxCount,
                            price = (long)_destination.Price
                        }
                    }
                };

                // GET SNAP TOKEN FROM MIDTRANS
                string snapToken = await midtransService.GetSnapTokenAsync(transactionRequest);

                if (string.IsNullOrEmpty(snapToken))
                {
                    throw new Exception("Failed to generate payment token from Midtrans.");
                }

                // SAVE SNAP TOKEN TO BOOKING RECORD
                string updateTokenSql = "UPDATE bookings SET snap_token = @SnapToken WHERE bookingid = @BookingId";
                await db.ExecuteAsync(updateTokenSql, new Dictionary<string, object>
                {
                    { "@SnapToken", snapToken },
                    { "@BookingId", bookingId }
                });

                // OPEN MIDTRANS PAYMENT PAGE IN DEFAULT BROWSER
                string snapUrl = $"https://app.sandbox.midtrans.com/snap/v2/vtweb/{snapToken}";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = snapUrl,
                    UseShellExecute = true
                });

                //  NAVIGATE TO TRANSACTION HISTORY PAGE
                Application.Current.Dispatcher.Invoke(() =>
                {
                    navService?.Navigate(new TransactionHistoryPage());
                });

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
        private int _remainingQuota;
        private string _status;

        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public bool IsAvailable { get; set; }

        public int RemainingQuota
        {
            get => _remainingQuota;
            set
            {
                _remainingQuota = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
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