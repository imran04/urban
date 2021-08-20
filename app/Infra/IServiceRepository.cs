using app.Models.ViewModels;

namespace app.Infra
{
    public interface IServiceRepository
    {
        object ListAllService();
        object AddCategory(string CategoryName);
        object AddService(Services service);


    }
}
