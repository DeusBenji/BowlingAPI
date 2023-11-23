using BowlingApiService.BusinessLogicLayer;
using BowlingApiService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BowlingApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class BookingsController : ControllerBase
    {
        private readonly IBookingData _businessLogicCtrl;

       public BookingsController(IBookingData inBusinessLogicCtrl)
        {
            _businessLogicCtrl = inBusinessLogicCtrl;
        }

        // URL: api/bookings
        [HttpGet]
        [Authorize]
        public ActionResult<List<BookingDto>> Get()
        {
            ActionResult<List<BookingDto>> foundReturn;
            // retrieve data - converted to DTO
            List<BookingDto>? foundBookings = _businessLogicCtrl.Get();
            // evaluate
            if (foundBookings != null)
            {
                if (foundBookings.Count > 0)
                {
                    foundReturn = Ok(foundBookings);                 // Statuscode 200
                }
                else
                {
                    foundReturn = new StatusCodeResult(204);    // Ok, but no content
                }
            }
            else
            {
                foundReturn = new StatusCodeResult(500);        // Internal server error
            }
            // send response back to client
            return foundReturn;
        }

        // URL: api/bookings/{id}
        [HttpGet, Route("{id}")]
        
        public ActionResult<BookingDto> Get(int id)
        {
            ActionResult<BookingDto> foundReturn;
            // retrieve data - converted to DTO
            BookingDto? foundBooking = _businessLogicCtrl.Get(id);
            // evaluate
            if (foundBooking != null)
            {
                Response.Headers.Add("Id", foundBooking.Id.ToString());
                Response.Headers.Add("PriceId", foundBooking.PriceId.ToString());
                foundReturn = Ok(foundBooking);       // Statuscode 200
            }
            else
            {
                foundReturn = new StatusCodeResult(404);    // Not found
            }
            // send response back to client
            return foundReturn;
        }


        // URL: api/bookings
        [HttpPost]
        public ActionResult<int> PostNewBooking(BookingDto inBookingDto)
        {
            ActionResult<int> foundReturn;
            int insertedId = -1;
            if (inBookingDto != null)
            {
                // Map the NewBookingDto to BookingDto
                BookingDto bookingDto = new BookingDto
                {
                    StartDateTime = inBookingDto.StartDateTime,
                    HoursToPlay = inBookingDto.HoursToPlay,
                    NoOfPlayers = inBookingDto.NoOfPlayers,
                    Customer = inBookingDto.Customer,
                    PriceId = inBookingDto.PriceId,
                    LaneId = inBookingDto.LaneId
                };

                insertedId = _businessLogicCtrl.Add(bookingDto);
            }
            // Evaluate
            if (insertedId > 0)
            {
                foundReturn = Ok(insertedId);
            }
            else if (insertedId == 0)
            {
                foundReturn = BadRequest();     // missing input
            }
            else
            {
                foundReturn = new StatusCodeResult(500);    // Internal server error
            }
            return foundReturn;
        }
        // URL: api/bookings/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult Delete(int id)
        {
            ActionResult foundReturn;
            bool isDeleted = _businessLogicCtrl.Delete(id);
            // Evaluate
            if (isDeleted)
            {
                foundReturn = Ok(isDeleted);           // Statuscode 200
            }
            else
            {
                foundReturn = new StatusCodeResult(404);    // Not found
            }
            // send response back to client
            return foundReturn;
        }
        [HttpPut("{id}")]
        [Authorize]
        public ActionResult Put(int id, [FromBody] BookingDto updatedBookingDto)
        {
            if (updatedBookingDto == null)
            {
                return BadRequest();    // Bad request, missing input
            }

            // Retrieve the existing booking details
            BookingDto? existingBookingDto = _businessLogicCtrl.Get(id);

            if (existingBookingDto == null)
            {
                return NotFound();    // Booking not found
            }

            // Update the existing booking details with the new values
            existingBookingDto.StartDateTime = updatedBookingDto.StartDateTime;
            existingBookingDto.HoursToPlay = updatedBookingDto.HoursToPlay;
            existingBookingDto.NoOfPlayers = updatedBookingDto.NoOfPlayers;
            existingBookingDto.Customer = updatedBookingDto.Customer;

            bool isUpdated = _businessLogicCtrl.Put(existingBookingDto, id);
            if (isUpdated)
            {
                return Ok(isUpdated);     // Statuscode 200
            }
            else
            {
                return StatusCode(500); // Internal server error
            }
        }

        // URL: api/bookings/customer/phone/{phoneNumber}
        [HttpGet("customer/phone/{phoneNumber}")]
        public ActionResult<List<BookingDto>> GetBookingsByCustomerPhone(string phoneNumber)
        {
            ActionResult<List<BookingDto>> foundReturn;
            // retrieve data - converted to DTO
            List<BookingDto>? foundBookings = _businessLogicCtrl.GetBookingsByCustomerPhone(phoneNumber);
            // evaluate
            if (foundBookings != null)
            {
                if (foundBookings.Count > 0)
                {
                    foundReturn = Ok(foundBookings);                 // Statuscode 200
                }
                else
                {
                    foundReturn = new StatusCodeResult(204);    // Ok, but no content
                }
            }
            else
            {
                foundReturn = new StatusCodeResult(500);        // Internal server error
            }

            return foundReturn;
        }
    }
}