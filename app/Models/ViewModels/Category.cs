using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Models.ViewModels
{
    public class Category
    {
        [Key]
        public int service_category_id { get; set; }
        public string category_name { get; set; }
        public string image { get; set; }

        public  ICollection<ServiceVm> Services { get; set; }

    }

}
