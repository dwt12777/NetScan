using NetScan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;
using IPAddressTools;
using System.Runtime.InteropServices;

namespace NetScan
{
    public class NetworkScanner : INetworkScanner
    {
        public NetworkInfo NetworkInfo { get; set; }

        public NetworkScanner()
        {
            NetworkInfo = new NetworkInfo()
            {
                Gateway = GetLocalGateway(),
                SubnetMask = GetLocalSubnetMask()
            };
        }

        public List<HostInfo> GetAllHosts()
        {
            var hosts = new List<HostInfo>();

            IPAddress startIp = IPAddress.Parse("192.168.22.1");
            IPAddress endIp = IPAddress.Parse("192.168.22.254");

            var rangeFinder = new RangeFinder();

            var ipRange = rangeFinder.GetIPRange(startIp, endIp);

            var validIps = new List<string>();

            var tasks = new List<Task>();

            foreach (string ip in ipRange)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"Pinging {ip}");
                    var isValidIp = ArpHelper.SendArpRequest(IPAddress.Parse(ip));

                    if (isValidIp)
                    {
                        validIps.Add(ip);
                        Console.WriteLine($"We got one!");

                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

            Parallel.ForEach(validIps, ip =>
            {
                var host = GetHostByIp(ip);
                Console.WriteLine($"{host.IpAddress.PadRight(17)}{host.MacAddress.PadRight(20)}{host.HostName}");
                hosts.Add(host);
            });


            //foreach (string ip in ipRange)
            //{
            //    Thread thread = new Thread(() =>
            //    {
            //        var isValidIp = ArpHelper.SendArpRequest(IPAddress.Parse(ip));

            //        if (isValidIp)
            //        { 
            //            var host = GetHostByIp(ip);

            //            Console.WriteLine($"{host.IpAddress.PadRight(17)}{host.MacAddress.PadRight(20)}{host.HostName}");
            //            hosts.Add(host);
            //        }
            //    });
            //    thread.Start();

            //}

            //var options = new ParallelOptions
            //{
            //    MaxDegreeOfParallelism = 150
            //};

            //Parallel.ForEach(ipRange, options, ip =>
            //{
            //    Console.WriteLine($"Pinging {ip}");
            //    var isValidIp = ArpHelper.SendArpRequest(IPAddress.Parse(ip));

            //    if (isValidIp)
            //    {
            //        var host = GetHostByIp(ip);

            //        Console.WriteLine($"{host.IpAddress.PadRight(17)}{host.MacAddress.PadRight(20)}{host.HostName}");
            //        hosts.Add(host);
            //    }
            //});


            //foreach (var ip in ipRange)
            //{
            //    Console.WriteLine($"ARPing {ip}");
            //    var isValidIp = ArpHelper.SendArpRequest(IPAddress.Parse(ip));

            //    if (isValidIp)
            //    {
            //        Console.WriteLine($"Response from {ip}");

            //        var host = GetHostByIp(ip);
            //        hosts.Add(host);
            //    }
            //}

            this.NetworkInfo.Hosts = hosts;
            return hosts;
        }

        public HostInfo GetHostByIp(string ipAddress)
        {
            var host = new HostInfo()
            {
                IpAddress = ipAddress,
                MacAddress = GetMacByIp(ipAddress),
                HostName = Dns.GetHostEntry(ipAddress).HostName
            };

            return host;
        }

        public HostInfo GetHostByMac(string macAddress)
        {
            var ip = GetIpByMac(macAddress);

            var host = new HostInfo()
            {
                IpAddress = ip,
                MacAddress = macAddress,
                HostName = Dns.GetHostEntry(ip).HostName
            };

            return host;
        }

        public HostInfo GetHostByName(string hostName)
        {
            var ipAddress = Dns.GetHostAddresses(hostName)[0].ToString();

            var host = GetHostByIp(ipAddress);

            return host;
        }

        public HostInfo GetLocalGateway()
        {
            var gatewayHost = new HostInfo();

            foreach (NetworkInterface f in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (f.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
                    {
                        gatewayHost.IpAddress = d.Address.ToString();
                        gatewayHost.MacAddress = GetMacByIp(d.Address.ToString());
                        gatewayHost.HostName = GetHostNameByIp(d.Address.ToString());
                        break;
                    }
                }
            }

            return gatewayHost;
        }

        public HostInfo GetLocalGateway_backup()
        {
            const int timeout = 10000;
            const int maxTTL = 30;
            const int bufferSize = 32;

            var hostname = "reardentools.com";

            var gatewayIp = string.Empty;

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            using (var pinger = new Ping())
            {
                for (int ttl = 1; ttl <= maxTTL; ttl++)
                {
                    PingOptions options = new PingOptions(ttl, true);
                    PingReply reply = pinger.Send(hostname, timeout, buffer, options);

                    // we've found a route at this ttl
                    if (reply.Status == IPStatus.Success || reply.Status == IPStatus.TtlExpired)
                    {
                        gatewayIp = reply.Address.ToString();
                        break;
                    }

                    // if we reach a status other than expired or timed out, we're done searching or there has been an error
                    if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                        break;
                }
            }
            var hostGateway = new HostInfo()
            {
                IpAddress = gatewayIp,
                MacAddress = GetMacByIp(gatewayIp),
                HostName = GetHostNameByIp(gatewayIp)
            };

            return hostGateway;
        }

        public HostInfo GetLocalHost()
        {
            var localHost = new HostInfo()
            {
                IpAddress = GetLocalIpAddress(),
                MacAddress = GetLocalMacAddress(),
                HostName = Dns.GetHostName()
            };

            return localHost;
        }

        public string GetLocalSubnetMask()
        {
            var localHost = GetLocalHost();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (localHost.IpAddress.Equals(unicastIPAddressInformation.Address.ToString()))
                        {
                            return unicastIPAddressInformation.IPv4Mask.ToString();
                        }
                    }
                }
            }
            throw new Exception(string.Format("Can't find subnetmask for IP address '{0}'", localHost.IpAddress));
        }

        private string GetLocalIpAddress()
        {
            IPAddress localIp;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIp = endPoint.Address;
            }

            return localIp.ToString();
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

        private string GetMacByIp(string ipAddress)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.IpAddress == ipAddress);
            if (index >= 0)
            {
                return macIpPairs[index].MacAddress.ToUpper();
            }
            else if (ipAddress == GetLocalIpAddress())
            {
                return GetLocalMacAddress();
            }
            else
            {
                return null;
            }
        }

        private string GetIpByMac(string macAddress)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.MacAddress == macAddress);
            if (index >= 0)
            {
                return macIpPairs[index].IpAddress.ToUpper();
            }
            else
            {
                return null;
            }
        }

        private List<MacIpPair> GetAllMacAddressesAndIppairs()
        {
            List<MacIpPair> mip = new List<MacIpPair>();
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                mip.Add(new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value.Replace("-", ":"),
                    IpAddress = m.Groups["ip"].Value
                });
            }

            return mip;
        }
        private struct MacIpPair
        {
            public string MacAddress;
            public string IpAddress;
        }

        private string GetHostNameByIp(string ipAddress)
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

            return string.Empty;
        }





    }
}
