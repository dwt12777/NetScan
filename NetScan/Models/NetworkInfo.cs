using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetScan.Models
{
    public class NetworkInfo
    {
        public HostInfo Gateway { get; set; }
        public IPAddress SubnetMask { get; set; }

        public List<HostInfo> Hosts { get; set; }
    }
}
