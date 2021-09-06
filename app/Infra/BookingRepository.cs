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

        public object UpdateConsumerRating(Booking booking, float rate)
        {
            throw new NotImplementedException();
        }

        public object UpdateProviderRating(Booking booking, float rate)
        {
            throw new NotImplementedException();
        }
    }

}
