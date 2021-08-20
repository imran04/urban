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
}
