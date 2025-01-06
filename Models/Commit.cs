using System.ComponentModel.DataAnnotations;

public class Commit
{
    [Key]
    public int Id { get; set; } // Clave primaria
    public string Sha { get; set; } = string.Empty;
    public required string Author { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public DateTime Date { get; set; }
    public required string Repository { get; set; }
}