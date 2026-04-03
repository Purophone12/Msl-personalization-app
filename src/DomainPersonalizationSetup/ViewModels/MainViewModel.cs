// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DomainPersonalizationSetup.Core;
using DomainPersonalizationSetup.ViewModels;
using DomainPersonalizationSetup.Views;
using DomainPersonalizationSetup.Services;

namespace DomainPersonalizationSetup.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _organizationName = ConfigManager.Current.Branding.OrganizationName;
        private string? _logoPath = ConfigManager.Current.Branding.LogoPath;
        private string _accentColor = ConfigManager.Current.Branding.AccentColor ?? "#0078D4";
        private string _nextButtonText = "Next";
        private bool _showBackButton = false;

        public string OrganizationName { get => _organizationName; set { _organizationName = value; OnPropertyChanged(); } }
        public string? LogoPath { get => _logoPath; set { _logoPath = value; OnPropertyChanged(); } }
        public string AccentColor { get => _accentColor; set { _accentColor = value; OnPropertyChanged(); } }
        public string NextButtonText { get => _nextButtonText; set { _nextButtonText = value; OnPropertyChanged(); } }
        public bool ShowBackButton { get => _showBackButton; set { _showBackButton = value; OnPropertyChanged(); } }
        public bool HasLogo => !string.IsNullOrEmpty(LogoPath);

        public ICommand NextCommand { get; }
        public ICommand BackCommand { get; }

        private int _currentPageIndex = 0;
        private readonly List<UIElement> _pages = new();
        private readonly Action<UIElement> _navigateAction;
        private readonly PersonalizationService _personalizationService = new();

        // Pending settings
        public string? PendingWallpaper { get; set; }
        public bool? PendingIsDark { get; set; }
        public bool? PendingTaskbarCenter { get; set; }
        public string? PendingNickname { get; set; }

        public MainViewModel(Action<UIElement> navigateAction)
        {
            _navigateAction = navigateAction;

            // Load Defaults from Config
            if (ConfigManager.Current.Defaults.TryGetValue("Wallpaper", out string? wp)) PendingWallpaper = wp;
            if (ConfigManager.Current.Defaults.TryGetValue("Theme", out string? theme)) PendingIsDark = theme.Equals("Dark", StringComparison.OrdinalIgnoreCase);
            if (ConfigManager.Current.Defaults.TryGetValue("TaskbarAlignment", out string? align)) PendingTaskbarCenter = align.Equals("Center", StringComparison.OrdinalIgnoreCase);

            // Initialize Pages
            _pages.Add(new WelcomePage());
            if (ConfigManager.Current.Features.EnableTheme) _pages.Add(new ThemePage());
            if (ConfigManager.Current.Features.EnableWallpaper) _pages.Add(new WallpaperPage());
            if (ConfigManager.Current.Features.EnableDefaultApps || ConfigManager.Current.Features.EnableTaskbar)
                _pages.Add(new ExtraSettingsPage());
            _pages.Add(new FinishPage());

            NextCommand = new RelayCommand(_ => GoNext());
            BackCommand = new RelayCommand(_ => GoBack(), _ => _currentPageIndex > 0);

            UpdatePage();
        }

        private void GoNext()
        {
            if (_currentPageIndex < _pages.Count - 1)
            {
                _currentPageIndex++;
                UpdatePage();
            }
            else
            {
                Finish();
            }
        }

        private void GoBack()
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                UpdatePage();
            }
        }

        private void UpdatePage()
        {
            _navigateAction(_pages[_currentPageIndex]);
            ShowBackButton = _currentPageIndex > 0 && _currentPageIndex < _pages.Count - 1;
            NextButtonText = _currentPageIndex == _pages.Count - 1 ? "Finish" : "Next";
        }

        private void Finish()
        {
            ApplyPendingSettings();
            EnvironmentChecker.MarkSetupComplete();
            Application.Current.Shutdown();
        }

        private void ApplyPendingSettings()
        {
            try
            {
                if (!string.IsNullOrEmpty(PendingNickname)) _personalizationService.SetNickname(PendingNickname);
                if (PendingIsDark.HasValue) _personalizationService.SetTheme(PendingIsDark.Value);
                if (!string.IsNullOrEmpty(PendingWallpaper)) _personalizationService.SetWallpaper(PendingWallpaper);
                if (PendingTaskbarCenter.HasValue) _personalizationService.SetTaskbarAlignment(PendingTaskbarCenter.Value);

                _personalizationService.NotifyShell();
                Logger.Info("All pending settings applied on Finish.");
            }
            catch (Exception ex)
            {
                Logger.Error("Error applying pending settings during Finish", ex);
            }
        }
    }
}
