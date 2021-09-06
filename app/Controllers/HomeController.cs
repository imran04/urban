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
using app.Infra;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace app.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly IConfiguration configuration;

        private IProviderRepository provider;

        private IUserRepository Userprovider;
        public HomeController(ILogger<HomeController> logger,IConfiguration _configuration,IProviderRepository Provider, IUserRepository Userprovider)
        {
            _logger = logger;
            configuration=_configuration;
            provider = Provider;
            this.Userprovider = Userprovider;
        }
       [AllowAnonymous]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name);
         
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
        }

        [AllowAnonymous]
        public IActionResult Search(string Search,int page=0,int size=30)
        {
            var data = provider.Search(new Models.ViewModels.Search { Service = Search, Page = page, Size = size }) as ResultObject;
            if (data.status == ResultType.SUCCESS)
                return View(data.Payload as IEnumerable<ProfileSearchModel>);
            else
                throw new Exception(message: data.Message);
            
        }
        [AllowAnonymous]
        public IActionResult Professional(string id)
        {
            _logger.LogInformation(WebUtility.HtmlDecode(id));
            var par =int.Parse(new AESEncrytDecryt().DecryptStringAES(WebUtility.HtmlDecode(id)));
            var data = provider.ProviderDetails(par) as ResultObject;
            if (data.status == ResultType.SUCCESS)
                return View(data.Payload as ProfileSearchModel);
            else
                throw new Exception(message: data.Message);
        }

        [AllowAnonymous]
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
            var result = Userprovider.Profile() as ResultObject;
            if (result.status == ResultType.SUCCESS)
            {
                return View(result.Payload as Profile);
            }
            throw new Exception(result.Message);
        }


        public IActionResult Bookings(){
            return View();
        }
        
    }
}
