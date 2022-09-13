using NetScan.Models;

namespace NetScan
{
    interface INetworkScanner
    {
        public NetworkInfo NetworkInfo { get; set; }

        public List<HostInfo> GetAllHosts();

        public HostInfo GetHostByIp(string ipAddress);

        public HostInfo GetHostByName(string hostName);

        public HostInfo GetHostByMac(string macAddress);

        public HostInfo GetLocalHost();

        public string GetLocalSubnetMask();

        public HostInfo GetLocalGateway();
    }
}
