using NetScan;
using NetScan.Models;
using System.Text;
using System.Text.Json;

_args = new ArgsParser(args);

if (_args.HelpRequested)
{
    DisplayHelp();
    Environment.Exit(0);
}

if (_args.ClearCacheRequested)
{
    MacVendorCache.ClearCache();
    Console.WriteLine("MAC vendor cache cleared");
    Environment.Exit(0);
}

// Scan local network
var networkScanner = new NetworkScanner();
networkScanner.IpScanStarted += NetworkScanner_IpScanStarted;
networkScanner.IpScanProgressUpdated += NetworkScanner_IpScanProgressUpdated;
networkScanner.IpScanCompleted += NetworkScanner_IpScanCompleted;
networkScanner.GetAllHosts();

// Update MAC cache
MacVendorCache.UpdateMacVendorsProgressUpdate += MacVendorLookup_RefrechMacProgressUpdated;
MacVendorCache.UpdateMacVendorsComplete += MacVendorLookup_RefreshMacVendorsComplete;
MacVendorCache.UpdateMacVendorsForHosts(networkScanner.NetworkInfo.Hosts);

if (_args.JsonRequested)
{
    networkScanner.NetworkInfo.Hosts = networkScanner.NetworkInfo.Hosts.OrderBy(h => h.IpAddressLabel).ToList();
    var jsonOptions = new JsonSerializerOptions() { WriteIndented = true };
    string json = JsonSerializer.Serialize<NetworkInfo>(networkScanner.NetworkInfo, jsonOptions);
    Console.WriteLine(json);
}
else
{
    // Print Network Summary
    PrintNetworkSummary(networkScanner);

    // Print Host List
    PrintHosts(networkScanner.NetworkInfo.Hosts);
}

void DisplayHelp()
{
    string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

    string help = NetScan.Properties.Resources.Help.Replace("{version}", version);

    Utilities.WriteLineWordWrap(help, 4);

}

// Network Scanner Event Handlers
void NetworkScanner_IpScanStarted(object? sender, EventArgs e)
{
    Console.Error.Write("Scanning local area network... ");
}

void NetworkScanner_IpScanProgressUpdated(object? sender, NetworkScanner.IpScanProgressUpdatedEventArgs e)
{
    if (_args.VerboseRequested)
    {
        Console.Error.Write($"\rScanning local area network... Progress: {e.ProgressPercent.ToString("P0")}, Addresses Scanned: {e.AddressesScanned}, Hosts Found: {e.HostsFound}, Processing Time: {string.Format("{0:0.00}s", e.ElapsedTime.TotalSeconds)}");
    }
    else
    {
        Console.Error.Write($"\rScanning local area network... Progress: {e.ProgressPercent.ToString("P0")}");
    }
}

void NetworkScanner_IpScanCompleted(object? sender, EventArgs e)
{   
    Console.Error.WriteLine();
}

// MAC Vendor Lookup event handlers
void MacVendorLookup_RefreshMacVendorsComplete(object? sender, EventArgs e)
{
    Console.Error.WriteLine();
}

void MacVendorLookup_RefrechMacProgressUpdated(object? sender, MacVendorCache.ProgressUpdatedEventArgs e)
{
    if (_args.VerboseRequested)
    {
        Console.Error.Write($"\rUpdating MAC vendor cache...   Progress: {e.ProgressPercent.ToString("P0")}, Current: {e.CacheItemsCurrent}, Added: {e.CacheItemsAdded}, Updated: {e.CacheItemsUpdated}, Processing Time: {string.Format("{0:0.00}s", e.ElapsedTime.TotalSeconds)}");
    }
    else
    {
        Console.Error.Write($"\rUpdating MAC vendor cache...   Progress: {e.ProgressPercent.ToString("P0")}");
    }
}

void PrintNetworkSummary(NetworkScanner networkScanner)
{
    Console.Error.WriteLine();

    PrintTwoColumns("WAN IP", networkScanner.NetworkInfo.WanIp.ToString());
    PrintTwoColumns("Gateway", networkScanner.NetworkInfo.Gateway.IpAddress.ToString());
    PrintTwoColumns("Subnet Mask", networkScanner.NetworkInfo.SubnetMask.ToString());
    PrintTwoColumns("Network", networkScanner.NetworkInfo.Network);
    PrintTwoColumns("Hosts Found", networkScanner.NetworkInfo.Hosts.Count.ToString());
}

void PrintTwoColumns(string col1, string col2, bool newLine = true)
{
    var sep = " : ";
    var col1Width = 11;

    if (newLine)
    {
        Console.WriteLine(col1.PadRight(col1Width) + sep + col2);
    }
    else
    {
        Console.Write($"\r{col1.PadRight(col1Width)}{sep}{col2}");
    }
}

void PrintHosts(List<HostInfo> hostInfos)
{
    var sb = new StringBuilder();

    sb.AppendLine();

    if (hostInfos == null || hostInfos.Count == 0)
    {
        return;
    }

    hostInfos = hostInfos.OrderBy(h => h.IpAddressLabel).ToList();

    var maxIpLength = hostInfos.Max(h => h.IpAddress.ToString().Length);
    var maxHostLength = hostInfos.Where(h => h.HostName != null).Max(h => h.HostName.Length);
    var maxMacLength = hostInfos.Max(h => h.MacAddress.Length);
    var maxVendorLength = hostInfos.Where(H => H.MacVendor != null).Max(h => h.MacVendor.Length);

    var colSpace = 2;

    // Column Headers
    sb.Append("IP Address".PadRight(maxIpLength + colSpace));
    sb.Append("Host Name".PadRight(maxHostLength + colSpace));
    sb.Append("MAC Address".PadRight(maxMacLength + colSpace));
    sb.Append("MAC Vendor".PadRight(maxVendorLength + colSpace));
    sb.AppendLine();

    // Column Header Underlines
    sb.Append(new String('-', maxIpLength).PadRight(maxIpLength + colSpace));
    sb.Append(new String('-', maxHostLength).PadRight(maxHostLength + colSpace));
    sb.Append(new String('-', maxMacLength).PadRight(maxMacLength + colSpace));
    sb.Append(new String('-', maxVendorLength).PadRight(maxMacLength + colSpace));
    sb.AppendLine();

    foreach (var hi in hostInfos)
    {
        sb.Append(hi.IpAddress.ToString().PadRight(maxIpLength + colSpace));

        if (hi.HostName == null)
            sb.Append("".PadRight(maxHostLength + colSpace));
        else
            sb.Append(hi.HostName.PadRight(maxHostLength + colSpace));

        sb.Append(hi.MacAddress.PadRight(maxMacLength + colSpace));

        if (hi.MacVendor == null)
            sb.Append("".PadRight(maxVendorLength + colSpace));
        else
            sb.Append(hi.MacVendor.PadRight(maxVendorLength + colSpace));

        sb.AppendLine();
    }

    Console.Write(sb.ToString());
}

public partial class Program
{
    private static ArgsParser _args;
}
