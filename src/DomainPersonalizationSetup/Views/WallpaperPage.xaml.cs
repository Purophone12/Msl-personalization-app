using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using DomainPersonalizationSetup.ViewModels;

namespace DomainPersonalizationSetup.Views
{
    public partial class WallpaperPage : Page
    {
        private MainViewModel? ViewModel => Application.Current.MainWindow.DataContext as MainViewModel;

        public WallpaperPage()
        {
            InitializeComponent();
            if (ViewModel != null && !string.IsNullOrEmpty(ViewModel.PendingWallpaper))
            {
                SelectedFilePathText.Text = ViewModel.PendingWallpaper;
                ApplyWallpaperBtn.IsEnabled = true;
            }
        }

        private void BrowseWallpaper_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                if (ViewModel != null)
                {
                    ViewModel.PendingWallpaper = dialog.FileName;
                    SelectedFilePathText.Text = ViewModel.PendingWallpaper;
                    ApplyWallpaperBtn.IsEnabled = true;
                }
            }
        }

        private void ApplyWallpaper_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wallpaper selected! It will be applied when you click Finish.", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
