using NetScan;
using NetScan.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

var nmapResult = Nmap.RunNmap();

List<HostInfo> hostInfos = BuildHostList(nmapResult);

WriteHostInfosToScreen(hostInfos);

Console.WriteLine();
Console.WriteLine($"Total Hosts Online: {hostInfos.Count}");
Console.WriteLine();

void WriteHostInfosToScreen(List<HostInfo> hostInfos)
{
    var maxIpLength = hostInfos.Max(h => h.IpAddress.Length);
    var maxHostLength = hostInfos.Max(h => h.HostName.Length);
    var maxMacLength = hostInfos.Max(h => h.MacAddress.Length);
    var maxVendorLength = hostInfos.Max(h => h.MacVendor.Length);

    var colSpace = 2;

    Console.WriteLine("IP Address".PadRight(maxIpLength + colSpace) + "Host Name".PadRight(maxHostLength + colSpace) + "MAC Address".PadRight(maxMacLength + colSpace) + "MAC Vendor".PadRight(maxVendorLength + colSpace));
    Console.WriteLine(new String('-', maxIpLength).PadRight(maxIpLength + colSpace) + new String('-', maxHostLength).PadRight(maxHostLength + colSpace) + new String('-', maxMacLength).PadRight(maxMacLength + colSpace) + new String('-', maxVendorLength).PadRight(maxVendorLength + colSpace));

    foreach (var hi in hostInfos)
    {
        Console.WriteLine($"{hi.IpAddress.PadRight(maxIpLength + 2)}{hi.HostName.PadRight(maxHostLength + 2)}{hi.MacAddress.PadRight(maxMacLength + 2)}{hi.MacVendor.PadRight(maxVendorLength + 2)}");
    }
}

List<HostInfo> BuildHostList(nmaprun nmapResult)
{
    var hosts = new List<HostInfo>();
    foreach (var h in nmapResult.host)
    {
        var hostInfo = new HostInfo();

        hostInfo.IpAddress = h.address.FirstOrDefault(a => a.addrtype == "ipv4")?.addr;
        hostInfo.HostName = GetHostName(hostInfo.IpAddress);
        hostInfo.MacAddress = h.address.FirstOrDefault(a => a.addrtype == "mac")?.addr;
        hostInfo.MacVendor = h.address.FirstOrDefault(a => a.addrtype == "mac")?.vendor;

        if (hostInfo.MacAddress == null && hostInfo.IpAddress == GetIpAddressByHostName(hostInfo.HostName))
        {
            hostInfo.MacAddress = GetLocalMacAddress();
        }

        if (hostInfo.MacVendor == null)
            hostInfo.MacVendor = string.Empty;

        hosts.Add(hostInfo);
    }

    return hosts.OrderBy(h => h.IpAddressLabel).ToList();
}

string? GetLocalMacAddress()
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

string GetHostName(string ipAddress)
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

string GetIpAddressByHostName(string hostName)
{
    var host = Dns.GetHostEntry(hostName);
    var localIp = string.Empty;

    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            localIp = ip.ToString();
        }
    }
    return localIp;
}