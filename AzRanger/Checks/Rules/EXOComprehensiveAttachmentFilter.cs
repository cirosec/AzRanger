using AzRanger.Models;
using AzRanger.Models.ExchangeOnline;
using System.Collections.Generic;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class EXOComprehensiveAttachmentFilter : BaseCheck
    {
        // Reference list of dangerous file extensions
        private static readonly HashSet<string> AttachExts = new HashSet<string>
        {
            "7z", "a3x", "ace", "ade", "adp", "ani", "app", "appinstaller",
            "applescript", "application", "appref-ms", "appx", "appxbundle", "arj",
            "asd", "asx", "bas", "bat", "bgi", "bz2", "cab", "chm", "cmd", "com",
            "cpl", "crt", "cs", "csh", "daa", "dbf", "dcr", "deb",
            "desktopthemepackfile", "dex", "diagcab", "dif", "dir", "dll", "dmg",
            "doc", "docm", "dot", "dotm", "elf", "eml", "exe", "fxp", "gadget", "gz",
            "hlp", "hta", "htc", "htm", "html", "hwpx", "ics", "img",
            "inf", "ins", "iqy", "iso", "isp", "jar", "jnlp", "js", "jse", "kext",
            "ksh", "lha", "lib", "library-ms", "lnk", "lzh", "macho", "mam", "mda",
            "mdb", "mde", "mdt", "mdw", "mdz", "mht", "mhtml", "mof", "msc", "msi",
            "msix", "msp", "msrcincident", "mst", "ocx", "odt", "ops", "oxps", "pcd",
            "pif", "plg", "pot", "potm", "ppa", "ppam", "ppkg", "pps", "ppsm", "ppt",
            "pptm", "prf", "prg", "ps1", "ps11", "ps11xml", "ps1xml", "ps2",
            "ps2xml", "psc1", "psc2", "pub", "py", "pyc", "pyo", "pyw", "pyz",
            "pyzw", "rar", "reg", "rev", "rtf", "scf", "scpt", "scr", "sct",
            "searchConnector-ms", "service", "settingcontent-ms", "sh", "shb", "shs",
            "shtm", "shtml", "sldm", "slk", "so", "spl", "stm", "svg", "swf", "sys",
            "tar", "theme", "themepack", "timer", "uif", "url", "uue", "vb", "vbe",
            "vbs", "vhd", "vhdx", "vxd", "wbk", "website", "wim", "wiz", "ws", "wsc",
            "wsf", "wsh", "xla", "xlam", "xlc", "xll", "xlm", "xls", "xlsb", "xlsm",
            "xlt", "xltm", "xlw", "xnk", "xps", "xsl", "xz", "z"
        };

        // Policy must have at least 90% of extensions to pass
        private const double PassingValue = 0.90;

        public override CheckResult Audit(Tenant tenant)
        {
            int failThreshold = (int)(AttachExts.Count * (1 - PassingValue));

            // Find comprehensive policies (more than 120 extensions defined)
            var comprehensivePolicies = tenant.ExchangeOnlineSettings.MalwareFilterPolicy
                .Where(p => p.FileTypes != null && p.FileTypes.Length > 120)
                .ToList();

            if (comprehensivePolicies.Count == 0)
            {
                this.SetReason("No comprehensive malware filter policy found with more than 120 file types.");
                return CheckResult.Finding;
            }

            foreach (var policy in comprehensivePolicies)
            {
                // Count missing extensions
                var policyFileTypes = new HashSet<string>(policy.FileTypes.Select(f => f.ToLowerInvariant()));
                int missingCount = AttachExts.Count(ext => !policyFileTypes.Contains(ext.ToLowerInvariant()));

                // Find associated rule
                var rule = tenant.ExchangeOnlineSettings.MalwareFilterRule
                    .FirstOrDefault(r => r.MalwareFilterPolicy == policy.Id);

                // Check passing conditions
                bool hasEnoughExtensions = missingCount < failThreshold;
                bool ruleEnabled = rule != null && rule.State == "Enabled";
                bool fileFilterEnabled = policy.EnableFileFilter;

                if (hasEnoughExtensions && ruleEnabled && fileFilterEnabled)
                {
                    return CheckResult.NoFinding;
                }
            }

            return CheckResult.Finding;
        }
    }
}