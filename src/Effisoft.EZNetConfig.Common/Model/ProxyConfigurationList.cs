using System.Collections.Generic;

namespace Effisoft.EZNetConfig.Common.Model
{
    public class ProxyConfigurationList
    {
        public ProxyConfigurationList()
        {
            ProxyConfList = new List<ProxyConfiguration>();
        }

        public List<ProxyConfiguration> ProxyConfList { get; set; }
    }
}