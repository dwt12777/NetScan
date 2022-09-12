using NetScan;
using NetScan.Models;

var network = new Network();

var hosts = network.GetAllHosts();

WriteHostsToScreen(hosts);

Console.WriteLine();
Console.WriteLine($"Total Hosts Online: {hosts.Count}");
Console.WriteLine();

var defaultGatewayIp = network.GetTraceRoute("google.com");


var test = defaultGatewayIp.FirstOrDefault().Address.ToString();

Console.ReadLine();


//Console.Write("Press any key to exit...");
//Console.ReadKey();

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