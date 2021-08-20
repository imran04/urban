using app.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace app.Infra
{
    public class ServiceRepository : IServiceRepository
    {

        ILogger<ServiceRepository> Logger;
        IConfiguration Configuration;
        IHttpContextAccessor HttpContext;

        public ServiceRepository(ILogger<ServiceRepository> logger, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public object AddCategory(string CategoryName)
        {
            throw new NotImplementedException();
        }

        public object AddService(Services service)
        {
            throw new NotImplementedException();
        }

        public object ListAllService()
        {
            try
            {
                string query = @"select sc.service_category_id,category_name,sc.image,service_id,servicesubcategory from service_category sc join [services] s on s.service_category_id=sc.service_category_id
                            where sc.status=1 and s.status='Active'";
                using (var connection = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Dictionary<int, Category> keyValues = new Dictionary<int, Category>();
                    var data = connection.Query<Category, ServiceVm, Category>(query, (a, b) =>
                    {
                        Category cat;
                        if (!keyValues.TryGetValue(a.service_category_id, out cat))
                        {
                            keyValues.Add(a.service_category_id, a);
                            cat = a;
                            a.Services = new List<ServiceVm>();
                        }
                        cat.Services.Add(b);
                        return cat;
                    },splitOn: "service_id").Distinct();
                    return new ResultObject { status = ResultType.SUCCESS, Payload = data, Message = "List of category" };
                }
            }
            catch(Exception ex)
            {
                return new ResultObject { status = ResultType.FAILED, Payload = null, Message = "List of category failed :" +ex.Message };
            }
        }
    }
}
