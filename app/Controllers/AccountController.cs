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

namespace app.Controllers
{
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




        public IActionResult Provider(){
                return View();
        }

        public IActionResult Register(){
            return View();
        }



        public IActionResult Login()
        {
                

            return View();
        }
        [HttpPost]
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



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}
