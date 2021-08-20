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

namespace app.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly IConfiguration configuration; 

        public AccountController(ILogger<HomeController> logger,IConfiguration _configuration)
        {
            _logger = logger;
            configuration=_configuration;
        }




        public IActionResult Provider(){
                return View();
        }

        public IActionResult Consumer(){
            return View();
        }



        public IActionResult Login()
        {
                // using (var connection = new SqlConnection(configuration.GetConnectionString("default")))
                // {
                //     var sql ="select * from servicesprofessional";
                //     var data = connection.Query<servicesprofessional>(sql).ToList();
                //     return View(data);
                // }


            return View();
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
