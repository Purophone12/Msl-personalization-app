using System.Windows;
using System.Windows.Controls;
using DomainPersonalizationSetup.Core;
using DomainPersonalizationSetup.Services;

namespace DomainPersonalizationSetup.Views
{
    public partial class WelcomePage : Page
    {
        private readonly PersonalizationService _personalizationService = new();

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
            if (!string.IsNullOrWhiteSpace(NicknameInput.Text))
            {
                _personalizationService.SetNickname(NicknameInput.Text);
            }
        }
    }
}
