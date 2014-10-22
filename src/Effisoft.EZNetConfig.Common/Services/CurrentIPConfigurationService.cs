using Effisoft.EZNetConfig.Common.Interfaces;
using Effisoft.EZNetConfig.Common.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Services
{
    /// <summary>
    /// Implementation of the ICurrentIPConfigurationService Interface
    /// </summary>
    public class CurrentIPConfigurationService : ICurrentIPConfigurationService
    {
        RegistryKey _registry;

        public async Task<IPConfiguration> GetCurrentIPConfigurationAsync()
        {
            var availableInterfaces = await Tools.GetConnectedNetworkInterfacesAsync();
            return Tools.GetCurrentInterfaceIPConfiguration(availableInterfaces.FirstOrDefault(), true);
        }

        public async Task<ProxyConfiguration> GetCurrentProxyConfiguration()
        {
            _registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", false);
            string[] proxyServer = _registry.GetValue("ProxyServer").ToString().Split(':');
            int proxyEnabled = Convert.ToInt16(_registry.GetValue("ProxyEnable").ToString());

            if(proxyEnabled == 0)
            {
                return new ProxyConfiguration();
            }

            if(proxyServer.Length > 0)
            {
                return new ProxyConfiguration()
                {
                    ProxyAddress = proxyServer[0],
                    ProxyPort = Convert.ToInt32(proxyServer[1]),
                    ProxyExceptions = _registry.GetValue("ProxyOverride").ToString()
                };
            }
            
            return new ProxyConfiguration();
        }
    }
}
