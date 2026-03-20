# <img src="Logo.png" width="50" style="vertical-align:middle;margin-right:8px"/> AzRanger

AzRanger is a Windows command-line tool that audits the security configuration of a Microsoft 365 and Azure tenant. It collects data from ~13 Microsoft APIs — covering EntraIDy, Exchange Online, SharePoint Online, Microsoft Teams, and Azure subscriptions — then evaluates the collected settings against security recommendations and produces an HTML, JSON, or console report.

**Goal:** Give security practitioners and administrators a single, self-contained tool that finds misconfigurations and hardening gaps across a Microsoft cloud tenant, without requiring PowerShell modules or manual API queries.

Feedback on inaccurate results, missed checks, or new recommendations is very welcome — please open an issue.

## Acknowledgement

Most of the APIs are already integrated into other tools, so most of the credit goes to its creators:

* [Dr. Nestori Syynimaa](https://x.com/DrAzureAD)
* [Przemysław Kłys](https://x.com/PrzemyslawKlys)
* [Dirk-Jan](https://x.com/_dirkjan)

Thank you for your work!

## Prerequisites

- .NET Framework 4.8 (Windows only)
- Visual Studio 2019+ or MSBuild 16+
- The user should be assigned either the **Global Reader** or **Global Admin** role
- For auditing SharePoint, the **SharePoint Admin** role is additionally required (no reader-only role exists)

## Building

```bash
nuget restore AzRanger.sln
msbuild AzRanger.sln /p:Configuration=Release
```

The release binary is written to `AzRanger/bin/Release/AzRanger.exe`. All dependencies are embedded via Costura.Fody, so the single EXE is self-contained.

## Usage

There are three authentication methods:

### Interactive Login (recommended)

```
AzRanger.exe
```

You will be prompted to sign in interactively. Multiple authentication prompts may appear because AzRanger uses different client IDs to access various resources (AAD PowerShell, Power Automate, SPO Management Shell).

### Username and Password

```
AzRanger.exe -u user@contoso.com -p "MyPassword"
```

The tenant ID is resolved automatically from the domain. You can override it with `-t`.

### Device Code Flow (headless/SSH)

```
AzRanger.exe -d -t <tenant-id>
```

Use this on headless systems without a browser (SSH, Docker, Server Core). You will see a code and a URL — open the URL on any device, enter the code, and authenticate. The `-t` (tenant) parameter is **required**.

### Service Principal

```
AzRanger.exe -c <client-id> -s <client-secret> -t <tenant-id>
```

When using a service principal, the `-t` (tenant) parameter is **required**.

### Options

```
  -u, --username         Specify the username.
  -p, --password         Specify the password.
  -c, --clientid         Specify the client id.
  -s, --secret           Specify the client secret.
  -t, --tenant           Specify a tenant id.
  -d, --devicecode       (Default: false) Use device code flow for authentication (headless environments).
  --nocache              Disable persistent token cache and delete existing cache file.
  --proxy                Specify a proxy (e.g. http://127.0.0.1:8080).
  --debug                Enable verbose logging.
  --logfile              Set the logfile path.
  --outpath              Path/File to write results.
  --writeallresults      Write all results to console. Can result in a very large output.
  --output               (Default: HTML) Output format for audit mode: console, html or json.
  --scope                Scopes to audit (comma-separated). See below.
  --batch                (Default: false) Batch mode. Suppresses "press any key" prompts.
  --mode                 (Default: Audit) Operation mode: audit, dumpsettings or dumpall.
  --help                 Display this help screen.
  --version              Display version information.
```

### Scopes

| Scope   | Description                                                  |
|---------|--------------------------------------------------------------|
| `AAD`   | Azure Active Directory (users, groups, roles, policies, ...) |
| `Teams` | Microsoft Teams settings                                     |
| `SPO`   | SharePoint Online                                            |
| `EXO`   | Exchange Online                                              |
| `Azure` | Azure subscriptions, resources and security settings         |
| `M365`  | Shorthand for AAD + Teams + SPO + EXO                        |

If `--scope` is not set, all scopes are audited (AAD + Teams + SPO + EXO + Azure).

Examples:

```bash
# Audit only Azure AD and Exchange Online
AzRanger.exe -u user@contoso.com -p "pw" --scope AAD,EXO

# Audit everything except Azure resources
AzRanger.exe -u user@contoso.com -p "pw" --scope M365

# Dump all collected data as JSON
AzRanger.exe -u user@contoso.com -p "pw" --mode dumpall --outpath tenant_dump.json
```

### Output

- **Audit mode** (default): Runs all checks and produces a report.
  - `--output html` (default): Creates an HTML report in `./<date>_AZRangerReport/`
  - `--output json`: Writes a JSON report to `./<date>_AZRangerReport/`
  - `--output console`: Prints results to stdout
- **DumpAll mode**: Scans the tenant and writes all collected data as JSON.
- **DumpSettings mode**: Same as DumpAll but scans only settings (no resource enumeration).
