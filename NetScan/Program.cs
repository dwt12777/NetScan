using NetScan;
using NetScan.Models;
using System.Text;

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

// Print Network Summary
PrintNetworkSummary(networkScanner);

// Print Host List
PrintHosts(networkScanner.NetworkInfo.Hosts);

// Network Scanner Event Handlers
void NetworkScanner_IpScanStarted(object? sender, EventArgs e)
{
    Console.Write("Scanning local area network... ");
}

void NetworkScanner_IpScanProgressUpdated(object? sender, NetworkScanner.IpScanProgressUpdatedEventArgs e)
{
    Console.Write(String.Format("\rScanning local area network... {0}", e.ProgressPercent.ToString("P0")));
}

void NetworkScanner_IpScanCompleted(object? sender, EventArgs e)
{
    Console.WriteLine(string.Format(" ({0:.00}s)", networkScanner.ScanDuration.TotalSeconds));
}

// MAC Vendor Lookup event handlers
void MacVendorLookup_RefreshMacVendorsComplete(object? sender, MacVendorLookup.RefreshMacVendorsCompleteEventArgs e)
{
    if (e.MacCacheChanged)
        Console.WriteLine(string.Format(" ({0:.00}s)", MacVendorLookup.ScanDuration.TotalSeconds));
}

void MacVendorLookup_RefrechMacProgressUpdated(object? sender, MacVendorLookup.RefrechMacProgressUpdatedProgressUpdatedEventArgs e)
{
    Console.Write(String.Format("\rUpdating MAC vendor cache... {0}", e.ProgressPercent.ToString("P0")));
}

void PrintNetworkSummary(NetworkScanner networkScanner)
{
    Console.WriteLine();

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
    var maxHostLength = hostInfos.Max(h => h.HostName.Length);
    var maxMacLength = hostInfos.Max(h => h.MacAddress.Length);
    var maxVendorLength = hostInfos.Max(h => h.MacVendor.Length);

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
        sb.Append(hi.IpAddress.ToString().PadRight(maxIpLength + 2));
        sb.Append(hi.HostName.PadRight(maxHostLength + 2));
        sb.Append(hi.MacAddress.PadRight(maxMacLength + 2));
        sb.Append(hi.MacVendor.PadRight(maxMacLength + 2));
        sb.AppendLine();
    }

    Console.Write(sb.ToString());
}

