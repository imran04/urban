using app.Models.ViewModels;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace app.Infra
{
    public interface IUserRepository
    {
        object AddUser(Users user);
        object Login(string UserName, string Password);

    }

    public class UserRepository : IUserRepository
    {

        IConfiguration configuration;
        ILogger<UserRepository> logger;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                        var token = GenrateToken(count);
                        return new ResultObject { status = ResultType.SUCCESS, Payload = new { token, count.username, count.address, count.emailid, count.latitute, count.longitude, count.userid, profile_complete=ProfileComplete, count.type }, Message = "Login Complete" };
                    }
                }
                
                return new ResultObject { status = ResultType.FAILED, Message = "Invalid User/Password" };
                
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
    public static class PasswordHelper
    {

        
        public static (string, string) HashPassword(this string Password)
        {
            string SecurityStamp = Guid.NewGuid().ToString();
            return Password.HashPassword(SecurityStamp);
        }
        public static (string, string) HashPassword(this string Password, string SecurityStamp)
        {
            string hash;
            using (Rfc2898DeriveBytes h = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(SecurityStamp), 10))
            {
                hash = Convert.ToBase64String(h.GetBytes(20));
            }
            return (hash, SecurityStamp);
        }

        public static bool CheckValidPasswprd(this string Password, string SecurityStamp, string passwordHash)
        {
            var (hash, _) = Password.HashPassword(SecurityStamp);
            if (hash == passwordHash)
                return true;
            else return false;

        }
    }
    public static class JWTHelper
    {
        //public static IConfiguration Configuration { get; set; }
        public static string GenerateJwtToken(this Claim[] claims)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("THisISMySceret12");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(30),
                Audience = "*",
                Issuer = "*",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
