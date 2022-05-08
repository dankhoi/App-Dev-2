using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App_Dev_2.Data;
using App_Dev_2.Utility;
using App_Dev_2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App_Dev_2.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    public class UsersManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UsersManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //dùng để tránh trường hợp xóa nhầm role của mình
            // lấy toàn bộ user trừ id của người đăng nhập
            var userList = _db.ApplicationUsers.Where(u => u.Id != claims.Value); 

            foreach (var user in userList)
            {
                //Lấy toàn bộ role của user trong userlist - 
                //user.Role = roleTemp.FirstOrDefault => để lấy role đầu tiên của user ( user.roleTemp trong trường hợp user có nhiều role )
                
                //var userTemp = await _userManager.FindByIdAsync(user.Id);
                var roleTemp = await _userManager.GetRolesAsync(user);
                user.Role = roleTemp.FirstOrDefault();
            }

            return View(userList.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> LockUnLock(string id)
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userNeedToLock = _db.ApplicationUsers.Where(u => u.Id == id).First();
            if (userNeedToLock.Id == claims.Value)
            {
                //hien ra loi ban dang khoa tai khoan cua chinh minh
            }

            if (userNeedToLock.LockoutEnd != null && userNeedToLock.LockoutEnd > DateTime.Now)
            {
                userNeedToLock.LockoutEnd = DateTime.Now;
            }
            else
            {
                userNeedToLock.LockoutEnd = DateTime.Now.AddYears(1);
            }

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

       

        // public async Task<IActionResult> Update(String id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     var userTemp = await _userManager.FindByIdAsync(id);
        //     var roleTemp = await _userManager.GetRolesAsync(userTemp);
        //     if (roleTemp.First() == "Admin" || roleTemp.First() == "Staff")
        //     {
        //         return RedirectToAction("EditAdmin", "Users", new {id = id});
        //     }
        //     if (roleTemp.First() =="Trainer")
        //     {
        //         return RedirectToAction("EditTrainer", "Users", new {id = id});
        //     }
        //     if (roleTemp.First() =="Trainee")
        //     {
        //         return RedirectToAction("EditTrainee", "Users", new {id = id});
        //     }
        //
        //     return NoContent();
        // }

        // [HttpGet]
        // public async Task<IActionResult> EditAdmin(String id)
        // {
        //     if (id != null)
        //     {
        //         UserVM userVm = new UserVM();
        //         var user = _db.ApplicationUsers.Find(id);
        //         userVm.ApplicationUser = user;
        //
        //         userVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //         {
        //             Text = i,
        //             Value = i
        //         });
        //         return View(userVm);
        //     }
        //     return NotFound();
        // }
        
        // [HttpPost]
        // public async Task<IActionResult> EditAdmin(UserVM userVm)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var user = _db.ApplicationUsers.Find(userVm.ApplicationUser.Id);
        //         user.FullName = userVm.ApplicationUser.FullName;
        //         
        //         var roleOld = await _userManager.GetRolesAsync(user);
        //         await _userManager.RemoveFromRoleAsync(user, roleOld.First());
        //         await _userManager.AddToRoleAsync(user, userVm.Role);
        //
        //         _db.ApplicationUsers.Update(user);
        //         _db.SaveChanges();
        //         return RedirectToAction(nameof(Index));
        //     }
        //
        //     userVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //     {
        //         Text = i,
        //         Value = i
        //     });
        //     
        //     return View(userVm);
        // }

        // [HttpGet]
        // public async Task<IActionResult> EditTrainer(String id)
        // {
        //     if (id != null)
        //     {
        //         if (id != null)
        //         {
        //             TrainerUserVM trainerUserVm = new TrainerUserVM();
        //             var trainerUser = _db.TrainerUsers.Find(id);
        //             trainerUserVm.TrainerUser = trainerUser;
        //
        //             trainerUserVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //             {
        //                 Text = i,
        //                 Value = i
        //             });
        //             return View(trainerUserVm);
        //         }
        //         return NotFound();
        //     }
        //     return NotFound();
        // }
        // [HttpPost]
        // public async Task<IActionResult> EditTrainer(TrainerUserVM trainerUserVm)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var trainerUser = _db.TrainerUsers.Find(trainerUserVm.TrainerUser.Id);
        //         trainerUser.FullName = trainerUserVm.TrainerUser.FullName;
        //         trainerUser.Accounts = trainerUserVm.TrainerUser.Accounts;
        //         trainerUser.Age = trainerUserVm.TrainerUser.Age;
        //         trainerUser.DateOfBirth = trainerUserVm.TrainerUser.DateOfBirth;
        //         trainerUser.Education = trainerUserVm.TrainerUser.Education;
        //         trainerUser.MainProgrammingLanguage = trainerUserVm.TrainerUser.MainProgrammingLanguage;
        //         trainerUser.ToeicScore = trainerUserVm.TrainerUser.ToeicScore;
        //         trainerUser.ExperienceDetails = trainerUserVm.TrainerUser.ExperienceDetails;
        //         trainerUser.Department = trainerUserVm.TrainerUser.Department;
        //         trainerUser.Location = trainerUserVm.TrainerUser.Location;
        //
        //         var roleOld = await _userManager.GetRolesAsync(trainerUser);
        //         await _userManager.RemoveFromRoleAsync(trainerUser, roleOld.First());
        //         await _userManager.AddToRoleAsync(trainerUser, trainerUserVm.Role);
        //
        //         _db.ApplicationUsers.Update(trainerUser);
        //         _db.SaveChanges();
        //         return RedirectToAction(nameof(Index));
        //     }
        //
        //     trainerUserVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //     {
        //         Text = i,
        //         Value = i
        //     });
        //     
        //     return View(trainerUserVm);
        // }

        // [HttpGet]
        // public async Task<IActionResult> EditTrainee(String id)
        // {
        //     TraineeUserVM traineeUserVm = new TraineeUserVM();
        //     var traineeUserUser = _db.TraineeUsers.Find(id);
        //     traineeUserVm.TraineeUser = traineeUserUser;
        //
        //     traineeUserVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //     {
        //         Text = i,
        //         Value = i
        //     });
        //
        //     return View(traineeUserVm);
        // }
        //
        // [HttpPost]
        // public async Task<IActionResult> EditTrainee(TraineeUserVM traineeUserVm)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var traineeUser = _db.TraineeUsers.Find(traineeUserVm.TraineeUser.Id);
        //         traineeUser.ExternalOrInternalType = traineeUserVm.TraineeUser.ExternalOrInternalType;
        //         traineeUser.Education = traineeUserVm.TraineeUser.Education;
        //         traineeUser.WorkingPlace = traineeUserVm.TraineeUser.WorkingPlace;
        //         traineeUser.Telephone = traineeUserVm.TraineeUser.Telephone;
        //
        //         var roleOld = await _userManager.GetRolesAsync(traineeUser);
        //         await _userManager.RemoveFromRoleAsync(traineeUser, roleOld.First());
        //         await _userManager.AddToRoleAsync(traineeUser, traineeUserVm.Role);
        //
        //         _db.TraineeUsers.Update(traineeUser);
        //         _db.SaveChanges();
        //         return RedirectToAction(nameof(Index));
        //     }
        //
        //     traineeUserVm.Rolelist = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem()
        //     {
        //         Text = i,
        //         Value = i
        //     });
        //
        //     return View(traineeUserVm);
        // }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmEmail(string id)
        {
            var user = _db.ApplicationUsers.Find(id);

            if (user == null)
            {
                return View();
            }

            ConfirmEmailVM confirmEmailVm = new ConfirmEmailVM()
            {
                Email = user.Email
            };

            return View(confirmEmailVm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM confirmEmailVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(confirmEmailVm.Email);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                return RedirectToAction("ResetPassword", "UsersManagement", new {token = token, email = user.Email});
            }

            return View(confirmEmailVm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("","Invalid password reset token");
            }

            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel()
            {
                Email = email,
                Token = token
            };
            return View(resetPasswordViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token,
                        resetPasswordViewModel.Password);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }

            return View(resetPasswordViewModel);
        }
    }
}