using System;

namespace app.Models.ViewModels
{
    public class BookingVm{
        public int booking_id { get; set; }
        public string ServiceName {get;set;}
        public string instruction { get; set; }
        public string ConsumerName{get;set;}
        public string ConsumerEmail{get;set;}
        public float consumerrating { get; set; }
        public string ProviderName {get;set;}
        public string ProviderEmail {get;set;}
        public float providerrating { get; set; }
        public bool completed { get; set; }
        public DateTime OnDate{get;set;}

    }
}
