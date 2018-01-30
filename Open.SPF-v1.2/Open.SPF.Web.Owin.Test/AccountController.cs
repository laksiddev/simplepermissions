using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using System.Web.Http;
using System.Net.Http;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
//using System.Web.Mvc;

namespace Open.SPF.Web.Test.Owin
{
    [Authorize]
    public class AccountController : ApiController
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }
        
        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

                return _userManager;
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Account/Login 
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        
        // 
        // POST: /Account/Login 
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form 
            return View(model);
        }
    }
}
