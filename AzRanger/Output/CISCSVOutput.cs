using AzRanger.Checks;
using AzRanger.Models;
using CsvHelper;
using CsvHelper.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace AzRanger.Output
{
    class CISCSVOutput
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void Print(Auditor auditor, String outPath)
        {
            Directory.CreateDirectory(outPath);
            CsvConfiguration config = new CsvConfiguration(CultureInfo.CurrentCulture);
            config.Delimiter = ";";
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.WriteField("CheckIdentifier");
                csvWriter.WriteField("CIS Section");
                csvWriter.WriteField("CIS Checkname");
                csvWriter.WriteField("Scope");
                csvWriter.WriteField("Maturity");
                csvWriter.WriteField("Result");
                csvWriter.WriteField("ID");
                csvWriter.WriteField("Name");
                csvWriter.WriteField("PortalLink");
                csvWriter.NextRecord();

                WriteChecks(csvWriter, auditor.Finding, "Failed");
                WriteChecks(csvWriter, auditor.NoFinding, "Passed");
                WriteChecks(csvWriter, auditor.Error, "Error");

                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());
                var fileDestination = Path.Combine(outPath, "CISResult.csv");
                File.WriteAllText(fileDestination, result);
            }
        }

        private static void WriteChecks(CsvWriter csvWriter, IEnumerable<BaseCheck> checks, string result)
        {
            foreach (BaseCheck check in checks)
            {
                if (!RuleInfo.TryGet(check.GetType().Name, out RuleInfo ruleInfo))
                {
                    logger.Error("[-] CISCSVOutput: Failed to get rule info of {0}", check.GetType().Name);
                    continue;
                }

                if (ruleInfo.CISM365Section != null)
                    WriteRows(csvWriter, ruleInfo, ruleInfo.CISM365Section, ruleInfo.ShortDescription, "CISM365", result, check);
                if (ruleInfo.CISAZSection != null)
                    WriteRows(csvWriter, ruleInfo, ruleInfo.CISAZSection, ruleInfo.ShortDescription, "CISAZ", result, check);
            }
        }

        private static void WriteRows(CsvWriter csvWriter, RuleInfo ruleInfo,
            string section, string title, string scope, string result, BaseCheck check)
        {
            var entities = check.GetAffectedEntity();
            if (result == "Failed" && entities.Count > 0)
            {
                foreach (IReporting entity in entities)
                {
                    string[] data = entity.PrintCSV().Split(';');
                    string id = data.Length > 0 ? data[0] : "";
                    string name = data.Length > 1 ? data[1] : "";
                    WriteRow(csvWriter, ruleInfo, section, title, scope, result, id, name);
                }
            }
            else
            {
                WriteRow(csvWriter, ruleInfo, section, title, scope, result, "", "");
            }
        }

        private static void WriteRow(CsvWriter csvWriter, RuleInfo ruleInfo,
            string section, string title, string scope, string result, string id, string name)
        {
            csvWriter.WriteField(ruleInfo.ShortName);
            csvWriter.WriteField(section);
            csvWriter.WriteField(title);
            csvWriter.WriteField(scope);
            csvWriter.WriteField(ruleInfo.MaturityLevel.ToString());
            csvWriter.WriteField(result);
            csvWriter.WriteField(id);
            csvWriter.WriteField(name);
            csvWriter.WriteField(ruleInfo.PortalUrl);
            csvWriter.NextRecord();
        }
    }
}
