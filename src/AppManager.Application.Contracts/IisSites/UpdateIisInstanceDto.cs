namespace AppManager.IisSites;

public class UpdateIisInstanceDto
{
    [System.ComponentModel.DataAnnotations.StringLength(256)]
    public string? Name { get; set; }

    [System.ComponentModel.DataAnnotations.StringLength(1024)]
    public string? ConfigPath { get; set; }
}
