using System.ComponentModel.DataAnnotations;

namespace closirissystem.Models;

public class User
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public  string Email { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$", ErrorMessage = "El campo {0} no acepta numeros o caracteres especiales")]
    [Display(Name = "Nombre")]
    public  string Name { get; set; }
    public  string Plan { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public  string Password { get; set; }


    public IFormFile? ImageProfileFormFile { get; set; }

    public string? ImageProfile { get; set; }
    public  decimal FreeStorage { get; set; }

    public List<string>? Folders {get; set;}
}