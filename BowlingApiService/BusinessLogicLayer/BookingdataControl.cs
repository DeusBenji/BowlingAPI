using BowlingApiService.DTOs;
using BowlingData.DatabaseLayer;
using Microsoft.AspNetCore.Mvc;
using BowlingData.ModelLayer;
using BowlingApiService.ModelConversion;
using System.Diagnostics.Metrics;

namespace BowlingApiService.BusinessLogicLayer
{
    public class BookingdataControl : IBookingData
    {
        private readonly IBookingAccess _bookingAccess;

        public BookingdataControl(IBookingAccess inBookingAccess)
        {
            _bookingAccess = inBookingAccess;
        }

        // Deletes a booking from the database by ID
        public bool Delete(int id)
        {
            try
            {
                // Delete the booking from the database by ID
                bool isDeleted = _bookingAccess.DeleteBookingById(id);
                return isDeleted;
            }
            catch
            {
                // If an exception occurs, return false
                return false;
            }
        }

        // Retrieves a booking from the database by ID
        public BookingDto? Get(int id)
        {
            BookingDto? foundBookingDto;
            try
            {
                // Get the booking from the database by ID
                Booking? foundBooking = _bookingAccess.GetBookingById(id);
                // Convert the retrieved Booking object to BookingDto
                foundBookingDto = BookingDtoConvert.FromBooking(foundBooking);

                // Retrieve Price and Lane information from their respective tables
                if (foundBooking != null)
                {
                    foundBookingDto.PriceId = foundBooking.PriceId;
                    foundBookingDto.LaneId = foundBooking.LaneId;
                }
            }
            catch
            {
                // If an exception occurs, set foundBookingDto to null
                foundBookingDto = null;
            }
            return foundBookingDto;
        }

        // Retrieves all bookings from the database
        public List<BookingDto>? Get()
        {
            List<BookingDto>? foundDtos;
            try
            {
                // Get all bookings from the database
                List<Booking>? foundBookings = _bookingAccess.GetAllBookings();
                // Convert the list of Booking objects to BookingDto objects
                foundDtos = ModelConversion.BookingDtoConvert.FromBookingCollection(foundBookings);

                // Retrieve Price and Lane information from their respective tables
                if (foundDtos != null)
                {
                    for (int i = 0; i < foundDtos.Count; i++)
                    {
                        Booking? foundBooking = foundBookings.ElementAtOrDefault(i);
                        if (foundBooking != null)
                        {
                            foundDtos[i].PriceId = foundBooking.PriceId;
                            foundDtos[i].LaneId = foundBooking.LaneId;
                            foundDtos[i].Id = foundBooking.Id;
                        }
                    }
                }
            }
            catch
            {
                // If an exception occurs, set foundDtos to null
                foundDtos = null;
            }
            return foundDtos;
        }

        // Updates a booking in the database
        public bool Put(BookingDto bookingToUpdate, int idToUpdate)
        {
            try
            {
                // Convert the updated BookingDto object to Booking object
                Booking? updatedBooking = ModelConversion.BookingDtoConvert.ToBooking(bookingToUpdate, idToUpdate);
                // Update the booking in the database
                return _bookingAccess.UpdateBooking(updatedBooking);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex);
                // If an exception occurs, return false
                return false;
            }
        }

        // Retrieves all bookings associated with a customer's phone number
        public List<BookingDto>? GetBookingsByCustomerPhone(string phoneNumber)
        {
            List<BookingDto>? foundDtos;
            try
            {
                // Get bookings from the database by customer's phone number
                List<Booking>? foundBookings = _bookingAccess.GetBookingsByCustomerPhone(phoneNumber);
                // Convert the list of Booking objects to BookingDto objects
                foundDtos = ModelConversion.BookingDtoConvert.FromBookingCollection(foundBookings);

                // Assign the booking ID to each BookingDto
                if (foundDtos != null)
                {
                    for (int i = 0; i < foundDtos.Count; i++)
                    {
                        Booking? foundBooking = foundBookings.ElementAtOrDefault(i);
                        if (foundBooking != null)
                        {
                            foundDtos[i].Id = foundBooking.Id;
                        }
                    }
                }
            }
            catch
            {
                // If an exception occurs, set foundDtos to null
                foundDtos = null;
            }
            return foundDtos;
        }

        // Adds a new booking to the database
        public int Add(BookingDto newBooking)
        {
            int insertedId = 0;
            try
            {
                // Convert the new BookingDto object to Booking object
                Booking? foundBooking = ModelConversion.BookingDtoConvert.ToBooking(newBooking);
                if (foundBooking != null)
                {
                    // Create the booking in the database and retrieve the inserted ID
                    insertedId = _bookingAccess.CreateBooking(foundBooking);

                    if (insertedId != 0)
                    {
                        // Finds the day of the booking and matches it with the weekday of the price
                        string startDay = _bookingAccess.GetBookingStartDay(insertedId);
                        int priceId = _bookingAccess.GetPriceIdByWeekday(startDay);
                        // Update the booking's price ID in the database
                        _bookingAccess.UpdateBookingPrice(insertedId, priceId);
                        foundBooking.PriceId = priceId;
                    }
                }
            }
            catch
            {
                // If an exception occurs, set insertedId to -1
                insertedId = -1;
            }
            return insertedId;
        }
    }
}
