using NetScan;
using NetScan.Models;
using System.Reflection;

Console.WriteLine();
Console.WriteLine(GetWelcomeMessage());

var networkScanner = new NetworkScanner();

networkScanner.GetAllHosts();

WriteHostsToScreen(networkScanner.NetworkInfo.Hosts);

Console.WriteLine();
Console.WriteLine($"Total Hosts Online: {networkScanner.NetworkInfo.Hosts.Count}");
Console.WriteLine();

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

    var colSpace = 2;

    Console.WriteLine("IP Address".PadRight(maxIpLength + colSpace) + "Host Name".PadRight(maxHostLength + colSpace) + "MAC Address".PadRight(maxMacLength + colSpace));
    Console.WriteLine(new String('-', maxIpLength).PadRight(maxIpLength + colSpace) + new String('-', maxHostLength).PadRight(maxHostLength + colSpace) + new String('-', maxMacLength).PadRight(maxMacLength + colSpace));

    foreach (var hi in hostInfos)
    {
        Console.WriteLine($"{hi.IpAddress.PadRight(maxIpLength + 2)}{hi.HostName.PadRight(maxHostLength + 2)}{hi.MacAddress.PadRight(maxMacLength + 2)}");
    }
}