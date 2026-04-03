using System.Windows;
using System.Windows.Controls;
using DomainPersonalizationSetup.Core;
using DomainPersonalizationSetup.ViewModels;

namespace DomainPersonalizationSetup.Views
{
    public partial class WelcomePage : Page
    {
        private MainViewModel? ViewModel => Application.Current.MainWindow.DataContext as MainViewModel;

        public WelcomePage()
        {
            InitializeComponent();
            if (ConfigManager.Current.Features.EnableNickname)
            {
                NicknameSection.Visibility = Visibility.Visible;
            }
        }

        private void NicknameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PendingNickname = NicknameInput.Text;
            }
        }
    }
}
