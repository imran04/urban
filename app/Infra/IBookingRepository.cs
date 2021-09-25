using app.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.Infra
{
    public interface IBookingRepository
    {
        object AddBooking(Booking booking);
        object ListAllBooking(int Page,int Size);
        object UpdateConsumerRating(BookingVm booking,string comment ,float rate);
        object UpdateProviderRating(BookingVm booking, string comment, float rate);
        object DeleteBooking(Booking booking);

        object SelectBooking(int Id);

        object AddComment(BookingVm bookingVm, string comment, int c_to_p);
    }

}
