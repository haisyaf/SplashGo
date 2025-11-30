using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        //private int? _dayOfWeek;
        //private TimeSpan? _openTime;
        //private TimeSpan? _closeTime;
        private ObservableCollection<DestinationSchedule> _schedule;


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

        //public int? DayOfWeek
        //{
        //    get => _dayOfWeek;
        //    set { _dayOfWeek = value; OnPropertyChanged(); }
        //}

        //public TimeSpan? OpenTime
        //{
        //    get => _openTime;
        //    set { _openTime = value; OnPropertyChanged(); }
        //}

        //public TimeSpan? CloseTime
        //{
        //    get => _closeTime;
        //    set { _closeTime = value; OnPropertyChanged(); }
        //}

        //public List<DestinationSchedule> Schedule
        //{
        //    get => _schedule;
        //    set { _schedule = value; OnPropertyChanged(); }
        //}

        //private ObservableCollection<DestinationSchedule> _schedule
        //    = new ObservableCollection<DestinationSchedule>();

        public ObservableCollection<DestinationSchedule> Schedule
        {
            get => _schedule;
            set
            {
                _schedule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TodayScheduleDisplay)); // dependent
                OnPropertyChanged(nameof(ScheduleRotated));     // dependent
            }
        }

        public string TodayScheduleDisplay
        {
            get
            {
                if (Schedule == null || Schedule.Count == 0)
                    return "No schedule data";

                foreach (var s in Schedule)
                {
                    Debug.WriteLine($"DB Schedule Day: {s.DayOfWeek}");
                }

                // Output all DayOfWeek values in Schedule
                var allDays = string.Join(", ", Schedule.Select(s => DestinationSchedule.DayOfWeekToName(s.DayOfWeek)));
                Debug.WriteLine($"All schedule days: {allDays}");

                int today = (int)DateTime.Now.DayOfWeek;
                var todaySch = Schedule.FirstOrDefault(x => x.DayOfWeek == today);
                
                return todaySch?.Display ?? "Closed today";
            }
        }


        public List<DestinationSchedule> ScheduleRotated
        {
            get
            {
                if (Schedule == null || Schedule.Count == 0)
                    return new List<DestinationSchedule>();

                int today = (int)DateTime.Now.DayOfWeek; // 0 = Sunday, 1 = Monday, ...

                return Schedule
                    .OrderBy(s => (s.DayOfWeek - today + 7) % 7)
                    .ToList();
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}