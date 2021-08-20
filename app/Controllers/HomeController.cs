using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using app.Models;
using Microsoft.Extensions.Configuration;

using Dapper;
using System.Data.SqlClient;
using app.Models.ViewModels;

namespace app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly IConfiguration configuration; 

        public HomeController(ILogger<HomeController> logger,IConfiguration _configuration)
        {
            _logger = logger;
            configuration=_configuration;
        }

        public IActionResult Index()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("default")))
            {
                var sql = @"select category_name CategoryName,image Image,display Display from service_category where status=1;
                            select Count(*) from users where type=0;
                            select count(*) from users where type=1;
                            select count(*) from services;
                            ";
                using(var result = connection.QueryMultiple(sql))
                {
                    var data =result.Read<ServiceCategoryVM>().ToList();
                    var Consumer = result.ReadFirst<int>();
                    var Provider = result.ReadFirst<int>();
                    var services = result.ReadFirst<int>();
                    ViewBag.Consumer = Consumer;
                    ViewBag.Provider = Provider;
                    ViewBag.Services = services;
                    return View(data);
                }
                




                
            }


         //   return View();
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


        public IActionResult Profile()
        {
            return View();
        }


        public IActionResult Bookings(){
            return View();
        }
        
    }
}
