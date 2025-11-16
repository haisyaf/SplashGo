using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Models;

namespace SplashGoJunpro.Views
{
    public partial class DashboardWindow : Window
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardWindow()
        {
            InitializeComponent();

            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            if (_viewModel != null)
            {
                _viewModel.NavigateToHistory += OnNavigateToHistory;
                _viewModel.NavigateToSaved += OnNavigateToSaved;
                _viewModel.NavigateToAddBusiness += OnNavigateToAddBusiness;
                _viewModel.NavigateToProfile += OnNavigateToProfile;
                _viewModel.NavigateToLogin += OnNavigateToLogin;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe to prevent memory leaks
            if (_viewModel != null)
            {
                _viewModel.NavigateToHistory -= OnNavigateToHistory;
                _viewModel.NavigateToSaved -= OnNavigateToSaved;
                _viewModel.NavigateToAddBusiness -= OnNavigateToAddBusiness;
                _viewModel.NavigateToProfile -= OnNavigateToProfile;
                _viewModel.NavigateToLogin -= OnNavigateToLogin;
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// EVENT HANDLERS
        /// </summary>
        private void OnNavigateToHistory(object sender, EventArgs e)
        {
            // Navigation logic remains in ViewModel; UI may open windows if ViewModel requests it.
            // Example placeholder - uncomment if you want window-level navigation here:
            // new HistoryWindow().Show();
            // Close();
        }

        private void OnNavigateToSaved(object sender, EventArgs e)
        {
            // new SavedWindow().Show();
            // Close();
        }

        private void OnNavigateToAddBusiness(object sender, EventArgs e)
        {
            // new AddBusinessWindow().Show();
            // Close();
        }

        private void OnNavigateToProfile(object sender, EventArgs e)
        {
            // new ProfileWindow().Show();
            // Close();
        }

        private void OnNavigateToLogin(object sender, EventArgs e)
        {
            // Dashboard requested navigation to Login. Handle at UI-level.
            new LoginWindow().Show();
            Close();
        }

        // ------------------------------
        // PURE UI BEHAVIOR (allowed)
        // ------------------------------
        private void UpdateBookmarkIcon(Button button, bool isBookmarked)
        {
            if (button?.Content is Path path)
            {
                var brush = isBookmarked
                    ? (SolidColorBrush)FindResource("PrimaryBlue")
                    : (SolidColorBrush)FindResource("TextGray");

                path.Fill = brush;
            }
        }

        private void UpdateCategoryButtonStyles(Button selectedButton, bool deselectAll)
        {
            if (selectedButton?.Parent is StackPanel parent)
            {
                foreach (var child in parent.Children)
                {
                    if (child is Button btn)
                    {
                        btn.Style = deselectAll
                            ? (Style)FindResource("CategoryButtonStyle")
                            : (btn == selectedButton)
                                ? (Style)FindResource("ActiveCategoryButtonStyle")
                                : (Style)FindResource("CategoryButtonStyle");
                    }
                }
            }
        }

        // Placeholder behavior: clear placeholder when focused, restore when lost
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (tb.Text == "Search for beaches..." || tb.Text == "Search for activities..." || tb.Text.Contains("Search"))
                {
                    tb.Text = string.Empty;
                }
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    if (tb.Name == "BeachSearchBox")
                    {
                        tb.Text = "Search for beaches...";
                        if (_viewModel != null)
                        {
                            _viewModel.BeachSearchText = string.Empty;
                            // Let ViewModel react to property change
                        }
                    }
                    else if (tb.Name == "ActivitySearchBox")
                    {
                        tb.Text = "Search for activities...";
                        if (_viewModel != null)
                        {
                            _viewModel.ActivitySearchText = string.Empty;
                            // Let ViewModel react to property change
                        }
                    }
                }
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && _viewModel != null)
            {
                string category = button.Tag?.ToString() ?? string.Empty;

                // Toggle selection: clear if same category clicked
                bool isSameCategory = string.Equals(_viewModel.SelectedCategory, category, StringComparison.Ordinal);

                _viewModel.SelectedCategory = isSameCategory ? string.Empty : category;

                // Update button styles visually
                UpdateCategoryButtonStyles(button, isSameCategory);
            }
        }

        private void BookmarkButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Destination destination)
            {
                if (_viewModel?.ToggleBookmarkCommand != null && _viewModel.ToggleBookmarkCommand.CanExecute(destination))
                {
                    // Prefer invoking the ViewModel command so business logic stays in ViewModel
                    _viewModel.ToggleBookmarkCommand.Execute(destination);
                }
                else
                {
                    // Fallback: toggle model property locally and update UI
                    destination.IsBookmarked = !destination.IsBookmarked;
                }

                // Update bookmark icon color to reflect the model
                UpdateBookmarkIcon(button, destination.IsBookmarked);
            }
        }

        private void BeachSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                _viewModel.BeachSearchText = textBox.Text;
                // Let ViewModel execute search reactively on property change (preferred)
            }
        }

        private void ActivitySearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && _viewModel != null)
            {
                _viewModel.ActivitySearchText = textBox.Text;
                // Let ViewModel execute search reactively on property change (preferred)
            }
        }

        private void PriceToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // UI-only: popup toggle handled by XAML bindings
        }

        private void ApplyPriceFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                if (_viewModel.FilterPriceCommand != null && _viewModel.FilterPriceCommand.CanExecute(null))
                    _viewModel.FilterPriceCommand.Execute(null);

                // Close popup if named control exists in XAML
                try
                {
                    if (FindName("PriceToggleButton") is System.Windows.Controls.Primitives.ToggleButton tb)
                        tb.IsChecked = false;
                }
                catch { /* ignore if control not available at runtime */ }
            }
        }

        private void PriceTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _viewModel != null)
            {
                if (_viewModel.FilterPriceCommand != null && _viewModel.FilterPriceCommand.CanExecute(null))
                    _viewModel.FilterPriceCommand.Execute(null);

                try
                {
                    if (FindName("PriceToggleButton") is System.Windows.Controls.Primitives.ToggleButton tb)
                        tb.IsChecked = false;
                }
                catch { /* ignore if control not available at runtime */ }
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem && _viewModel != null)
            {
                string sortOption = selectedItem.Content?.ToString() ?? string.Empty;
                _viewModel.SelectedSortOption = sortOption;

                // Optionally let ViewModel handle sorting reactively on property change
                if (_viewModel.SortCommand != null && _viewModel.SortCommand.CanExecute(null))
                {
                    _viewModel.SortCommand.Execute(null);
                }
            }
        }
    }
}