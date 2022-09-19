using System.Net;

namespace NetScan.Models
{
    public class NetworkInfo
    {
        public string WanIp { get; set; }
        public HostInfo Gateway { get; set; }
        public string SubnetMask { get; set; }
        public string Network { get; set; }
        public DateTime LastScanDate { get; set; }
        public double ScanDurationSeconds { get; set; }

        public List<HostInfo> Hosts { get; set; }
    }
}
