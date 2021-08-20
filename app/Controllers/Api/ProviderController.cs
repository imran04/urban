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
    public class ProviderController : ControllerBase
    {
        IProviderRepository providerRepository;
        ILogger<ProviderController> logger;
        IServiceRepository serviceRepository;

        public ProviderController(IProviderRepository providerRepository, ILogger<ProviderController> logger, IServiceRepository serviceRepository)
        {
            this.providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
        }

        [HttpGet]
        [Route("[action]")]
        public object Services()
        {
           return providerRepository.ListServices();
        }
    }
}
