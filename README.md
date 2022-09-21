# NetScan
From the help file:

```
Netscan v1.2.3.0

DESCRIPTION
-----------
Scan your local network and retrieve a list of all active hosts.

USAGE
-----

  netscan [options]

    Options:

      /?, -?, -h, --help    Show this help screen
      /c, -c, --clear       Clear the mac vendor cache (see below)
      /j, -j, --json        Output results in JSON format
      /v, -v, --verbose     Shows additional processing data while scanning network and updating cache


MAC VENDOR CACHE
----------------
Netscan uses a third party provider (https://macvendors.com/) to retrieve MAC vendor names. The process is throttled to
allow one lookup per second. However, Netscan caches the results so that subsequent executions use the cached data and
return your results much faster. By default, the items are cached for 365 days. You can change that by editing the file
"NetScan.dll.config" found in the installation directory.
```

## ABOUT
While there are many great and far more robust tools for such a task (for example: https://nmap.org/) that I highly recommend you check out, I wrote this for two reasons:

1. Nmap wasn't always returning host names, even when I tried to force it.
2. I wanted the output in a clean, easy-to-read table.

## Example 1: `netscan` (with no parameters)
(Actual IP addresses and MAC addresses have been masked with xx here)
```
C:\>netscan
Scanning local area network... Progress: 100%
Updating MAC vendor cache...   Progress: 100%

WAN IP      : 47.187.xxx.xxx
Gateway     : 192.168.xxx.xxx
Subnet Mask : 255.255.255.0
Network     : 192.168.xxx.0/24
Hosts Found : 20

IP Address      Host Name                 MAC Address        MAC Vendor
--------------  ------------------------  -----------------  -------------------------------------------------
192.168.xx.xxx  taxxx.home                50:87:B8:xx:xx:xx  Nuvyyo Inc
192.168.xx.xxx  dixxx.home                4C:E1:73:xx:xx:xx  Huizhou Dehong Technology Co., Ltd.
192.168.xx.xxx  roxxxx.home               78:D2:94:xx:xx:xx  NETGEAR
192.168.xx.xxx  unxxxxxx.home             E0:63:DA:xx:xx:xx  Ubiquiti Networks Inc.
192.168.xx.xxx  spxxxxxxxxxxxx.home       A0:D0:DC:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  rexxxxxx.home             DC:A6:32:xx:xx:xx  Raspberry Pi Trading Ltd
192.168.xx.xxx  psx.home                  70:9E:29:xx:xx:xx  Sony Interactive Entertainment Inc.
192.168.xx.xxx  spxxxxxxx.home            9C:50:D1:xx:xx:xx  Murata Manufacturing Co., Ltd.
192.168.xx.xxx  nixxxxxxxxx.home          DC:4F:22:xx:xx:xx  Espressif Inc.
192.168.xx.xxx  lixxxxxxxx.home           84:0D:8E:xx:xx:xx  Espressif Inc.
192.168.xx.xxx  phxxxxxxxx.home           14:2D:4D:xx:xx:xx  Apple, Inc.
192.168.xx.xxx  tvxxxxxxxxxxx.home        D8:14:DF:xx:xx:xx  TCL King Electrical Appliances (Huizhou) Co., Ltd
192.168.xx.xxx  chxxxxxxxx.home           F0:EF:86:xx:xx:xx  Google, Inc.
192.168.xx.xxx  lixxxxxxxxxxxx.home       1C:12:B0:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  raxxxx.home               E8:40:F2:xx:xx:xx  PEGATRON CORPORATION
192.168.xx.xxx  roxxxxxxxxxx.home         AC:3A:7A:xx:xx:xx  Roku, Inc.
192.168.xx.xxx  phxxxxxxxxx.home          74:74:46:xx:xx:xx  Google, Inc.
192.168.xx.xxx  spxxxxxxxxxx.home         AC:63:BE:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  taxxxxxxxxx.home          B8:FF:61:xx:xx:xx  Apple, Inc.
192.168.xx.xxx  phxxxxxxxx.home           14:C2:13:xx:xx:xx  Apple, Inc.
```

## Example 2: `netscan /j` (results in JSON format) 
(Note: if you pipe the output to a file like `netscan /j > netscan.json` it'll omit the progess lines on top and just give you the JSON string)
```
C:\>netscan /j
Scanning local area network... Progress: 100%
Updating MAC vendor cache...   Progress: 100%
{
  "WanIp": "47.187.xxx.xxx",
  "Gateway": {
    "IpAddress": "192.168.xx.xx",
    "HostName": "roxxx.home",
    "MacAddress": "78:D2:94:xx:xx:xx",
    "MacVendor": "NETGEAR"
  },
  "SubnetMask": "255.255.255.0",
  "Network": "192.168.xx.0/24",
  "ScanDate": "2022-09-20T23:12:22.0780241-05:00",
  "ScanDurationSeconds": 4.4177117,
  "Hosts": [
    {
      "IpAddress": "192.168.xx.xx",
      "HostName": "taxxx.home",
      "MacAddress": "50:87:B8:xx:xx:xx",
      "MacVendor": "Nuvyyo Inc"
    },
    {
      "IpAddress": "192.168.xx.xx",
      "HostName": "dixxx.home",
      "MacAddress": "4C:E1:73:xx:xx:xx",
      "MacVendor": "Huizhou Dehong Technology Co., Ltd."
    },
    {
      "IpAddress": "192.168.xx.xx",
      "HostName": "roxxxx.home",
      "MacAddress": "78:D2:94:xx:xx:xx",
      "MacVendor": "NETGEAR"
    },
    {
      "IpAddress": "192.168.xx.xx",
```

## Example 3: `netscan /v` (verbose)
"Verbose" might be overstating it.  It just shows you a few more details in the 2 progress lines on top.

```
C:\>netscan /v
Scanning local area network... Progress: 100%, Addresses Scanned: 254, Hosts Found: 20, Processing Time: 4.32s
Updating MAC vendor cache...   Progress: 100%, Current: 20, Added: 0, Updated: 0, Processing Time: 0.01s

WAN IP      : 47.187.xxx.xxx
Gateway     : 192.168.xxx.xxx
Subnet Mask : 255.255.255.0
Network     : 192.168.xxx.0/24
Hosts Found : 20

IP Address      Host Name                 MAC Address        MAC Vendor
--------------  ------------------------  -----------------  -------------------------------------------------
192.168.xx.xxx  taxxx.home                50:87:B8:xx:xx:xx  Nuvyyo Inc
192.168.xx.xxx  dixxx.home                4C:E1:73:xx:xx:xx  Huizhou Dehong Technology Co., Ltd.
192.168.xx.xxx  roxxxx.home               78:D2:94:xx:xx:xx  NETGEAR
192.168.xx.xxx  unxxxxxx.home             E0:63:DA:xx:xx:xx  Ubiquiti Networks Inc.
192.168.xx.xxx  spxxxxxxxxxxxx.home       A0:D0:DC:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  rexxxxxx.home             DC:A6:32:xx:xx:xx  Raspberry Pi Trading Ltd
192.168.xx.xxx  psx.home                  70:9E:29:xx:xx:xx  Sony Interactive Entertainment Inc.
192.168.xx.xxx  spxxxxxxx.home            9C:50:D1:xx:xx:xx  Murata Manufacturing Co., Ltd.
192.168.xx.xxx  nixxxxxxxxx.home          DC:4F:22:xx:xx:xx  Espressif Inc.
192.168.xx.xxx  lixxxxxxxx.home           84:0D:8E:xx:xx:xx  Espressif Inc.
192.168.xx.xxx  phxxxxxxxx.home           14:2D:4D:xx:xx:xx  Apple, Inc.
192.168.xx.xxx  tvxxxxxxxxxxx.home        D8:14:DF:xx:xx:xx  TCL King Electrical Appliances (Huizhou) Co., Ltd
192.168.xx.xxx  chxxxxxxxx.home           F0:EF:86:xx:xx:xx  Google, Inc.
192.168.xx.xxx  lixxxxxxxxxxxx.home       1C:12:B0:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  raxxxx.home               E8:40:F2:xx:xx:xx  PEGATRON CORPORATION
192.168.xx.xxx  roxxxxxxxxxx.home         AC:3A:7A:xx:xx:xx  Roku, Inc.
192.168.xx.xxx  phxxxxxxxxx.home          74:74:46:xx:xx:xx  Google, Inc.
192.168.xx.xxx  spxxxxxxxxxx.home         AC:63:BE:xx:xx:xx  Amazon Technologies Inc.
192.168.xx.xxx  taxxxxxxxxx.home          B8:FF:61:xx:xx:xx  Apple, Inc.
192.168.xx.xxx  phxxxxxxxx.home           14:C2:13:xx:xx:xx  Apple, Inc.
```
In particular, during the network scan it shows you:
* Addresses Scanned = the number of IP addresses scanned
* Hosts Found = the number of devices that responded to the scan
* Processing Time = How long it took to scan the network.

And when updating the MAC vendor cache, verbose mode shows you:
* Current = The number of MAC addresses from this current scan that were already in the cache and haven't expired (meaning netscan didn't have to look up the MAC vendors externally and was able to used the cached entry instead)
* Added = The number of MAC addresses from this current scan that weren't in the cache and had to be looked up externally and were then added to the cache
* Updated = The number of MAC addresses that were already in the cache but had expired and were then updated
* Processing Time: how long it took to check (and update if necessary) the cache

## About the MAC Vendor Cache
MAC vendor names are cached in a JSON file located at `%AppData%\NetScan\MacVendorCache.json`.  If you run `netscan /c` it'll delete that file and build a fresh one the next time you run a scan.  Just remember, if you're rebuilding the cache from scratch it'll take longer to run because look-ups are throttled to 1 per second.

Also, by default cached values are used for up to 365 days after being added to the cache.  This is controlled in a config file found wherever you installed the program, typically "C:\Program Files\ReardenTools\NetScan\NetScan.dll.config"  

```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="CacheDayThreshold" value="365" />
  </appSettings>
</configuration>
```
Just change the value for CacheDayThreshold to any positive integer.

## CREDIT
* [IPNetwork by Luc Dvchosal](https://github.com/lduchosal/ipnetwork) - Great library for some of the network discovery tasks.  Thank you for making your hard work available!
* [MACVendors.com](https://macvendors.com/) - A free and very robust MAC vendor database.  This is the site netscan uses to find and cache mac vendor names.
