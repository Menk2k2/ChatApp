using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ChatApp.Models;
using System.Net;
using ChatApp.Models.Extensions;
using System.Data.Entity;
using System.Collections.Generic;

namespace ChatApp.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ChatAppEntities db = new ChatAppEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrEditGroup(AspNetRole role)
        {
            try
            {
                if (role.Id == "-1")
                {
                    var roleTmp = db.AspNetRoles.ToList().OrderByDescending(r => r.Id).FirstOrDefault();
                    role.Id = (int.Parse(roleTmp != null ? roleTmp.Id : "-1") + 1).ToString();
                    db.AspNetRoles.Add(role);
                }
                else
                {
                    role.Name = role.Name;
                    db.Entry(role).State = EntityState.Modified;
                }
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
            }

            return RedirectToAction("AccountsRoles");
        }

        [HttpPost]
        public async Task<ActionResult> DeletetRole(string roleId)
        {
            try
            {
                var queryDelete = string.Format("DELETE FROM AspNetRoles WHERE Id = '{0}'", roleId);
                db.Database.ExecuteSqlCommand(queryDelete);
            }
            catch (Exception)
            {
            }

            return RedirectToAction("AccountsRoles");
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };

            var userInfo = db.AspNetUsers.Where(x => x.Id == userId).FirstOrDefault();
            ViewBag.email = userInfo.Email;
            ViewBag.fullName = userInfo.FullName;
            ViewBag.phoneNumber = userInfo.PhoneNumber;

            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion

        public ActionResult Accounts(string search)
        {
            if (search != null)
            {
                ViewBag.Search = search;
                return View(db.AspNetUsers.Where(x => ((x.FullName.Contains(search) || x.Email.Contains(search) || x.UserName.Contains(search) || x.PhoneNumber.Contains(search)) && x.Disable == false)).ToList());
            }
            return View(db.AspNetUsers.Where(x => x.Disable == false).ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var user = UserManager.Users.FirstOrDefault(u => u.Id.Equals(id));

            if (user != null)
            {
                var restcode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var uresult = await UserManager.ResetPasswordAsync(user.Id, restcode, "ChatApp@2024");
                if (!uresult.Succeeded)
                {
                    AddErrors(uresult);
                    return Json(uresult);
                }
            }

            else
            {
                return Json("Error");
                //AddErrors(uresult);
            }

            return Json("Ok");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            var logins = user.Logins;
            var rolesForUser = await UserManager.GetRolesAsync(id);

            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    // item should be the name of the role
                    var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                }
            }
            user.Disable = true;

            await UserManager.UpdateAsync(user);

            return Json("Ok");
        }

        /// <summary>
        /// Khóa tài khoản 100 năm
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"> =0: mở; =1: khóa</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Lockout(string id, int status)
        {
            var acc = await UserManager.FindByIdAsync(id);
            if (acc == null) return HttpNotFound();
            acc.LockoutEnabled = status != 0;
            acc.LockoutEndDateUtc = DateTime.Now.AddYears(100);
            await UserManager.UpdateAsync(acc);
            return RedirectToAction("Accounts");
        }

        [Authorize] //(Roles = "Admin")
        public ActionResult AccountEdit(string id)
        {
            ViewBag.Id = id != null ? 1 : 0;

            var user = UserManager.Users.FirstOrDefault(u => u.Id.Equals(id)) ?? new ApplicationUser();
            return View(user.ToAccountVM());
        }
        [Authorize] //(Roles = "Admin")
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AccountEdit(AccountViewModel acc)
        {
            if (!ModelState.IsValid) return View(acc);
            var user = UserManager.Users.FirstOrDefault(u => u.Id.Equals(acc.Id));
            if (user == null)
            {
                var u = new ApplicationUser { UserName = acc.Email, PhoneNumber = acc.PhoneNumber, Email = acc.Email, FullName = acc.FullName };
                var result = await UserManager.CreateAsync(u);
                if (result.Succeeded)
                {
                    user = UserManager.Users.FirstOrDefault(uu => uu.UserName.Equals(acc.Email));
                    var restcode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var uresult2 = await UserManager.ResetPasswordAsync(user.Id, restcode, acc.ConfirmPassword);
                    if (!uresult2.Succeeded)
                    {
                        AddErrors(uresult2);
                        return Json("Mật khẩu không hợp lệ! mật khẩu tối thiểu 4 ký tự " + acc.ConfirmPassword);
                    }

                    return RedirectToAction("Accounts");
                }
                AddErrors(result);
                return View(acc);
            }
            user.FullName = acc.FullName;
            user.Email = acc.Email;
            user.PhoneNumber = acc.PhoneNumber;
            user.UserName = acc.Email;
            var uresult = await UserManager.UpdateAsync(user);
            if (!uresult.Succeeded)
            {
                AddErrors(uresult);
                return View(acc);
            }

            return RedirectToAction("Accounts");
        }

        [Authorize] //(Roles = "Admin")
        public ActionResult AccountsRoles(string tim)
        {
            return View();
        }

        [Authorize] //(Roles = "Admin")
        [HttpPost]
        public async Task<ActionResult> AddUserRole(string userid, string rolename)
        {
            await UserManager.AddToRoleAsync(userid, rolename);
            return Json("Ok");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> DeleteUserRole(string userid, string rolename)
        {
            await UserManager.RemoveFromRoleAsync(userid, rolename);
            return Json("Ok");
        }
    }
}