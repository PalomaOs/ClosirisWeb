namespace closirissystem.Models;

public class AuthUser {
    public required string Email {get; set;}
    public required string Name {get; set;}
    public required string Role {get; set;}
    public required string Jwt {get; set;}
}