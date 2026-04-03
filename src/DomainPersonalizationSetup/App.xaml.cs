using System.Windows;
using DomainPersonalizationSetup.Core;
using DomainPersonalizationSetup.Views;

namespace DomainPersonalizationSetup
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Initialize Logging
            Logger.Info("Application starting...");

            // 2. Load Config
            ConfigManager.Load();

            // 3. Environment Checks
            if (!EnvironmentChecker.IsDomainJoined())
            {
                Logger.Info("Not domain joined. Showing warning.");
                var warning = new DomainWarningDialog();
                warning.ShowDialog();
                if (!warning.ShouldContinue)
                {
                    Logger.Info("User chose to exit from warning dialog.");
                    Shutdown();
                    return;
                }
                Logger.Info("User chose to continue despite not being domain joined.");
            }

            if (EnvironmentChecker.IsSetupComplete())
            {
                Logger.Info("Setup already completed for this user. Exiting.");
                Shutdown();
                return;
            }

            Logger.Info("Starting UI...");
        }
    }
}
