using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Models;
using System;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private DashboardViewModel ViewModel => DataContext as DashboardViewModel;

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        /// <summary>
        /// Event handler saat hotel card diklik
        /// </summary>
        private void HotelCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Destination destination)
            {
                // Navigate to detail page
                var detailPage = new DestinationDetailPage(destination);
                NavigationService?.Navigate(detailPage);
            }
        }

        /// <summary>
        /// Event handler untuk toggle price filter popup
        /// </summary>
        private void PriceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.IsPricePopupOpen = !ViewModel.IsPricePopupOpen;
            }
        }

        /// <summary>
        /// Event handler untuk close price filter popup
        /// </summary>
        private void ClosePriceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.IsPricePopupOpen = false;
            }
        }

        /// <summary>
        /// Event handler untuk clear price filter
        /// </summary>
        private void ClearPriceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PriceFrom = "";
                ViewModel.PriceTo = "";
                ViewModel.ApplyPriceFilterCommand?.Execute(null);
            }
        }

        private void PriceInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Save old text and cursor position
            string oldText = textBox.Text;
            int oldSelectionStart = textBox.SelectionStart;

            // Remove all dots for parsing
            string rawText = oldText.Replace(".", "");
            if (decimal.TryParse(rawText, out decimal value))
            {
                // Format with dot as thousands separator
                string formatted = string.Format("{0:N0}", value).Replace(",", ".");

                // Only update if formatting changes the text
                if (textBox.Text != formatted)
                {
                    int diff = formatted.Length - oldText.Length;
                    textBox.Text = formatted;

                    // Adjust cursor position: if a dot was added before the cursor, move cursor forward
                    textBox.SelectionStart = Math.Max(0, oldSelectionStart + diff);
                }
            }
        }
    }
}