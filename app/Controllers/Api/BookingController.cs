using app.Infra;
using app.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        ILogger<BookingController> logger;
        IConfiguration configuration;
        IBookingRepository bookingRepository;

        IProviderRepository providerRepository;
        public BookingController(ILogger<BookingController> logger, IConfiguration configuration, IBookingRepository bookingRepository,IProviderRepository providerRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
            this.providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public object ListAll(int page=1,int size=20)
        {
            return bookingRepository.ListAllBooking(page, size);
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public object Search(string Search,int page=0,int size=20)
        {
            return providerRepository.Search(new Models.ViewModels.Search{ Service=Search,Page=page,Size=size});
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public object Book(Booking booking){
            return bookingRepository.AddBooking(booking);
        }


    }
}
