using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using DomainPersonalizationSetup.Services;
using DomainPersonalizationSetup.Core;

namespace DomainPersonalizationSetup.Views
{
    public partial class ExtraSettingsPage : Page
    {
        private readonly PersonalizationService _personalizationService = new();

        public ExtraSettingsPage()
        {
            InitializeComponent();
            TaskbarSettings.Visibility = ConfigManager.Current.Features.EnableTaskbar ? Visibility.Visible : Visibility.Collapsed;
            DefaultAppsSettings.Visibility = ConfigManager.Current.Features.EnableDefaultApps ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyAlignment_Click(object sender, RoutedEventArgs e)
        {
            bool center = CenterAlignmentRadio.IsChecked == true;
            _personalizationService.SetTaskbarAlignment(center);
            MessageBox.Show("Taskbar alignment updated successfully!", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenDefaultApps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo("ms-settings:defaultapps") { UseShellExecute = true });
            }
            catch (System.Exception ex)
            {
                Logger.Error("Error opening default apps settings", ex);
                MessageBox.Show("Could not open the Settings app.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
