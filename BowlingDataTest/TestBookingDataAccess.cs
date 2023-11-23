using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace BowlingDataTest
{
    public class TestBookingDataAccess
    {
        private readonly ITestOutputHelper _extraOutput;

        readonly private IBookingAccess _bAccess;

        readonly private ICustomerAccess _cAccess;
        
        readonly private ILaneAccess _laneAccess;

        readonly string _connectionString = "Server=localhost; Integrated Security=true; Database=BowlingTest";

        public TestBookingDataAccess(ITestOutputHelper output)
        {
            _extraOutput = output;
            _bAccess = new BookingDatabaseAccess(_connectionString);
            _cAccess = new CustomerDatabaseAccess(_connectionString);
            _laneAccess = new LaneDatabaseAccess(_connectionString);

        }
        [Fact]
        public void TestGetAllBookings()
        {
            // Arrange

            // Act
            List<Booking> readBookings = _bAccess.GetAllBookings();
            bool pricesWereRead = (readBookings.Count > 0);
            // Print additional output
            _extraOutput.WriteLine("Number of bookings: " + readBookings.Count);

            // Assert
            Assert.True(pricesWereRead);
        }
        [Fact]
        public void TestCreateBooking()
        {
            //Arrange
            DateTime dateTime = DateTime.Now;
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "123123123");
            int cID = _cAccess.CreateCustomer(cus);
            Customer customerRetrived = _cAccess.GetCustomerById(cID);

            Booking bk = new Booking(dateTime, 3, 5, customerRetrived); // Create a new Booking object

            // Act
            int insertedId = _bAccess.CreateBooking(bk);

            // Assert
            Assert.True(insertedId > 0);
            _bAccess.DeleteBookingById(insertedId);
        }

        [Fact]
        public void TestGetBookingById()
        {
            //Arrange
            DateTime dateTime = DateTime.Now;
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "567567567");
            int cID = _cAccess.CreateCustomer(cus);
            Customer customerRetrived = _cAccess.GetCustomerById(cID);

            int actualNoOfPlayers = 5;

            Booking bk = new Booking(dateTime, 3, 5, customerRetrived); // Create a new Booking object
            int insertedId = _bAccess.CreateBooking(bk);

            // Act
            Booking retrievedBooking = _bAccess.GetBookingById(insertedId);

            // Assert
            Assert.NotNull(retrievedBooking);
            Assert.Equal(insertedId, retrievedBooking.Id);
            Assert.Equal(actualNoOfPlayers, retrievedBooking.NoOfPlayers);
            _bAccess.DeleteBookingById(insertedId);
        }

        [Fact]
        public void TestDeleteBookingById()
        {
            DateTime dateTime = DateTime.Now;
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "234234234");
            int cID = _cAccess.CreateCustomer(cus);
            Customer customerRetrived = _cAccess.GetCustomerById(cID);

            Booking bk = new Booking(dateTime, 3, 5, customerRetrived); // Create a new Booking object
            int insertedId = _bAccess.CreateBooking(bk); // Insert the Booking into the database

            // Act
            bool isDeleted = _bAccess.DeleteBookingById(insertedId);

            // Assert
            Assert.True(isDeleted);
        }
        [Fact]
        public void TestUpdateBooking()
        {
            // Arrange
            DateTime dateTime = DateTime.Now;
            Customer cus = new Customer("Karl", "Hansen", "karl@gmail.com", "345345345");
            int cID = _cAccess.CreateCustomer(cus);
            Customer customerRetrieved = _cAccess.GetCustomerById(cID);

            Booking bk = new Booking(dateTime, 3, 5, customerRetrieved); // Create a new Booking object
            int insertedId = _bAccess.CreateBooking(bk); // Insert the Booking into the database

            // Update the booking with new values
            Booking updatedBooking = _bAccess.GetBookingById(insertedId);
            updatedBooking.StartDateTime = dateTime.AddDays(0); // Update the start date/time
            updatedBooking.HoursToPlay = 4; // Update the hours to play
            updatedBooking.NoOfPlayers = 6; // Update the number of players

            // Act
            bool isUpdated = _bAccess.UpdateBooking(updatedBooking);

            // Assert
            Assert.True(isUpdated);

            // Retrieve the updated booking from the database
            Booking retrievedBooking = _bAccess.GetBookingById(insertedId);

            // Verify that the booking has been updated correctly
            Assert.NotNull(retrievedBooking);
            Assert.Equal(insertedId, retrievedBooking.Id);
            Assert.Equal(updatedBooking.HoursToPlay, retrievedBooking.HoursToPlay);
            Assert.Equal(updatedBooking.NoOfPlayers, retrievedBooking.NoOfPlayers);
            _bAccess.DeleteBookingById(insertedId);
        }
   

    }
}
