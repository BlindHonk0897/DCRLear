using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using DCRSystem.Models;
using DCRSystem.DA;
using System.Web.Security;
using System.Collections.Generic;

namespace DCRSystem.Controllers
{
    [Authorize] // DO NOT DELETE IMPORTANT COMMENT
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private lear_DailiesCertificationRequirementEntities _AccountManager;
        private gatepassEntities learUser;
        private commonEmployeesEntities _EmployeesManager;
        
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager ,
            lear_DailiesCertificationRequirementEntities accountManager, 
            commonEmployeesEntities employeesManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _AccountManager = accountManager;
          
            _EmployeesManager = employeesManager;
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Account model, string returnUrl)
        {
            _EmployeesManager = new commonEmployeesEntities();
            learUser = new gatepassEntities();
            _AccountManager = new lear_DailiesCertificationRequirementEntities();
            PasswordSecurity ps = new PasswordSecurity();
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Account user = new Account() { BagdeNo = model.BagdeNo, Roles = "", Password = model.Password };

            user = Repository.GetAccountDetails(user); // Calling getAccountDetailsFunction from Repository Class

            // check if User is exist..
            if (user != null)
            {
                // if true..

                // Get Employee Details
                Employees_Details userrr = _EmployeesManager.Employees_Details.Where(em => em.Employee_ID == user.BagdeNo).FirstOrDefault();
                var intBadge = System.Int32.Parse(model.BagdeNo).ToString();
                // Get User info from user_vw using BadgeNo
                users_vw usertemp = learUser.users_vw.Where(use => use.Employee_ID == user.BagdeNo ).FirstOrDefault();
               
                // Get total Number of Employees 
                var countEmployees = _EmployeesManager.Employees_Details.ToList();

                // Get total Number of Active Employees 
                var countActiveEmployees = _EmployeesManager.Employees_Details.Where(emp => emp.Job_Status.ToUpper().Contains("CURRENT")).ToList().Count();
                
                // Get total Number of Inactive Employees 
                var countNewlyEmployees = _EmployeesManager.newlyEmployees.ToList();
              
                // Set Authentication Cookie to User's EMAIL ADDRESS
                FormsAuthentication.SetAuthCookie(usertemp.Email, false);

                // [ BEGIN -- Authentication Configuration
                var authTicket = new FormsAuthenticationTicket(1, usertemp.Email, DateTime.Now, DateTime.Now.AddMinutes(720), false, user.Roles);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                // -- END Authentication Configuration ]

                // [ BEGIN -- Session Configuration
                Session["User"] = usertemp.Email;
                Session["RoleUser"] = user.Roles;
                Session["UserId"] = user.BagdeNo;
                Session["NumberOfEmployees"] = countEmployees.Count();
                Session["NumberOfUnderEmployees"] = _EmployeesManager.Emp_Route.Where(emp => emp.Checker_1_ID == user.BagdeNo).ToList().Count();
                Session["NumberOfNewlyEmployees"] = countNewlyEmployees.Count();
                Session["NumberOfActiveEmployees"] = countActiveEmployees;
                Session["NumberOfInactiveEmployees"] = countEmployees.Count() - countActiveEmployees;
                if (userrr != null) { Session["UserPosition"] = userrr.Position; }
                // -- END Session Configuration ]

                return RedirectToAction("Home", "Home");
            }
            else if (model.BagdeNo.ToString() == "1234" && model.Password.ToString() =="IT")  // Hardcoded User For IT admin
            {
                // Get total Number of Employees 
                var countEmployees = _EmployeesManager.Employees_Details.ToList();

                // Get total Number of Inactive Employees 
                var countActiveEmployees = _EmployeesManager.Employees_Details.Where(emp => emp.Job_Status.ToUpper().Contains("CURRENT")).ToList().Count();

                // Get total Number of Inactive Employees 
                var countNewlyEmployees = _EmployeesManager.newlyEmployees.ToList();

                // Set Authentication Cookie to User's EMAIL ADDRESS -- ( Hardcoded )
                FormsAuthentication.SetAuthCookie("IT@lear.com", false);

                // [ BEGIN -- Authentication Configuration
                var authTicket = new FormsAuthenticationTicket(1, "IT@lear.com", DateTime.Now, DateTime.Now.AddMinutes(720), true, "IT");
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                // -- END Authentication Configuration ]

                // [ BEGIN -- Session Configuration
                Session["User"] = "IT@lear.com";
                Session["RoleUser"] = "IT";
                Session["UserId"] = "IT";
                Session["UserPosition"] = "ITAdmin";
                Session["NumberOfEmployees"] = countEmployees.Count();
                Session["NumberOfNewlyEmployees"] = countNewlyEmployees.Count();
                Session["NumberOfActiveEmployees"] = countActiveEmployees;
                Session["NumberOfInactiveEmployees"] = countEmployees.Count()- countActiveEmployees;
                Session["NumberOfRecertificationPlans"] = _AccountManager.ReCertificationPlans.ToList().Count();
                // [ BEGIN -- Session Configuration

                return RedirectToAction("Home", "Home");
            }

            else // else return View with error mesage.
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(Account model)
        {
            _EmployeesManager = new commonEmployeesEntities();
            _AccountManager = new lear_DailiesCertificationRequirementEntities();
           
            // Check if model is Valid
            if (ModelState.IsValid)
            {
                // Check if password is Match
                System.Diagnostics.Debug.WriteLine(Request["Confirm"].ToString() +"___"+ model.Password);
                if (model.Password == Request["Confirm"].ToString())
                {
                    // Check if BadgeNo is exist!;
                    var user = _EmployeesManager.Employees_Details.Where(u => u.Employee_ID == model.BagdeNo).SingleOrDefault();
                    if (user != null)
                    {
                        // Check if account is already exist
                        var account = _AccountManager.Users.Where(a => a.BadgeNo == model.BagdeNo).SingleOrDefault();
                        if(account != null)
                        {
                            ModelState.AddModelError("", "Account already exist");
                        }
                        else //if not save to User table
                        {
                            var users = _EmployeesManager.Database.SqlQuery<Approver>("Select * from approvers").ToList<Approver>();
                            model.Roles = "Default";
                            foreach (Approver app in users)
                            {
                                System.Diagnostics.Debug.WriteLine(app.approver);
                                if (model.BagdeNo.Equals(app.approver.ToString()))
                                {
                                    model.Roles = "Approver";
                                    break;
                                }
                            }
                            // Implement here Password:Encryption
                            PasswordSecurity ps = new PasswordSecurity();
                            var pass = ps.Encryptdata(model.Password);
                            //System.Diagnostics.Debug.WriteLine(pass);

                            //System.Diagnostics.Debug.WriteLine(ps.Decryptdata(pass)+"DECRYPTED");
                             User useraccount = new User() { BadgeNo = model.BagdeNo, Roles = model.Roles, Password = pass };
                             _AccountManager.Users.Add(useraccount);
                             _AccountManager.SaveChanges();

                            return RedirectToAction("Home", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "BadgeNo is not exist");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Password is not Match!");
                }
                //return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        
        // POST: /Account/LogOff
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            FormsAuthentication.SignOut();
            Session["User"] = "";
            Session["RoleUser"] = null;
            Session["UserId"] = "";
            Session["UserPosition"] = "";
            Session["NumberOfEmployees"] = "";
            return RedirectToAction("Login", "Account");
        }

        // You may think why there's logOff() and logOut() method ?
        // Never mind that because its method have different implementation
        // So never Delete one of the method else you will encounter some error 

        [HttpGet]
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session["User"] = "";
            Session["RoleUser"] = null;
            Session["UserId"] = "";
            Session["UserPosition"] = "";
            Session["NumberOfEmployees"] = "";
            return RedirectToAction("Login", "Account");
        }

        // not Used in this Application but Don't DELETE THIS METHOD it might be used in the future Application enhancement!
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}