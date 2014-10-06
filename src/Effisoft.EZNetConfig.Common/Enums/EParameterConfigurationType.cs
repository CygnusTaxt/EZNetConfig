using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Effisoft.EZNetConfig.Common.Enums
{
    /// <summary>
    /// Value to configure the parameters for the netsh command
    /// </summary>
    public enum EParameterConfgurationType
    {
        IP = 0,
        PrimaryDNS = 1,
        SecondaryDNS = 2,
        DeleteDNS = 3
    }
}
