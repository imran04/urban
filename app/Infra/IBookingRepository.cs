using app.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.Infra
{
    public interface IBookingRepository
    {
        object AddBooking(Booking booking);
        object ListAllBooking(int Page,int Size);
        object UpdateConsumerRating(Booking booking, float rate);
        object UpdateProviderRating(Booking booking, float rate);
        object DeleteBooking(Booking booking);

        object SelectBooking(int Id);

    }

}
