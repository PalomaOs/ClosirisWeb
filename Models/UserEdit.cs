using System.ComponentModel.DataAnnotations;

namespace closirissystem.Models;

public class UserEdit
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public  string? Email { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$", ErrorMessage = "El campo {0} no acepta numeros o caracteres especiales")]
    [Display(Name = "Nombre")]
    public  string? Name { get; set; }
    
    public  IFormFile? ImageProfileFormFile { get; set; }
    
    [Display(Name = "Imagen de perfil")]
    public  string? ImageProfile { get; set; }
}