namespace AppManager.Models;

public class SiteBinding
{
    public string Protocol { get; set; } = "http";
    public string IpAddress { get; set; } = "*";
    public int Port { get; set; } = 80;
    public string? HostName { get; set; }
    public string? CertificateHash { get; set; }
    public string? CertificateStoreName { get; set; }
}
