using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ApexZenith.Models
{
    public class NewsCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [ValidateNever]
        public ICollection<News> NewsList { get; set; } = [];
    }

    public class News
    {
        public int Id { get; set; }

        public string Headline { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }

        public string Author { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;

        public DateTime PostedDate { get; set; }

        public int NumberOfViews { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        // FK
        public int NewsCategoryId { get; set; }

        // NAVIGATION
        [ValidateNever]
        public ICollection<NewsCategory> Category { get; set; } = [];

        [ValidateNever]
        public ICollection<NewsComment> Comments { get; set; } = [];
    }

    public class NewsComment
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int NewsId { get; set; }

        [ValidateNever]
        public News News { get; set; } = null!;

        [ValidateNever]
        public ICollection<NewsCommentReply> Replies { get; set; } = [];
    }

    public class NewsCommentReply
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }

        public string? Content { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int NewsCommentId { get; set; }

        [ValidateNever]
        public NewsComment? Comment { get; set; }
    }
}
