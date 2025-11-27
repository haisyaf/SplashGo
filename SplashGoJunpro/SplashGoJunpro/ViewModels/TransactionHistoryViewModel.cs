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
    public class TransactionHistoryViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BookingTransaction> _transactions;
        private BookingTransaction _selectedTransaction;
        private bool _isLoading;
        private string _emptyMessage;

        public TransactionHistoryViewModel()
        {
            _transactions = new ObservableCollection<BookingTransaction>();
            _emptyMessage = "Loading transactions...";
            _isLoading = true;
        }

        #region Properties

        /// <summary>
        /// Collection of booking transactions
        /// </summary>
        public ObservableCollection<BookingTransaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTransactions));
            }
        }

        /// <summary>
        /// Selected transaction for detail view
        /// </summary>
        public BookingTransaction SelectedTransaction
        {
            get => _selectedTransaction;
            set
            {
                _selectedTransaction = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Loading state indicator
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Message to display when no transactions
        /// </summary>
        public string EmptyMessage
        {
            get => _emptyMessage;
            set
            {
                _emptyMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Check if there are any transactions
        /// </summary>
        public bool HasTransactions => _transactions != null && _transactions.Count > 0;

        #endregion

        #region Methods

        /// <summary>
        /// Load transaction history dari database
        /// </summary>
        public async Task LoadTransactionHistory()
        {
            try
            {
                IsLoading = true;

                // Check if user is logged in
                if (!SessionManager.IsLoggedIn)
                {
                    EmptyMessage = "Please login to view your transaction history";
                    Transactions = new ObservableCollection<BookingTransaction>();
                    return;
                }

                // TODO: Query dari database untuk mengambil transaction history
                // var db = new NeonDb();
                // string sql = @"
                //     SELECT 
                //         b.booking_id,
                //         b.order_code,
                //         b.destination_id,
                //         d.name as destination_name,
                //         d.location as destination_location,
                //         d.image_path as destination_image,
                //         b.full_name,
                //         b.id_number,
                //         b.mobile_number,
                //         b.booking_date,
                //         b.pax_count,
                //         b.total_amount,
                //         b.status,
                //         b.created_at
                //     FROM bookings b
                //     INNER JOIN destinations d ON b.destination_id = d.destination_id
                //     WHERE b.user_id = @UserId
                //     ORDER BY b.created_at DESC
                // ";
                // 
                // var parameters = new Dictionary<string, object>
                // {
                //     { "@UserId", SessionManager.CurrentUser.UserId }
                // };
                // 
                // var rows = await db.QueryAsync(sql, parameters);
                // 
                // var transactions = new ObservableCollection<BookingTransaction>();
                // 
                // foreach (var row in rows)
                // {
                //     transactions.Add(new BookingTransaction
                //     {
                //         BookingId = Convert.ToInt32(row["booking_id"]),
                //         OrderCode = row["order_code"].ToString(),
                //         DestinationId = Convert.ToInt32(row["destination_id"]),
                //         DestinationName = row["destination_name"].ToString(),
                //         DestinationLocation = row["destination_location"].ToString(),
                //         DestinationImage = row["destination_image"].ToString(),
                //         FullName = row["full_name"].ToString(),
                //         IdNumber = row["id_number"].ToString(),
                //         MobileNumber = row["mobile_number"].ToString(),
                //         BookingDate = Convert.ToDateTime(row["booking_date"]),
                //         PaxCount = Convert.ToInt32(row["pax_count"]),
                //         TotalAmount = Convert.ToDecimal(row["total_amount"]),
                //         Status = row["status"].ToString(),
                //         CreatedAt = Convert.ToDateTime(row["created_at"])
                //     });
                // }
                // 
                // Transactions = transactions;

                // For now, generate dummy data for demonstration
                await Task.Delay(1000); // Simulate database delay

                var dummyTransactions = new ObservableCollection<BookingTransaction>
                {
                    new BookingTransaction
                    {
                        BookingId = 1,
                        OrderCode = "64_74U_4P4",
                        DestinationId = 1,
                        DestinationName = "Hotel Malioboro Tipe smth smth",
                        DestinationLocation = "Lokasinya",
                        DestinationImage = "/Images/malioboro.jpg",
                        FullName = "John Doe",
                        IdNumber = "1234567890",
                        MobileNumber = "08123456789",
                        BookingDate = new DateTime(2025, 11, 23),
                        PaxCount = 2,
                        TotalAmount = 2000000,
                        Status = "Paid",
                        CreatedAt = DateTime.Now.AddDays(-5)
                    },
                    new BookingTransaction
                    {
                        BookingId = 2,
                        OrderCode = "64_74U_4P5",
                        DestinationId = 2,
                        DestinationName = "Pantai Parangtritis",
                        DestinationLocation = "Bantul",
                        DestinationImage = "/Images/parangtritis.jpg",
                        FullName = "John Doe",
                        IdNumber = "1234567890",
                        MobileNumber = "08123456789",
                        BookingDate = new DateTime(2025, 11, 15),
                        PaxCount = 4,
                        TotalAmount = 1500000,
                        Status = "Paid",
                        CreatedAt = DateTime.Now.AddDays(-10)
                    }
                };

                Transactions = dummyTransactions;

                if (Transactions.Count == 0)
                {
                    EmptyMessage = "No transactions found";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load transaction history: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                EmptyMessage = "Failed to load transactions";
                Transactions = new ObservableCollection<BookingTransaction>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Refresh transaction list
        /// </summary>
        public async Task RefreshTransactions()
        {
            await LoadTransactionHistory();
        }

        /// <summary>
        /// Filter transactions by status
        /// </summary>
        public async Task FilterByStatus(string status)
        {
            try
            {
                IsLoading = true;

                // TODO: Query dari database dengan filter status
                // var db = new NeonDb();
                // string sql = @"
                //     SELECT 
                //         b.booking_id,
                //         b.order_code,
                //         b.destination_id,
                //         d.name as destination_name,
                //         d.location as destination_location,
                //         d.image_path as destination_image,
                //         b.full_name,
                //         b.id_number,
                //         b.mobile_number,
                //         b.booking_date,
                //         b.pax_count,
                //         b.total_amount,
                //         b.status,
                //         b.created_at
                //     FROM bookings b
                //     INNER JOIN destinations d ON b.destination_id = d.destination_id
                //     WHERE b.user_id = @UserId AND b.status = @Status
                //     ORDER BY b.created_at DESC
                // ";
                // 
                // var parameters = new Dictionary<string, object>
                // {
                //     { "@UserId", SessionManager.CurrentUser.UserId },
                //     { "@Status", status }
                // };
                // 
                // var rows = await db.QueryAsync(sql, parameters);
                // Process rows and update Transactions

                // For now, just filter the existing dummy data
                await Task.Delay(500);

                if (string.IsNullOrEmpty(status) || status == "All")
                {
                    await LoadTransactionHistory();
                }
                else
                {
                    var filtered = _transactions.Where(t => t.Status == status).ToList();
                    Transactions = new ObservableCollection<BookingTransaction>(filtered);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to filter transactions: {ex.Message}",
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
        /// Search transactions by order code or destination name
        /// </summary>
        public async Task SearchTransactions(string searchText)
        {
            try
            {
                IsLoading = true;

                // TODO: Query dari database dengan search
                // var db = new NeonDb();
                // string sql = @"
                //     SELECT 
                //         b.booking_id,
                //         b.order_code,
                //         -- other fields...
                //     FROM bookings b
                //     INNER JOIN destinations d ON b.destination_id = d.destination_id
                //     WHERE b.user_id = @UserId 
                //     AND (b.order_code LIKE @SearchText OR d.name LIKE @SearchText)
                //     ORDER BY b.created_at DESC
                // ";
                // 
                // var parameters = new Dictionary<string, object>
                // {
                //     { "@UserId", SessionManager.CurrentUser.UserId },
                //     { "@SearchText", $"%{searchText}%" }
                // };
                // 
                // var rows = await db.QueryAsync(sql, parameters);
                // Process rows and update Transactions

                // For now, just filter the existing dummy data
                await Task.Delay(500);

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    await LoadTransactionHistory();
                }
                else
                {
                    var filtered = _transactions.Where(t =>
                        t.OrderCode.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.DestinationName.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList();

                    Transactions = new ObservableCollection<BookingTransaction>(filtered);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to search transactions: {ex.Message}",
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

    #region Helper Classes

    /// <summary>
    /// Model untuk booking transaction
    /// </summary>
    public class BookingTransaction : INotifyPropertyChanged
    {
        private int _bookingId;
        private string _orderCode;
        private int _destinationId;
        private string _destinationName;
        private string _destinationLocation;
        private string _destinationImage;
        private string _fullName;
        private string _idNumber;
        private string _mobileNumber;
        private DateTime _bookingDate;
        private int _paxCount;
        private decimal _totalAmount;
        private string _status;
        private DateTime _createdAt;

        public int BookingId
        {
            get => _bookingId;
            set
            {
                _bookingId = value;
                OnPropertyChanged();
            }
        }

        public string OrderCode
        {
            get => _orderCode;
            set
            {
                _orderCode = value;
                OnPropertyChanged();
            }
        }

        public int DestinationId
        {
            get => _destinationId;
            set
            {
                _destinationId = value;
                OnPropertyChanged();
            }
        }

        public string DestinationName
        {
            get => _destinationName;
            set
            {
                _destinationName = value;
                OnPropertyChanged();
            }
        }

        public string DestinationLocation
        {
            get => _destinationLocation;
            set
            {
                _destinationLocation = value;
                OnPropertyChanged();
            }
        }

        public string DestinationImage
        {
            get => _destinationImage;
            set
            {
                _destinationImage = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public string IdNumber
        {
            get => _idNumber;
            set
            {
                _idNumber = value;
                OnPropertyChanged();
            }
        }

        public string MobileNumber
        {
            get => _mobileNumber;
            set
            {
                _mobileNumber = value;
                OnPropertyChanged();
            }
        }

        public DateTime BookingDate
        {
            get => _bookingDate;
            set
            {
                _bookingDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BookingDateFormatted));
            }
        }

        public string BookingDateFormatted => BookingDate.ToString("dd MMMM yyyy");

        public int PaxCount
        {
            get => _paxCount;
            set
            {
                _paxCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PaxDisplay));
            }
        }

        public string PaxDisplay => $"PAX ({PaxCount}x)";

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAmountFormatted));
            }
        }

        public string TotalAmountFormatted
        {
            get
            {
                var culture = new System.Globalization.CultureInfo("id-ID");
                return $"Rp {TotalAmount.ToString("N0", culture)}";
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
            }
        }

        /// <summary>
        /// Get status color based on status value
        /// Paid = Green, Pending = Orange, Cancelled = Red
        /// </summary>
        public string StatusColor
        {
            get
            {
                if (Status == null) return "#9E9E9E";

                switch (Status.ToLower())
                {
                    case "paid":
                        return "#4CAF50";
                    case "pending":
                        return "#FF9800";
                    case "cancelled":
                        return "#F44336";
                    default:
                        return "#9E9E9E";
                }
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}