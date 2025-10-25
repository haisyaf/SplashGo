using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SplashGoJunpro.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels, implements INotifyPropertyChanged
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises PropertyChanged event
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets property value and raises PropertyChanged if value changed
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
