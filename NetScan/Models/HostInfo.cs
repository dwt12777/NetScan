using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScan.Models
{
    public class HostInfo
    {
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public long RoudtripTime { get; set; }

        public string MacVendor { get; set; }

        public string IpAddressLabel => string.Join(".", IpAddress.Split('.').Select(part => part.PadLeft(3, '0')));
    }
}
