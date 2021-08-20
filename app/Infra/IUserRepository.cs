using app.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace app.Infra
{
    public interface IUserRepository
    {
        object AddUser(Users user);
        object Login(string UserName, string Password);

    }
}
