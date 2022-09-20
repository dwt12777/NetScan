using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetScan
{
    public class ArgsParser
    {
        public enum ArgType
        {
            Verbose,
            ClearCache,
            Json,
            Help
        }

        public bool VerboseRequested { get; set; }
        public bool JsonRequested { get; set; }
        public bool ClearCacheRequested { get; set; }
        public bool HelpRequested { get; set; }

        public Dictionary<string, ArgType> AllowedArgs { get; }

        public ArgsParser(string[] args)
        {
            AllowedArgs = BuildAllowedArgs();
            ProcessArgs(args);
        }

        private void ProcessArgs(string[] args)
        {
            foreach (var arg in args)
            {
                ArgType myType;

                bool isValidArg = AllowedArgs.TryGetValue(arg, out myType);

                if (isValidArg)
                {
                    switch (myType)
                    {
                        case ArgType.Verbose:
                            VerboseRequested = true;
                            break;
                        case ArgType.ClearCache:
                            ClearCacheRequested = true;
                            break;
                        case ArgType.Json:
                            JsonRequested = true;
                            break;
                        case ArgType.Help:
                            HelpRequested = true;                            
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    HelpRequested = true;
                }
            }
        }

        private Dictionary<string, ArgType> BuildAllowedArgs()
        {
            var allowedArgs = new Dictionary<string, ArgType>();

            allowedArgs.Add("-v", ArgType.Verbose);
            allowedArgs.Add("--verbose", ArgType.Verbose);
            allowedArgs.Add("/v", ArgType.Verbose);

            allowedArgs.Add("-c", ArgType.ClearCache);
            allowedArgs.Add("--clear", ArgType.ClearCache);
            allowedArgs.Add("/c", ArgType.ClearCache);

            allowedArgs.Add("-j", ArgType.Json);
            allowedArgs.Add("--json", ArgType.Json);
            allowedArgs.Add("/j", ArgType.Json);

            allowedArgs.Add("-h", ArgType.Help);
            allowedArgs.Add("--help", ArgType.Help);
            allowedArgs.Add("/?", ArgType.Help);
            allowedArgs.Add("-?", ArgType.Help);

            return allowedArgs;
        }
    }
}
