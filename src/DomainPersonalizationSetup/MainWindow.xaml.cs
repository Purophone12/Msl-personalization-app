using System.Windows;
using System.Windows.Controls;
using DomainPersonalizationSetup.ViewModels;
using DomainPersonalizationSetup.Core;

namespace DomainPersonalizationSetup
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(Navigate);
        }

        private void Navigate(UIElement page)
        {
            MainFrame.Navigate(page);
        }
    }
}
