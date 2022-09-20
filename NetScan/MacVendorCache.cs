using NetScan.Models;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;

namespace NetScan
{
    public static class MacVendorCache
    {
        private static readonly HttpClient _client = new HttpClient();
        private static DateTime _lastRequestTime = DateTime.Now;
        private static List<MacVendor> _macVendorCache = GetMacVendorsFromCache();
        private static int _cacheItemsCurrent = 0;
        private static int _cacheItemsAdded = 0;
        private static int _cacheItemsUpdated = 0;

        public static event EventHandler<ProgressUpdatedEventArgs> UpdateMacVendorsProgressUpdate;
        public static event EventHandler<ProgressCompletedEventArgs> UpdateMacVendorsComplete;

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

            int cacheDays = int.Parse(ConfigurationManager.AppSettings.Get("CacheDayThreshold") ?? 365.ToString());

            var macVendor = _macVendorCache
                .Where(v => (DateTime.Now - v.LookupDate).TotalDays < cacheDays)
                .FirstOrDefault(v => v.MacAddress == macAddress);

            // if macVendor isn't null that means there's an existing entry in the cache within the right date range
            if (macVendor != null)
            {
                _cacheItemsCurrent++;
            }
            // if it is null, ether the cached entry has aged out or there isn't one at all, in eaither case, need to look it up through the API
            else
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

                // If API didn't return a value, create MV with empty name for cache so subsequent runs don't keep trying to look it up
                if (macVendor == null)
                {
                    macVendor = new MacVendor() { MacAddress = macAddress, LookupDate = DateTime.Now, Vendor = null };
                }

                // add the new entry to the cache
                _macVendorCache.Add(macVendor);
                _macVendorCache = _macVendorCache.OrderBy(c => c.LookupDate).ToList();
                string json = JsonSerializer.Serialize<List<MacVendor>>(_macVendorCache);
                File.WriteAllText(MacVendorCacheFile, json);
            }

            return macVendor.Vendor;
        }

        public static void UpdateMacVendorsForHosts(List<HostInfo> hosts)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var macAddresses = hosts.Select(x => x.MacAddress).Distinct().ToList();

            var progressArgs = new ProgressUpdatedEventArgs()
            {
                WorkItemCompletedCount = 0,
                WorkItemTotalCount = macAddresses.Count
            };

            _cacheItemsAdded = 0;
            _cacheItemsUpdated = 0;
            _cacheItemsCurrent = 0;

            foreach (var ma in macAddresses)
            {
                var macVendor = GetMacVendor(ma);
                hosts
                    .Where(h => h.MacAddress == ma)
                    .ToList()
                    .ForEach(h => h.MacVendor = macVendor);
                progressArgs.WorkItemCompletedCount++;
                progressArgs.ElapsedTime = stopwatch.Elapsed;
                OnRefrechMacProgressUpdated(progressArgs);
            }

            stopwatch.Stop();

            var completedArgs = new ProgressCompletedEventArgs()
            {
                CacheItemsCurrent = _cacheItemsCurrent,
                CacheItemsAdded = _cacheItemsAdded,
                CacheItemsUpdated = _cacheItemsUpdated,
                ProcessingTime = stopwatch.Elapsed
            };

            UpdateMacVendorsComplete?.Invoke(null, completedArgs);
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
            var requestString = $"{Properties.Resources.MacVendorApiUrl}{macAddress}";
            var throttle = int.Parse(Properties.Resources.MacVendorApiThrottleMilliseconds);

            TimeSpan lastRequestAge = DateTime.Now - _lastRequestTime;

            if (lastRequestAge.TotalMilliseconds < throttle)
            {
                var sleepMilliseconds = throttle - (int)lastRequestAge.TotalMilliseconds;
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
            EventHandler<ProgressUpdatedEventArgs> handler = UpdateMacVendorsProgressUpdate;
            handler?.Invoke(null, e);
        }

        public static void OnRefreshMacVendorsComplete(ProgressCompletedEventArgs e)
        {
            EventHandler<ProgressCompletedEventArgs> handler = UpdateMacVendorsComplete;
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
