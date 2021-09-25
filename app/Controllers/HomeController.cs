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
using Microsoft.Extensions.Caching.Memory;

namespace app.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly IConfiguration configuration;

        private IProviderRepository provider;

        private IUserRepository Userprovider;
        private IBookingRepository BookingRepository;

        private IMemoryCache _cache;
        public HomeController(ILogger<HomeController> logger,IConfiguration _configuration,IProviderRepository Provider, IUserRepository Userprovider,IBookingRepository bookingRepository,IMemoryCache cache)
        {
            _logger = logger;
            configuration=_configuration;
            provider = Provider;
            this.Userprovider = Userprovider;
            BookingRepository = bookingRepository;
            _cache = cache;
        }
       [AllowAnonymous]
        public IActionResult Index()
        {
            _logger.LogInformation(User.Identity.Name);
         
            using (var connection = new SqlConnection(configuration.GetConnectionString("default")))
            {
                var sql = @"select category_name CategoryName,image Image,display Display from service_category where status=1;
                            
                            ";
                
                    List<ServiceCategoryVM> data;
                    if(!_cache.TryGetValue("CategorySearch",out data))
                    {
                        data = connection.Query<ServiceCategoryVM>(sql).ToList();
                        _cache.Set<List<ServiceCategoryVM>>("CategorySearch", data);
                    }
                    
                    return View(data);
                
  
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
            //_logger.LogInformation(WebUtility.HtmlDecode(id));
            //var par =int.Parse(new AESEncrytDecryt().DecryptStringAES(WebUtility.HtmlDecode(id)));
            var data = provider.ProviderDetails(int.Parse(id)) as ResultObject;
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

        [HttpPost]
        public IActionResult Profile(Profile profile)
        {

            var result = Userprovider.Profile() as ResultObject;
            if (result.status == ResultType.SUCCESS)
            {
                var data = result.Payload as Profile;
                profile.UserId = data.UserId;
                if (User.IsConsumer()) {
                    profile.HeadLine = "";
                    profile.Rate = 0;
                }

               result= Userprovider.UpdateProfile(profile) as ResultObject;
                if(result.status==ResultType.SUCCESS)
                {
                    result = Userprovider.Profile() as ResultObject;
                    if (result.status == ResultType.SUCCESS)
                    {
                        ViewData["Success"] = "Profile Update";
                        return View(result.Payload as Profile);
                    }
                }
            }
            throw new Exception(result.Message);

        }


        public IActionResult Bookings(int page=0,int size=20)
        {
            var result = BookingRepository.ListAllBooking(page,size) as ResultObject;
            if (result.status == ResultType.SUCCESS)
            {
                return View(result.Payload as IEnumerable<BookingVm>);
            }
            throw new Exception(result.Message);
        }

        public IActionResult BookingDetails(int Id)
        {
            var data = BookingRepository.SelectBooking(Id) as ResultObject;
            if (data.status == ResultType.SUCCESS)
            {
                return View(data.Payload as BookingVm);
            }
            return View("NotFound");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Rate(int booking_id,int rate,string comment)
        {
            var data = BookingRepository.SelectBooking(booking_id) as ResultObject;
            if (data.status == ResultType.SUCCESS)
            {
                var b = (BookingVm)data.Payload;
               if (User.IsServiceProvider())
                {
                    BookingRepository.UpdateConsumerRating( b,comment, rate);
                }
                else
                {
                    BookingRepository.UpdateProviderRating(b,comment, rate);
                }

                return RedirectToAction("BookingDetails", new { Id = booking_id });
            }
            return View("NotFound");
        }

        [HttpPost]
        [Authorize]
        public IActionResult Followup(int id,string comment)
        {
            var data = BookingRepository.SelectBooking(id) as ResultObject;
            if (data.status == ResultType.SUCCESS)
            {
                int c_to_p = 0;
                var b = (BookingVm)data.Payload;
                if (User.IsConsumer())
                {
                    c_to_p = 1;   
                }
                BookingRepository.AddComment(b, comment,c_to_p);
                return RedirectToAction("BookingDetails", new { Id = id });
            }
            return View("NotFound");
        }
    }
}
