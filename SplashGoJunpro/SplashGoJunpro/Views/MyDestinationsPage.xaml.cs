using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using SplashGoJunpro.ViewModels;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for MyDestinationsPage.xaml
    /// </summary>
    public partial class MyDestinationsPage : Page
    {
        private MyDestinationsViewModel _viewModel;

        public MyDestinationsPage()
        {
            InitializeComponent();
            _viewModel = new MyDestinationsViewModel();
            this.DataContext = _viewModel;

            // Subscribe to property changes to update charts
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // Load destinations on page load
            Loaded += MyDestinationsPage_Loaded;
        }

        /// <summary>
        /// Event handler ketika page loaded
        /// </summary>
        private async void MyDestinationsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.LoadMyDestinations();
                UpdateCharts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading page: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        /// <summary>
        /// Update charts when data changes
        /// </summary>
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.Destinations) ||
                e.PropertyName == nameof(_viewModel.Analytics))
            {
                Dispatcher.Invoke(() => UpdateCharts());
            }
        }

        /// <summary>
        /// Update chart data
        /// </summary>
        private void UpdateCharts()
        {
            if (_viewModel.Destinations == null || _viewModel.Destinations.Count == 0)
                return;

            try
            {
                // Update Revenue Chart
                UpdateRevenueChart();

                // Update Category Distribution Chart
                UpdateCategoryChart();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating charts: {ex.Message}");
                // Don't show error to user, just log it
            }
        }

        /// <summary>
        /// Update revenue chart with monthly data (Revenue only)
        /// </summary>
        private void UpdateRevenueChart()
        {
            // Aggregate monthly data from all destinations
            var allMonthlyData = new Dictionary<string, decimal>();

            foreach (var dest in _viewModel.Destinations)
            {
                if (dest.MonthlyData != null)
                {
                    foreach (var monthData in dest.MonthlyData)
                    {
                        if (!allMonthlyData.ContainsKey(monthData.Month))
                        {
                            allMonthlyData[monthData.Month] = 0;
                        }

                        allMonthlyData[monthData.Month] += monthData.Revenue;
                    }
                }
            }

            // Create chart series (Revenue only)
            var revenueValues = new ChartValues<decimal>();
            var labels = new List<string>();

            foreach (var kvp in allMonthlyData.OrderBy(x => x.Key))
            {
                labels.Add(kvp.Key);
                revenueValues.Add(kvp.Value);
            }

            var series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Revenue",
                    Values = revenueValues,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 8,
                    Fill = System.Windows.Media.Brushes.Transparent,
                    Stroke = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(255, 152, 0)),
                    StrokeThickness = 3
                }
            };

            // Update chart
            RevenueChart.Series = series;
            RevenueChart.AxisX[0].Labels = labels;

            // Format Y axis for revenue
            RevenueChart.AxisY[0].LabelFormatter = value => $"Rp {(value / 1000000):F1}M";
        }

        /// <summary>
        /// Update category distribution pie chart
        /// </summary>
        private void UpdateCategoryChart()
        {
            if (_viewModel.Analytics?.CategoryDistribution == null)
                return;

            var series = new SeriesCollection();

            foreach (var category in _viewModel.Analytics.CategoryDistribution)
            {
                series.Add(new PieSeries
                {
                    Title = $"{category.Category} ({category.Count})",
                    Values = new ChartValues<double> { category.Count },
                    DataLabels = true,
                    LabelPoint = point => $"{point.Y} ({category.PercentageFormatted})"
                });
            }

            CategoryChart.Series = series;
        }

        /// <summary>
        /// Event handler untuk category filter
        /// </summary>
        private async void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;

            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string category = selectedItem.Tag?.ToString();
                await _viewModel.FilterByCategory(category);
            }
        }
    }
}