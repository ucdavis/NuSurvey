﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DataAnnotationsExtensions;
using NuSurvey.Core.Domain;
using NuSurvey.MVC.Controllers;
using NuSurvey.MVC.Controllers.Filters;
using UCDArch.Core.Utils;

namespace NuSurvey.MVC.Models
{

    #region Models

    public class ForgotPasswordModel
    {
        [Required]
        [Email]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string UserName { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        //[Required]
        //[Display(Name = "User name")]
        //public string UserName { get; set; }

        [Required]
        [Email]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]        
        public string Email { get; set; }

        //[Required]
        //[ValidatePasswordLength]
        //[DataType(DataType.Password)]
        //[Display(Name = "Password")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm password")]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }

    }

    public class OpenRegisterModel
    {
        [Required]
        [Email]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public string Agree { get; set; }

        public string[] Roles { get; set; }

        public User User { get; set; }

    }

    public class EditUserViewModel
    {
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
        public bool IsProgramDirector { get; set; }
        public bool Confirm { get; set; }
        public MembershipUser User { get; set; }

        public User UserDetails { get; set; }

        public static EditUserViewModel Create(string email, IMembershipService membershipService)
        {
            Check.Require(!string.IsNullOrWhiteSpace(email));

            var viewModel = new EditUserViewModel {Email = email};
            //viewModel.IsAdmin = Roles.IsUserInRole(viewModel.Email, RoleNames.Admin);
            //viewModel.IsUser = Roles.IsUserInRole(viewModel.Email, RoleNames.User);
            viewModel.IsAdmin = membershipService.IsUserInRole(viewModel.Email, RoleNames.Admin);
            viewModel.IsUser = membershipService.IsUserInRole(viewModel.Email, RoleNames.User);
            viewModel.IsProgramDirector = membershipService.IsUserInRole(viewModel.Email, RoleNames.ProgramDirector);

            viewModel.Confirm = false;

            return viewModel;
        }
    }

    #endregion

    #region Services
    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool ManageRoles(string userName, string[] roles);
        bool DeleteUser(string userName);
        MembershipUser GetUser(string userName);
        string ResetPassword(string userName);
        IQueryable<UsersRoles> GetUsersAndRoles(string exceptMe);
        bool IsUserInRole(string userName, string roleName);
        bool UnlockUser(string userName);
    }

    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            return Roles.IsUserInRole(userName, roleName);
        }

        public IQueryable<UsersRoles> GetUsersAndRoles(string exceptMe)
        {
            var users = Membership.GetAllUsers();

            var emails = (from MembershipUser user in users
                          where user.UserName.ToLower() != exceptMe.ToLower()
                          select user.UserName.ToLower()).ToList();

            var usersRoles = new List<UsersRoles>();
            foreach (var email in emails)
            {
                var userRole = new UsersRoles();
                userRole.UserName = email;
                userRole.User = Roles.IsUserInRole(email, RoleNames.User);
                userRole.Admin = Roles.IsUserInRole(email, RoleNames.Admin);
                userRole.ProgramDirector = Roles.IsUserInRole(email, RoleNames.ProgramDirector);
                usersRoles.Add(userRole);
            }

            return usersRoles.AsQueryable();
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public string ResetPassword(string userName)
        {
            //_provider.UnlockUser(userName);
            return _provider.ResetPassword(userName, null);
        }

        public bool UnlockUser(string userName)
        {
            return _provider.UnlockUser(userName);
        }

        public MembershipUser GetUser(string userName)
        {
            return _provider.GetUser(userName, false);
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Value cannot be null or empty.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Value cannot be null or empty.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool DeleteUser(string userName)
        {
            return _provider.DeleteUser(userName, true);
        }

        public bool ManageRoles(string userName, string[] roles)
        {
            //try
            //{
                if (roles == null)
                {
                    roles = new string[0];
                }

                var user = _provider.GetUser(userName, false);
                var allRoles = Roles.GetAllRoles();
                var usersRoles = Roles.GetRolesForUser(userName);
                foreach (var allRole in allRoles)
                {
                    //if the existing roles is not in what we are passing and the user has that role, remove it 
                    if (!roles.Contains(allRole) && usersRoles.Contains(allRole))
                    {
                        Roles.RemoveUsersFromRole(new string[] { userName }, allRole);
                    }
                }

                foreach (var role in roles)
                {
                    //If the role we are passing exists and the user doesn't have it, then add it.
                    if (allRoles.Contains(role) && !usersRoles.Contains(role))
                    {
                        Roles.AddUsersToRole(new string[] { userName },role);
                    }
                }
                
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}
            return true;

        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Value cannot be null or empty.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Value cannot be null or empty.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
    #endregion

    #region Validation
    public static class AccountValidation
    {
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User already exists. Please enter a different user.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValidatePasswordLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "'{0}' must be at least {1} characters long.";
        private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

        public ValidatePasswordLengthAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, _minCharacters);
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;
            return (valueAsString != null && valueAsString.Length >= _minCharacters);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationStringLengthRule(FormatErrorMessage(metadata.GetDisplayName()), _minCharacters, int.MaxValue)
            };
        }
    }
    #endregion

}
