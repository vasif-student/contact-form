using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.Areas.Admin.Models.ViewModel;
using RentCar.DAL;
using RentCar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(AppDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _dbContext.Users.ToListAsync();
            var userList = new List<UserViewModel>();

            foreach (var user in users)
            {
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = (await _userManager.GetRolesAsync(user))[0]
                });
            }
            return View(userList);
        }

        //***** Change Role *****//
        public async Task<IActionResult> ChangeRole(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            return View(new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user))[0]
            });
        }

        //[HttpPost]
        //[ActionName("Block")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> 

        //***** Block User *****//
        public async Task<IActionResult> Block(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);

            return View(new BlockUserViewModel {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                isBlock = user.LockoutEnabled,
                Role = (await _userManager.GetRolesAsync(user))[0]
            });
        }

        [HttpPost]
        [ActionName("Block")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            user.LockoutEnabled = false;

            return RedirectToAction(nameof(Index), "User");
        }
    }
}
