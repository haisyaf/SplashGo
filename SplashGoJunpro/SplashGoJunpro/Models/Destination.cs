using System;
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
        private bool _isBookmarked;

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

        public bool IsBookmarked
        {
            get => _isBookmarked;
            set
            {
                _isBookmarked = value;
                OnPropertyChanged();
            }
        }

        public void UpdateInfo(string name, string location, string desc, decimal price)
        {
            Name = name;
            Location = location;
            Description = desc;
            Price = price;
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}