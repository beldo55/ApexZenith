using System.ComponentModel.DataAnnotations;

namespace ApexZenith.Areas.Admin.Models;

using NewsEntity = ApexZenith.Models.News;

public class NewsFormModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Headline { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PhotoUrl { get; set; }

    [Required]
    [MaxLength(120)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Category")]
    public int NewsCategoryId { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public static NewsFormModel FromEntity(NewsEntity n) => new()
    {
        Id = n.Id,
        Headline = n.Headline,
        Content = n.Content,
        PhotoUrl = n.PhotoUrl,
        Author = n.Author,
        NewsCategoryId = n.NewsCategoryId,
        Date = n.Date
    };
}
