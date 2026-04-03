using System.Windows;

namespace DomainPersonalizationSetup.Views
{
    public partial class DomainWarningDialog : Window
    {
        public bool ShouldContinue { get; private set; }

        public DomainWarningDialog()
        {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ShouldContinue = true;
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ShouldContinue = false;
            this.Close();
        }
    }
}
