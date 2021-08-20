using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models.ViewModels
{
    public class Booking
    {

        public long booking_id{get;set;}
        public int consumer_id{get;set;}
        public int provider_id{get;set;}
        public int service_id{get;set;}
        public DateTime request_datetime{get;set;}
        public DateTime request_completion_date{get;set;}
        public float consumer_rating{get;set;}
        public float provider_rating{get;set;}
        public bool complete_status{get;set;}
        public string instruction{get;set;}
    }

    public class Services
    {
        public int service_id { get; set; }
        public string servicecategory { get; set; }
        public string servicesubcategory { get; set; }
        public string status  { get; set; }
        public int service_category_id { get; set; }
    }

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
