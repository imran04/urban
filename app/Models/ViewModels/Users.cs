using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models.ViewModels
{
    public class Users
    {
        public int userid { get; set; }
        public int type { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
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

    public class Search
    {
        public string Service { get; set; }
        public int ServiceId { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }


    public class Category
    {
        public int service_category_id { get; set; }
        public string category_name { get; set; }
        public string image { get; set; }

        public  ICollection<ServiceVm> Services { get; set; }

    }

    public class ServiceVm
    {
        public int service_id { get; set; }
        public string servicesubcategory { get; set; }
    }


    public class Profile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string AvgRating { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AlternateMobile { get; set; }

    }

}
