using System.Collections.Generic;

namespace Effisoft.EZNetConfig.Common.Model
{
    /// <summary>
    /// Class that holds the saved IP Configurations
    /// </summary>
    public class IPConfigurationList
    {
        /// <summary>
        /// List with IP Configuration items
        /// </summary>
        public List<IPConfiguration> IPConfList { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public IPConfigurationList()
        {
            IPConfList = new List<IPConfiguration>();
        }
    }
}