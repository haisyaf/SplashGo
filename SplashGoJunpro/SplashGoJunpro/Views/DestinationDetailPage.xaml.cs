using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Models;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for DestinationDetailPage.xaml
    /// </summary>
    public partial class DestinationDetailPage : Page
    {
        public DestinationDetailPage(int destinationId)
        {
            InitializeComponent();
            var vm = new DestinationDetailViewModel(null);
            DataContext = vm;
            _ = vm.LoadDestinationDetails(destinationId);
        }

        public DestinationDetailPage(Destination destination)
        {
            InitializeComponent();
            var vm = new DestinationDetailViewModel(destination);
            DataContext = vm;
            if (destination != null)
            {
                // Optionally, load details if you want to refresh from DB
                _ = vm.LoadDestinationDetails(destination.DestinationId);
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
        /// Event handler saat similar card diklik
        /// </summary>
        private void SimilarCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Destination destination)
            {
                // Navigate to detail page for the clicked destination
                var detailPage = new DestinationDetailPage(destination);
                NavigationService?.Navigate(detailPage);
            }
        }
    }
}