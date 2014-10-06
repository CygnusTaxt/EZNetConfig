using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Services
{
    public class ProxyConfigurationService : IProxyConfigurationService
    {
        const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        const int INTERNET_OPTION_REFRESH = 37;
        bool settingsReturn, refreshReturn;
        RegistryKey _registry;

        public ProxyConfigurationService()
        {
        }

        public async Task<ObservableCollection<ProxyConfiguration>> GetProxyConfEntriesAsync()
        {
            var proxyInfoEntries = await Tools.GetProxyConfigurationListAsync();
            ObservableCollection<ProxyConfiguration> result = new ObservableCollection<ProxyConfiguration>();
            proxyInfoEntries.ProxyConfList.ForEach(proxy => result.Add(proxy));
            return result;
        }

        public async Task<int> SaveProxyConfEntryAsync(ProxyConfiguration proxyConfEntry)
        {
            int result = 0;

            try
            {
                ProxyConfigurationList infoList = new ProxyConfigurationList();
                infoList.ProxyConfList.Add(proxyConfEntry);
                await Tools.SaveProxyConfigurationListAsync(infoList);
            }
            catch (Exception)
            {
                result = -1;
            }

            return result;
        }

        public async Task ApplyProxyConfAsync(ProxyConfiguration proxyConfiguration, IProgress<ProgressReport> progress)
        {
            ProgressReport prgReport = new ProgressReport
            {
                ProgressMessage = "Applying Proxy Configuration",
                ProgressValue = 0
            };

            progress.Report(prgReport);
            await Task.Run(async () =>
                {
                    _registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

                    prgReport.ProgressMessage = "Applying Proxy IP address and port";
                    prgReport.ProgressValue = ((1 * 100 / 5));
                    progress.Report(prgReport);
                    await Task.Delay(1000);
                    _registry.SetValue("ProxyServer", string.Format("{0}:{1}", proxyConfiguration.ProxyAddress, proxyConfiguration.ProxyPort.ToString()));

                    prgReport.ProgressMessage = "Clearing Advanced settings";
                    prgReport.ProgressValue = ((2 * 100 / 5));
                    progress.Report(prgReport);
                    await Task.Delay(1000);
                    _registry.SetValue("ProxyOverride", string.Empty);

                    string proxyExceptions = proxyConfiguration.ProxyExceptions == null ? string.Empty : proxyConfiguration.ProxyExceptions;

                    if (proxyConfiguration.UseProxyForLocalAddresses)
                    {
                        if (string.IsNullOrEmpty(proxyExceptions))
                        {
                            proxyExceptions += "<local>";
                        }
                        else
                        {
                            proxyExceptions += ";<local>";
                        }
                    }

                    prgReport.ProgressMessage = "Applying configuration advanced settings";
                    prgReport.ProgressValue = ((3 * 100 / 5));
                    progress.Report(prgReport);
                    await Task.Delay(1000);
                    _registry.SetValue("ProxyOverride", proxyExceptions);

                    prgReport.ProgressMessage = "Enabling Proxy Server";
                    prgReport.ProgressValue = ((4 * 100 / 5));
                    progress.Report(prgReport);
                    await Task.Delay(1000);
                    _registry.SetValue("ProxyEnable", 1);

                    _registry.Close();
                    _registry.Dispose();
                    await Task.Delay(1000);

                    await Task.Run(() =>
                        {
                            settingsReturn = SafeNativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                            refreshReturn = SafeNativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                        });

                    prgReport.ProgressMessage = "Proxy Configuration Applied";
                    prgReport.ProgressValue = ((5 * 100 / 5));
                    progress.Report(prgReport);
                });
        }

        public async Task ClearProxyConfAsync(IProgress<ProgressReport> progress)
        {
            await Task.Run(async () =>
                {
                    ProgressReport prgReport = new ProgressReport
                    {
                        ProgressMessage = "Clearing Proxy Configuration",
                        ProgressValue = 0
                    };
                    progress.Report(prgReport);
                    await Task.Delay(1000);

                    _registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                    prgReport.ProgressMessage = "Disabling Proxy Configuration";
                    prgReport.ProgressValue = ((1 * 100 / 2));
                    progress.Report(prgReport);
                    await Task.Delay(1000);
                    _registry.SetValue("ProxyEnable", 0);
                    _registry.Close();
                    _registry.Dispose();

                    await Task.Run(() =>
                    {
                        settingsReturn = SafeNativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0);
                        refreshReturn = SafeNativeMethods.InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                    });

                    prgReport.ProgressMessage = "Proxy Configuration Cleared";
                    prgReport.ProgressValue = ((2 * 100 / 2));
                    progress.Report(prgReport);
                    await Task.Delay(2000);
                });
        }
    }
}
