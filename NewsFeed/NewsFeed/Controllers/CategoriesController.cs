using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsFeed.Data;
using NewsFeed.Models;

namespace NewsFeed.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories/{categorySlug}
        // [Route("Categories/{categorySlug}")]
        // ViewBag.message = "hello"; //passing arguments to view

        // GET: Admin/Categories
        [Route("Admin/Categories")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            //var products = context.Products.Where(p => p.CategoryId == 1);
            var items = await _context.Category.ToListAsync();
            ViewBag.Categoryindexlist = items;
            return View("Index");
        }
        // GET: Admin/Categories/Create
        [Route("Admin/Categories/Create")]
        [HttpGet]
        public async Task<IActionResult> CreateForm()
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            return View("CreateForm");
        }
        // POST: Admin/Categories/Create
        [Route("Admin/Categories/Create")]
        [HttpPost]
        public async Task<IActionResult> Create(string nazwa)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            if (await _context.Category.Where(c => c.Name == nazwa).FirstOrDefaultAsync() != null || nazwa == null)
            {
                return Content("Taka kategoria już istnieje");
            }
            else
            {
                var kategoria = new Category
                {
                    Name = nazwa
                };
                _context.Category.Add(kategoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        // GET: Admin/Categories/EditForm/{kategoriaId}
        [Route("Admin/Categories/EditForm/{kategoriaId}")]
        [HttpGet]
        public async Task<IActionResult> EditForm(int kategoriaId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var kategoria = await _context.Category.Where(c => c.Id == kategoriaId).FirstOrDefaultAsync();
            if (kategoria != null)
            {
                ViewBag.CategoryEditFormItem = kategoria;
                return View("EditForm");
            }
            else
            {
                return Content("Nie istnieje taka kategoria");
            }
        }
        // POST: Admin/Categories/Edit/{kategoriaId}
        [Route("Admin/Categories/Edit/{kategoriaId}")]
        [HttpPost]
        public async Task<IActionResult> Edit(int kategoriaId, string nazwa)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            if (await _context.Category.Where(c => c.Name == nazwa).FirstOrDefaultAsync() != null || nazwa == null)
            {
                return Content("Taka kategoria już istnieje");
            }
            else
            {
                var kategoria = await _context.Category.Where(c => c.Id == kategoriaId).FirstOrDefaultAsync();
                if (kategoria != null)
                {
                    kategoria.Name = nazwa;
                    _context.Category.Update(kategoria);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return Content("Nie istnieje kategoria o takim Id");
                }
            }
        }
        // GET: Admin/Categories/DeleteForm/{kategoriaId}
        [Route("Admin/Categories/DeleteForm/{kategoriaId}")]
        [HttpGet]
        public async Task<IActionResult> DeleteForm(int kategoriaId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var kategoria = await _context.Category.Where(c => c.Id == kategoriaId).FirstOrDefaultAsync();
            if(kategoria != null)
            {
                ViewBag.CategoryDeleteFormItem = kategoria;
                return View("DeleteForm");
            }
            else
            {
                return Content("Nie istnieje kategoria o takim Id");
            }
        }
        // POST: Admin/Categories/Delete/{kategoriaId}
        [Route("Admin/Categories/Delete/{kategoriaId}")]
        [HttpPost]
        public async Task<IActionResult> Delete(int kategoriaId)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var kategoria = await _context.Category.Where(c => c.Id == kategoriaId).FirstOrDefaultAsync();
            if (kategoria != null)
            {
                var newsy = await _context.News.Where(c => c.Category_Id == kategoriaId).ToListAsync();
                if(newsy.LongCount() > 0)
                {
                    foreach (var news in newsy)
                    {
                        _context.News.Remove(news);
                    }
                }
                _context.Category.Remove(kategoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Content("Nie istnieje kategoria o takim Id");
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
