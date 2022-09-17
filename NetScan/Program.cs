using NetScan;
using NetScan.Models;
using System.Reflection;
using System.Text;

Console.WriteLine();

string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
WriteTwoColumns("NetScan Version", version);


var networkScanner = new NetworkScanner();

WriteTwoColumns("Network", networkScanner.Network);
WriteTwoColumns("Subnet", networkScanner.NetworkInfo.SubnetMask.ToString());
WriteTwoColumns("Gateway", networkScanner.NetworkInfo.Gateway.IpAddress.ToString());
WriteTwoColumns("WAN IP", networkScanner.NetworkInfo.WanIp.ToString());


networkScanner.GetAllHosts();

WriteTwoColumns("Hosts Found", networkScanner.NetworkInfo.Hosts.Count.ToString());

WriteHostsToScreen(networkScanner.NetworkInfo.Hosts);


void WriteTwoColumns(string col1, string col2)
{
    var sep = " : ";
    var col1Width = 15;

    Console.WriteLine(col1.PadRight(col1Width) + sep + col2);
}

void WriteHostsToScreen(List<HostInfo> hostInfos)
{
    var sb = new StringBuilder();

    sb.AppendLine();

    if (hostInfos == null || hostInfos.Count == 0)
    {
        return;
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

    Console.Write(sb.ToString());
}