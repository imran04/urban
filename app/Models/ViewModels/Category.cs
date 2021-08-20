using System.Collections.Generic;

namespace app.Models.ViewModels
{
    public class Category
    {
        public int service_category_id { get; set; }
        public string category_name { get; set; }
        public string image { get; set; }

        public  ICollection<ServiceVm> Services { get; set; }

    }

}
