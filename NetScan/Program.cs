using NetScan;
using NetScan.Models;
using System.Reflection;

Console.WriteLine();
Console.WriteLine(GetWelcomeMessage());

var nmapPath = @"C:\Program Files (x86)\Nmap\nmap.exe";
var nmap = new Nmap(nmapPath);

if (nmap.IsValidNmapClient)
{
    Console.WriteLine($"Scanning network with Nmap {nmap.NmapVersion}...");
    Console.WriteLine();

    var network = new Network(nmap);

    WriteHostsToScreen(network.Hosts);

    Console.WriteLine();
    Console.WriteLine($"Total Hosts Online: {network.Hosts.Count}");
    Console.WriteLine();

}
else
{
    Console.WriteLine(@"Nmap must be installed for this program to work correctly. Go to https://nmap.org/ to download and install it.");
}

string GetWelcomeMessage()
{
    string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    var welcome = $"NetScan {version}";
    return welcome;
}

void WriteHostsToScreen(List<HostInfo> hostInfos)
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