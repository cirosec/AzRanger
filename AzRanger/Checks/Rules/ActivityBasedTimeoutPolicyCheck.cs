using AzRanger.Models;
using System;
using System.Linq;

namespace AzRanger.Checks.Rules
{
    class ActivityBasedTimeoutPolicyCheck : BaseCheck
    {

        public override CheckResult Audit(Tenant tenant)
        {
            {
                var timeoutPolicies = tenant.TenantSettings?.ActivityBasedTimeoutPolicy;
                var benchmarkTimeSpan = TimeSpan.Parse("03:00:00"); // 3 hours

                if (timeoutPolicies != null && timeoutPolicies.Any())
                {
                    var timeoutPolicy = timeoutPolicies.FirstOrDefault();

                    if (timeoutPolicy?.definition != null && timeoutPolicy.definition.Length > 0)
                    {
                        var policyDefinition = timeoutPolicy.definition[0];
                        var applicationPolicies = policyDefinition.ActivityBasedTimeoutPolicy?.ApplicationPolicies;

                        if (applicationPolicies != null && applicationPolicies.Length > 0)
                        {
                            var timeout = applicationPolicies[0].WebSessionIdleTimeout;

                            if (!string.IsNullOrEmpty(timeout))
                            {
                                var timeSpan = TimeSpan.Parse(timeout);
                                var timeoutReadable = $"{timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes";

                                if (timeSpan <= benchmarkTimeSpan)
                                {
                                    this.SetReason($"Timeout is set to {timeoutReadable}.");
                                    return CheckResult.NoFinding;
                                }
                                else
                                {
                                    this.SetReason($"Timeout is too long. It is set to {timeoutReadable}.");
                                    return CheckResult.Finding;
                                }
                            }
                        }
                    }
                }

                this.SetReason("Idle session timeout is not configured.");
                return CheckResult.Finding;
            }
        }
    }
}
