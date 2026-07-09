namespace AppManager.Models;

public class FailureAction
{
    public string FailureType { get; set; } = "First";
    public string Action { get; set; } = "Restart";
    public int DelayMinutes { get; set; } = 1;
    public int ResetPeriodDays { get; set; } = 1;
}
