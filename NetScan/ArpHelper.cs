using System.Net;
using System.Runtime.InteropServices;

namespace NetScan
{
    public class ArpHelper
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);

        public static bool SendArpRequest(IPAddress dst)
        {
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            int uintAddress = BitConverter.ToInt32(dst.GetAddressBytes(), 0);

            if (SendARP(uintAddress, 0, macAddr, ref macAddrLen) == 0)
                return true;
            else
                return false;
        }
    }
}


