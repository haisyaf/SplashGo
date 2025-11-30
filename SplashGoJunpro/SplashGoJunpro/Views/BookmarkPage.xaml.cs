using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SplashGoJunpro.ViewModels;
using SplashGoJunpro.Models;

namespace SplashGoJunpro.Views
{
    /// <summary>
    /// Interaction logic for BookmarkPage.xaml
    /// </summary>
    public partial class BookmarkPage : Page
    {
        private BookmarkViewModel ViewModel => DataContext as BookmarkViewModel;

        public BookmarkPage()
        {
            InitializeComponent();
            DataContext = new BookmarkViewModel();

            // Subscribe to Loaded event untuk refresh bookmarks
            Loaded += BookmarkPage_Loaded;
        }

        /// <summary>
        /// Event handler saat bookmark card diklik untuk melihat detail
        /// </summary>
        private void BookmarkCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is Destination destination)
            {
                // Navigate to detail page
                var detailPage = new DestinationDetailPage(destination);
                NavigationService?.Navigate(detailPage);
            }
        }

        /// <summary>
        /// Event handler saat page loaded untuk refresh bookmarks
        /// </summary>
        private async void BookmarkPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                await ViewModel.RefreshBookmarks();
            }
        }
    }
}