using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.DB;
using OnlineShop.DB.Models;
using WomanShop.Areas.Admin.Models;
using WomanShop.Helpers;

using WomanShop.Models;

namespace WomanShop.Areas.Admin.Controllers
{
    [Area(OnlineShop.DB.Constants.AdminRoleName)]
    [Authorize(Roles = OnlineShop.DB.Constants.AdminRoleName)]
    public class UserController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
    
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            return View(Mapping.ToUsersViewModel(users));
        }

        public async Task<IActionResult> Details(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = new List<string>(await userManager.GetRolesAsync(user));
                return View(Mapping.ToUserViewModel(user, userRoles));
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return View("Details", userId);
                }
            }
            return RedirectToAction("Index");

        }

        public IActionResult Add()
        {
            ViewBag.Roles = new SelectList(roleManager.Roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
            return View();
        }
        [HttpPost]
        public IActionResult Add(AddUserViewModel user)
         {
            ViewBag.Roles = new SelectList(roleManager.Roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
            if (ModelState.IsValid)
            {
                var newUser = new User { UserName = user.Name, PhoneNumber = user.Phone, Email = user.Email };
                var result= userManager.CreateAsync(newUser,user.Password).Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(newUser, user.RoleName).Wait();
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public async Task<IActionResult> Update(string userId)
        {
            
            ViewBag.Roles = new SelectList(roleManager.Roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
            var user =await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = new List<string>(await userManager.GetRolesAsync(user));
                return View(Mapping.ToEditUserByAdminViewModel(user, userRoles));
            }
            return View("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(EditUserByAdminViewModel user)
        {
            var userUpdate = await userManager.FindByIdAsync(user.Id);

            if (user == null)
                ModelState.AddModelError("", "Пользователь не наеден");
            if (ModelState.IsValid)
            {
                userUpdate.Email = user.Email;
                userUpdate.PhoneNumber = user.Phone;
                userUpdate.UserName = user.Name;

                var result =await userManager.UpdateAsync(userUpdate);
                if (result.Succeeded)
                {
                    var userRoles = new List<string>(await userManager.GetRolesAsync(userUpdate));
                    return View("Details", Mapping.ToUserViewModel(userUpdate, userRoles));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);
                }
            }
            return View(user);
        }
        public IActionResult ResetPassword(string userId)
        {
            return View(new ResetPasswordInfo { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordInfo resetPasswordInfo)
        {
            var user = await userManager.FindByIdAsync(resetPasswordInfo.UserId);
            if (user == null)
                ModelState.AddModelError("", "Пользователь не наеден");
           
            if (ModelState.IsValid)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, token, resetPasswordInfo.Password);

                if (result.Succeeded)
                {
                    var userRoles = new List<string>(await userManager.GetRolesAsync(user));
                    return View("Details", Mapping.ToUserViewModel(user,userRoles));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(resetPasswordInfo);
                }

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateRoleAsync(string userId)
        {
            ViewBag.Roles = new SelectList(roleManager.Roles, nameof(IdentityRole.Name), nameof(IdentityRole.Name));
            var user = userManager.FindByIdAsync(userId).Result;
            var userRoles = new List<string>(await userManager.GetRolesAsync(user));
            if (userRoles == null || userRoles.Count==0)
            {
                userRoles=new List<string>() {Constants.UserRoleName };
                userManager.AddToRoleAsync(user, Constants.UserRoleName).Wait();
            }
            return View(new UpdateUserRoleViewModel {RoleName= userRoles.First(), UserId=user.Id });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRoleAsync(UpdateUserRoleViewModel updateUserRole)
        {
            var user = userManager.FindByIdAsync(updateUserRole.UserId).Result;

            if (ModelState.IsValid)
            {
                var oldRole = new List<string>(await userManager.GetRolesAsync(user)).FirstOrDefault();
                if (oldRole != updateUserRole.RoleName)
                {
                    if (!oldRole.IsNullOrEmpty())
                    {
                        userManager.RemoveFromRoleAsync(user, oldRole).Wait();
                        userManager.AddToRoleAsync(user, updateUserRole.RoleName).Wait();
                    }
                    else
                    {
                        userManager.AddToRoleAsync(user, Constants.UserRoleName).Wait();
                    }
                }
                
                
                return RedirectToAction("Index");
            }

            //var user = usersStorage.TryGetUserById(userId);
            //var role = rolesStorage.TryGetByName(roleName);
            //if (user == null)
            //{
            //    ModelState.AddModelError("", "Пользователь не наеден");
            //}
            return View(user);
        }

    }
}
