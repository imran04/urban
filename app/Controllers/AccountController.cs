using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using Microsoft.Extensions.Configuration;
// using Microsoft.Data.SqlClient;
using Dapper;
using app.Models.ViewModels;
using app.Infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace app.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly IConfiguration configuration;
        IUserRepository userRepository;

        public AccountController(ILogger<HomeController> logger,IConfiguration _configuration, IUserRepository userRepository)
        {
            _logger = logger;
            configuration=_configuration;
            this.userRepository = userRepository;
        }
        [AllowAnonymous]
        public IActionResult Register(){

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Register(Users users)
        {
            if (ModelState.IsValid)
            {
                ResultObject data = userRepository.AddUser(users) as ResultObject;

                if (data.status == ResultType.SUCCESS)
                {
                    return Redirect("/Account/RegisterSuccess/");
                }
                else
                {
                    ModelState.AddModelError("UserName", data.Message);
                    return View(users);
                }
            }
            return View(users);

        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            ResultObject data =userRepository.Login(model.UserName,model.Password) as ResultObject;
            if (data.status == ResultType.SUCCESS)
            {
                return Redirect("/Home/Profile/");
            }
            else
            {
                ModelState.AddModelError("UserName", data.Message);
                return View(model);
            }
        }
        [AllowAnonymous]
        public IActionResult LogOut()
        {
            HttpContext.Response.Cookies.Delete("kjllasic");
            return RedirectToAction("index", "Home");
        }

        [AllowAnonymous]
        public IActionResult RegisterSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult AuthorizationFailed()
        {
            return View();
        }
    }
}
