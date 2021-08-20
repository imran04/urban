using app.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {

        ILogger<ServicesController> logger;
        IServiceRepository serviceRepository;

        public ServicesController(ILogger<ServicesController> logger, IServiceRepository serviceRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
        }

        [Route("[action]")]
        [HttpGet]
        public object AllServices()
        {
            return serviceRepository.ListAllService();
        }
        
    }
}
