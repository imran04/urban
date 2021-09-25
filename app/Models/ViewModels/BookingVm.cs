using System;

namespace app.Models.ViewModels
{
    public class BookingVm{
        public int booking_id { get; set; }
        public string ServiceName {get;set;}
        public string instruction { get; set; }
        public string ConsumerName{get;set;}
        public string ConsumerEmail{get;set;}
        public string ConsumerAddress { get; set; }
        public float consumerrating { get; set; }
        public string ProviderName {get;set;}
        public string ProviderEmail {get;set;}
        public string ProviderAddress { get; set; }
        public float providerrating { get; set; }
        public bool completed { get; set; }
        public DateTime OnDate{get;set;}

    }

    public class BookingFollowUp
    {
        public Guid id { get; set; }
        public int booking_Id { get; set; }
        public int cid { get; set; }
        public int pid { get; set; }
        public bool c_to_p { get; set; }
        public string comment { get; set; }
        public DateTime on_datetime { get; set; }

    }
}
