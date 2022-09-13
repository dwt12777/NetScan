using System.Net;

namespace NetScan.Models
{
    public class HostInfo
    {
        public string MacAddress { get; set; }
        public IPAddress IpAddress { get; set; }
        public string HostName { get; set; }

        public string IpAddressLabel => string.Join(".", IpAddress.ToString().Split('.').Select(part => part.PadLeft(3, '0')));
    }
}
