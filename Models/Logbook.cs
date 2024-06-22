namespace closirissystem.Models;

public class Logbook
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public required string Action { get; set; }
    public required string User { get; set; }
    public required string Ip { get; set; }
    public DateTime InitDate { get; set; }
}