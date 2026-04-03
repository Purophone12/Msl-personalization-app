using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using DomainPersonalizationSetup.Services;

namespace DomainPersonalizationSetup.Views
{
    public partial class WallpaperPage : Page
    {
        private readonly PersonalizationService _personalizationService = new();
        private string? _selectedWallpaperPath;

        public WallpaperPage()
        {
            InitializeComponent();
        }

        private void BrowseWallpaper_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _selectedWallpaperPath = dialog.FileName;
                SelectedFilePathText.Text = _selectedWallpaperPath;
                ApplyWallpaperBtn.IsEnabled = true;
            }
        }

        private void ApplyWallpaper_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedWallpaperPath))
            {
                _personalizationService.SetWallpaper(_selectedWallpaperPath);
                MessageBox.Show("Wallpaper applied successfully!", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
