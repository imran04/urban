using app.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace app.Infra
{
    public class BookingRepository : IBookingRepository
    {
        ILogger<ProviderRepository> Logger;
        IConfiguration Configuration;
        IHttpContextAccessor HttpContext;

        public BookingRepository(ILogger<ProviderRepository> logger, IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public object AddBooking(Booking booking)
        {
            int count = 0;
            var UserId = HttpContext.HttpContext.User.Identity.Name;
            booking.consumer_id = int.Parse(UserId);
            booking.request_completion_date= DateTime.Now;
            booking.service_id=0;
            var Query = @"INSERT INTO [dbo].[booking]([consumer_id],[provider_id],[service_id],[request_datetime],[request_completion_date]
           ,[consumer_rating],[provider_rating],[complete_status],[instruction]) VALUES (
            @consumer_id,@provider_id,@service_id,@request_datetime,@request_completion_date
            ,@consumer_rating,@provider_rating,@complete_status,@instruction)";
            using (var con = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    count = con.Execute(Query, booking, transaction: tra);
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

        public object DeleteBooking(Booking booking)
        {
            int count = 0;
            var UserId = HttpContext.HttpContext.User.Identity.Name;
            
            var Query = @"delete from [dbo].[booking] where consumer_id=@UserId and booking_id=@booking_id";
            using (var con = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    count = con.Execute(Query, new {UserId,booking.booking_id }, transaction: tra);
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

        public object ListAllBooking(int Page=0,int Size=20)
        {
            try
            {
                string Query = "";
                var UserType = int.Parse(HttpContext.HttpContext.User.Claims.FirstOrDefault(o => o.Type == "TypeUser").Value);
                var UserId = HttpContext.HttpContext.User.Identity.Name;
                
                Logger.LogInformation($@"{UserId} : {UserType}");
                Logger.LogInformation($@"{Page*Size} :{Page} :{Size}");
                if (UserType == 0)
                {
                    Query = @"select b.booking_id, b.instruction,C.Name ConsumerName,c.Email ConsumerEmail,c.AvgRating consumerrating,P.Name ProviderName,p.Email ProviderEmail,p.AvgRating providerrating,b.request_datetime OnDate,
                                pu.Address as ProviderAddress,cu.Address as ConsumerAddress,
                                b.complete_status complete from booking b join 
                                profile p on p.userid=b.provider_id join
                                Users pu on pu.userid=b.provider_id join
                                profile c on c.UserId=b.consumer_id join
                                Users cu on cu.userid=b.provider_id 
                                where b.consumer_id=@UserId order by booking_id desc 
                                offset @page*@size rows
                                fetch next @size rows only";

                }
                else
                {
                    Query = @"select b.booking_id, b.instruction,C.Name ConsumerName,c.Email ConsumerEmail,c.AvgRating consumerrating,P.Name ProviderName,p.Email ProviderEmail,p.AvgRating providerrating,b.request_datetime OnDate,
                                pu.Address as ProviderAddress,cu.Address as ConsumerAddress,
                                b.complete_status complete from booking b join 
                                profile p on p.userid=b.provider_id join
                                Users pu on pu.userid=b.provider_id join
                                profile c on c.UserId=b.consumer_id join
                                Users cu on cu.userid=b.provider_id  
                                where b.provider_id=@UserId order by booking_id desc 
                                offset @page*@size rows
                                fetch next @size rows only";
                }
                using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Logger.LogInformation(Query);
                    var count = cn.Query<BookingVm>(Query, new { UserId,page=Page,size=Size }).Distinct().ToList();
                    return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };
                }
            }
            catch(Exception ex)
            {
                return new ResultObject { status = ResultType.FAILED, Message = "Failed: "+ex.Message};
            }
        }

        public object UpdateConsumerRating(BookingVm booking,string comment, float rate)
        {
            var Query = @"UPDATE booking SET consumer_rating = @rate, provider_comment=@comment, provider_comment_date=getdate() WHERE booking_id=@Id";
            using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
            { cn.Open();
                try
                {
                    using (var tran = cn.BeginTransaction())
                    {
                        Logger.LogInformation(Query);
                        var count = cn.Execute(Query, new { Id = booking.booking_id, comment,rate }, tran);
                        if (count == 1)
                        {
                            tran.Commit();
                            return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count }; }
                        else
                            return new ResultObject { status = ResultType.FAILED, Message = "Failed", Payload = count };
                    }
                }
                catch (Exception ex)
                    {
                    return new ResultObject { status = ResultType.FAILED, Message = ex.Message, Payload = null };
                }
            }

        }

        public object UpdateProviderRating(BookingVm booking,string comment, float rate)
        {
            var Query = @"UPDATE booking SET request_completion_date =getdate(),provider_rating = @rate,complete_status = 1 ,consumer_comment=@comment,consumer_comment_date=getdate() m WHERE booking_id=@Id";
            using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                cn.Open();
                try
                {
                    using (var tran = cn.BeginTransaction())
                    {
                        Logger.LogInformation(Query);
                        var count = cn.Execute(Query, new { Id = booking.booking_id,comment, rate }, tran);
                        if (count == 1)
                        {
                            tran.Commit();
                            return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count }; }
                        else
                            return new ResultObject { status = ResultType.FAILED, Message = "Failed", Payload = count };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultObject { status = ResultType.FAILED, Message = ex.Message, Payload = null };
                }
            }
        }

        public object SelectBooking(int Id)
        {
            try
            {
                string Query = "";
                var UserType = int.Parse(HttpContext.HttpContext.User.Claims.FirstOrDefault(o => o.Type == "TypeUser").Value);
                var userid = HttpContext.HttpContext.User.Identity.Name;

                Logger.LogInformation($@"{userid} : {UserType}");

                if (UserType == 0)
                {
                    Query = @" select b.booking_id, b.instruction,
C.name ConsumerName,c.email ConsumerEmail,b.consumer_rating consumerrating,
 P.name ProviderName,p.email ProviderEmail,b.provider_rating providerrating,b.request_datetime OnDate,
                               pu.address as ProviderAddress,cu.address as ConsumerAddress,
                                b.complete_status complete from booking b join 
                                profile p on p.userid=b.provider_id join
                                Users pu on pu.userid=b.provider_id join
                                profile c on c.userid=b.consumer_id join
                                Users cu on cu.userid=b.provider_id 
                                where booking_id=@Id";

                }
                else
                {
                    Query = @"select b.booking_id, b.instruction,
C.name ConsumerName,c.email ConsumerEmail,b.consumer_rating consumerrating,
 P.name ProviderName,p.email ProviderEmail,b.provider_rating providerrating,b.request_datetime OnDate,
                               pu.address as ProviderAddress,cu.address as ConsumerAddress,
                                b.complete_status complete from booking b join  
                                profile p on p.userid=b.provider_id join
                                Users pu on pu.userid=b.provider_id join
                                profile c on c.userid=b.consumer_id join
                                Users cu on cu.userid=b.provider_id  
                                where  booking_id=@Id";
                }
                using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
                {
                    Logger.LogInformation(Query);
                    var count = cn.QueryFirstOrDefault<BookingVm>(Query, new { Id, });
                    return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };
                }
            }
            catch (Exception ex)
            {
                return new ResultObject { status = ResultType.FAILED, Message = ex.Message, Payload = null };
            }
        }

        public object AddComment(BookingVm id, string comment,int c_to_p)
        {
            string Query = @"insert into booking_follow_up
 (id, booking_Id, cid, pid, c_to_p, comment, on_datetime)
select NEWID(),booking_Id,consumer_id,provider_id,@c_to_p,@comment,getdate()  from booking
where booking_id = @booking_id";
            using (var cn = new SqlConnection(Configuration.GetConnectionString("default")))
            {
                cn.Open();
                try
                {
                    using (var tran = cn.BeginTransaction())
                    {
                        Logger.LogInformation(Query);
                        var count = cn.Execute(Query, new {  id.booking_id, c_to_p ,comment}, tran);
                        if (count == 1)
                        {
                            tran.Commit();
                            return new ResultObject { status = ResultType.SUCCESS, Message = "Success", Payload = count };
                        }
                        else
                            return new ResultObject { status = ResultType.FAILED, Message = "Failed", Payload = count };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultObject { status = ResultType.FAILED, Message = ex.Message, Payload = null };
                }
            }
        }
    }

}
