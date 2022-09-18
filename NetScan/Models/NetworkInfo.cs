using System.Net;

namespace NetScan.Models
{
    public class NetworkInfo
    {
        public IPAddress WanIp { get; set; }
        public HostInfo Gateway { get; set; }
        public IPAddress SubnetMask { get; set; }
        public string Network { get; set; } 

        public List<HostInfo> Hosts { get; set; }
    }
}
