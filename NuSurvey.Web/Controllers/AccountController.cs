using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MvcContrib;
using NuSurvey.Core.Domain;
using NuSurvey.Web.Controllers.Filters;
using NuSurvey.Web.Models;
using NuSurvey.Web.Services;
using System.Linq.Expressions;
using System.Linq;
using UCDArch.Core.PersistanceSupport;
using UCDArch.Core.Utils;

namespace NuSurvey.Web.Controllers
{
    public class AccountController : ApplicationController
    {
        private readonly IEmailService _emailService;
        private readonly IRepositoryWithTypedId<User, string> _userRepository;
        private IFormsAuthenticationService FormsService { get; set; }
        private IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public AccountController(IEmailService emailService, IFormsAuthenticationService formsAuthenticationService, IMembershipService membershipService, IRepositoryWithTypedId<User, string> userRepository)
        {
            _emailService = emailService;
            _userRepository = userRepository;
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
                    FormsService.SignIn(model.UserName.ToLower(), model.RememberMe);
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


        public ActionResult OpenRegister()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.UserRole = RoleNames.User;
            ViewBag.ProgramDirectorRole = RoleNames.ProgramDirector;

            var viewModel = new OpenRegisterModel();
            viewModel.User = new User();

            return View(viewModel);
        }

        [CaptchaValidator]
        [HttpPost]
        public ActionResult OpenRegister(OpenRegisterModel model, bool captchaValid)
        {
            ViewBag.UserRole = RoleNames.User;
            ViewBag.ProgramDirectorRole = RoleNames.ProgramDirector;

            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }
            if (model.Agree != "agreed")
            {
                ModelState.AddModelError("Agree", "You must agree to the terms to register");
            }           

            if(!ModelState.IsValid)
            {
                return View(model);
            }

            model.Email = model.Email.ToLower();

            MembershipCreateStatus createStatus = MembershipService.CreateUser(model.Email, "BTDF4hd7ehd6@!", model.Email);
            if (createStatus == MembershipCreateStatus.Success)
            {
                //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                if (MembershipService.ManageRoles(model.Email, model.Roles))
                {
                    Message = "Registered and roles created.";
                }
                else
                {
                    Message = "User created, but problem with roles.";
                }
                var tempPass = MembershipService.ResetPassword(model.Email.ToLower());
                //_emailService.SendNewUser(Request, Url, model.Email.ToLower(), tempPass);

                Message = string.Format("{0} {1}", Message, "You will received an email with instructions");
                try
                {
                    var user = new User(model.Email.ToLower().Trim());
                    user.Firstname = model.User.Firstname;
                    user.LastName = model.User.LastName;
                    user.Title = model.User.Title;
                    user.Agency = model.User.Agency;
                    user.City = model.User.City;
                    user.State = model.User.State;

                    _userRepository.EnsurePersistent(user);
                }
                catch (Exception ex)
                {
                    var yyy = ex.Message;

                }

                return this.RedirectToAction(a => a.LogOn());

            }
            else if(createStatus == MembershipCreateStatus.DuplicateUserName || createStatus == MembershipCreateStatus.DuplicateEmail)
            {
                Message = "You have already registered";
            }
            else
            {
                Message = "There was an error registering you.";
            }

            return View(model);


        }

        /// <summary>
        /// #4
        /// URL: /Account/Register
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            ViewBag.UserRole = RoleNames.User; //Educator
            ViewBag.AdminRole = RoleNames.Admin;
            ViewBag.ProgramDirectorRole = RoleNames.ProgramDirector;            

            var viewModel = new RegisterModel();

            return View(viewModel);
        }

        /// <summary>
        /// #5
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        public ActionResult Register(RegisterModel model, string[] roles )
        {
            ViewBag.UserRole = RoleNames.User;
            ViewBag.AdminRole = RoleNames.Admin;
            ViewBag.ProgramDirectorRole = RoleNames.ProgramDirector;

            model.Email = model.Email.ToLower();

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.Email, "BTDF4hd7ehd6@!", model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    if(MembershipService.ManageRoles(model.Email, roles))
                    {
                        Message = "User and roles created.";
                    }
                    else
                    {
                        Message = "User created, but problem with roles.";
                    }

                    var tempPass = MembershipService.ResetPassword(model.Email.ToLower());
                    _emailService.SendNewUser(Request, Url, model.Email.ToLower(), tempPass);

                    Message = string.Format("{0} {1}", Message, "User emailed");

                    return this.RedirectToAction(a => a.ManageUsers(false, false, false, false));
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

        /// <summary>
        /// #6
        /// </summary>
        /// <returns></returns>
        [Admin]
        public ActionResult ManageUsers(bool hideAdmin = false, bool hideUser= false, bool hidePublic = true, bool hideProgramDirector = false)
        {
            var viewModel = ManageUsersViewModel.Create(MembershipService.GetUsersAndRoles(CurrentUser.Identity.Name.ToLower()), hideAdmin, hideUser, hidePublic, hideProgramDirector);

            return View(viewModel);
        }

        /// <summary>
        /// #7
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Admin]
        public ActionResult Edit(string id)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name.ToLower())
            {
                Message = "Can't change yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (MembershipService.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            }

            var viewModel = EditUserViewModel.Create(id, MembershipService);
            viewModel.User = MembershipService.GetUser(id);

            return View(viewModel);
        }

        /// <summary>
        /// #8
        /// </summary>
        /// <param name="editUserViewModel"></param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        public ActionResult Edit(EditUserViewModel editUserViewModel)
        {
            if (editUserViewModel.Email.Trim().ToLower() == CurrentUser.Identity.Name.ToLower())
            {
                Message = "Can't change yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (MembershipService.GetUser(editUserViewModel.Email) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            }

            var roles = new string[]{"", "", ""};
            
            if (editUserViewModel.IsAdmin)
            {
                roles[0] = RoleNames.Admin;
            }
            if (editUserViewModel.IsUser)
            {
                roles[1] = RoleNames.User;
            }
            if (editUserViewModel.IsProgramDirector)
            {
                roles[2] = RoleNames.ProgramDirector;
            }
            if(MembershipService.ManageRoles(editUserViewModel.Email, roles) == true)
            {
                Message = "Roles Updated";
            }
            else
            {
                Message = "Problem with Updating Roles";
            }

            return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
        }

        /// <summary>
        /// #9
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Admin]
        public ActionResult Delete(string id)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name.ToLower())
            {
                Message = "Can't delete yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (MembershipService.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
            }

            var viewModel = EditUserViewModel.Create(id, MembershipService);
            viewModel.User = MembershipService.GetUser(id);

            return View(viewModel);
        }

        /// <summary>
        /// #10
        /// </summary>
        /// <param name="id"></param>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        public ActionResult Delete(string id, bool confirm)
        {
            if (id.Trim().ToLower() == CurrentUser.Identity.Name.ToLower())
            {
                Message = "Can't delete yourself";
                return this.RedirectToAction<ErrorController>(a => a.NotAuthorized());
            }
            if (MembershipService.GetUser(id) == null)
            {
                Message = "User Not Found";
                return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
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

            return this.RedirectToAction<AccountController>(a => a.ManageUsers(false, false, false, false));
        }

        /// <summary>
        /// #11
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword()
        {
            var viewModel = new ForgotPasswordModel();

            return View(viewModel);
        }

        /// <summary>
        /// #12
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="captchaValid"></param>
        /// <returns></returns>
        [CaptchaValidator]
        [HttpPost]
        public ActionResult ForgotPassword(string userName, bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("Captcha", "Recaptcha value not valid");
            }
            userName = userName.Trim().ToLower();

            if (MembershipService.GetUser(userName) == null)
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

        /// <summary>
        /// #13
        /// URL: /Account/ChangePassword
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        /// <summary>
        /// #14
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    //return RedirectToAction("ChangePasswordSuccess");
                    return this.RedirectToAction(a => a.ChangePasswordSuccess());
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

        /// <summary>
        /// #15
        /// URL: /Account/ChangePasswordSuccess
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult ChangeRoles()
        {
            var id = CurrentUser.Identity.Name.ToLower();
            var viewModel = EditUserViewModel.Create(id, MembershipService);
            viewModel.User = MembershipService.GetUser(id);

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangeRoles(EditUserViewModel model)
        {
            Check.Require(model.Email.ToLower() == CurrentUser.Identity.Name.ToLower());
            if (model.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "You must agree to the Terms and Conditions before making any changes.");
            }


            var roles = new string[] { "", "", "" };

            if (MembershipService.IsUserInRole(model.Email, RoleNames.Admin)) //Don't allow admin role to be changed.
            {
                roles[0] = RoleNames.Admin;
            }
            if (model.IsUser)
            {
                roles[1] = RoleNames.User;
            }
            if (model.IsProgramDirector)
            {
                roles[2] = RoleNames.ProgramDirector;
            }
            if (MembershipService.ManageRoles(model.Email, roles) == true)
            {
                Message = "Roles Updated";
            }
            else
            {
                Message = "Problem with Updating Roles";
            }


            return this.RedirectToAction(a => a.ChangeRoles());
        }

        public ActionResult ManageAccount()
        {
            return View();
        }

        public ActionResult TermsAndConditions()
        {
            return View();
        }

    }

    public class UsersRoles
    {
        public string UserName { get; set; }
        public bool Admin { get; set; }
        public bool User { get; set; }
        public bool ProgramDirector { get; set; }
    }

    public class ManageUsersViewModel
    {
        public bool HideAdmin { get; set; }
        public bool HideUser { get; set; }
        public bool HideProgramDirector { get; set; }
        public bool HidePublic { get; set; }
        public IQueryable<UsersRoles> Users { get; set; }

        public static ManageUsersViewModel Create(IQueryable<UsersRoles> users, bool hideAdmin, bool hideUser, bool hidePublic, bool hideProgramDirector)
        {
            var viewModel = new ManageUsersViewModel
                                {HideAdmin = hideAdmin, HidePublic = hidePublic, HideUser = hideUser, Users = users, HideProgramDirector = hideProgramDirector};

            if (viewModel.HidePublic)
            {
                viewModel.Users = viewModel.Users.Where(a => a.Admin || a.User || a.ProgramDirector);
            }

                if (viewModel.HideAdmin)
                {
                    viewModel.Users = viewModel.Users.Where(a => !a.Admin);
                }
                if (viewModel.HideUser)
                {
                    viewModel.Users = viewModel.Users.Where(a => !a.User);
                }
                if (viewModel.HideProgramDirector)
                {
                    viewModel.Users = viewModel.Users.Where(a => !a.ProgramDirector);
                }
  

            return viewModel;

        }
    }
}
