using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SplashGoJunpro.Models
{
    public class Destination : INotifyPropertyChanged
    {
        private int _destinationId;
        private string _name;
        private string _location;
        private string _description;
        private decimal _price;
        private string _category;
        private string _imagePath;
        private string _owner;
        private List<string> _offer;
        private int? _dayOfWeek;
        private TimeSpan? _openTime;
        private TimeSpan? _closeTime;

        public int DestinationId
        {
            get => _destinationId;
            set
            {
                _destinationId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                _location = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        // Harga dalam IDR
        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedPrice));
            }
        }

        // Properti tambahan untuk format harga
        public string FormattedPrice
        {
            get
            {
                return $"Rp {Price:N0}".Replace(",", ".");
            }
        }

        // Properties tambahan untuk MVVM
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        public string Owner
        {
            get => _owner;
            set { _owner = value; OnPropertyChanged(); }
        }


        public List<string> Offer
        {
            get => _offer;
            set { _offer = value; OnPropertyChanged(); }
        }

        public int? DayOfWeek
        {
            get => _dayOfWeek;
            set { _dayOfWeek = value; OnPropertyChanged(); }
        }

        public TimeSpan? OpenTime
        {
            get => _openTime;
            set { _openTime = value; OnPropertyChanged(); }
        }

        public TimeSpan? CloseTime
        {
            get => _closeTime;
            set { _closeTime = value; OnPropertyChanged(); }
        }

        public string ScheduleDisplay
        {
            get
            {
                if (DayOfWeek == null)
                    return "N/A";

                string dayName = DayOfWeekToName(DayOfWeek.Value);

                if (OpenTime == null || CloseTime == null)
                    return $"{dayName} · Closed";

                return $"{dayName} · {OpenTime:hh\\:mm}–{CloseTime:hh\\:mm}";
            }
        }

        public string ScheduleTimeDisplay
        {
            get
            {
                if (OpenTime == null || CloseTime == null)
                    return $"Closed";

                return $"{OpenTime:hh\\:mm}–{CloseTime:hh\\:mm}";
            }
        }

        // helper that maps 0..6 -> Sunday..Saturday (adjust if your DB uses different mapping)
        private static string DayOfWeekToName(int dow)
        {
            // ensure valid range
            string[] names = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            if (dow < 0 || dow > 6) return "Unknown";
            return names[dow];
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}