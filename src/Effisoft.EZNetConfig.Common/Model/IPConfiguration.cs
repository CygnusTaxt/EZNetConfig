using System;
using System.Xml.Serialization;

namespace Effisoft.EZNetConfig.Common.Model
{
    /// <summary>
    /// Class that holds the IP Configuration information
    /// </summary>
    public class IPConfiguration : IEquatable<IPConfiguration>
    {
        /// <summary>
        /// The configuration ID name
        /// </summary>
        [XmlAttribute("ConfigurationName")]
        public string IPConfigurationName { get; set; }

        /// <summary>
        /// IP Address
        /// </summary>
        [XmlElement("IP")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Subnet Mask address
        /// </summary>
        [XmlElement("Subnet")]
        public string SubnetMask { get; set; }

        /// <summary>
        /// Gateway address
        /// </summary>
        [XmlElement("Gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// Primary DNS address
        /// </summary>
        [XmlElement("PreferredDNS")]
        public string PreferredDNS { get; set; }

        /// <summary>
        /// Secondary DNS address
        /// </summary>
        [XmlElement("SecondaryDNS")]
        public string SecondaryDNS { get; set; }

        /// <summary>
        /// Indicates wether the configuration is DHCP or not
        /// </summary>
        [XmlElement("DHCP")]
        public bool IsDHCP { get; set; }

        /// <summary>
        /// Override Equals method to compare two IPConfiguration instances
        /// </summary>
        /// <param name="other">The object to compare with</param>
        /// <returns>true if objects are the same</returns>
        public bool Equals(IPConfiguration other)
        {
            if (!string.IsNullOrWhiteSpace(other.IPConfigurationName))
            {
                if (IPConfigurationName == other.IPConfigurationName)
                {
                    return true;
                }
            }
            else
            {
                if (!IsDHCP)
                {
                    if (IPAddress == other.IPAddress &&
                        SubnetMask == other.SubnetMask &&
                        Gateway == other.Gateway &&
                        PreferredDNS == other.PreferredDNS &&
                        SecondaryDNS == other.SecondaryDNS &&
                        IsDHCP == other.IsDHCP)
                    {
                        return true;
                    }
                }
                else
                {
                    if (IsDHCP == other.IsDHCP &&
                        PreferredDNS == other.PreferredDNS &&
                        SecondaryDNS == other.SecondaryDNS)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get hashcode based on the configuration name
        /// </summary>
        /// <returns>Integer representing the ipconfigurationname hashcode</returns>
        public override int GetHashCode()
        {
            int hashIPInformationName = IPConfigurationName == null ? 0 : IPConfigurationName.GetHashCode();

            return hashIPInformationName;
        }
    }
}