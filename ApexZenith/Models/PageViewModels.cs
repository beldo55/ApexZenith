using System.Reflection.Metadata;

namespace ApexZenith.Models;

public class HomePageViewModel
{
    public About? About { get; set; }
    public List<Services> Services { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<News> News { get; set; } = new();
}

public class AboutPageViewModel
{
    public About About { get; set; } = new About();
    public List<Team> Team { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
}

public class TeamPageViewModel
{
    public List<Team> Team { get; set; } = new();
}

public class ServicesPageViewModel
{
    public List<Services> Services { get; set; } = new();
}

public class TestimonialsPageViewModel
{
    public List<Testimonial> Testimonials { get; set; } = new();
}

public class NewsPageViewModel
{
    public News CurrentNews { get; set; } = new News();
    public List<News> AllNews { get; set; } = new();
}


public class ContactPageView
{
    public Contact Contact { get; set; } = new Contact();
    public Address Address { get; set; } = new Address();
}

public class RegisterVM
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginVM
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
