using NetScan.Models;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace NetScan
{
    public static class MacVendorLookup
    {
        private static readonly HttpClient _client = new HttpClient();
        private static DateTime _lastRequestTime = DateTime.Now;
        private static List<MacVendor> _macVendorCache = GetMacVendorsFromCache();
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int _cacheItemsCurrent = 0;
        private static int _cacheItemsAdded = 0;
        private static int _cacheItemsUpdated = 0;

        public static event EventHandler<ProgressCompletedEventArgs> RefreshMacVendorsComplete;
        public static event EventHandler<ProgressUpdatedEventArgs> RefrechMacProgressUpdated;

        public static string MacVendorCacheFile
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var appName = Properties.Resources.AppName;
                Directory.CreateDirectory(Path.Combine(appData, appName));
                return Path.Combine(appData, appName, Properties.Resources.MacVendorCacheFileName);
            }
        }

        public static string GetMacVendor(string macAddress)
        {
            int cacheDayThreshold = int.Parse(ConfigurationManager.AppSettings.Get("CacheDayThreshold"));

            var macVendor = _macVendorCache
                .Where(v => (DateTime.Now - v.LookupDate).TotalDays < cacheDayThreshold)
                .FirstOrDefault(v => v.MacAddress == macAddress);

            if (macVendor == null)
            {
                macVendor = GetMacVendorFromApi(macAddress);

                // remove the cached entry, if any (there may be one in there older than cachDayThreshold)
                var cachedEntry = _macVendorCache.FirstOrDefault(c => c.MacAddress == macAddress);
                if (cachedEntry != null)
                {
                    _macVendorCache.Remove(cachedEntry);
                    _cacheItemsUpdated++;
                }
                else
                {
                    _cacheItemsAdded++;
                }

                // If API didn't return a value, create MV with empty name for cache
                if (macVendor == null)
                {
                    macVendor = new MacVendor() { MacAddress = macAddress, LookupDate = DateTime.Now, Vendor = null };
                }
                // add the new entry retrived from API (will have updated vendor name and timestamp)
                _macVendorCache.Add(macVendor);
                _macVendorCache = _macVendorCache.OrderBy(c => c.LookupDate).ToList();
                string json = JsonSerializer.Serialize<List<MacVendor>>(_macVendorCache);
                File.WriteAllText(MacVendorCacheFile, json);
            }
            else
            {
                _cacheItemsCurrent++;
            }

            return macVendor.Vendor;
        }

        public static List<HostInfo> RefreshMacVendors(List<HostInfo> hosts)
        {
            _stopwatch.Start();

            List<string> macAddresses = hosts.Select(x => x.MacAddress).Distinct().ToList();

            var progressArgs = new ProgressUpdatedEventArgs()
            {
                WorkItemCompletedCount = 0,
                WorkItemTotalCount = macAddresses.Count
            };

            _cacheItemsAdded = 0;
            _cacheItemsUpdated = 0;
            _cacheItemsCurrent = 0;

            foreach (var m in macAddresses)
            {
                var macVendor = GetMacVendor(m);
                hosts
                    .Where(h => h.MacAddress == m)
                    .ToList()
                    .ForEach(h => h.MacVendor = macVendor);
                progressArgs.WorkItemCompletedCount++;
                progressArgs.ElapsedTime = _stopwatch.Elapsed;
                OnRefrechMacProgressUpdated(progressArgs);
            }

            _stopwatch.Stop();

            var completedArgs = new ProgressCompletedEventArgs()
            {
                CacheItemsCurrent = _cacheItemsCurrent,
                CacheItemsAdded = _cacheItemsAdded,
                CacheItemsUpdated = _cacheItemsUpdated,
                ProcessingTime = _stopwatch.Elapsed
            };

            RefreshMacVendorsComplete?.Invoke(null, completedArgs);

            return hosts;
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

            string macVendorString;
            try
            {
                macVendorString = _client
                .GetStringAsync(requestString)
                .Result
                .Replace("\\r\\n", "")
                .Replace("\\n", "")
                .Trim();
            }
            catch (Exception)
            {
                macVendorString = null;
            }


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

        public static void OnRefrechMacProgressUpdated(ProgressUpdatedEventArgs e)
        {
            EventHandler<ProgressUpdatedEventArgs> handler = RefrechMacProgressUpdated;
            handler?.Invoke(null, e);
        }

        public static void OnRefreshMacVendorsComplete(ProgressCompletedEventArgs e)
        {
            EventHandler<ProgressCompletedEventArgs> handler = RefreshMacVendorsComplete;
            handler?.Invoke(null, e);
        }

        public class ProgressUpdatedEventArgs : EventArgs
        {
            public int WorkItemCompletedCount { get; set; }
            public int WorkItemTotalCount { get; set; }
            public double ProgressPercent => (double)WorkItemCompletedCount / (double)WorkItemTotalCount;
            public TimeSpan ElapsedTime { get; set; }
        }

        public class ProgressCompletedEventArgs : EventArgs
        {
            public int CacheItemsCurrent { get; set; }
            public int CacheItemsAdded { get; set; }
            public int CacheItemsUpdated { get; set; }
            public TimeSpan ProcessingTime { get; set; }
        }
    }
}
