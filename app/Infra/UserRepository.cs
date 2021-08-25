using app.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;

namespace app.Infra
{
    public class UserRepository : IUserRepository
    {

        IConfiguration configuration;
        ILogger<UserRepository> logger;

        IHttpContextAccessor HttpContext;
        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger, IHttpContextAccessor HttpContext)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.HttpContext = HttpContext ?? throw new ArgumentNullException(nameof(HttpContext));
        }

        public object AddUser(Users user)
        {
           


            int insertCount = 0;
            var CountCheck = "select count(*) from [users] where emailid=@email or username=@username or mobile=@mobile";
            var insertCommand = @"INSERT INTO[dbo].[users] ([type],[username],[password],[latitute],[longitude],[address],[mobile],[emailid],[last_login_datetime],[status],[securitystamp]) VALUES
            (@type,@username,@password,@latitute,@longitude,@address,@mobile,@emailid,@last_login_datetime,@status,@securitystamp)";
            var ( password,secu) = user.password.HashPassword();

            user.password = password;
            user.securitystamp = secu;
            using(var cn= new SqlConnection(configuration.GetConnectionString("default")))
            {
                var count = cn.QueryFirst<int>(CountCheck,new {username=user.username,email=user.emailid,mobile=user.mobile });
                if (count == 0)
                {
                    cn.Open();
                    using(var tra= cn.BeginTransaction())
                    {
                        user.last_login_datetime = DateTime.Now;

                        insertCount = cn.Execute(insertCommand, user,transaction:tra);
                        if (insertCount == 1)
                        {
                            tra.Commit();
                            return new ResultObject { status = ResultType.SUCCESS, Message = "User was created Successfully" };
                        }
                        else
                        {
                            tra.Rollback();
                            return new ResultObject { status = ResultType.FAILED, Message = "User was not Created! Please try again" };
                           
                        }

                    }
                }
                else
                {
                    return new ResultObject { status = ResultType.FAILED, Message = "User was not Created! User email or username Exists" };
                }
            }
        }

        public object Login(string UserName, string Password)
        {
            string Query = "Select * from Users where username=@UserName";
            var ProfileComplete = false;
            using (var cn = new SqlConnection(configuration.GetConnectionString("default")))
            {
                var count = cn.QueryFirst<Users>(Query, new { UserName});
                if (count !=null)
                {
                    if (Password.CheckValidPasswprd(count.securitystamp, count.password))
                    {
                        var query1 = "Select count(*) from selected_services where uid=@uid";
                        if (count.type == 1)
                        {
                            var r = cn.QueryFirst<int>(query1, new { uid = count.userid });
                            ProfileComplete = r > 1;
                        }
                        this.Profile();
                        var token = GenrateToken(count);
                        return new ResultObject { status = ResultType.SUCCESS, Payload = new { token, count.username, count.address, count.emailid, count.latitute, count.longitude, count.userid, profile_complete=ProfileComplete, count.type }, Message = "Login Complete" };
                    }
                }
                
                return new ResultObject { status = ResultType.FAILED, Message = "Invalid User/Password" };
                
            }
        }

        public object Profile(){
            try{
                var UserId = HttpContext.HttpContext.User.Identity.Name;
                var Sql ="Select * from Profile where UserId=@UserId";
                using(var connection=new SqlConnection(configuration.GetConnectionString("default"))){
                        var data = connection.QueryFirstOrDefault<Profile>(Sql,new{UserId});
                        if(data==null){
                            var sql1=@"insert into profile (UserId,Name,AvgRating,Mobile,Email,AlternateMobile) 
                                select userid,username,0,mobile,emailid,mobile from users where userid = @UserId";
                           connection.Execute(sql1,new{UserId});
                           data = connection.QueryFirstOrDefault<Profile>(Sql,new{UserId});
                        }

                        return new ResultObject {status = ResultType.SUCCESS,Payload=data,Message="Sccucess"};
                }
            }
            catch(Exception ex){
                return new ResultObject {status = ResultType.FAILED,Payload=null,Message="Failed:" + ex.Message};
            }

        }
        public string GenrateToken(Users data)
        {
            logger.LogInformation(JsonConvert.SerializeObject(data));
            List<Claim> cliams = new List<Claim>();
            cliams.Add(new Claim("emailid", data.emailid));
            cliams.Add(new Claim(ClaimTypes.Name, data.userid.ToString()));
            cliams.Add(new Claim("UserName", data.username));
            cliams.Add(new Claim("LatLong", data.latitute.ToString() + "|" + data.longitude.ToString()));
            cliams.Add(new Claim("Address", data.address));
            cliams.Add(new Claim("TypeUser", data.type.ToString()));
            return JWTHelper.GenerateJwtToken(cliams.ToArray());

        }
    }
}
