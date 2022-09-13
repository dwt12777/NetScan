using NetScan;
using NetScan.Models;
using System.Reflection;
using System.Text;

Console.WriteLine(GetWelcomeMessage());

var networkScanner = new NetworkScanner();

Console.Write($"Scan in progress... ");
networkScanner.GetAllHosts();
Console.WriteLine();

Console.WriteLine();
Console.WriteLine($"Total Hosts Found: {networkScanner.NetworkInfo.Hosts.Count}");
Console.WriteLine();

Console.WriteLine(WriteHostsToScreen(networkScanner.NetworkInfo.Hosts));

string GetWelcomeMessage()
{
    var sb = new StringBuilder();

    string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
    sb.AppendLine();
    sb.AppendLine($"NetScan {version}");
    return sb.ToString();
}

string WriteHostsToScreen(List<HostInfo> hostInfos)
{
    var sb = new StringBuilder();

    if (hostInfos == null || hostInfos.Count == 0)
    {
        sb.AppendLine("No hosts found");
        return sb.ToString();
    }

    hostInfos = hostInfos.OrderBy(h => h.IpAddressLabel).ToList();

    var maxIpLength = hostInfos.Max(h => h.IpAddress.ToString().Length);
    var maxHostLength = hostInfos.Max(h => h.HostName.Length);
    var maxMacLength = hostInfos.Max(h => h.MacAddress.Length);

    var colSpace = 2;

    sb.AppendLine("IP Address".PadRight(maxIpLength + colSpace) + "Host Name".PadRight(maxHostLength + colSpace) + "MAC Address".PadRight(maxMacLength + colSpace));
    sb.AppendLine(new String('-', maxIpLength).PadRight(maxIpLength + colSpace) + new String('-', maxHostLength).PadRight(maxHostLength + colSpace) + new String('-', maxMacLength).PadRight(maxMacLength + colSpace));

    foreach (var hi in hostInfos)
    {
        sb.AppendLine($"{hi.IpAddress.ToString().PadRight(maxIpLength + 2)}{hi.HostName.PadRight(maxHostLength + 2)}{hi.MacAddress.PadRight(maxMacLength + 2)}");
    }

    return sb.ToString();
}