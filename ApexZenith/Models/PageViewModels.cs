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
    public About About { get; set; }
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
    
        public News? CurrentNews { get; set; }
        public List<News>? AllNews { get; set; }
    
}


public class ContactPageView
{
    public Contact Contact { get; set; } 
    public Address Address { get; set; } = new Address();
}

public class RegisterVM
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class LoginVM
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}

