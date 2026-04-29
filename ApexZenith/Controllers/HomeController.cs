using ApexZenith.Data;
using ApexZenith.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApexZenith.Controllers
{
    public class HomeController : Controller

    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
    
        public IActionResult Index()
        {
            var model = new HomePageViewModel
            {
                About = _context.About.FirstOrDefault(),
                Services = _context.Services.OrderBy(s => s.Id).Take(6).ToList(),
                Clients = _context.Client.OrderBy(c => c.Id).ToList(),
                News = _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Take(6).ToList()
            };
            return View(model);
        }

        public IActionResult Contact()


        {

            var address = _context.Client.OrderBy(c => c.Id).ToList();

            ContactPageView model = new ContactPageView();

           
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactPageView model)
        {
            if (model == null)
            {
                return BadRequest("Invalid form submission");
            }

            var contact = new Contact
            {
                FullName = model.Contact.FullName,
                Email = model.Contact.Email,
                Subject = model.Contact.Subject,
                Message = model.Contact.Message
            };

            _context.Add(contact);
            var Id= await _context.SaveChangesAsync();

            return RedirectToAction("Success", contact);
        }
        public IActionResult Success(Contact model)
            
        {
            var conctact = new Contact
            {
                FullName = model.FullName,
                Email = model.Email,
                Subject = model.Subject,
                Message = model.Message

            };

            return View(model);
        }

        public IActionResult About()
        {
            var model = new AboutPageViewModel
            {
                About = _context.About.FirstOrDefault(),
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
        public IActionResult News()
        {
            return View(new NewsPageViewModel
            {
                AllNews = _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Select(n => new News
                {
                    Id = n.Id,
                    Headline = n.Headline,
                    Content = n.Content,
                    PhotoUrl = n.PhotoUrl,
                    Author = n.Author,
                    PostedBy = n.PostedBy,
                    Date = n.Date,
                    Category = new List<NewsCategory> { new NewsCategory { Name = n.PostedBy } }
                }).ToList()
            });
        }
        public IActionResult NewsDetail(int id)
        {
            var news = _context.News.FirstOrDefault(n => n.Id == id && !n.IsDeleted);
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
                AllNews = _context.News.Where(n => !n.IsDeleted).OrderByDescending(n => n.Date).Select(n => new News
                {
                    Id = n.Id,
                    Headline = n.Headline,
                    Content = n.Content,
                    PhotoUrl = n.PhotoUrl,
                    Author = n.Author,
                    PostedBy = n.PostedBy,
                    PostedDate = n.Date,
                    Category = new List<NewsCategory> { new NewsCategory { Name = n.PostedBy } }
                }).ToList()
            };
            return View(model);
        }


        public IActionResult Pricing()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }
        public IActionResult PortfolioDetails()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Receive the data from the form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News model)
        {
            if (ModelState.IsValid)
            {
                // Add the blog to the context
                _context.Add(model);
                // Save to PostgreSQL
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
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
