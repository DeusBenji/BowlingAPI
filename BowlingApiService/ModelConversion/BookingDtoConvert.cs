using BowlingApiService.DTOs;
using BowlingData.ModelLayer;
using System;
using System.Transactions;

namespace BowlingApiService.ModelConversion
{
    public class BookingDtoConvert
    {
        // Convert from Booking objects to BookingDto objects
        public static List<BookingDto>? FromBookingCollection(List<Booking> inBookings)
        {
            List<BookingDto>? bookingDtoList = null;
            if (inBookings != null)
            {
                bookingDtoList = new List<BookingDto>();
                BookingDto? tempDto;
                foreach (Booking booking in inBookings)
                {
                    if (booking != null)
                    {
                        tempDto = FromBooking(booking);
                        bookingDtoList.Add(tempDto);
                    }
                }
            }
            return bookingDtoList;
        }

        // Convert from Booking object to BookingDto object
        public static BookingDto? FromBooking(Booking inBooking)
        {
            BookingDto? aBookingReadDto = null;
            if (inBooking != null)
            {
                aBookingReadDto = new BookingDto(inBooking.StartDateTime, inBooking.HoursToPlay, inBooking.NoOfPlayers, inBooking.Customer);
                aBookingReadDto.Id = inBooking.Id;
                aBookingReadDto.PriceId = inBooking.PriceId;
                aBookingReadDto.LaneId = inBooking.LaneId;
            }
            return aBookingReadDto;
        }

        // Convert from BookingDto object to Booking object
        public static Booking? ToBooking(BookingDto inDto, int idToUpdate)
        {
            Booking? aBooking = null;
            if (inDto != null)
            {
                aBooking = new Booking(inDto.StartDateTime, inDto.HoursToPlay, inDto.NoOfPlayers, inDto.Customer);
                aBooking.PriceId = inDto.PriceId;
                aBooking.LaneId = inDto.LaneId;
                aBooking.Id = idToUpdate;
            }
            return aBooking;
        }
        // Convert from BookingDto object to Booking object
        public static Booking? ToBooking(BookingDto inDto)
        {
            Booking? aBooking = null;
            if (inDto != null)
            {
                aBooking = new Booking(inDto.StartDateTime, inDto.HoursToPlay, inDto.NoOfPlayers, inDto.Customer);
                aBooking.PriceId = inDto.PriceId;
                aBooking.LaneId = inDto.LaneId;
            }
            return aBooking;
        }
    }
}