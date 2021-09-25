using System.ComponentModel.DataAnnotations;

namespace app.Models.ViewModels
{
    public class Services
    {
        [Key]
        public int service_id { get; set; }
        public string servicecategory { get; set; }
        public string servicesubcategory { get; set; }
        public string status  { get; set; }
        public int service_category_id { get; set; }
    }
}
