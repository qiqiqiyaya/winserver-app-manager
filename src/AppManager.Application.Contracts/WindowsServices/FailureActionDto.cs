namespace AppManager.WindowsServices;

public class FailureActionDto
{
    public string FailureType { get; set; } = "First";
    public string Action { get; set; } = "Restart";
    public int DelayMinutes { get; set; } = 1;
    public int ResetPeriodDays { get; set; } = 1;
}
