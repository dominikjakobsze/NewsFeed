using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NewsFeed.Data;
using NewsFeed.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace NewsFeed.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Dictionary<string, List<News>> listaNewsow = new Dictionary<string, List<News>>();
            var kategorie = await _context.Category.ToListAsync();
            foreach(var item in kategorie)
            {
                var newsy = await _context.News.Where(n => n.Category_Id == item.Id).OrderByDescending(n => n.Id).Take(4).ToListAsync();
                if(newsy.Count > 0)
                {
                    foreach (var news in newsy)
                    {
                        if (news.Article.Length <= 140)
                        {
                            continue;
                        }
                        else
                        {
                            news.Article = news.Article.Substring(0, 140);
                        }
                    }
                    foreach (var news in newsy)
                    {
                        if (news.Title.Length <= 24)
                        {
                            continue;
                        }
                        else
                        {
                            news.Title = news.Title.Substring(0, 24);
                        }
                    }
                    listaNewsow.Add(item.Name, newsy);
                }
            }
            ViewBag.ListaNewsow = listaNewsow;
            ViewBag.DisplayPaginacja = false;
            ViewBag.CurrentPage = 0;
            ViewBag.Pagination = 0;
            ViewBag.CurrentCategory = null;
            return View("Index");
        }

        // GET: R/News/{newsId}
        [Route("R/News/{newsId}")]
        [HttpGet]
        public async Task<IActionResult> ReadNews(int newsId)
        {
            var news = await _context.News.Where(n => n.Id == newsId).FirstOrDefaultAsync();
            if (news == null)
            {
                return Redirect("/");
            }
            ViewBag.ReadNews = news;
            var komentarze = await _context.Comment.OrderByDescending(c => c.Id).Where(c => c.News_Id == newsId).Take(5).ToListAsync();
            var users = await _context.Users.ToListAsync();
            if(komentarze.Count() > 0)
            {
                if(users.Count() > 0)
                {
                    foreach(var item in komentarze)
                    {
                        var found = users.Find(u => u.Id == item.User_Id);
                        if(found != null)
                        {
                            item.User_Id = found.UserName;
                        }
                    }
                }
                ViewBag.Comments = komentarze;
                ViewBag.CommentExist = true;
            }
            else
            {
                ViewBag.CommentExist = false;
            }
            return View("ReadNews");
        }
        // GET: R/Comments/{newsId}
        [Route("R/Comments/{newsId}")]
        [HttpGet]
        public async Task<IActionResult> ReadAllComments(int newsId)
        {
            var news = await _context.News.Where(n => n.Id == newsId).FirstOrDefaultAsync();
            if (news == null)
            {
                return Redirect("/");
            }
            var comments = await _context.Comment.OrderByDescending(c => c.Id).Where(c => c.News_Id == newsId).ToListAsync();
            if(comments == null || comments.Count <= 0)
            {
                return Content("Brak Komentarzy");
            }
            var users = await _context.Users.ToListAsync();
            foreach (var item in comments)
            {
                var found = users.Find(u => u.Id == item.User_Id);
                if (found != null)
                {
                    item.User_Id = found.UserName;
                }
            }
            ViewBag.Comments = comments;
            return View("ReadAllComments");
        }
        //POST: Comment/Add/{newsId}
        [Route("Comment/Add/{newsId}")]
        [HttpPost]
        public async Task<IActionResult> AddComment(string content, int newsId)
        {
            if(User.Identity.Name == null)
            {
                return Content("Musisz się zalogować");
            }
            var news = await _context.News.Where(n => n.Id == newsId).FirstOrDefaultAsync();
            if(news == null)
            {
                return Content("Nie istnieje taki News");
            }
            var user = await _context.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefaultAsync();
            var comment = new Comment();
            if(content != null)
            {
                comment.Content = content;
                comment.News_Id = newsId;
                comment.User_Id = user.Id;
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect("/R/News/"+newsId);
            }
            else
            {
                return Content("Komentarz musi mieć treść");
            }
        }

        // GET: News/C/{categoryName}/P/{pageNumber}
        [Route("News/C/{categoryName}/P/{pageNumber}")]
        [HttpGet]
        public async Task<IActionResult> CategoryIndex(string categoryName, int pageNumber)
        {
            var kategoria = await _context.Category.Where(c => c.Name == categoryName).FirstOrDefaultAsync();
            if(kategoria == null)
            {
                return Redirect("/");
            }
            int pagination = await _context.News.Where(n => n.Category_Id == kategoria.Id).CountAsync();
            if (pagination % 8 != 0)
            {
                pagination = (pagination / 8) + 1;
            }
            else
            {
                pagination = (pagination / 8);
            }

            if (pageNumber <= 0 || pageNumber > pagination)
            {
                return Redirect("/News/C/"+categoryName+"/P/1");
            }
            var newsy = await _context.News.Where(n => n.Category_Id == kategoria.Id).OrderByDescending(n => n.Id).Skip((pageNumber - 1) * 8).Take(8).ToListAsync();
            Dictionary<string, List<News>> listaNewsow = new Dictionary<string, List<News>>();
            foreach (var news in newsy)
            {
                if (news.Article.Length <= 140)
                {
                    continue;
                }
                else
                {
                    news.Article = news.Article.Substring(0, 140);
                }
            }
            foreach (var news in newsy)
            {
                if (news.Title.Length <= 24)
                {
                    continue;
                }
                else
                {
                    news.Title = news.Title.Substring(0, 24);
                }
            }
            listaNewsow.Add(kategoria.Name, newsy);
            ViewBag.ListaNewsow = listaNewsow;
            ViewBag.DisplayPaginacja = true;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.Pagination = pagination;
            ViewBag.CurrentCategory = kategoria.Name;
            return View("Index");
        }

        // GET: Autorzy
        [Route("Autorzy")]
        [HttpGet]
        public async Task<IActionResult> Autorzy()
        {
            return View("Autorzy");
        }

        // GET: Latest
        [Route("Latest")]
        [HttpGet]
        public async Task<IActionResult> Latest()
        {
            var news = await _context.News.OrderByDescending(n => n.Id).Take(1).FirstOrDefaultAsync();
            if (news == null)
            {
                return Redirect("/");
            }
            ViewBag.ReadNews = news;
            var komentarze = await _context.Comment.OrderByDescending(c => c.Id).Where(c => c.News_Id == news.Id).Take(5).ToListAsync();
            var users = await _context.Users.ToListAsync();
            if (komentarze.Count() > 0)
            {
                if (users.Count() > 0)
                {
                    foreach (var item in komentarze)
                    {
                        var found = users.Find(u => u.Id == item.User_Id);
                        if (found != null)
                        {
                            item.User_Id = found.UserName;
                        }
                    }
                }
                ViewBag.Comments = komentarze;
                ViewBag.CommentExist = true;
            }
            else
            {
                ViewBag.CommentExist = false;
            }
            return View("ReadNews");
        }

        // GET: Admin
        [Authorize]
        [Route("Admin")]
        [HttpGet]
        public async Task<IActionResult> Admin()
        {
            return Redirect("/Admin/News/P/1");
        }
        // GET: Profil
        [Authorize]
        [Route("Profil")]
        [HttpGet]
        public async Task<IActionResult> Profil()
        {
            var name = User.Identity.Name;
            var user = await _context.Users.Where(u => u.UserName == name).FirstOrDefaultAsync();
            var comments = await _context.Comment.OrderByDescending(c => c.Id).Where(c => c.User_Id == user.Id).Take(10).ToListAsync();
            var role = await _context.AdminRole.Where(a => a.User_Id == user.Id).FirstOrDefaultAsync();
            var nocomments = await _context.Comment.Where(c => c.User_Id == user.Id).CountAsync();
            var nonews = await _context.Comment.Where(c => c.User_Id == user.Id).Select(c => c.News_Id).Distinct().CountAsync();
            if(role == null)
            {
                ViewBag.Role = false;
            }
            else
            {
                if(role.IsAdmin == true)
                {
                    ViewBag.Role = true;
                }
                else
                {
                    ViewBag.Role = false;
                }
            }
            ViewBag.User = user;
            ViewBag.Comments = comments;
            ViewBag.Nonews = nonews;
            ViewBag.Nocomments = nocomments;
            return View("Profil");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}