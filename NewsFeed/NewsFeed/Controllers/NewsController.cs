using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using NewsFeed.Data;
using NewsFeed.Models;
using static System.Net.WebRequestMethods;

namespace NewsFeed.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> extensions = new List<string> { "image/jpeg", "image/jpg", "image/png" };
        private readonly Random _random = new Random();

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/News/P/{page}
        [Route("Admin/News/P/{page}")]
        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            int pagination = await _context.News.CountAsync();
            if (pagination % 10 != 0)
            {
                pagination = (pagination / 10) + 1;
            }
            else
            {
                pagination = (pagination / 10);
            }

            if (page <= 0 || page > pagination)
            {
                return Content("Taka strona nie istnieje!");
            }
            var allNews = await _context.News.OrderByDescending(n => n.Id).Skip((page-1) * 10).Take(10).ToListAsync();
            var allCategories = await _context.Category.ToListAsync();
            foreach (var news in allNews)
            {
                if(news.Article.Length <= 140)
                {
                    continue;
                }
                else
                {
                    news.Article = news.Article.Substring(0, 140);
                }
            }
            foreach (var news in allNews)
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
            ViewBag.AllNewsAdmin = allNews;
            ViewBag.AllCategories = allCategories;
            ViewBag.SpecifiedCategory = -1;
            ViewBag.Pagination = pagination;
            ViewBag.CurrentPage = page;
            return View("Index");
        }
        // GET: Admin/News/{categoryId}/P/{page}
        [Route("Admin/News/{categoryId}/P/{page}")]
        [HttpGet]
        public async Task<IActionResult> Index(int categoryId, int page)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            if (categoryId == -1)
            {
                return Redirect("/Admin/News/P/1");
            }
            var getCategory = await _context.Category.Where(c => c.Id == categoryId).FirstOrDefaultAsync();
            if(getCategory == null)
            {
                return Content("Taka kategoria nie istnieje!");
            }
            int pagination = await _context.News.Where(n => n.Category_Id == categoryId).CountAsync();
            if (pagination % 10 != 0)
            {
                pagination = (pagination / 10) + 1;
            }
            else
            {
                pagination = (pagination / 10);
            }
            if (page <= 0 || page > pagination)
            {
                return Content("Taka strona nie istnieje!");
            }
            var allNews = await _context.News.Where(n => n.Category_Id == categoryId).OrderByDescending(n => n.Id).Skip((page - 1) * 10).Take(10).ToListAsync();
            var allCategories = await _context.Category.ToListAsync();
            foreach (var news in allNews)
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
            foreach (var news in allNews)
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
            ViewBag.AllNewsAdmin = allNews;
            ViewBag.AllCategories = allCategories;
            ViewBag.SpecifiedCategory = categoryId;
            ViewBag.Pagination = pagination;
            ViewBag.CurrentPage = page;
            return View("Index");
        }
        // GET: Admin/News/CreateForm
        [Route("Admin/News/CreateForm")]
        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var kategorie = await _context.Category.ToListAsync();
            if (kategorie.LongCount() == 0)
            {
                return Content("<a href='/Admin/Categories/Create'>Brak Kategorii!</a>", "text/html");
            }
            else
            {
                ViewBag.KategorieCreateForm = kategorie;
                return View("CreateForm");
            }
        }
        // POST: Admin/News/Create
        [Route("Admin/News/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(string title, int category, string article, IFormFile baner)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            if (baner == null)
            {
                return Content("Zdjęcie jest obowiązkowe :)");
            }
            if (baner.Length > 926027 || baner.Length == 0)
            {
                return Content("Plik jest za duży!");
            }
            if (!extensions.Contains(baner.Headers.ContentType))
            {
                return Content("Plik musi być zdjęciem!");
            }
            if(await _context.Category.Where(c => c.Id == category).FirstOrDefaultAsync() == null)
            {
                return Content("Nie istnieje taka kategoria");
            }
            if(title == null || article == null)
            {
                return Content("Należy uzupełnić Tytuł i Treść");
            }
            var fileId = _random.Next();
            var newFileName = fileId + baner.FileName;
            if(await _context.News.Where(n => n.ImgPath == newFileName).FirstOrDefaultAsync() != null)
            {
                return Content("Coś poszło nie tak, spróbuj później...");
            }
            var filePath = Path.Combine(Directory.GetCurrentDirectory()+"/wwwroot/images/", newFileName);
            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await baner.CopyToAsync(stream);
            }
            var news = new News
            {
                Title = title,
                Article = article,
                Category_Id = category,
                ImgPath = newFileName
            };
            _context.News.Add(news);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/News/P/1");
        }
        // GET: Admin/News/EditForm/{newsId}
        [Route("Admin/News/EditForm/{newsId}")]
        [HttpGet]
        public async Task<IActionResult> EditForm(int newsId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var specifiedNews = await _context.News.Where(n => n.Id == newsId).FirstOrDefaultAsync();
            if (specifiedNews == null)
            {
                return Content("Nie istnieje News o takim Id!");
            }
            ViewBag.SpecifiedNews = specifiedNews;
            var kategorie = await _context.Category.ToListAsync();
            ViewBag.Kategorie = kategorie;
            return View("EditForm");
        }
        // POST: Admin/News/Edit/{newsId}
        [Route("Admin/News/Edit/{newsId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(int newsId, string title, int category, string article, IFormFile baner)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var specifiedNews = await _context.News.Where(n => n.Id == newsId).FirstOrDefaultAsync();

            if(specifiedNews == null)
            {
                return Content("Nie istnieje News o takim Id!");
            }
            if (await _context.Category.Where(c => c.Id == category).FirstOrDefaultAsync() == null)
            {
                return Content("Nie istnieje taka kategoria");
            }
            if (title == null || article == null)
            {
                return Content("Należy uzupełnić Tytuł i Treść");
            }
            if (baner != null)
            {
                if (baner.Length > 926027 || baner.Length == 0)
                {
                    return Content("Plik jest za duży!");
                }
                if (!extensions.Contains(baner.Headers.ContentType))
                {
                    return Content("Plik musi być zdjęciem!");
                }
                var filePathEdit = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images/", specifiedNews.ImgPath);
                FileInfo file = new FileInfo(filePathEdit);
                if (file.Exists)
                {
                    file.Delete();
                    var fileId = _random.Next();
                    var newFileName = fileId + baner.FileName;
                    if (await _context.News.Where(n => n.ImgPath == newFileName).FirstOrDefaultAsync() != null)
                    {
                        return Content("Coś poszło nie tak, spróbuj później...");
                    }
                    var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images/", newFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await baner.CopyToAsync(stream);
                    }
                    specifiedNews.ImgPath = newFileName;
                }
                else
                {
                    return Content("Nie można znaleźć pliku!");
                }
            }
            specifiedNews.Title = title;
            specifiedNews.Article = article;
            specifiedNews.Category_Id = category;
            _context.News.Update(specifiedNews);
            await _context.SaveChangesAsync();
            return Redirect("/Admin/News/P/1");
        }
        // GET: Admin/News/DeleteForm/{newsId}
        [Route("Admin/News/DeleteForm/{newsId}")]
        [HttpGet]
        public async Task<IActionResult> DeleteForm(int newsId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var news = await _context.News.Where(c => c.Id == newsId).FirstOrDefaultAsync();
            if (news != null)
            {
                ViewBag.NewsDeleteFormItem = news;
                return View("DeleteForm");
            }
            else
            {
                return Content("Nie istnieje News o takim Id");
            }
        }
        // POST: Admin/News/Delete/{newsId}
        [Route("Admin/News/Delete/{newsId}")]
        [HttpPost]
        public async Task<IActionResult> Delete(int newsId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var news = await _context.News.Where(c => c.Id == newsId).FirstOrDefaultAsync();
            if (news != null)
            {
                //var filePathEdit = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/images/", news.ImgPath);
                //FileInfo file = new FileInfo(filePathEdit);
                //if (file.Exists)
                //{
                //file.Delete();
                //}
                var komentarze = await _context.Comment.Where(c => c.News_Id == news.Id).ToListAsync();
                if(komentarze != null && komentarze.Count() > 0)
                {
                    foreach(var item in komentarze)
                    {
                        _context.Comment.Remove(item);
                    }
                }
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                return Redirect("/Admin/News/P/1");
            }
            else
            {
                return Content("Nie istnieje News o takim Id");
            }
        }
        private bool IsAdmin()
        {
            var name = User.Identity.Name;
            var id = _context.Users.Where(u => u.UserName == name).FirstOrDefault();
            if (id == null)
            {
                return false;
            }
            else
            {
                var exist = _context.AdminRole.Where(ar => ar.User_Id == id.Id).FirstOrDefault();
                if (exist != null)
                {
                    if (exist.IsAdmin == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
        }
    }
}
