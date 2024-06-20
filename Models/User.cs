using System.ComponentModel.DataAnnotations;

namespace closirissystem.Models;

public class User
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$", ErrorMessage = "El campo {0} no acepta numeros o caracteres especiales")]
    [Display(Name = "Nombre")]
    public required string Name { get; set; }
    public required string Plan { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string Password { get; set; }

    
    public  IFormFile? ImageProfileFormFile { get; set; }

    public string? ImageProfile { get; set; }
    public required decimal FreeStorage { get; set; }
}