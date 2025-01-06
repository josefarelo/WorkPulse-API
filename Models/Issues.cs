using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class Issue
{
    [Key]
    public int Id { get; set; } // Clave primaria

    [JsonPropertyName("user")]
    public User? Author { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("merged_at")]
    public DateTime? MergedAt { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
}