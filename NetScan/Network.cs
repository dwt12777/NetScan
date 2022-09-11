﻿using NetScan.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace NetScan
{
    public class Network
    {
        public IPAddress LocalIp { get; set; }
        public IPAddress LocalSubnetMask { get; set; }

        private string _nmapTargetString;

        public Network()
        {
            LocalIp = GetLocalIpAddress();
            LocalSubnetMask = GetSubnetMask(LocalIp);
            _nmapTargetString = GetNmapTargetString(LocalIp, LocalSubnetMask);
        }

        public List<HostInfo> GetAllHosts()
        {
            var nmapResult = Nmap.RunNmap(_nmapTargetString);
            var hosts = new List<HostInfo>();
            foreach (var h in nmapResult.host)
            {
                var hostInfo = new HostInfo();

                hostInfo.IpAddress = h.address.FirstOrDefault(a => a.addrtype == "ipv4")?.addr;
                hostInfo.HostName = GetHostByIp(hostInfo.IpAddress);
                hostInfo.MacAddress = h.address.FirstOrDefault(a => a.addrtype == "mac")?.addr;
                hostInfo.MacVendor = h.address.FirstOrDefault(a => a.addrtype == "mac")?.vendor;

                if (hostInfo.MacAddress == null && hostInfo.IpAddress == GetIpByHost(hostInfo.HostName).ToString())
                {
                    hostInfo.MacAddress = GetLocalMacAddress();
                }

                if (hostInfo.MacVendor == null)
                    hostInfo.MacVendor = string.Empty;

                hosts.Add(hostInfo);
            }

            return hosts.OrderBy(h => h.IpAddressLabel).ToList();
        }

        public string GetHostByIp(string ipAddress)
        {
            try
            {
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);
                if (entry != null)
                {
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                //unknown host or
                //not every IP has a name
                //log exception (manage it)

                return string.Empty;
            }

            return null;
        }

        public IPAddress GetIpByHost(string hostName)
        {
            var host = Dns.GetHostEntry(hostName);

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        private string GetLocalMacAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            var localMac = String.Empty;

            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up && adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    localMac = string.Join(":", adapter.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2")));
                }
            }

            return localMac;
        }

        public IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public IPAddress GetLocalIpAddress()
        {
            IPAddress localIp;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIp = endPoint.Address;
            }

            return localIp;
        }

        private string GetNmapTargetString(IPAddress ipAddress, IPAddress subnetMask)
        {
            var subnet = subnetMask.ToString().Split('.');
            var ip = ipAddress.ToString().Split('.');
            var sb = new StringBuilder();

            for (int i = 0; i < subnet.Length; i++)
            {
                if (subnet[i] == "255")
                    sb.Append(ip[i] + ".");
                else
                    sb.Append("0/24");
            }

            return sb.ToString();
        }
    }
}