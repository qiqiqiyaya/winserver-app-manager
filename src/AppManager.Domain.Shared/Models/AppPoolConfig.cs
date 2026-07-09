namespace AppManager.Models;

public class AppPoolConfig
{
    public string Name { get; set; } = string.Empty;
    public string ClrVersion { get; set; } = "v4.0";
    public string ManagedPipelineMode { get; set; } = "Integrated";
    public string StartMode { get; set; } = "OnDemand";
    public int IdleTimeoutMinutes { get; set; } = 20;
    public int MaxWorkerProcesses { get; set; } = 1;
    public int RecyclingPeriodicMinutes { get; set; } = 1740;
    public string ProcessModelIdentityType { get; set; } = "ApplicationPoolIdentity";
    public bool RapidFailProtectionEnabled { get; set; } = true;
    public int RapidFailProtectionMaxCrashes { get; set; } = 5;
    public int RapidFailProtectionIntervalMinutes { get; set; } = 5;
}
