using app.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Collections.Generic;

namespace app.Infra
{
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
                        Logger.LogInformation($"{UserId} : {serviceId} | added");
                        return new ResultObject { status = ResultType.SUCCESS, Message = "Added " };
                    }
                    else
                    {
                        tra.Rollback();
                        Logger.LogInformation($"{UserId} : {serviceId} | failed");
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
            Logger.LogInformation($"------------------------------Search--{s.Service}------------------------------------");
            var serach =@"select distinct p.*,s.* from users u join
                profile p on u.userid=p.userid join
                selected_services ss on ss.uid=u.userid  join
                services s on ss.service_id=s.service_id where servicesubcategory=@Service or servicecategory=@Service 
                order by userid
                offset @page*@size rows
                fetch next @size rows only";
            try{
            using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Logger.LogInformation(serach);
                    var hash = new Dictionary<int, ProfileSearchModel>();
                    var count = cn.Query<ProfileSearchModel,Services, ProfileSearchModel>(serach,(s,d)=> {
                        ProfileSearchModel p ;
                        if(!hash.TryGetValue(s.UserId,out p))
                        {
                            p = s;
                            p.Service = new List<Services>();
                            hash.Add(p.UserId, p);
                        }
                        p.Service.Add(d);
                        return p;
                    
                    } ,new { s.Service,page=s.Page,size=s.Size },splitOn: "service_id").Distinct().ToList();

                    Logger.LogInformation("--------------------------------------------------------------------");
                    return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };

                }
            }
            catch(Exception ex){
                Logger.LogInformation($"------------------------------Exception : {ex.Message} --------------------------------------");
                 return new ResultObject { status = ResultType.FAILED, Message = "Failed: "+ex.Message};
            }


            
        }

        public object ListServices()
        {
            try
            {
                var UserId = HttpContext.HttpContext.User.Identity.Name;
                using (var con = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    var s = @"select ss.uid UserId,
                        ss.service_id ServiceId,
                        s.servicesubcategory Service,
                        sc.category_name Category from selected_services ss
                        join services s on s.service_id=ss.service_id
                        join service_category sc on sc.service_category_id = s.service_category_id
                        where uid =@UserId";

                    var data = con.Query<ProviderServices>(s, new { UserId }).ToList();

                    return new ResultObject { status = ResultType.SUCCESS, Payload = data, Message = "Data" };
                }
            }
            catch(Exception ex)
            {
                return new ResultObject { status = ResultType.FAILED, Payload = null, Message = "Error:" +ex.Message };
            }
        }

        public object ProviderDetails(int Id)
        {
            
            var serach = @"select distinct p.*,s.* from users u join
                profile p on u.userid=p.userid join
                selected_services ss on ss.uid=u.userid  join
                services s on ss.service_id=s.service_id where p.userid=@Id  
                order by userid";
            try
            {
                using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Logger.LogInformation(serach);
                    var hash = new Dictionary<int, ProfileSearchModel>();
                    var count = cn.Query<ProfileSearchModel, Services, ProfileSearchModel>(serach, (s, d) => {
                        ProfileSearchModel p;
                        if (!hash.TryGetValue(s.UserId, out p))
                        {
                            p = s;
                            p.Service = new List<Services>();
                            hash.Add(p.UserId, p);
                        }
                        p.Service.Add(d);
                        return p;

                    }, new { Id }, splitOn: "service_id").Distinct().FirstOrDefault();

                    
                    return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };

                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"------------------------------Exception : {ex.Message} --------------------------------------");
                return new ResultObject { status = ResultType.FAILED, Message = "Failed: " + ex.Message };
            }

        }
    }
}
