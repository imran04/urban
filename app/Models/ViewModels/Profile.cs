using app.Infra;
using System.Collections.Generic;
using System.Net;

namespace app.Models.ViewModels
{
    public class Profile
    {
        
        public int userid { get; set; }
        public string name { get; set; }
        public float avgrating { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string alternatemobile { get; set; }
        public string headline { get; set; }
        public string about { get; set; }
        public string gender { get; set; }
        public float rate { get; set; }

        public string id => WebUtility.HtmlDecode(new AESEncrytDecryt().EncriptStringAES(userid.ToString()));

    }

    public class ProfileSearchModel : Profile
    {
        public List<Services> Service { get; set; }
    }

}
