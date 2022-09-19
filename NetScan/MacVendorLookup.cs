using NetScan.Models;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace NetScan
{
    public static class MacVendorLookup
    {
        public static event EventHandler<RefreshMacVendorsCompleteEventArgs> RefreshMacVendorsComplete;

        public class RefreshMacVendorsCompleteEventArgs : EventArgs
        {
            public bool MacCacheChanged { get; set; }
        }

        public static void OnRefreshMacVendorsComplete(RefreshMacVendorsCompleteEventArgs e)
        {
            EventHandler<RefreshMacVendorsCompleteEventArgs> handler = RefreshMacVendorsComplete;
            handler?.Invoke(null, e);
        }

        public static event EventHandler<RefrechMacProgressUpdatedProgressUpdatedEventArgs> RefrechMacProgressUpdated;

        public static void OnRefrechMacProgressUpdated(RefrechMacProgressUpdatedProgressUpdatedEventArgs e)
        {
            EventHandler<RefrechMacProgressUpdatedProgressUpdatedEventArgs> handler = RefrechMacProgressUpdated;
            handler?.Invoke(null, e);
        }

        public class RefrechMacProgressUpdatedProgressUpdatedEventArgs : EventArgs
        {
            public int WorkItemCompletedCount { get; set; }
            public int WorkItemTotalCount { get; set; }
            public double ProgressPercent => (double)WorkItemCompletedCount / (double)WorkItemTotalCount;
            public TimeSpan ElapsedTime { get; set; }
        }

        private static readonly HttpClient _client = new HttpClient();
        private static DateTime _lastRequestTime = DateTime.Now;
        private static List<MacVendor> _macVendorCache = GetMacVendorsFromCache();
        public static TimeSpan ScanDuration { get; set; }

        public static string MacVendorCacheFile
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appName = "NetScan";
                Directory.CreateDirectory(Path.Combine(appData, appName));
                return Path.Combine(appData, appName, "MacVendorCache.json");
            }
        }

        private static Stopwatch _stopwatch = new Stopwatch();

        private static void UpdateProgress(int workItemCompletedCount, int workItemTotalCount, TimeSpan elapstedTime)
        {
            var args = new RefrechMacProgressUpdatedProgressUpdatedEventArgs()
            {
                WorkItemCompletedCount = workItemCompletedCount,
                WorkItemTotalCount = workItemTotalCount,
                ElapsedTime = elapstedTime
            };

            OnRefrechMacProgressUpdated(args);
        }

        public static string GetMacVendor(string macAddress)
        {
            var macVendor = _macVendorCache.FirstOrDefault(x => x.MacAddress == macAddress)?.Vendor;

            if (macVendor == null)
            {
                var v = GetMacVendorFromApi(macAddress);
                macVendor = v.Vendor;
                _macVendorCache.Add(v);
                string json = JsonSerializer.Serialize<List<MacVendor>>(_macVendorCache);
                File.WriteAllText(MacVendorCacheFile, json);
            }

            return macVendor;
        }

        public static List<HostInfo> RefreshMacVendors(List<HostInfo> hosts)
        {
            _stopwatch.Start();

            int cacheDayThreshold = int.Parse(ConfigurationManager.AppSettings.Get("CacheDayThreshold"));

            var macVendorsFromApi = new List<MacVendor>();

            var macCache = GetMacVendorsFromCache();

            // read from cache first
            if (File.Exists(MacVendorCacheFile))
            {
                foreach (var h in hosts)
                {
                    var macVendor = _macVendorCache
                        .Where(v => (DateTime.Now - v.LookupDate).TotalDays < cacheDayThreshold)
                        .FirstOrDefault(v => v.MacAddress == h.MacAddress);
                    if (macVendor != null)
                    {
                        h.MacVendor = macVendor.Vendor;
                    }
                }
            }

            int workItemTotalCount = hosts.Where(h => string.IsNullOrEmpty(h.MacVendor)).ToList().Count;
            int workItemCompletedCount = 0;

            var args = new RefreshMacVendorsCompleteEventArgs()
            {
                MacCacheChanged = false
            };

            // lookup mac vendors from API for hosts not in cache
            foreach (var h in hosts)
            {
                if (string.IsNullOrEmpty(h.MacVendor))
                {
                    var macVendor = GetMacVendorFromApi(h.MacAddress);
                    macVendorsFromApi.Add(macVendor);
                    h.MacVendor = macVendor.Vendor;
                    args.MacCacheChanged = true;
                    workItemCompletedCount += 1;
                    UpdateProgress(workItemCompletedCount, workItemTotalCount, _stopwatch.Elapsed);
                }
            }

            _stopwatch.Stop();
            ScanDuration = _stopwatch.Elapsed;

            RefreshMacVendorsComplete?.Invoke(null, args);

            // rebuild and save cache
            SaveMacVendorsToCache(macVendorsFromApi);

            return hosts;
        }

        private static void SaveMacVendorsToCache(List<MacVendor> macVendors)
        {
            
            foreach (var v in macVendors)
            {
                if (_macVendorCache.FirstOrDefault(c => c.MacAddress == v.MacAddress) == null)
                {
                    _macVendorCache.Add(v);
                }
                else
                {
                    var existingMacVendor = _macVendorCache.FirstOrDefault(c => c.MacAddress == v.MacAddress);
                    _macVendorCache.Remove(existingMacVendor);
                    _macVendorCache.Add(v);
                }
            }

            _macVendorCache = _macVendorCache.OrderBy(v => v.LookupDate).ToList();

            string json = JsonSerializer.Serialize<List<MacVendor>>(_macVendorCache);
            File.WriteAllText(MacVendorCacheFile, json);
        }

        private static List<MacVendor> GetMacVendorsFromCache()
        {
            var macVendors = new List<MacVendor>();

            if (File.Exists(MacVendorCacheFile))
            {
                var jsonString = File.ReadAllText(MacVendorCacheFile);
                macVendors = JsonSerializer.Deserialize<List<MacVendor>>(jsonString);
            }

            return macVendors;
        }

        private static MacVendor GetMacVendorFromApi(string macAddress)
        {
            var requestString = $"https://api.macvendors.com/{macAddress}";
            var requestIntervalInMilliseconts = 1000;

            TimeSpan lastRequestAge = DateTime.Now - _lastRequestTime;

            if (lastRequestAge.TotalMilliseconds < requestIntervalInMilliseconts)
            {
                var sleepMilliseconds = requestIntervalInMilliseconts - (int)lastRequestAge.TotalMilliseconds;
                Thread.Sleep(sleepMilliseconds);
            }

            string macVendorString = _client
                .GetStringAsync(requestString)
                .Result
                .Replace("\\r\\n", "")
                .Replace("\\n", "")
                .Trim();

            _lastRequestTime = DateTime.Now;

            if (!string.IsNullOrEmpty(macVendorString))
                return new MacVendor()
                {
                    MacAddress = macAddress,
                    Vendor = macVendorString,
                    LookupDate = DateTime.Now
                };
            else
                return null;
        }

        public static void ClearCache()
        {
            File.Delete(MacVendorCacheFile);
        }
    }
}
