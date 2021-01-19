using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotorControl.Commons;
using MotorControl.Commons.Views.Tabs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using StartupWindow = MotorControl.Commons.Views.MainWindow;

namespace MotorControl.Technician
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<StartupWindow>();
                    services.AddTransient<TabsControl, TechnicalTabsControl>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            ServiceCollectionKeeper.Inititalize(_host.Services);

            _host.Services
                .GetRequiredService<StartupWindow>()
                .Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync();
            }

            base.OnExit(e);
        }
    }
}
