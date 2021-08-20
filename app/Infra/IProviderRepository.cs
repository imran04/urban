using app.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace app.Infra
{
    public interface IProviderRepository
    {
        object Search(Search s);
        object AddService(int serviceId);
        object RemoveService(int serviceId);

    
    }

    public class ProviderRepository : IProviderRepository
    {

        ILogger<ProviderRepository> Logger;
        IConfiguration Configuration;
        IHttpContextAccessor HttpContext;

        public ProviderRepository(ILogger<ProviderRepository> logger, IConfiguration configuration, IHttpContextAccessor _HttpContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HttpContext = _HttpContext ?? throw new AccessViolationException(nameof(_HttpContext));
        }

        public object AddService(int serviceId)
        {
            int count = 0;
            var UserId = HttpContext.HttpContext.User.Identity.Name;
            using (var con = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                con.Open();
                using(var tra= con.BeginTransaction())
                {
                    count = con.Execute("Insert Into selected_services (uid,service_id) values (@uid,@service_id)", new { uid = UserId, service_id = serviceId },transaction:tra );
                    if (count == 1)
                    {
                        tra.Commit();
                        return new ResultObject { status = ResultType.SUCCESS, Message = "Added " };
                    }
                    else
                    {
                        tra.Rollback();
                        return new ResultObject { status = ResultType.FAILED, Message = "Failed " };
                    }
                }
            }

        }

        public object RemoveService(int serviceId)
        {
            int count = 0;
            var UserId = HttpContext.HttpContext.User.Identity.Name;
            using (var con = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    count = con.Execute("delete from selected_services  where uid=@uid and service_id=@service_id)", new { uid = UserId, service_id = serviceId }, transaction: tra);
                    if (count == 1)
                    {
                        tra.Commit();
                        return new ResultObject { status = ResultType.SUCCESS, Message = "Removed " };
                    }
                    else
                    {
                        tra.Rollback();
                        return new ResultObject { status = ResultType.FAILED, Message = "Failed " };
                    }
                }
            }
        }

        public object Search(Search s)
        {
            var serach =@"select p.* from users u join
                profile p on u.userid=p.userid join
                selected_services ss on ss.uid=u.userid  join
                services s on ss.service_id=s.service_id where servicesubcategory=@Service 
                order by userid
                offset @page*@size rows
                fetch next @size rows only";
            try{
            using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Logger.LogInformation(serach);
                    var count = cn.Query<Profile>(serach, new { s.Service,page=s.Page,size=s.Size }).Distinct().ToList();


                    return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };

                }
            }
            catch(Exception ex){
                 return new ResultObject { status = ResultType.FAILED, Message = "Failed: "+ex.Message};
            }


            
        }
    }

    public interface IServiceRepository
    {
        object ListAllService();
        object AddCategory(string CategoryName);
        object AddService(Services service);


    }

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
