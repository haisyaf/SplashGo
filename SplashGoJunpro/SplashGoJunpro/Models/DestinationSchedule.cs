using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class DestinationSchedule : INotifyPropertyChanged
    {
        private int _dayOfWeek;
        private TimeSpan? _openTime;
        private TimeSpan? _closeTime;
        private int _destinationId;

        public DateTime Date { get; set; }
        public string DateString { get; set; }
        public bool IsAvailable { get; set; }
        public int RemainingQuota { get; set; }

        public int DayOfWeek
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

        public int DestinationId
        {
            get => _destinationId;
            set { _destinationId = value; OnPropertyChanged(); }
        }

        public string Display =>
            OpenTime == null || CloseTime == null
            ? $"{DayOfWeekToName(DayOfWeek)} • Closed"
            : $"{DayOfWeekToName(DayOfWeek)} • {OpenTime:hh\\:mm}–{CloseTime:hh\\:mm}";

        public static string DayOfWeekToName(int dow)
        {
            string[] names =
                { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

            return (dow >= 0 && dow <= 6) ? names[dow] : "Unknown";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
