using app.Infra;
using app.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IUserRepository user;

        public AccountController(IUserRepository user)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user));
        }

        [Route("[action]")]
        [HttpPost]

        public object Register(Users data)
        {
            return user.AddUser(data);
        }


        [Route("[action]")]
        [HttpPost]

        public object Login(LoginViewModel data)
        {
            return user.Login(data.UserName,data.Password);
        }

    }
}
