using NetScan;
using NetScan.Models;
using System.Text;
using System.Text.Json;

if (args.Length == 1 && HelpRequested(args[0]))
{
    DisplayHelp();
    Environment.Exit(0);
}

if (ClearCacheRequested(args))
{
    MacVendorLookup.ClearCache();
}

// Scan local network
var networkScanner = new NetworkScanner();
networkScanner.IpScanStarted += NetworkScanner_IpScanStarted;
networkScanner.IpScanProgressUpdated += NetworkScanner_IpScanProgressUpdated;
networkScanner.IpScanCompleted += NetworkScanner_IpScanCompleted;
networkScanner.GetAllHosts();

// Update MAC cache
MacVendorLookup.RefrechMacProgressUpdated += MacVendorLookup_RefrechMacProgressUpdated;
MacVendorLookup.RefreshMacVendorsComplete += MacVendorLookup_RefreshMacVendorsComplete;
networkScanner.NetworkInfo.Hosts = MacVendorLookup.RefreshMacVendors(networkScanner.NetworkInfo.Hosts);

if (JsonRequested(args))
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

bool ClearCacheRequested(string[] args)
{
    var clearCacheRequested = false;

    foreach (var a in args)
    {
        if (a == "-c" || a == "--clear" || a == "/c")
            clearCacheRequested = true;
    }

    return clearCacheRequested;
}

bool JsonRequested(string[] args)
{
    var jsonRequested = false;

    foreach (var a in args)
    {
        if (a == "-j" || a == "--json" || a == "/j")
            jsonRequested = true;
    }

    return jsonRequested;
}

static bool HelpRequested(string param)
{
    return param == "-h" || param == "--help" || param == "/?" || param == "-?";
}

// Network Scanner Event Handlers
void NetworkScanner_IpScanStarted(object? sender, EventArgs e)
{
    Console.Error.Write("Scanning local area network... ");
}

void NetworkScanner_IpScanProgressUpdated(object? sender, NetworkScanner.IpScanProgressUpdatedEventArgs e)
{
    Console.Error.Write(String.Format("\rScanning local area network... {0}", e.ProgressPercent.ToString("P0")));
}

void NetworkScanner_IpScanCompleted(object? sender, EventArgs e)
{   
    Console.Error.WriteLine(string.Format(" ({0:0.00}s)", networkScanner.ScanDuration.TotalSeconds));
}

// MAC Vendor Lookup event handlers
void MacVendorLookup_RefreshMacVendorsComplete(object? sender, MacVendorLookup.ProgressCompletedEventArgs e)
{
    Console.Error.WriteLine($" - {e.CacheItemsCurrent} Current, {e.CacheItemsAdded} Added, {e.CacheItemsUpdated} Updated");
}

void MacVendorLookup_RefrechMacProgressUpdated(object? sender, MacVendorLookup.ProgressUpdatedEventArgs e)
{
    Console.Error.Write(String.Format("\rUpdating MAC vendor cache... {0}", e.ProgressPercent.ToString("P0")));
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

