using System.Net;

namespace NetScan.Models
{
    public class NetworkInfo
    {
        public HostInfo Gateway { get; set; }
        public IPAddress SubnetMask { get; set; }

        public List<HostInfo> Hosts { get; set; }
    }
}
