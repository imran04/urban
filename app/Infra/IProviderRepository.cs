using app.Models.ViewModels;
using System.Threading.Tasks;

namespace app.Infra
{
    public interface IProviderRepository
    {
        object Search(Search s);
        object AddService(int serviceId);
        object RemoveService(int serviceId);

        object ListServices();

        object ProviderDetails(int Id);


    }
}
