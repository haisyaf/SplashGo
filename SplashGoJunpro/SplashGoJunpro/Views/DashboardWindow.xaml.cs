using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;

namespace SplashGoJunpro.Views
{
    public partial class DashboardWindow : Window
    {
        private DashboardViewModel _viewModel;

        public DashboardWindow()
        {
            InitializeComponent();

            // Initialize ViewModel
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;
        }

        // Event handlers untuk sidebar buttons
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Already on dashboard
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to History window
            // var historyWindow = new HistoryWindow();
            // historyWindow.Show();
            // this.Close();
        }

        private void SavedButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Saved window
            // var savedWindow = new SavedWindow();
            // savedWindow.Show();
            // this.Close();
        }

        private void AddBusinessButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Add Business window
            // var addBusinessWindow = new AddBusinessWindow();
            // addBusinessWindow.Show();
            // this.Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Profile window
            // var profileWindow = new ProfileWindow();
            // profileWindow.Show();
            // this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Logout Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Navigate to Login window
                // var loginWindow = new LoginWindow();
                // loginWindow.Show();
                // this.Close();

                MessageBox.Show("Logged out successfully!");
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && _viewModel != null)
            {
                string category = button.Tag?.ToString() ?? string.Empty;

                // Cek apakah kategori yang sama diklik lagi
                bool isSameCategory = _viewModel.SelectedCategory == category;

                _viewModel.SelectCategoryCommand.Execute(category);

                // Update button styles visually
                UpdateCategoryButtonStyles(button, isSameCategory);
            }
        }

        private void UpdateCategoryButtonStyles(Button selectedButton, bool deselectAll)
        {
            // Find parent StackPanel containing all category buttons
            var parent = selectedButton.Parent as StackPanel;
            if (parent != null)
            {
                foreach (var child in parent.Children)
                {
                    if (child is Button btn)
                    {
                        if (deselectAll)
                        {
                            // Deselect all buttons (All mode)
                            btn.Style = (Style)FindResource("CategoryButtonStyle");
                        }
                        else if (btn == selectedButton)
                        {
                            // Select clicked button
                            btn.Style = (Style)FindResource("ActiveCategoryButtonStyle");
                        }
                        else
                        {
                            // Deselect other buttons
                            btn.Style = (Style)FindResource("CategoryButtonStyle");
                        }
                    }
                }
            }
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Models.Destination destination && _viewModel != null)
            {
                _viewModel.ToggleBookmarkCommand.Execute(destination);

                // Update bookmark icon color
                UpdateBookmarkIcon(button, destination.IsBookmarked);
            }
        }

        private void UpdateBookmarkIcon(Button button, bool isBookmarked)
        {
            if (button.Content is System.Windows.Shapes.Path path)
            {
                path.Fill = isBookmarked
                    ? (System.Windows.Media.SolidColorBrush)FindResource("PrimaryBlue")
                    : (System.Windows.Media.SolidColorBrush)FindResource("TextGray");
            }
        }

        private void BeachSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                _viewModel.BeachSearchText = textBox.Text;
                _viewModel.SearchCommand.Execute(null);
            }
        }

        private void ActivitySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                _viewModel.ActivitySearchText = textBox.Text;
                _viewModel.SearchCommand.Execute(null);
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Clear placeholder text when focused
                if (textBox.Text == "Search for beaches..." || textBox.Text == "Search for activities...")
                {
                    textBox.Text = string.Empty;
                }
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Restore placeholder text if empty
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox.Name == "BeachSearchBox")
                    {
                        textBox.Text = "Search for beaches...";
                        // Clear filter when empty
                        if (_viewModel != null)
                        {
                            _viewModel.BeachSearchText = string.Empty;
                            _viewModel.SearchCommand.Execute(null);
                        }
                    }
                    else if (textBox.Name == "ActivitySearchBox")
                    {
                        textBox.Text = "Search for activities...";
                        // Clear filter when empty
                        if (_viewModel != null)
                        {
                            _viewModel.ActivitySearchText = string.Empty;
                            _viewModel.SearchCommand.Execute(null);
                        }
                    }
                }
            }
        }

        private void PriceToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle button akan otomatis toggle popup via binding
        }

        private void ApplyPriceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.FilterPriceCommand.Execute(null);

                // Close popup
                PriceToggleButton.IsChecked = false;
            }
        }

        private void PriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _viewModel != null)
            {
                _viewModel.FilterPriceCommand.Execute(null);

                // Close popup
                PriceToggleButton.IsChecked = false;
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem && _viewModel != null)
            {
                string sortOption = selectedItem.Content.ToString();
                _viewModel.SelectedSortOption = sortOption;
            }
        }
    }
}