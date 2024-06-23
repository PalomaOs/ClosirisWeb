using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace closirissystem.Models;

public class File
{


    public int Id { get; set; }
    public string? FolderName { get; set; }
    public string? FileName { get; set; }

    public string? Size { get; set; }

    public string? FileImage { get; set; }

    public string? FileExtension { get; set; }

    public IFormFile? FileSelected { get; set; }

    public string? PreviewFile {get; set;}
    public DateTime? CreationDate { get; set; }

    public List<string>? Folders { get; set; }

    public List<File>? Files { get; set; }


    public double? FileSize {get; set;}


}