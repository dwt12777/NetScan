using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetScan
{
    public class ArpHelper
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        static void Main(string[] args)
        {
            List<IPAddress> ipAddressList = new List<IPAddress>();

            //Generating 192.168.0.1/24 IP Range
            for (int i = 1; i < 255; i++)
            {
                //Obviously you'll want to safely parse user input to catch exceptions.
                ipAddressList.Add(IPAddress.Parse("192.168.0." + i));
            }

            foreach (IPAddress ip in ipAddressList)
            {
                Thread thread = new Thread(() => SendArpRequest(ip));
                thread.Start();

            }
        }

        public static bool SendArpRequest(IPAddress dst)
        {
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            int uintAddress = BitConverter.ToInt32(dst.GetAddressBytes(), 0);

            if (SendARP(uintAddress, 0, macAddr, ref macAddrLen) == 0)
            {
                return true;

                // Console.WriteLine("{0} responded to ping", dst.ToString());
            }
            else
                return false;
        }
    }
}


