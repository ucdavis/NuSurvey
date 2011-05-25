using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Models;
using MvcContrib;
using NuSurvey.Web.Services;

namespace NuSurvey.Web.Controllers
{
    public class AccountController : ApplicationController
    {
        private readonly IEmailService _emailService;

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public AccountController(IEmailService emailService, IFormsAuthenticationService formsAuthenticationService, IMembershipService membershipService)
        {
            _emailService = emailService;
            if (formsAuthenticationService != null)
            {
                FormsService = formsAuthenticationService;
            }
            if (membershipService != null)
            {
                MembershipService = membershipService;
            }
        }

        /// <summary>
        /// #1
        /// URL: /Account/LogOn
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// #2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName.ToLower(), model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The email or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// #3
        /// URL: /Account/LogOff
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************
        [Admin]
        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.UserRole = RoleNames.User;
            ViewBag.AdminRole = RoleNames.Admin;

            return View();
        }

        [Admin]
        [HttpPost]
        public ActionResult Register(RegisterModel model, string[] roles )
        {
            ViewBag.UserRole = RoleNames.User;
            ViewBag.AdminRole = RoleNames.Admin;

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.Email, "BTDF4hd7ehd6@!", model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    if(MembershipService.ManageRoles(model.Email, roles))
                    {
                        Message = "User and roles created";
                    }
                    else
                    {
                        Message = "User created, but problem with roles";
                    }

                    var tempPass = MembershipService.ResetPassword(model.Email.ToLower());
                    _emailService.SendNewUser(Request, Url, model.Email.ToLower(), tempPass);

                    Message = string.Format("{0} {1}", Message, "And user emailed");

                    return this.RedirectToAction(a => a.ManageUsers());
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;

            return View(model);
        }

        [Admin]
        public ActionResult ManageUsers()
        {
            var users = Membership.GetAllUsers();

            var emails = (from MembershipUser user in users
                    where user.UserName.ToLower() != CurrentUser.Identity.Name.ToLower()
                    select user.UserName.ToLower()).ToList();

            var usersRoles = new List<UsersRoles>();
            foreach (var email in emails)
            {
                var userRole = new UsersRoles();
                userRole.UserName = email;
                userRole.User = Roles.IsUserInRole(email, RoleNames.User);
                userRole.Admin = Roles.IsUserInRole(email, RoleNames.Admin);
                usersRoles.Add(userRole);
            }

            return View(usersRoles.AsQueryable());
        }

        [Admin]
        public ActionResult Edit(string id)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name)
            {
                Message = "Can't change yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (MembershipService.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers());
            }

            var viewModel = EditUserViewModel.Create(id);
            viewModel.User = MembershipService.GetUser(id);

            return View(viewModel);
        }

        [Admin]
        [HttpPost]
        public ActionResult Edit(EditUserViewModel editUserViewModel)
        {
            if (editUserViewModel.Email.Trim().ToLower() == CurrentUser.Identity.Name)
            {
                Message = "Can't change yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (Membership.GetUser(editUserViewModel.Email) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers());
            }

            var roles = new string[]{"", ""};
            
            if (editUserViewModel.IsAdmin)
            {
                roles[0] = RoleNames.Admin;
            }
            if (editUserViewModel.IsUser)
            {
                roles[1] = RoleNames.User;
            }
            if(MembershipService.ManageRoles(editUserViewModel.Email, roles) == true)
            {
                Message = "Roles Updated";
            }
            else
            {
                Message = "Problem with Updating Roles";
            }

            return this.RedirectToAction<AccountController>(a => a.ManageUsers());
        }

        [Admin]
        public ActionResult Delete(string id)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name)
            {
                Message = "Can't delete yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (Membership.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers());
            }

            var viewModel = EditUserViewModel.Create(id);
            viewModel.User = MembershipService.GetUser(id);

            return View(viewModel);
        }

        [Admin]
        [HttpPost]
        public ActionResult Delete(string id, bool confirm)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name)
            {
                Message = "Can't delete yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (Membership.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers());
            }
            if (confirm)
            {
                if (MembershipService.DeleteUser(id))
                {
                    Message = "User Removed";
                }
                else
                {
                    Message = "Remove User Failed";
                }
            }

            return this.RedirectToAction<AccountController>(a => a.ManageUsers());
        }

        public ActionResult ForgotPassword()
        {
            var viewModel = new ForgotPasswordModel();

            return View(viewModel);
        }

        [CaptchaValidator]
        [HttpPost]
        public ActionResult ForgotPassword(string userName, bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }
            userName = userName.Trim().ToLower();

            if (Membership.GetUser(userName) == null)
            {
                ModelState.AddModelError("UserName", "Email not found");
            }

            if (ModelState.IsValid)
            {
                var tempPass = MembershipService.ResetPassword(userName);
                _emailService.SendPasswordReset(userName, tempPass);

                Message = "A new password has been sent to your email. It should arrive in a few minutes. If you do not receive it, please check your email filters.";
                return this.RedirectToAction(a => a.LogOn());
            }

            Message = "Unable to reset password";
            var viewModel = new ForgotPasswordModel();
            viewModel.UserName = userName;
            return View(viewModel);
        }


        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

    }

    public class UsersRoles
    {
        public string UserName { get; set; }
        public bool Admin { get; set; }
        public bool User { get; set; }
    }
}
