using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Rozdzial_2C.Controllers
{
    [Authorize(Roles = "Admin")]
    //[Authorize]
    public class RoleController : Controller
    {
        protected UserManager<IdentityUser> UserManager { get; }
        protected RoleManager<IdentityRole> RoleManager { get; }

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            RoleManager = roleManager;
            UserManager = userManager;
        }

        public IActionResult Index()
        {
            var roles = RoleManager.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var role = new IdentityRole(roleName);
                await RoleManager.CreateAsync(role);
                return RedirectToAction("Index");
            }
            return View(roleName);
        }

        [HttpGet]
        public IActionResult Remove()
        {
            var roles = RoleManager.Roles.ToList();

            //var selectList = new List<SelectListItem>();
            //foreach (var role in roles)
            //{
            //    selectList.Add(new SelectListItem(role.Name, role.Id));
            //}

            var selectList = roles.Select(role => new SelectListItem(role.Name, role.Id));
            ViewData["Roles"] = selectList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(string roleId)
        {
            if (!string.IsNullOrWhiteSpace(roleId))
            {
                var role = await RoleManager.FindByIdAsync(roleId);
                await RoleManager.DeleteAsync(role);
                return RedirectToAction("Index");
            }

            return View(roleId);
        }

        [HttpGet]
        public async Task<IActionResult> AddToRole()
        {
            var roles = await RoleManager.Roles.ToListAsync();
            var users = await UserManager.Users.ToListAsync();

            ViewData["Roles"] = roles.Select(role => new SelectListItem(role.Name, role.Id)).ToList();
            ViewData["Users"] = users.Select(user => new SelectListItem(user.UserName, user.Id)).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddToRole(string roleId, string userId)
        {
            if (!string.IsNullOrWhiteSpace(roleId) && !string.IsNullOrWhiteSpace(userId))
            {
                var user = await UserManager.FindByIdAsync(userId);
                var role = await RoleManager.FindByIdAsync(roleId);
                await UserManager.AddToRoleAsync(user, role.Name);
                return RedirectToAction("Index");
            }

            return BadRequest(); // :)
        }
    }
}