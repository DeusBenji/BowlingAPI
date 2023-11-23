using Microsoft.Extensions.Configuration;
using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Transactions;

namespace BowlingData.DatabaseLayer
{

    public class BookingDatabaseAccess : IBookingAccess
    {
        readonly string? _connectionString;

        public BookingDatabaseAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CompanyConnection");
        }

        public BookingDatabaseAccess(string inConnectionString)
        {
            _connectionString = inConnectionString;
        }

        // Constructor to create BookingDatabaseAccess object with IConfiguration parameter for connection string retrieval
        public int CreateBooking(Booking aBooking)
        {
            int insertedId = -1;
            string insertBookingString = "INSERT INTO Booking (startDateTime, hoursToPlay, customerID, noOfPlayers) " +
                                         "OUTPUT INSERTED.ID VALUES (@StartDateTime, @HoursToPlay, @Customer, @NoOfPlayers)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Check lane availability and acquire a lock on the selected lane
                int availableLaneId;
                bool isLaneAvailable = IsLaneAvailable(aBooking.StartDateTime, aBooking.HoursToPlay, out availableLaneId);

                if (!isLaneAvailable)
                {
                    // Lane is not available
                    throw new Exception("The selected lane is not available for the specified time.");
                }

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Set the transaction for the command
                        SqlCommand createBookingCommand = new SqlCommand(insertBookingString, con, transaction);

                        SqlParameter startDateTimeParam = new SqlParameter("@StartDateTime", aBooking.StartDateTime);
                        createBookingCommand.Parameters.Add(startDateTimeParam);

                        SqlParameter hoursToPlayParam = new SqlParameter("@HoursToPlay", aBooking.HoursToPlay);
                        createBookingCommand.Parameters.Add(hoursToPlayParam);

                        SqlParameter customerNumberParam = new SqlParameter("@Customer", aBooking.Customer.Id);
                        createBookingCommand.Parameters.Add(customerNumberParam);

                        SqlParameter noOfPlayersParam = new SqlParameter("@NoOfPlayers", aBooking.NoOfPlayers);
                        createBookingCommand.Parameters.Add(noOfPlayersParam);

                        // Execute the insert command within the transaction and retrieve the inserted booking ID
                        insertedId = (int)createBookingCommand.ExecuteScalar();

                        // Insert the lane ID into the LaneBooking table within the transaction
                        CreateLaneBooking(availableLaneId, insertedId, transaction);

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // An error occurred, rollback the transaction
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return insertedId;
        }

        // Method to delete a booking by its ID
        public bool DeleteBookingById(int id)
        {
            bool isDeleted = false;
            string deleteLaneBookingString = "DELETE FROM LaneBooking WHERE BookingId = @Id";
            string deleteBookingString = "DELETE FROM Booking WHERE id = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlCommand deleteLaneBookingCommand = new SqlCommand(deleteLaneBookingString, con))
                using (SqlCommand deleteBookingCommand = new SqlCommand(deleteBookingString, con))
                {
                    deleteLaneBookingCommand.Parameters.AddWithValue("@Id", id);
                    deleteBookingCommand.Parameters.AddWithValue("@Id", id);

                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            deleteLaneBookingCommand.Transaction = transaction;
                            int rowsAffectedLaneBooking = deleteLaneBookingCommand.ExecuteNonQuery();

                            deleteBookingCommand.Transaction = transaction;
                            int rowsAffectedBooking = deleteBookingCommand.ExecuteNonQuery();

                            if (rowsAffectedLaneBooking > 0 && rowsAffectedBooking > 0)
                            {
                                transaction.Commit();
                                isDeleted = true;
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                        }
                    }
                }
            }

            return isDeleted;
        }

        //Method to get all bookings.
        public List<Booking> GetAllBookings()
        {
            List<Booking> foundBookings;
            Booking readBooking;
            string queryString = "select id, hoursToPlay, startDateTime, noOfPlayers, customerID from Booking";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand readCommand = new SqlCommand(queryString, con, transaction))
                        {
                            SqlDataReader bookingReader = readCommand.ExecuteReader();
                            foundBookings = new List<Booking>();

                            while (bookingReader.Read())
                            {
                                readBooking = GetBookingFromReader(bookingReader);
                                //Finds the priceID and laneID
                                string startDay = GetBookingStartDay(readBooking.Id);
                                int priceID = GetPriceIdByWeekday(startDay);
                                int laneID = GetLaneIdByBookingId(readBooking.Id);
                                //Adds the id's to the booking.
                                readBooking.LaneId = laneID;
                                readBooking.PriceId = priceID;
                                foundBookings.Add(readBooking);
                            }
                            bookingReader.Close(); // Close the DataReader
                        }

                        // Commit the transaction
                        
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // An error occurred, rollback the transaction
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return foundBookings;
        }
        //Method to get a list of bookings searching on phone number.
        public List<Booking> GetBookingsByCustomerPhone(string phone)
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string queryString = "SELECT b.*, c.* FROM Booking b INNER JOIN Customer c ON b.CustomerID = c.id WHERE phone = @Phone";

                    con.Open(); 

                    // Start a transaction to handle concurrency
                    using (SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand command = new SqlCommand(queryString, con, transaction);
                            command.Parameters.AddWithValue("@Phone", phone);

                            SqlDataReader reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                Booking booking = new Booking
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    StartDateTime = reader.GetDateTime(reader.GetOrdinal("StartDateTime")),
                                    HoursToPlay = reader.GetInt32(reader.GetOrdinal("HoursToPlay")),
                                    NoOfPlayers = reader.GetInt32(reader.GetOrdinal("NoOfPlayers")),
                                    Customer = new Customer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("customerID")),
                                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        Phone = reader.GetString(reader.GetOrdinal("Phone"))
                                    }
                                };
                                bookings.Add(booking);
                            }
                            reader.Close();
                            // Commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // An error occurred, rollback the transaction
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately or log the error
                Console.WriteLine($"An error occurred while retrieving bookings for customer phone {phone}: {ex.Message}");
                return null;
            }

            return bookings;
        }

        //Method to updateBooking
        public bool UpdateBooking(Booking bookingToUpdate)
        {
            bool isUpdated = false;
            string updateString = "UPDATE Booking SET startDateTime = @StartDateTime, hoursToPlay = @HoursToPlay, customerID = @CustomerID, noOfPlayers = @NoOfPlayers WHERE id = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand updateCommand = new SqlCommand(updateString, con))
            {
                //Updating the booking details in the database
                updateCommand.Parameters.AddWithValue("@Id", bookingToUpdate.Id);
                updateCommand.Parameters.AddWithValue("@StartDateTime", bookingToUpdate.StartDateTime);
                updateCommand.Parameters.AddWithValue("@HoursToPlay", bookingToUpdate.HoursToPlay);
                updateCommand.Parameters.AddWithValue("@CustomerID", bookingToUpdate.Customer.Id);
                updateCommand.Parameters.AddWithValue("@NoOfPlayers", bookingToUpdate.NoOfPlayers);

                con.Open();
                int rowsAffected = updateCommand.ExecuteNonQuery();

                isUpdated = (rowsAffected > 0);
            }

            return isUpdated;
        }
        //Crates a booking from the sql data.
        private Booking GetBookingFromReader(SqlDataReader bookingReader)
        {
            int customerId = bookingReader.GetInt32(bookingReader.GetOrdinal("CustomerID"));

            Customer customer = GetCustomerById(customerId); // Call a method to retrieve the customer by ID

            Booking foundBooking;
            int tempId = bookingReader.GetInt32(bookingReader.GetOrdinal("id"));
            DateTime tempStartDateTime = bookingReader.GetDateTime(bookingReader.GetOrdinal("StartDateTime"));
            int tempHoursToPlay = bookingReader.GetInt32(bookingReader.GetOrdinal("HoursToPlay"));
            int tempNoOfPlayers = bookingReader.GetInt32(bookingReader.GetOrdinal("NoOfPlayers"));

            foundBooking = new Booking(tempId, tempStartDateTime, tempHoursToPlay, tempNoOfPlayers, customer);
            return foundBooking;
        }

        //private method to find a customer by searching on the customer's ID
        private Customer GetCustomerById(int customerId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand("SELECT * FROM Customer WHERE id = @customerId", con))
            {
                command.Parameters.AddWithValue("@customerId", customerId);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Retrieve customer details from the reader and return the customer object
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                    string lastName = reader.GetString(reader.GetOrdinal("LastName"));
                    string email = reader.GetString(reader.GetOrdinal("Email"));
                    string phone = reader.GetString(reader.GetOrdinal("Phone"));

                    return new Customer(id, firstName, lastName, email, phone);
                }
            }

            return null; // Customer not found
        }

        //Method to create the LaneBookign that gets posted to sql.
        public bool CreateLaneBooking(int laneId, int bookingId, SqlTransaction transaction)
        {
            bool isCreated = false;
            string insertString = "INSERT INTO LaneBooking (LaneId, BookingId) VALUES (@LaneId, @BookingId)";

            using (SqlCommand createCommand = new SqlCommand(insertString, transaction.Connection, transaction))
            {
                createCommand.Parameters.AddWithValue("@LaneId", laneId);
                createCommand.Parameters.AddWithValue("@BookingId", bookingId);

                int rowsAffected = createCommand.ExecuteNonQuery();
                isCreated = (rowsAffected > 0);
            }

            return isCreated;
        }
        //Method to find a booking by ID
        public Booking GetBookingById(int id)
        {
            string queryString = @"SELECT b.Id, b.hoursToPlay, b.startDateTime, b.noOfPlayers, c.Id AS CustomerId, c.FirstName, c.LastName, c.Email, c.Phone, lb.LaneId, b.PriceId
                                FROM Booking AS b
                                JOIN LaneBooking AS lb ON b.Id = lb.BookingId
                                JOIN Customer AS c ON b.CustomerId = c.Id
                                WHERE b.Id = @Id";


            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                readCommand.Parameters.AddWithValue("@Id", id);

                con.Open();
                SqlDataReader bookingReader = readCommand.ExecuteReader();

                while (bookingReader.Read())
                {
                    Booking foundBooking = new Booking
                    {
                        Id = bookingReader.GetInt32(bookingReader.GetOrdinal("Id")),
                        HoursToPlay = bookingReader.GetInt32(bookingReader.GetOrdinal("hoursToPlay")),
                        StartDateTime = bookingReader.GetDateTime(bookingReader.GetOrdinal("startDateTime")),
                        NoOfPlayers = bookingReader.GetInt32(bookingReader.GetOrdinal("noOfPlayers")),
                        Customer = new Customer
                        {
                            Id = bookingReader.GetInt32(bookingReader.GetOrdinal("CustomerId")),
                            FirstName = bookingReader.GetString(bookingReader.GetOrdinal("FirstName")),
                            LastName = bookingReader.GetString(bookingReader.GetOrdinal("LastName")),
                            Email = bookingReader.GetString(bookingReader.GetOrdinal("Email")),
                            Phone = bookingReader.GetString(bookingReader.GetOrdinal("Phone"))
                        },
                        PriceId = bookingReader.GetInt32(bookingReader.GetOrdinal("PriceId")),
                        LaneId = bookingReader.GetInt32(bookingReader.GetOrdinal("LaneId"))
                    };

                    return foundBooking; // Exit the loop after processing the first row
                }
            }

            return null; // Return null if no booking is found
        }

        //Method to check if a lane is available for that day and time.
        private bool IsLaneAvailable(DateTime startDateTime, int hoursToPlay, out int availableLaneId)
        {
            DateTime endDateTime = startDateTime.AddHours(hoursToPlay);

            string query = @"
        WITH AvailableLanes AS (
            SELECT L.Id AS LaneID
            FROM Lane L
            LEFT JOIN LaneBooking LB ON L.Id = LB.LaneID
            LEFT JOIN Booking B ON LB.BookingID = B.Id
            WHERE NOT EXISTS (
                SELECT 1
                FROM LaneBooking LB2
                JOIN Booking B2 ON LB2.BookingID = B2.Id
                WHERE LB2.LaneID = L.Id
                AND B2.StartDateTime <= @EndDateTime
                AND DATEADD(HOUR, B2.HoursToPlay, B2.StartDateTime) >= @StartDateTime
            )
        )
        SELECT TOP 1 LaneID
        FROM AvailableLanes
        ORDER BY NEWID()";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(query, con))
            {
                command.Parameters.AddWithValue("@StartDateTime", startDateTime);
                command.Parameters.AddWithValue("@EndDateTime", endDateTime);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    availableLaneId = reader.GetInt32(0);
                    return true;
                }
            }

            availableLaneId = -1;
            return false;
        }
        //Find the speicifc day of the week
        public string GetBookingStartDay(int bookingId)
        {
            string selectStartDayQuery = "SELECT StartDateTime FROM Booking WHERE Id = @BookingId";
            string startDay = null;

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand selectCommand = new SqlCommand(selectStartDayQuery, con))
            {
                selectCommand.Parameters.AddWithValue("@BookingId", bookingId);

                con.Open();
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        DateTime startDateTime = reader.GetDateTime(reader.GetOrdinal("StartDateTime"));
                        startDay = startDateTime.ToString("dddd");
                    }
                }
            }

            return startDay;
        }

        public int GetPriceIdByWeekday(string weekday)
        {
            string selectPriceIdQuery = "SELECT Id FROM Price WHERE Weekday = @Weekday";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand selectCommand = new SqlCommand(selectPriceIdQuery, con))
            {
                selectCommand.Parameters.AddWithValue("@Weekday", weekday);

                con.Open();
                int priceId = (int)selectCommand.ExecuteScalar();

                return priceId;
            }
        }
        public int GetLaneIdByBookingId(int bookingId)
        {
            int laneId = -1; // Default value if laneId is not found

            string queryString = "SELECT LaneId FROM LaneBooking WHERE BookingId = @BookingId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(queryString, con))
            {
                command.Parameters.AddWithValue("@BookingId", bookingId);

                con.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    laneId = Convert.ToInt32(result);
                }
            }

            return laneId;
        }
        public bool UpdateBookingPrice(int bookingId, int newPriceId)
        {
            string updateString = "UPDATE Booking SET priceId = @NewPriceId WHERE Id = @BookingId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand updateCommand = new SqlCommand(updateString, con))
            {
                updateCommand.Parameters.AddWithValue("@NewPriceId", newPriceId);
                updateCommand.Parameters.AddWithValue("@BookingId", bookingId);

                con.Open();
                int rowsAffected = updateCommand.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }

}
