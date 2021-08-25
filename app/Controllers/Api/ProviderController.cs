using app.Infra;
using app.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
         [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

        public object Services()
        {
           return providerRepository.ListServices();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public object AddServices(int[] serviceId){

            var providerService = providerRepository.ListServices() as ResultObject;
            if(providerService.status== ResultType.SUCCESS && (providerService.Payload as List<ProviderServices>).Count>0){
                    var data=  (providerService.Payload as List<ProviderServices>).Select( i=> i.ServiceId).ToList();
                    logger.LogInformation($"{string.Join(",",data)}");
                    //data.RemoveAll(s=> serviceId.Contains(s));
                    foreach (var item in data)
                    {
                        providerRepository.RemoveService(item);
                    }

                    
            }
            foreach(var i in serviceId)
                providerRepository.AddService(i);
            return serviceId;
        }
    }
}
