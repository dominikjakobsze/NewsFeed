using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsFeed.Data;
using NewsFeed.Models;

namespace NewsFeed.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Admin/Comments/{page}
        [Route("Admin/Comments/{page}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int page)
        {
            if(IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            int pagination = await _context.Comment.CountAsync();
            if (pagination % 20 != 0)
            {
                pagination = (pagination / 20) + 1;
            }
            else
            {
                pagination = (pagination / 20);
            }

            if (page <= 0 || page > pagination)
            {
                return Content("Taka strona nie istnieje!");
            }
            var allComments = await _context.Comment.OrderByDescending(n => n.Id).Skip((page - 1) * 20).Take(20).ToListAsync();
            var allUsers = await _context.Users.ToListAsync();
            foreach (var comment in allComments)
            {
                var found = allUsers.Find(u => u.Id == comment.User_Id);
                if(found != null)
                {
                    comment.User_Id = found.Email;
                }
            }
            ViewBag.Comments = allComments;
            ViewBag.Pagination = pagination;
            ViewBag.CurrentPage = page;
            return View("Index");
        }
        // GET: /Admin/Comments/DeleteForm/{comment}
        [Route("Admin/Comments/DeleteForm/{comment}")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteForm(int comment)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var komentarz = await _context.Comment.Where(c => c.Id == comment).FirstOrDefaultAsync();
            if (komentarz != null)
            {
                ViewBag.Komentarz = komentarz;
                return View("DeleteForm");
            }
            else
            {
                return Content("Nie istnieje Komentarz o takim Id");
            }
        }
        // GET: /Admin/Comments/DeleteForm/{comment}
        [Route("Admin/Comments/Delete/{comment}")]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int comment)
        {
            if (IsAdmin() == false)
            {
                return Content("Brak dostępu");
            }
            var komentarz = await _context.Comment.Where(c => c.Id == comment).FirstOrDefaultAsync();
            if (komentarz != null)
            {
                _context.Comment.Remove(komentarz);
                await _context.SaveChangesAsync();
                return Redirect("/Admin/Comments/1");
            }
            else
            {
                return Content("Nie istnieje komentarz o takim Id");
            }
        }
        private bool IsAdmin()
        {
            var name = User.Identity.Name;
            var id =  _context.Users.Where(u => u.UserName == name).FirstOrDefault();
            if(id == null)
            {
                return false;
            }
            else
            {
                var exist =  _context.AdminRole.Where(ar => ar.User_Id == id.Id).FirstOrDefault();
                if(exist != null)
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
