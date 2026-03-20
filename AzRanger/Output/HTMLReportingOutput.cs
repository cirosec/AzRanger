using AzRanger.Checks;
using AzRanger.Models;
using AzRanger.Utilities;
using ICSharpCode.SharpZipLib.Zip;
using NLog;
using System;
using System.IO;
using System.Resources;

namespace AzRanger.Output
{
    internal class HTMLReportingOutput
    {
        internal static Logger logger = LogManager.GetCurrentClassLogger();
        public static void Print(Auditor auditor, Tenant tenant, String outPath)
        {
            if (string.IsNullOrEmpty(outPath))
            {
                outPath = ".";
            }
            else
            {
                if (Directory.Exists(outPath))
                {
                    Directory.Delete(outPath, true);
                }
                Directory.CreateDirectory(outPath);
            }
            try
            {
                byte[] objData = Properties.Resource.ReportTemplate;
                using (MemoryStream objMS = new MemoryStream(objData))
                using (ZipInputStream objZIP = new ZipInputStream(objMS))
                {
                    ZipEntry theEntry;
                    Directory.CreateDirectory(Path.Combine(outPath, "css"));
                    Directory.CreateDirectory(Path.Combine(outPath, "js"));
                    while ((theEntry = objZIP.GetNextEntry()) != null)
                    {
                        if (theEntry.Name == "css/" || theEntry.Name == "js/")
                        {
                            continue;
                        }
                        using (FileStream streamWriter = File.Create(Path.Combine(outPath, theEntry.Name)))
                        {
                            int size;
                            byte[] data = new byte[8192];
                            while ((size = objZIP.Read(data, 0, data.Length)) > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                        }
                    }
                }
            }
            catch (MissingManifestResourceException ex)
            {
                logger.Debug("[-] " + ex.Message);
            }
            catch (Exception e1)
            {
                logger.Debug("[-] " + e1.Message);
            }

            JSONDumper.WriteToFile(tenant, Path.Combine(outPath, "js/data.js"), "var tenantData = ");
            JSONDumper.WriteToFile(JSONOutput.CreateJSON(auditor), Path.Combine(outPath, "js/report.js"), "var reportData = ");
        }
    }
}
