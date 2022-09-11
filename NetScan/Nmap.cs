using NetScan.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NetScan
{
    public class Nmap
    {
        public static nmaprun RunNmap()
        {
            nmaprun result;

            var tempFile = Path.Combine(Path.GetTempPath(), Path.ChangeExtension("nmap-" + Guid.NewGuid().ToString(), ".xml"));

            using (StreamReader nmapStream = ExecuteCommandLine("nmap", $"-sn 192.168.22.0/24 -oX {tempFile}"))
            {
               nmapStream.ReadToEnd();
            }


            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.MaxCharactersFromEntities = 1024;

            XmlSerializer serializer = new XmlSerializer(typeof(nmaprun));

            FileStream fileStream = new FileStream(tempFile, FileMode.Open);
            using (XmlReader reader = XmlReader.Create(fileStream, settings))
            {
                result = (nmaprun)serializer.Deserialize(reader);
            }
            fileStream.Close();

            File.Delete(tempFile);
           
            return result;
        }


        private static StreamReader ExecuteCommandLine(String file, String arguments = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = file;
            startInfo.Arguments = arguments;

            Process process = Process.Start(startInfo);

            return process.StandardOutput;
        }

    }
}
