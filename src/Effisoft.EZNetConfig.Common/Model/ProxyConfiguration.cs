using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Effisoft.EZNetConfig.Common.Model
{
    public class ProxyConfiguration : IEquatable<ProxyConfiguration>
    {
        [XmlAttribute("ConfigurationName")]
        public string ProxyConfigurationName { get; set; }

        [XmlElement("Address")]
        public string ProxyAddress { get; set; }
        
        [XmlElement("Port")]
        public int ProxyPort { get; set; }
        
        [XmlElement("Exceptions")]
        public string ProxyExceptions { get; set; }

        [XmlElement("UseForLocalAddress")]
        public bool UseProxyForLocalAddresses { get; set; }

        public bool Equals(ProxyConfiguration other)
        {
            if (!string.IsNullOrWhiteSpace(other.ProxyConfigurationName))
            {
                if (ProxyConfigurationName == other.ProxyConfigurationName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
