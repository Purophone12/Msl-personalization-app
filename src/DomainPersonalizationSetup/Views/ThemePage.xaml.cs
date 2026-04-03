using System.Windows;
using System.Windows.Controls;
using DomainPersonalizationSetup.Services;

namespace DomainPersonalizationSetup.Views
{
    public partial class ThemePage : Page
    {
        private readonly PersonalizationService _personalizationService = new();

        public ThemePage()
        {
            InitializeComponent();
        }

        private void ApplyTheme_Click(object sender, RoutedEventArgs e)
        {
            bool isDark = DarkThemeRadio.IsChecked == true;
            _personalizationService.SetTheme(isDark);
            MessageBox.Show($"{(isDark ? "Dark" : "Light")} theme applied successfully!", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
