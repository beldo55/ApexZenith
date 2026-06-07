using ApexZenith.Data;
using ApexZenith.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ApexZenith.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                About = await _context.About.FirstOrDefaultAsync(),
                Services = await _context.Services.OrderBy(s => s.Id).Take(6).ToListAsync(),
                Clients = await _context.Client.OrderBy(c => c.Id).ToListAsync(),
                News = await _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Take(6)
                    .Select(n => new News
                    {
                        Id = n.Id,
                        Headline = n.Headline,
                        Content = n.Content,
                        PhotoUrl = n.PhotoUrl,
                        Author = n.Author,
                        PostedBy = n.PostedBy,
                        Date = n.Date,
                        Category = new List<NewsCategory> { new NewsCategory { Name = n.PostedBy } }
                    }).ToListAsync()
            };
            return View(model);
        }

        public IActionResult Contact()
        {
            var model = new ContactPageView
            {
                Address = _context.Address.FirstOrDefault() ?? new Address()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactPageView model)
        {
            if (model?.Contact == null)
            {
                return BadRequest("Invalid form submission");
            }

            if (!ModelState.IsValid)
            {
                // Re-populate any view data required by the view (e.g. Address) before returning
                model.Address ??= _context.Address.FirstOrDefault() ?? new Address();
                return View(model);
            }

            var contact = new Contact
            {
                FullName = model.Contact.FullName,
                Email = model.Contact.Email,
                Subject = model.Contact.Subject,
                Message = model.Contact.Message
            };

            _context.Add(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success), new { id = contact.Id });
        }

        public async Task<IActionResult> Success(int id)
        {
            var contact = await _context.Set<Contact>().FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        public IActionResult About()
        {
            var model = new AboutPageViewModel
            {
                About = _context.About.FirstOrDefault() ?? new About(),
                Team = _context.Team.OrderBy(t => t.Id).ToList(),
                Clients = _context.Client.OrderBy(c => c.Id).ToList(),
            };
            return View(model);
        }

        public IActionResult Testimonials()
        {
            return View(new TestimonialsPageViewModel
            {
                Testimonials = _context.Testimonial.OrderBy(t => t.Id).ToList()
            });
        }

        public IActionResult Team()
        {
            return View(new TeamPageViewModel
            {
                Team = _context.Team.OrderBy(t => t.Id).ToList()
            });
        }

        public IActionResult Services()
        {
            return View(new ServicesPageViewModel
            {
                Services = _context.Services.OrderBy(s => s.Id).ToList()
            });
        }

        public async Task<IActionResult> News()
        {
            return View(new NewsPageViewModel
            {
                AllNews = await _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Select(n => new News
                {
                    Id = n.Id,
                    Headline = n.Headline,
                    Content = n.Content,
                    PhotoUrl = n.PhotoUrl,
                    Author = n.Author,
                    PostedBy = n.PostedBy,
                    Date = n.Date,
                    Category = new List<NewsCategory> { new NewsCategory { Name = n.PostedBy } }
                }).ToListAsync()
            });
        }

        public async Task<IActionResult> NewsDetail(int id)
        {
            var news = await _context.News.FirstOrDefaultAsync(n => n.Id == id && !n.IsDeleted);
            if (news == null)
            {
                return NotFound();
            }

            var model = new NewsPageViewModel
            {
                CurrentNews = new News
                {
                    Id = news.Id,
                    Headline = news.Headline,
                    Content = news.Content,
                    PhotoUrl = news.PhotoUrl,
                    Author = news.Author,
                    PostedBy = news.PostedBy,
                    Date = news.Date,
                    Category = new List<NewsCategory> { new NewsCategory { Name = news.PostedBy } }
                },
                AllNews = await _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Select(n => new News
                {
                    Id = n.Id,
                    Headline = n.Headline,
                    Content = n.Content,
                    PhotoUrl = n.PhotoUrl,
                    Author = n.Author,
                    PostedBy = n.PostedBy,
                    Date = n.Date,
                    Category = new List<NewsCategory> { new NewsCategory { Name = n.PostedBy } }
                }).ToListAsync()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
