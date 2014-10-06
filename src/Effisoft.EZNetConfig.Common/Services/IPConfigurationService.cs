using Effisoft.EZNetConfig.Common.Enums;
using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Services
{
    /// <summary>
    /// Implementation of the IIPConfigurationService Interface
    /// </summary>
    public class IPConfigurationService : IIPConfigurationService
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public IPConfigurationService()
        {
        }

        /// <summary>
        /// Get a list of IP Configuration items
        /// </summary>
        /// <returns>Observable Collection of IP Configuration Items</returns>
        public async Task<ObservableCollection<IPConfiguration>> GetIPInformationEntriesAsync()
        {
            var ipInfoEntries = await Tools.GetIPInformationListAsync();
            ObservableCollection<IPConfiguration> result = new ObservableCollection<IPConfiguration>();
            ipInfoEntries.IPConfList.ForEach(ip => result.Add(ip));
            return result;
        }

        /// <summary>
        /// Saves the IP Configuration instance
        /// </summary>
        /// <param name="ipInformationEntry">IP Configuration instance to save</param>
        /// <returns>Integer indicating wether the operation was successful</returns>
        public async Task<int> SaveIPInformationEntryAsync(IPConfiguration ipInformationEntry)
        {
            int result = 0;

            try
            {
                IPConfigurationList infoList = new IPConfigurationList();
                infoList.IPConfList.Add(ipInformationEntry);
                await Tools.SaveIPInformationListAsync(infoList);
            }
            catch (Exception)
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// Applies the sent IP Configuration item
        /// </summary>
        /// <param name="ipConfiguration">The configuration to apply</param>
        /// <param name="adapterName">The adapter which will be updated</param>
        /// <returns>Task type for asynchronous operations</returns>
        public async Task ApplyIPConfigurationAsync(IPConfiguration ipConfiguration, NetworkInterface netInterface,
            IProgress<ProgressReport> progress)
        {
            ProgressReport prgReport = new ProgressReport
            {
                ProgressMessage = "Clearing old settings",
                ProgressValue = 0
            };

            Task ipSetTask = Tools.RunProcess(Tools.BuildNetshParameters(ipConfiguration,
                netInterface.Name, EParameterConfgurationType.IP));

            Task delDnsTask = Tools.RunProcess(Tools.BuildNetshParameters(ipConfiguration,
                netInterface.Name, EParameterConfgurationType.DeleteDNS));

            Task dnsPriSetTask = null;
            Task dnsSecSetTask = null;

            progress.Report(prgReport);
            await delDnsTask;

            if (!string.IsNullOrWhiteSpace(ipConfiguration.PreferredDNS))
            {
                dnsPriSetTask = Tools.RunProcess(Tools.BuildNetshParameters(ipConfiguration,
                netInterface.Name, EParameterConfgurationType.PrimaryDNS));

                prgReport.ProgressMessage = "Applying Preferred DNS";
                prgReport.ProgressValue = (1 * 100 / 5);
                progress.Report(prgReport);
                await dnsPriSetTask;

                if (!string.IsNullOrWhiteSpace(ipConfiguration.SecondaryDNS))
                {
                    dnsSecSetTask = Tools.RunProcess(Tools.BuildNetshParameters(ipConfiguration,
                            netInterface.Name, EParameterConfgurationType.SecondaryDNS));

                    prgReport.ProgressMessage = "Applying Secondary DNS";
                    prgReport.ProgressValue = (2 * 100 / 5);
                    progress.Report(prgReport);
                    await dnsSecSetTask;
                }
            }

            prgReport.ProgressMessage = "Applying IP Configuration";
            prgReport.ProgressValue = (3 * 100 / 5);
            progress.Report(prgReport);
            await ipSetTask;

            prgReport.ProgressMessage = "Validating Configuration";
            prgReport.ProgressValue = (4 * 100 / 5);
            progress.Report(prgReport);
            if (!ValidateInterfaceSettings(ipConfiguration, netInterface))
            {
                throw new Exception("Configure settings do not match the selected configuration");
            }

            prgReport.ProgressMessage = "Configuration Applied";
            prgReport.ProgressValue = (5 * 100 / 5);
            progress.Report(prgReport);
        }

        /// <summary>
        /// Validates that the current configuration matches the selected configuration
        /// </summary>
        /// <param name="ipConfiguration">Selected configuration</param>
        /// <param name="netInterface">Current configuration for the selected adapter</param>
        /// <returns>true if configuration matches</returns>
        private bool ValidateInterfaceSettings(IPConfiguration ipConfiguration, NetworkInterface netInterface)
        {
            bool result = false;

            IPConfiguration configuredIP = Tools.GetCurrentInterfaceIPConfiguration(netInterface);

            if (configuredIP != null)
            {
                if (ipConfiguration.Equals(configuredIP))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}