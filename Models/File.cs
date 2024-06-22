using System.ComponentModel.DataAnnotations;

namespace closirissystem.Models;

public class File
{


    public int Id { get; set; }
    public string? FolderName { get; set; }
    public string? FileName { get; set; }

    public IFormFile? FileSelected { get; set; }
    public DateTime? CreationDate { get; set; }

    public List<string>? Folders {get; set;}

}