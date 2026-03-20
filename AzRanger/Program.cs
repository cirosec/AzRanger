using AzRanger.AzScanner;
using AzRanger.Checks;
using AzRanger.Models;
using AzRanger.Output;
using AzRanger.Utilities;
using CommandLine;
using CommandLine.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzRanger
{
    class Program
    {
        // String AzurePowerShell = "1950a258-227b-4e31-a9cf-717495945fc2";
        // Azure Active Directory Powershell = "1b730954-1685-4b74-9bfd-dac224a7b894"
        // AzureCli = "04b07795-8ddb-461a-bbee-02f9e1bf7b46"
        // MS Graph CommandLineTool = "14d82eec-204b-4c2f-b7e8-296a70dab67e"
        // PowerAutomate = "386ce8c0-7421-48c9-a1df-2a532400339f"
        // SPO Mgmt Shell = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        // VSCode = "aebc6443-996d-45c2-90f0-388ff96faa56";

        private static string AADPwsh = "1b730954-1685-4b74-9bfd-dac224a7b894";

        private static string PowerAutomateID = "386ce8c0-7421-48c9-a1df-2a532400339f";
        private static string SPOMgmtShell = "9bc3ab49-b65d-410a-85ad-de819febfddc";
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static async Task Main(string[] args)
        {
            PrintBanner();
            var parser = new CommandLine.Parser(settings =>
            {
                settings.HelpWriter = null;
                settings.CaseSensitive = false;
                settings.CaseInsensitiveEnumValues = true;
            });
            var parserResult = parser.ParseArguments<CommandlineOptions>(args);
            await parserResult.MapResult(
                    (CommandlineOptions opts) => RunOptions(opts),
                    errs => DisplayHelp(parserResult));
        }

        private static Task DisplayHelp(ParserResult<CommandlineOptions> parserResult)
        {
            var helpText = HelpText.AutoBuild(parserResult, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "AzRanger 0.1.2"; //change header
                h.Copyright = "Made with love by @HackmichNet";
                return HelpText.DefaultParsingErrorsHandler(parserResult, h);
            }, e => e);
            Console.WriteLine(helpText);
            return Task.FromResult(1);
        }


        private static void PrintBanner()
        {
            String banner = @"    _       ___
   /_\   __| _ \__ _ _ _  __ _ ___ _ _
  / _ \ |_ /   / _` | ' \/ _` / -_) '_|
 /_/ \_\/__|_|_\__,_|_||_\__, \___|_|
                         |___/         ";
            Console.WriteLine();
            Console.WriteLine(banner);
            Console.WriteLine();
        }

        private static void ConfigureLogging(CommandlineOptions opts)
        {
            var config = new LoggingConfiguration();
            var consoleTarget = new ConsoleTarget
            {
                Name = "console",
                Layout = "${level:uppercase=true}: ${message}",
            };
            if (opts.Debug)
            {
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget, "*");
                if (opts.Logfile != null)
                {
                    var fileTarget = new FileTarget
                    {
                        Name = "file",
                        Layout = "${level:uppercase=true}: ${message}",
                        FileName = opts.Logfile,
                    };
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget, "*");
                }
            }
            else
            {
                config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget, "*");
            }
            LogManager.Configuration = config;
        }

        private static string ResolveOutputPath(CommandlineOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.OutPath))
            {
                return opts.OutPath;
            }
            string datePrefix = DateTime.Now.ToString("ddMMyyyy") + "_AZRangerReport";
            if (opts.Mode == AzRangerModes.DumpAll || opts.Mode == AzRangerModes.DumpSettings)
            {
                return datePrefix + ".json";
            }
            return datePrefix;
        }

        private static List<ScopeEnum> ResolveScopes(IEnumerable<ScopeEnum> requestedScopes)
        {
            List<ScopeEnum> scopes = requestedScopes != null ? new List<ScopeEnum>(requestedScopes) : new List<ScopeEnum>();
            if (scopes.Count == 0)
            {
                return new List<ScopeEnum>() {
                    ScopeEnum.Azure, ScopeEnum.SPO, ScopeEnum.EXO, ScopeEnum.Teams, ScopeEnum.AAD
                };
            }
            if (scopes.Count == 1 && scopes[0].Equals(ScopeEnum.M365))
            {
                return new List<ScopeEnum>() {
                    ScopeEnum.SPO, ScopeEnum.EXO, ScopeEnum.Teams, ScopeEnum.AAD
                };
            }
            return scopes;
        }

        private static Microsoft.Identity.Client.IMsalHttpClientFactory CreateMsalHttpClientFactory(string proxy)
        {
            if (proxy == null) return null;

            var handler = new HttpClientHandler
            {
                Proxy = new WebProxy
                {
                    Address = new Uri($"http://{proxy}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = true,
                },
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            return new HttpFactoryWithProxy(new HttpClient(handler));
        }

        private static async Task<Tuple<MainCollector, string>> CreateScanner(CommandlineOptions opts)
        {
            Microsoft.Identity.Client.IMsalHttpClientFactory msalFactory = CreateMsalHttpClientFactory(opts.Proxy);

            if (opts.NoCache)
            {
                UserAuthenticator.ClearCache();
            }

            if (opts.Username != null && opts.Password != null)
            {
                String tenantId = opts.TenantId;
                if (tenantId == null)
                {
                    tenantId = await Helper.GetTenantIdToDomain(opts.Username.Split('@')[1], opts.Proxy);
                }
                if (tenantId == null)
                {
                    return Tuple.Create<MainCollector, string>(null, "[-] Could not find TenantId.... this should not happen, when providing the correct username.");
                }
                var aadAuth = UserAuthenticator.CreateWithPassword(opts.Username, opts.Password, tenantId, msalFactory, AADPwsh, disableCache: opts.NoCache);
                var paAuth = UserAuthenticator.CreateWithPassword(opts.Username, opts.Password, tenantId, msalFactory, PowerAutomateID, "ms-appx-web://microsoft.aad.brokerplugin/386ce8c0-7421-48c9-a1df-2a532400339f", disableCache: opts.NoCache);
                var spoAuth = UserAuthenticator.CreateWithPassword(opts.Username, opts.Password, tenantId, msalFactory, SPOMgmtShell, "https://oauth.spops.microsoft.com/", disableCache: opts.NoCache);
                return Tuple.Create(await MainCollector.CreateAsync(aadAuth, paAuth, spoAuth, opts.Proxy, tenantId), (string)null);
            }

            if (opts.ClientId != null && opts.ClientSecret != null)
            {
                if (opts.TenantId == null)
                {
                    return Tuple.Create<MainCollector, string>(null, "[-] You must provide the TenantId, when using application id and secret. Use the -t pararmeter.");
                }
                var authenticator = new AppAuthenticator(opts.ClientId, opts.ClientSecret, opts.TenantId, msalFactory);
                return Tuple.Create(await MainCollector.CreateAsync(authenticator, authenticator, authenticator, opts.Proxy, opts.TenantId), (string)null);
            }

            // Device code flow
            if (opts.DeviceCode)
            {
                if (opts.TenantId == null)
                {
                    return Tuple.Create<MainCollector, string>(null, "[-] You must provide the TenantId when using device code flow. Use the -t pararmeter.");
                }
                var aadAuthDevice = UserAuthenticator.CreateDeviceCode(opts.TenantId, msalFactory, AADPwsh, disableCache: opts.NoCache);
                var paAuthDevice = UserAuthenticator.CreateDeviceCode(opts.TenantId, msalFactory, PowerAutomateID, disableCache: opts.NoCache);
                var spoAuthDevice = UserAuthenticator.CreateDeviceCode(opts.TenantId, msalFactory, SPOMgmtShell, disableCache: opts.NoCache);
                return Tuple.Create(await MainCollector.CreateAsync(aadAuthDevice, paAuthDevice, spoAuthDevice, opts.Proxy, opts.TenantId), (string)null);
            }

            // Interactive login
            var aadAuthInteractive = UserAuthenticator.CreateInteractive(opts.TenantId, msalFactory, AADPwsh, disableCache: opts.NoCache);
            var paAuthInteractive = UserAuthenticator.CreateInteractive(opts.TenantId, msalFactory, PowerAutomateID, "ms-appx-web://microsoft.aad.brokerplugin/386ce8c0-7421-48c9-a1df-2a532400339f", disableCache: opts.NoCache);
            var spoAuthInteractive = UserAuthenticator.CreateInteractive(opts.TenantId, msalFactory, SPOMgmtShell, "https://oauth.spops.microsoft.com/", disableCache: opts.NoCache);
            return Tuple.Create(await MainCollector.CreateAsync(aadAuthInteractive, paAuthInteractive, spoAuthInteractive, opts.Proxy, opts.TenantId), (string)null);
        }

        private static async Task ExitEarly(string message, bool batch)
        {
            Console.WriteLine(message);
            if (!batch)
            {
                await Helper.PressKeyToContinue("[+] AzRanger finished... Press any key to exit!");
            }
        }

        private static async Task RunOptions(CommandlineOptions opts)
        {
            ConfigureLogging(opts);
            string outputPath = ResolveOutputPath(opts);
            List<ScopeEnum> scopes = ResolveScopes(opts.Scope);

            Console.WriteLine("[+] AzRanger started.");
            if (opts.Proxy != null)
            {
                Console.WriteLine("[!] WARNING: TLS certificate validation is disabled for proxy connections. Ensure the proxy is trusted.");
            }
            var scannerResult = await CreateScanner(opts);
            MainCollector scanner = scannerResult.Item1;
            string scannerError = scannerResult.Item2;
            if (scanner == null)
            {
                await ExitEarly(scannerError, opts.Batch);
                return;
            }

            // All modes need a tenant scan
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Tenant tenant = await scanner.ScanTenant(scopes);
            watch.Stop();
            Console.WriteLine($"[+] Scan Time: {watch.ElapsedMilliseconds} ms");

            if (tenant == null)
            {
                if (!opts.Debug)
                {
                    Console.WriteLine("[-] Something went wrong. Please run the tool with --debug and notify me.");
                }
                if (!opts.Batch)
                {
                    await Helper.PressKeyToContinue("[+] AzRanger finished... Press any key to exit!");
                }
                return;
            }

            if (opts.Mode == AzRangerModes.Audit)
            {
                Auditor auditor = new Auditor(tenant);
                auditor.Init(scopes);
                auditor.PerformAudit();

                switch (opts.Output)
                {
                    case AzRangerOutput.Console:
                        ConsoleOutput.Print(auditor, opts.WriteAllResults);
                        break;
                    case AzRangerOutput.HTML:
                        HTMLReportingOutput.Print(auditor, tenant, outputPath);
                        Console.WriteLine("[+] Report written to: " + outputPath);
                        break;
                    case AzRangerOutput.JSON:
                        JSONOutput.Print(auditor, outputPath);
                        Console.WriteLine("[+] Report written to: " + outputPath);
                        break;
                    case AzRangerOutput.CSV:
                        CISCSVOutput.Print(auditor, outputPath);
                        Console.WriteLine("[+] Report written to: " + outputPath);
                        break;
                }
            }
            else
            {
                // DumpAll and DumpSettings both just write JSON
                JSONDumper.WriteToFile(tenant, outputPath);
                Console.WriteLine("[+] Successfully written to " + outputPath);
            }

            if (!opts.Batch)
            {
                await Helper.PressKeyToContinue("[+] AzRanger finished... Press any key to exit!");
            }
        }
    }
}
