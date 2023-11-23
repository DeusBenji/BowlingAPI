using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BowlingData.DatabaseLayer
{
    public interface IBookingAccess
    {
        List<Booking> GetBookingsByCustomerPhone(string phone);
        Booking GetBookingById(int id);
        List<Booking> GetAllBookings();
        int CreateBooking(Booking aBooking);
        bool UpdateBooking(Booking BookingToUpdate);
        bool DeleteBookingById(int id);
        bool CreateLaneBooking(int laneId, int bookingId, SqlTransaction transaction);
        int GetPriceIdByWeekday(string weekday);
        string GetBookingStartDay(int bookingId);
        int GetLaneIdByBookingId(int bookingId);
        bool UpdateBookingPrice(int bookingId, int newPriceId);

    }
}
