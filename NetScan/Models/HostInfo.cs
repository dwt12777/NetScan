using System.Net;
using System.Text.Json.Serialization;

namespace NetScan.Models
{
    public class HostInfo
    {
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string MacAddress { get; set; }
        public string MacVendor { get; set; }

        [JsonIgnore]
        public string IpAddressLabel => string.Join(".", IpAddress.ToString().Split('.').Select(part => part.PadLeft(3, '0')));
    }
}
