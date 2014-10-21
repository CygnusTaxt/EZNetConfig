using Effisoft.EZNetConfig.Common.Enums;
using Effisoft.EZNetConfig.Common.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Effisoft.EZNetConfig.Common
{
    public static class Tools
    {
        private const string NETSHCMD = "C:\\Windows\\System32\\netsh.exe";
        public static string IPCONFFILENAME = AppDomain.CurrentDomain.BaseDirectory + "Resources\\IPInformation.xml";
        public static string PROXYCONFFILENAME = AppDomain.CurrentDomain.BaseDirectory + "Resources\\ProxyInformation.xml";

        public static string BuildNetshParameters(IPConfiguration ipInformation,
            string interfaceName, EParameterConfgurationType configType)
        {
            StringBuilder cmdArguments = new StringBuilder();

            switch (configType)
            {
                case EParameterConfgurationType.IP:
                    cmdArguments.Append("interface ip set address \"");
                    cmdArguments.Append(interfaceName);
                    cmdArguments.Append("\"");

                    if (ipInformation.IsDHCP)
                    {
                        cmdArguments.Append(" dhcp");
                    }
                    else
                    {
                        cmdArguments.Append(" static ");
                        cmdArguments.Append(ipInformation.IPAddress);
                        cmdArguments.Append(" ");
                        cmdArguments.Append(ipInformation.SubnetMask);
                        cmdArguments.Append(" ");
                        cmdArguments.Append(ipInformation.Gateway);
                    }
                    break;

                case EParameterConfgurationType.PrimaryDNS:
                    cmdArguments.Append("interface ip add dns \"");
                    cmdArguments.Append(interfaceName);
                    cmdArguments.Append("\" ");
                    cmdArguments.Append(ipInformation.PreferredDNS);
                    break;

                case EParameterConfgurationType.SecondaryDNS:
                    cmdArguments.Append("interface ip add dns \"");
                    cmdArguments.Append(interfaceName);
                    cmdArguments.Append("\" ");
                    cmdArguments.Append(ipInformation.SecondaryDNS);
                    cmdArguments.Append(" index=2");
                    break;

                case EParameterConfgurationType.DeleteDNS:
                    cmdArguments.Append("interface ip delete dns \"");
                    cmdArguments.Append(interfaceName);
                    cmdArguments.Append("\" all");
                    break;
            }

            return cmdArguments.ToString();
        }

        public static IPConfiguration GetCurrentInterfaceIPConfiguration(NetworkInterface netInterface, bool getAllInfo = false)
        {
            IPConfiguration configuredIP = null;

            var currentInterface = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.Name == netInterface.Name).FirstOrDefault();
            var currentAdapterProperties = currentInterface.GetIPProperties();
            var currentIPV4Properties = currentAdapterProperties.GetIPv4Properties();
            var ipV4Address = currentAdapterProperties.UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();

            if (!currentIPV4Properties.IsDhcpEnabled)
            {
                configuredIP = new IPConfiguration
                {
                    IPAddress = ipV4Address.Address.ToString(),
                    SubnetMask = ipV4Address.IPv4Mask.ToString(),
                    Gateway = currentAdapterProperties.GatewayAddresses.FirstOrDefault().Address.ToString(),
                    IsDHCP = currentIPV4Properties.IsDhcpEnabled
                };
            }
            else
            {
                if (getAllInfo)
                {
                    configuredIP = new IPConfiguration()
                    {
                        IPAddress = ipV4Address.Address.ToString(),
                        SubnetMask = ipV4Address.IPv4Mask.ToString(),
                        Gateway = currentAdapterProperties.GatewayAddresses.FirstOrDefault().Address.ToString(),
                        IsDHCP = currentIPV4Properties.IsDhcpEnabled
                    };
                }
                else
                {
                    configuredIP = new IPConfiguration
                    {
                        IsDHCP = currentIPV4Properties.IsDhcpEnabled
                    };
                }
            }

            var IPV4Dns = currentAdapterProperties.DnsAddresses.ToList().Where(x => x.AddressFamily == AddressFamily.InterNetwork).ToList();

            for (int i = 0; i < IPV4Dns.Count; i++)
            {
                if (i == 0)
                {
                    configuredIP.PreferredDNS = currentAdapterProperties.DnsAddresses[i].ToString();
                }
                else if (i == 1)
                {
                    configuredIP.SecondaryDNS = currentAdapterProperties.DnsAddresses[i].ToString();
                }
            }

            return configuredIP;
        }

        public static async Task<IPConfigurationList> GetIPInformationListAsync()
        {
            var list = await Task<IPConfigurationList>.Factory.StartNew(() =>
                {
                    var result = DeserializeObject<IPConfigurationList>(IPCONFFILENAME);
                    return result;
                });
            return list;
        }

        public static async Task<ProxyConfigurationList> GetProxyConfigurationListAsync()
        {
            var list = await Task<ProxyConfigurationList>.Factory.StartNew(() =>
                {
                    var result = DeserializeObject<ProxyConfigurationList>(PROXYCONFFILENAME);
                    return result;
                });
            return list;
        }

        public static async Task RunProcess(string processArguments)
        {
            string procResult = string.Empty;

            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = NETSHCMD,
                    Arguments = processArguments,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            await Task.Factory.StartNew(() =>
            {
                proc.Start();
                proc.WaitForExit();
                proc.Close();
            });
        }

        public static async Task SaveIPInformationListAsync(IPConfigurationList infoList)
        {
            await Task.Factory.StartNew(() =>
                {
                    foreach (var info in infoList.IPConfList)
                    {
                        info.IPConfigurationName = info.IPConfigurationName.Trim();
                        info.IPAddress = string.IsNullOrWhiteSpace(info.IPAddress) ? null : info.IPAddress.Trim();
                        info.SubnetMask = string.IsNullOrWhiteSpace(info.SubnetMask) ? null : info.SubnetMask.Trim();
                        info.Gateway = string.IsNullOrWhiteSpace(info.Gateway) ? null : info.Gateway.Trim();
                        info.PreferredDNS = string.IsNullOrWhiteSpace(info.PreferredDNS) ? null : info.PreferredDNS.Trim();
                        info.SecondaryDNS = string.IsNullOrWhiteSpace(info.SecondaryDNS) ? null : info.SecondaryDNS.Trim();
                    };

                    SerializeObject<IPConfigurationList>(infoList, IPCONFFILENAME);
                });
        }

        public static async Task SaveProxyConfigurationListAsync(ProxyConfigurationList infoList)
        {
            await Task.Factory.StartNew(() =>
                {
                    foreach (var info in infoList.ProxyConfList)
                    {
                        info.ProxyConfigurationName = info.ProxyConfigurationName.Trim();
                        info.ProxyAddress = string.IsNullOrWhiteSpace(info.ProxyAddress) ? null : info.ProxyAddress.Trim();
                        info.ProxyExceptions = string.IsNullOrWhiteSpace(info.ProxyExceptions) ? null : info.ProxyExceptions.Trim();
                        info.ProxyPort = info.ProxyPort == 0 ? 0 : info.ProxyPort;
                    };

                    SerializeObject<ProxyConfigurationList>(infoList, PROXYCONFFILENAME);
                });
        }

        private static T DeserializeObject<T>(string fileName) where T : new()
        {
            if (File.Exists(fileName))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(T));
                TextReader reader = new StreamReader(fileName);
                var result = (T)deserializer.Deserialize(reader);
                reader.Close();
                return result;
            }
            else
            {
                return new T();
            }
        }

        private static void SerializeObject<T>(T objToSerialize, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(fileName))
            {
                serializer.Serialize(writer, objToSerialize);
            }
        }

        public static async Task<List<NetworkInterface>> GetConnectedNetworkInterfacesAsync()
        {
            Task<List<NetworkInterface>> t1 = Task<List<NetworkInterface>>.Factory.StartNew(() =>
            {
                return NetworkInterface.GetAllNetworkInterfaces().ToList(); ;
            });

            var availableInterfaces = await t1;

            availableInterfaces = availableInterfaces.Where(aInt => (aInt.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit || aInt.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT || aInt.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet ||
                aInt.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && aInt.OperationalStatus == OperationalStatus.Up &&
                !(aInt.Description.ToUpper().Contains("VIRTUAL"))).ToList();

            return availableInterfaces;
        }
    }
}