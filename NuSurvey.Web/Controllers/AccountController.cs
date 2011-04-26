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

namespace NuSurvey.Web.Controllers
{
    public class AccountController : ApplicationController
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

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

        // **************************************
        // URL: /Account/LogOff
        // **************************************

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
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.Email, model.Password, model.Email);

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

                    return RedirectToAction("Index", "Home");
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
            if(Membership.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers());
            }

            var viewModel = EditUserViewModel.Create(id);

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
