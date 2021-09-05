using System.Collections.Generic;

namespace app.Models.ViewModels
{
    public class Profile
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string AvgRating { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AlternateMobile { get; set; }

    }

    public class ProfileSearchModel : Profile
    {
        public List<Services> Service { get; set; }
    }

}
