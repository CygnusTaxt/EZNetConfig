using Effisoft.EZNetConfig.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Interfaces
{
    /// <summary>
    /// Interface for Current IP Configuration Services
    /// </summary>
    public interface ICurrentIPConfigurationService
    {
        Task<IPConfiguration> GetCurrentIPConfigurationAsync();
        Task<ProxyConfiguration> GetCurrentProxyConfiguration();
    }
}
