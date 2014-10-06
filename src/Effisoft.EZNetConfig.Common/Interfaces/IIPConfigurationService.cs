using Effisoft.EZNetConfig.Common.Model;
using System;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Interfaces
{
    /// <summary>
    /// Interface for the IP Configuration Services
    /// </summary>
    public interface IIPConfigurationService
    {
        /// <summary>
        /// Get a list of IP Configuration items
        /// </summary>
        /// <returns>Observable Collection of IP Configuration Items</returns>
        Task<ObservableCollection<IPConfiguration>> GetIPInformationEntriesAsync();

        /// <summary>
        /// Saves the IP Configuration instance
        /// </summary>
        /// <param name="ipInformationEntry">IP Configuration instance to save</param>
        /// <returns>Integer indicating wether the operation was successful</returns>
        Task<int> SaveIPInformationEntryAsync(IPConfiguration ipInformationEntry);

        /// <summary>
        /// Applies the sent IP Configuration item
        /// </summary>
        /// <param name="ipConfiguration">The configuration to apply</param>
        /// <param name="adapterName">The adapter which will be updated</param>
        /// <returns>Task type for asynchronous operations</returns>
        Task ApplyIPConfigurationAsync(IPConfiguration ipConfiguration, NetworkInterface adapterName,
            IProgress<ProgressReport> progress);
    }
}