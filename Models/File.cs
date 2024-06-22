using System.ComponentModel.DataAnnotations;

namespace closirissystem.Models;

public class File
{


    public int Id { get; set; }
    public string? FolderName { get; set; }
    public string? FileName { get; set; }
    public string? FileExtension { get; set; }
    public string? FilePath { get; set; }

    public IFormFile? FileSelected { get; set; }
    public string? FileBase64 { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? FileImage { get; set; }
    public string? Size { get; set; }

}