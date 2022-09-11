using NetScan.Models;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace NetScan
{
    public class Nmap
    {
        public static nmaprun RunNmap(string nmapSearch)
        {
            nmaprun result;

            var tempFile = Path.Combine(Path.GetTempPath(), Path.ChangeExtension("nmap-" + Guid.NewGuid().ToString(), ".xml"));

            using (StreamReader nmapStream = ExecuteCommandLine("nmap", $"-sn {nmapSearch} -oX {tempFile}"))
            {
                Console.WriteLine($"Scanning network {nmapSearch} ...");
                nmapStream.ReadToEnd();
                Console.WriteLine();
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

            // To Do: throw exception if nmap not installed
            Process process = Process.Start(startInfo);

            return process.StandardOutput;
        }

    }
}
