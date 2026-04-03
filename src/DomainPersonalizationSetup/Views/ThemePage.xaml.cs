using System.Windows;
using System.Windows.Controls;
using DomainPersonalizationSetup.ViewModels;

namespace DomainPersonalizationSetup.Views
{
    public partial class ThemePage : Page
    {
        private MainViewModel? ViewModel => Application.Current.MainWindow.DataContext as MainViewModel;

        public ThemePage()
        {
            InitializeComponent();
            if (ViewModel != null && ViewModel.PendingIsDark.HasValue)
            {
                DarkThemeRadio.IsChecked = ViewModel.PendingIsDark.Value;
                LightThemeRadio.IsChecked = !ViewModel.PendingIsDark.Value;
            }
        }

        private void ApplyTheme_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PendingIsDark = DarkThemeRadio.IsChecked == true;
                MessageBox.Show($"Theme preference updated! Settings will be applied when you click Finish.", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
