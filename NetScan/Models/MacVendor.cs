using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScan.Models
{
    public class MacVendor
    {
        public string MacAddress { get; set; }
        public string Vendor { get; set; }
        public DateTime LookupDate { get; set; }
    }
}
