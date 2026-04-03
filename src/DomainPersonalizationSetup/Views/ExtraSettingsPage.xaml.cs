using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using DomainPersonalizationSetup.Core;
using DomainPersonalizationSetup.ViewModels;

namespace DomainPersonalizationSetup.Views
{
    public partial class ExtraSettingsPage : Page
    {
        private MainViewModel? ViewModel => Application.Current.MainWindow.DataContext as MainViewModel;

        public ExtraSettingsPage()
        {
            InitializeComponent();
            TaskbarSettings.Visibility = ConfigManager.Current.Features.EnableTaskbar ? Visibility.Visible : Visibility.Collapsed;
            DefaultAppsSettings.Visibility = ConfigManager.Current.Features.EnableDefaultApps ? Visibility.Visible : Visibility.Collapsed;

            if (ViewModel != null && ViewModel.PendingTaskbarCenter.HasValue)
            {
                CenterAlignmentRadio.IsChecked = ViewModel.PendingTaskbarCenter.Value;
                LeftAlignmentRadio.IsChecked = !ViewModel.PendingTaskbarCenter.Value;
            }
        }

        private void ApplyAlignment_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PendingTaskbarCenter = CenterAlignmentRadio.IsChecked == true;
                MessageBox.Show("Taskbar alignment updated! Settings will be applied when you click Finish.", "Personalization", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
