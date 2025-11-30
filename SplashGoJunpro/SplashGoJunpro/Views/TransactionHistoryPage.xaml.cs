using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for TransactionHistoryPage.xaml
    /// </summary>
    public partial class TransactionHistoryPage : Page
    {
        private TransactionHistoryViewModel _viewModel;

        public TransactionHistoryPage()
        {
            InitializeComponent();
            _viewModel = new TransactionHistoryViewModel();
            DataContext = _viewModel;

            // Load transaction history on page load
            Loaded += TransactionHistoryPage_Loaded;
        }

        /// <summary>
        /// Event handler ketika page loaded
        /// </summary>
        private async void TransactionHistoryPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadTransactionHistory();
        }

        /// <summary>
        /// Event handler untuk refresh button
        /// </summary>
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.RefreshTransactions();
        }

        /// <summary>
        /// Event handler untuk search textbox - search saat user ketik
        /// </summary>
        private async void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Debounce search - tunggu user selesai ketik
                // TODO: Implement proper debouncing untuk performance yang lebih baik
                await _viewModel.SearchTransactions(textBox.Text);
            }
        }

        /// <summary>
        /// Event handler untuk filter status combobox
        /// </summary>
        private async void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string status = selectedItem.Tag?.ToString();
                await _viewModel.FilterByStatus(status);
            }
        }

        /// <summary>
        /// Event handler untuk back button
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Event handler untuk tombol bayar
        /// </summary>
        private async void PayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                // Ambil transaction dari Tag
                var transaction = button.Tag;

                // Panggil method untuk proses pembayaran di ViewModel
                await _viewModel.ProcessPayment(transaction);
            }
        }

        /// <summary>
        /// Event handler untuk tombol cancel
        /// </summary>
        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                // Ambil transaction dari Tag
                var transaction = button.Tag;

                // Konfirmasi dulu sebelum cancel
                var result = MessageBox.Show(
                    "Are you sure you want to cancel this transaction?",
                    "Cancel Transaction",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Panggil method untuk cancel transaksi di ViewModel
                    await _viewModel.CancelTransaction(transaction);
                }
            }
        }
    }
}