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
    public class AdminRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminRolesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Users/P/{page}
        [Route("Admin/Users/P/{page}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            int pagination = await _context.Users.CountAsync();
            if (pagination % 7 != 0)
            {
                pagination = (pagination / 7) + 1;
            }
            else
            {
                pagination = (pagination / 7);
            }

            if (page <= 0 || page > pagination)
            {
                return Content("Taka strona nie istnieje!");
            }
            var allUsers = await _context.Users.OrderByDescending(n => n.Id).Skip((page - 1) * 7).Take(7).ToListAsync();
            var allRoles = await _context.AdminRole.OrderByDescending(a => a.Id).ToListAsync();
            foreach(var item in allUsers)
            {
                var isAdmin = allRoles.Find(a => a.User_Id == item.Id);
                if(isAdmin == null || isAdmin.IsAdmin == false || isAdmin.IsAdmin == null)
                {
                    item.EmailConfirmed = false;
                }
                else
                {
                    item.EmailConfirmed = isAdmin.IsAdmin;
                }
            }
            ViewBag.AllUsers = allUsers;
            ViewBag.Pagination = pagination;
            ViewBag.CurrentPage = page;
            return View("Index");

        }
        [Route("Admin/Users/RoleForm/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> RoleForm(string id)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var user = await _context.Users.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                ViewBag.User = user;
                return View("RoleForm");
            }
            else
            {
                return Content("Nie istnieje taki użytkownik");
            }
        }
        [Route("Admin/Users/RoleEdit/{id}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RoleEdit(string id, string uprawnienia)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var user = await _context.Users.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                var isAdmin = await _context.AdminRole.Where(ar => ar.User_Id == id).FirstOrDefaultAsync();
                if(isAdmin != null)
                {
                    if(uprawnienia == "admin")
                    {
                        isAdmin.IsAdmin = true;
                        _context.AdminRole.Update(isAdmin);
                        await _context.SaveChangesAsync();
                        return Redirect("/Admin/Users/P/1");
                    }
                    else
                    {
                        isAdmin.IsAdmin = false;
                        _context.AdminRole.Update(isAdmin);
                        await _context.SaveChangesAsync();
                        return Redirect("/Admin/Users/P/1");
                    }
                }
                else
                {
                    if (uprawnienia == "admin")
                    {
                        var newRole = new AdminRole();
                        newRole.User_Id = id;
                        newRole.IsAdmin = true;
                        _context.AdminRole.Add(newRole);
                        await _context.SaveChangesAsync();
                        return Redirect("/Admin/Users/P/1");
                    }
                    else
                    {
                        var newRole = new AdminRole();
                        newRole.User_Id = id;
                        newRole.IsAdmin = false;
                        _context.AdminRole.Add(newRole);
                        await _context.SaveChangesAsync();
                        return Redirect("/Admin/Users/P/1");
                    }
                }
            }
            else
            {
                return Content("Nie istnieje taki użytkownik");
            }
        }
        [Route("Admin/Users/DeleteForm/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteForm(string id)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var user = await _context.Users.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                ViewBag.User = user;
                return View("DeleteForm");
            }
            else
            {
                return Content("Nie istnieje taki użytkownik");
            }
        }
        [Route("Admin/Users/Delete/{id}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var user = await _context.Users.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                var komentarze = await _context.Comment.Where(c => c.User_Id == id).ToListAsync();
                if(komentarze != null && komentarze.Count() > 0)
                {
                    foreach(var item in komentarze)
                    {
                        _context.Comment.Remove(item);
                    }
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Redirect("/Admin/Users/P/1");
            }
            else
            {
                return Content("Nie istnieje taki użytkownik");
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
