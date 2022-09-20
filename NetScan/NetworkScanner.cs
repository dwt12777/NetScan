using NetScan.Models;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using IPAddressCollection = System.Net.IPAddressCollection;

namespace NetScan
{
    public class NetworkScanner : INetworkScanner
    {
        public event EventHandler IpScanStarted;
        public event EventHandler IpScanCompleted;

        public event EventHandler<IpScanProgressUpdatedEventArgs> IpScanProgressUpdated;

        protected virtual void OnIpScanProgressUpdated(IpScanProgressUpdatedEventArgs e)
        {
            EventHandler<IpScanProgressUpdatedEventArgs> handler = IpScanProgressUpdated;
            handler?.Invoke(this, e);
        }

        public class IpScanProgressUpdatedEventArgs : EventArgs
        {
            public int WorkItemCompletedCount { get; set; }
            public int WorkItemTotalCount { get; set; }
            public double ProgressPercent => (double)WorkItemCompletedCount / (double)WorkItemTotalCount;
        }

        public NetworkInfo NetworkInfo { get; set; }
        public TimeSpan ScanDuration { get; set; }

        private IPNetwork _ipNetwork;
        private Stopwatch _stopwatch;
        private readonly HttpClient _client = new HttpClient();

        public NetworkScanner()
        {
            var subnetMask = GetLocalSubnetMask();

            _ipNetwork = IPNetwork.Parse(GetLocalIpAddress().ToString(), subnetMask.ToString());

            NetworkInfo = new NetworkInfo()
            {
                Gateway = GetLocalGateway(),
                SubnetMask = subnetMask.ToString(),
                WanIp = GetWanIp().ToString(),
                Network = _ipNetwork.Value
            };

            _stopwatch = new Stopwatch();
        }

        public List<HostInfo> GetAllHosts()
        {

            this.NetworkInfo.ScanDate = DateTime.Now;
            _stopwatch.Start();
            this.IpScanStarted?.Invoke(this, EventArgs.Empty);

            var ipRange = _ipNetwork.ListIPAddress(FilterEnum.Usable);

            this.NetworkInfo.Hosts = ArpPing(ipRange);

            _stopwatch.Stop();

            this.ScanDuration = _stopwatch.Elapsed;
            NetworkInfo.ScanDurationSeconds = _stopwatch.Elapsed.TotalSeconds;

            this.IpScanCompleted?.Invoke(this, EventArgs.Empty);

            

            return this.NetworkInfo.Hosts;
        }

        public HostInfo GetHostByIp(IPAddress ipAddress)
        {
            var ip = ipAddress.ToString();
            var macAddress = GetMacByIp(ipAddress);
            string hostName;

            try
            {
                hostName = Dns.GetHostEntry(ipAddress)?.HostName;
            }
            catch (Exception)
            {
                hostName = null;
            }

            var host = new HostInfo()
            {
                IpAddress = ip,
                MacAddress = macAddress,
                HostName = hostName
            };

            return host;
        }

        public HostInfo GetHostByMac(string macAddress)
        {
            var ip = GetIpByMac(macAddress);

            var host = new HostInfo()
            {
                IpAddress = ip.ToString(),
                MacAddress = macAddress,
                HostName = Dns.GetHostEntry(ip).HostName
            };

            return host;
        }

        public HostInfo GetHostByName(string hostName)
        {
            var ipAddress = Dns.GetHostAddresses(hostName)[0].ToString();

            var host = GetHostByIp(IPAddress.Parse(ipAddress));

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
                        gatewayHost.MacAddress = GetMacByIp(d.Address);
                        gatewayHost.HostName = GetHostNameByIp(d.Address);
                        gatewayHost.MacVendor = MacVendorCache.GetMacVendor(gatewayHost.MacAddress);
                        break;
                    }
                }
            }

            return gatewayHost;
        }

        public HostInfo GetLocalHost()
        {
            var localHost = new HostInfo()
            {
                IpAddress = GetLocalIpAddress().ToString(),
                MacAddress = GetLocalMacAddress(),
                HostName = Dns.GetHostName()
            };

            return localHost;
        }

        public IPAddress GetLocalSubnetMask()
        {
            var localHost = GetLocalHost();

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (IPAddress.Parse(localHost.IpAddress).Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new Exception(string.Format("Can't find subnetmask for IP address '{0}'", localHost.IpAddress));
        }

        private List<HostInfo> ArpPing(IPAddressCollection ipRange)
        {
            int workItemTotalCount = (int)ipRange.Count * 2;
            int workItemCompletedCount = 0;
            
            var hosts = new List<HostInfo>();

            List<Thread> threads = new List<Thread>();

            foreach (IPAddress ip in ipRange)
            {
                Thread thread = new Thread(() =>
                {
                    var isValidIp = ArpHelper.SendArpRequest(IPAddress.Parse(ip.ToString()));

                    if (isValidIp)
                    {
                        var host = GetHostByIp(ip);
                        hosts.Add(host);
                    }
                });
                thread.Start();
                threads.Add(thread);
                workItemCompletedCount += 1;
                UpdateProgress(workItemCompletedCount, workItemTotalCount);
            }

            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();

                workItemCompletedCount += 1;
                UpdateProgress(workItemCompletedCount, workItemTotalCount);
            }

            return hosts;
        }

        private void UpdateProgress(int workItemCompletedCount, int workItemTotalCount)
        {
            var args = new IpScanProgressUpdatedEventArgs()
            {
                WorkItemCompletedCount = workItemCompletedCount,
                WorkItemTotalCount = workItemTotalCount
            };

            this.OnIpScanProgressUpdated(args);
        }

        private IPAddress GetLocalIpAddress()
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

        private string GetMacByIp(IPAddress ipAddress)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.IpAddress == ipAddress.ToString());
            if (index >= 0)
            {
                return macIpPairs[index].MacAddress.ToUpper();
            }
            else if (ipAddress.ToString() == GetLocalIpAddress().ToString())
            {
                return GetLocalMacAddress();
            }
            else
            {
                return null;
            }
        }

        private IPAddress GetIpByMac(string macAddress)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.MacAddress == macAddress);
            if (index >= 0)
            {
                return IPAddress.Parse(macIpPairs[index].IpAddress.ToUpper());
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

        private string GetHostNameByIp(IPAddress ipAddress)
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

        private IPAddress GetWanIp()
        {
            string externalIpString = _client
                .GetStringAsync("http://icanhazip.com")
                .Result
                .Replace("\\r\\n", "")
                .Replace("\\n", "")
                .Trim();

                return IPAddress.Parse(externalIpString);
        }
    }
}
