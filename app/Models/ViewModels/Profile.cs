using app.Infra;
using System.Collections.Generic;
using System.Net;

namespace app.Models.ViewModels
{
    public class Profile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public float AvgRating { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AlternateMobile { get; set; }
        public string HeadLine { get; set; }
        public string About { get; set; }
        public string Gender { get; set; }
        public float Rate { get; set; }

        public string Id => WebUtility.HtmlDecode(new AESEncrytDecryt().EncriptStringAES(UserId.ToString()));

    }

    public class ProfileSearchModel : Profile
    {
        public List<Services> Service { get; set; }
    }

}
