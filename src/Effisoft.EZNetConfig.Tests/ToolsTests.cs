using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effisoft.EZNetConfig.Common;
using Effisoft.EZNetConfig.Common.Model;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Tests
{
    [TestClass]
    public class ToolsTests
    {
        [TestMethod]
        public void TestSaveProxyConfigurationXML()
        {
            ProxyConfigurationList proxyList = new ProxyConfigurationList();
            proxyList.ProxyConfList.Add(new ProxyConfiguration
                {
                    ProxyConfigurationName = "SAT",
                    ProxyAddress = "10.69.41.203",
                    ProxyExceptions = "99.90*;138.91.75.88;200.34.175.8;sattfserver01",
                    ProxyPort = 8080,
                    UseProxyForLocalAddresses = false
                });

            Tools.SaveProxyConfigurationListAsync(proxyList);
            Assert.IsTrue(File.Exists(Tools.PROXYCONFFILENAME));
        }

        [TestMethod]
        public void TestSaveIPConfigurationXML()
        {
            IPConfigurationList ipList = new IPConfigurationList();
            ipList.IPConfList.Add(new IPConfiguration
                {
                    IPConfigurationName = "SAT",
                    IPAddress = "99.90.32.213",
                    Gateway = "99.90.32.1",
                    SubnetMask = "255.255.252.0",
                    PreferredDNS = "99.90.4.210",
                    SecondaryDNS = "10.4.5.5",
                    IsDHCP = false
                });
            ipList.IPConfList.Add(new IPConfiguration
                {
                    IPConfigurationName = "Home",
                    IPAddress = "10.0.0.1",
                    Gateway = "10.0.0.254",
                    SubnetMask = "255.255.255.0",
                    PreferredDNS = "208.67.222.222",
                    SecondaryDNS = "208.67.220.220",
                    IsDHCP = false
                });
            ipList.IPConfList.Add(new IPConfiguration
            {
                IPConfigurationName = "DHCP",
                IPAddress = "",
                Gateway = "",
                SubnetMask = "",
                PreferredDNS = "",
                SecondaryDNS = "",
                IsDHCP = true
            });

            Tools.SaveIPInformationListAsync(ipList).Wait();

            Assert.IsTrue(File.Exists(Tools.IPCONFFILENAME));
        }

        [TestMethod]
        public void LoadProxyConfigurationXMLFile()
        {
            var proxyInfoList = Tools.GetProxyConfigurationListAsync().Result;

            Assert.IsNotNull(proxyInfoList);
        }

        [TestMethod]
        public void LoadIPConfigurationXMLFile()
        {
            var ipInfoList = Tools.GetIPInformationListAsync().Result;

            Assert.IsNotNull(ipInfoList);
        }
    }
}
