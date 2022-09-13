using NetScan;
using NetScan.Models;
using System.Reflection;

Console.WriteLine();
Console.WriteLine(GetWelcomeMessage());
Console.WriteLine();

var networkScanner = new NetworkScanner();

Console.Write($"Scan in progress... ");
networkScanner.GetAllHosts();
Console.WriteLine();

Console.WriteLine();
Console.WriteLine($"Total Hosts Found: {networkScanner.NetworkInfo.Hosts.Count}");
Console.WriteLine();

WriteHostsToScreen(networkScanner.NetworkInfo.Hosts);

string GetWelcomeMessage()
{
    string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    var welcome = $"NetScan {version}";
    return welcome;
}

void WriteHostsToScreen(List<HostInfo> hostInfos)
{
    if (hostInfos == null || hostInfos.Count == 0)
    {
        return;
    }

    hostInfos = hostInfos.OrderBy(h => h.IpAddressLabel).ToList();

    var maxIpLength = hostInfos.Max(h => h.IpAddress.ToString().Length);
    var maxHostLength = hostInfos.Max(h => h.HostName.Length);
    var maxMacLength = hostInfos.Max(h => h.MacAddress.Length);

    var colSpace = 2;

    Console.WriteLine("IP Address".PadRight(maxIpLength + colSpace) + "Host Name".PadRight(maxHostLength + colSpace) + "MAC Address".PadRight(maxMacLength + colSpace));
    Console.WriteLine(new String('-', maxIpLength).PadRight(maxIpLength + colSpace) + new String('-', maxHostLength).PadRight(maxHostLength + colSpace) + new String('-', maxMacLength).PadRight(maxMacLength + colSpace));

    foreach (var hi in hostInfos)
    {
        Console.WriteLine($"{hi.IpAddress.ToString().PadRight(maxIpLength + 2)}{hi.HostName.PadRight(maxHostLength + 2)}{hi.MacAddress.PadRight(maxMacLength + 2)}");
    }
}