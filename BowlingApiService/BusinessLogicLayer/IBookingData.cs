using BowlingApiService.DTOs;

namespace BowlingApiService.BusinessLogicLayer
{
    public interface IBookingData
    {
        BookingDto? Get(int id);
        List<BookingDto>? Get();
        int Add(BookingDto bookingToAdd);
        bool Put(BookingDto bookingToUpdate, int idToUpdate);
        bool Delete(int id);
        List<BookingDto>? GetBookingsByCustomerPhone(string phoneNumber);
    }
}
