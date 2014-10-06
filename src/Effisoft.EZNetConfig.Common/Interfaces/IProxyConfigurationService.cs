using Effisoft.EZNetConfig.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Interfaces
{
    public interface IProxyConfigurationService
    {
        Task<ObservableCollection<ProxyConfiguration>> GetProxyConfEntriesAsync();

        Task<int> SaveProxyConfEntryAsync(ProxyConfiguration proxyConfEntry);

        Task ApplyProxyConfAsync(ProxyConfiguration proxyConfiguration, IProgress<ProgressReport> progress);

        Task ClearProxyConfAsync(IProgress<ProgressReport> progress);
    }
}
