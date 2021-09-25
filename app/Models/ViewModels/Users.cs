using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models.ViewModels
{
    public class Users
    {
        [Key]
        public int userid { get; set; }
        public int type { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",ErrorMessage ="One Special Charater,One Lower case,One Upper case,One Number and atleast 8 charater Required")]
        public string password { get; set; }
        public float latitute { get; set; }
        public float longitude { get; set; }
        public string address { get; set; }
        [Required]
        public string mobile { get; set; }
        [Required]
        public string emailid { get; set; }
        public DateTime last_login_datetime { get; set; }
        public bool status { get; set; }
        public string securitystamp { get; set; }

    }

    public class ServiceVm
    {
        public int service_id { get; set; }
        public string servicesubcategory { get; set; }
    }

    public class ProviderServices
    {
        public int UserId { get; set; }
        public int ServiceId {get;set;}
        public string Service   {get;set;}
        public string Category  {get;set;}
    }

    public class ServiceCategoryVM
    {
        public string CategoryName { get; set; }
        public string Image { get; set; }
        public string Display { get; set; }
    }
}
