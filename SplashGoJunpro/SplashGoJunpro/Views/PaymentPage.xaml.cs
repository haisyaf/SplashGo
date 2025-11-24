using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Models;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for PaymentPage.xaml
    /// </summary>
    public partial class PaymentPage : Page
    {
        private PaymentViewModel _viewModel;

        public PaymentPage(Destination destination)
        {
            InitializeComponent();
            _viewModel = new PaymentViewModel(destination);
            DataContext = _viewModel;

            // Load available dates
            _ = _viewModel.LoadAvailableDates();
        }

        /// <summary>
        /// Event handler untuk date button click
        /// </summary>
        private void DateButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is AvailableDate selectedDate)
            {
                _viewModel.SelectDate(selectedDate);

                // Update visual state
                UpdateDateButtonStyles();
            }
        }

        /// <summary>
        /// Update visual state untuk date buttons
        /// </summary>
        private void UpdateDateButtonStyles()
        {
            // TODO: Implement visual feedback for selected date
            // You can iterate through ItemsControl and update Border styles
        }

        /// <summary>
        /// Event handler untuk decrease PAX button
        /// </summary>
        private void DecreasePax_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DecreasePax();
        }

        /// <summary>
        /// Event handler untuk increase PAX button
        /// </summary>
        private void IncreasePax_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IncreasePax();
        }

        /// <summary>
        /// Event handler untuk Pay Now button
        /// </summary>
        private void PayNow_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter your full name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(IdNumberTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter your ID number.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(MobileNumberTextBox.Text))
            {
                MessageBox.Show(
                    "Please enter your mobile number.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (_viewModel.SelectedDate == null)
            {
                MessageBox.Show(
                    "Please select a date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Process payment
            var paymentData = new PaymentData
            {
                FullName = FullNameTextBox.Text,
                IdNumber = IdNumberTextBox.Text,
                MobileNumber = MobileNumberTextBox.Text,
                SelectedDate = _viewModel.SelectedDate,
                PaxCount = _viewModel.PaxCount,
                TotalAmount = _viewModel.TotalPriceValue
            };

            _viewModel.ProcessPayment(paymentData);
        }

        /// <summary>
        /// Event handler untuk cancel button
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }
    }
}