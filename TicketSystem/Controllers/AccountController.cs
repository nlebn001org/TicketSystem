using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketSystem.Web.Models;
using TicketSystem.Web.RepositoryServices;
using TicketSystem.Web.ViewModels;

namespace TicketSystem.Web.Controllers
{

    public class AccountController : Controller
    {
        readonly SystemDbContext _db;

        public AccountController(SystemDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginModel model)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        User user = await _db.Users.Include(u => u.Role)
        //            .FirstOrDefaultAsync(u => model.Username == u.Username && model.Password == u.Password);

        //        if (user != null)
        //        {
        //            await Authenticate(user); //authentication

        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError("", "Incorrect Username or Password");
        //    }
        //    return View(model);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
        //        if (user == null)
        //        {
        //            User newUser = new User { Username = model.Username, Password = model.Password }; //could be registered only customer
        //            Role userRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "customer");
        //            if (userRole != null) newUser.Role = userRole;
        //            await _db.Users.AddAsync(newUser);
        //            await _db.SaveChangesAsync();
        //            await Authenticate(newUser); //authentication
        //            return RedirectToAction("Index", "Home"); // home index
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", "Probably user with this username is already exists.");
        //        }
        //    }
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MyAccount(MyAccountModel accountModel)
        //{
        //    User user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == accountModel.Username);
        //    accountModel.Role = user.Role;
        //    if (ModelState.IsValid)
        //    {
        //        if (user != null)
        //        {
        //            if (!string.IsNullOrEmpty(accountModel.NewPassword) || !string.IsNullOrEmpty(accountModel.OldPassword) ||
        //                !string.IsNullOrEmpty(accountModel.ConfirmPassword))
        //            {
        //                if (user.Password != accountModel.OldPassword)
        //                    ModelState.AddModelError("", "You must enter your previous password for change");
        //                if (accountModel.NewPassword.Length < 3)
        //                    ModelState.AddModelError("", "Your password must not be empty or less than 3 symbols.");
        //                else
        //                    user.Password = accountModel.NewPassword;
        //            }

        //            user.Email = string.IsNullOrEmpty(accountModel.Email) ? user.Email : accountModel.Email;
        //            user.Name = string.IsNullOrEmpty(accountModel.Name) ? user.Name : accountModel.Name;
        //            user.Surname = string.IsNullOrEmpty(accountModel.Surname) ? user.Surname : accountModel.Surname;
        //            user.DateChanged = DateTime.Now;
        //            _db.Update(user);
        //            await _db.SaveChangesAsync();
        //        }
        //        else
        //            ModelState.AddModelError("", "User does not exist.");
        //    }
        //    return View(accountModel);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            string encryptedPassword = Encrypt(model.Password);

            if (ModelState.IsValid)
            {
                User user = await _db.Users.Include(u => u.Role)
                    .FirstOrDefaultAsync(u => model.Username == u.Username && model.Password == u.Password);

                if (user != null)
                {
                    await Authenticate(user); //authentication

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect Username or Password");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user == null)
                {
                    User newUser = new User { Username = model.Username, Password = model.Password }; //could be registered only customer
                    Role userRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == "customer");
                    if (userRole != null) newUser.Role = userRole;
                    await _db.Users.AddAsync(newUser);
                    await _db.SaveChangesAsync();
                    await Authenticate(newUser); //authentication
                    return RedirectToAction("Index", "Home"); // home index
                }
                else
                {
                    ModelState.AddModelError("", "Probably user with this username is already exists.");
                }
            }
            return View(model);
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> MyAccount()
        {
            User user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
                ModelState.AddModelError("", "Something went wrong.");

            MyAccountModel accountModel = new MyAccountModel
            {
                Username = user.Username,
                Role = user.Role,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname
            };
            return View(accountModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(MyAccountModel accountModel)
        {
            User user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == accountModel.Username);
            accountModel.Role = user.Role;
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(accountModel.NewPassword) || !string.IsNullOrEmpty(accountModel.OldPassword) ||
                        !string.IsNullOrEmpty(accountModel.ConfirmPassword))
                    {
                        if (user.Password != accountModel.OldPassword)
                            ModelState.AddModelError("", "You must enter your previous password for change");
                        if (accountModel.NewPassword.Length < 3)
                            ModelState.AddModelError("", "Your password must not be empty or less than 3 symbols.");
                        else
                            user.Password = accountModel.NewPassword;
                    }

                    user.Email = string.IsNullOrEmpty(accountModel.Email) ? user.Email : accountModel.Email;
                    user.Name = string.IsNullOrEmpty(accountModel.Name) ? user.Name : accountModel.Name;
                    user.Surname = string.IsNullOrEmpty(accountModel.Surname) ? user.Surname : accountModel.Surname;
                    user.DateChanged = DateTime.Now;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }
                else
                    ModelState.AddModelError("", "User does not exist.");
            }
            return View(accountModel);
        }

        public async Task<IActionResult> DeleteAccount()
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Username == User.Identity.Name);
            if (user == null)
                ModelState.AddModelError("", "Something went wrong.");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(User user)
        {
            //create one claim
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Username),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.RoleName)
            };

            //create ClaimsIdentity object
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            //set up the cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        static string Encrypt(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }


        static string Decrypt(string encryptedPassword)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptedPassword);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new string(decoded_char);
            return result;
        }

    }
}
