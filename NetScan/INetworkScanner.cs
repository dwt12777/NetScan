using NetScan.Models;
using System.Net;

namespace NetScan
{
    interface INetworkScanner
    {
        public NetworkInfo NetworkInfo { get; set; }

        public List<HostInfo> GetAllHosts();

        public HostInfo GetHostByIp(IPAddress ipAddress);

        public HostInfo GetHostByName(string hostName);

        public HostInfo GetHostByMac(string macAddress);

        public HostInfo GetLocalHost();

        public IPAddress GetLocalSubnetMask();

        public HostInfo GetLocalGateway();
    }
}
