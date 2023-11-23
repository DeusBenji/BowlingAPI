using Microsoft.Extensions.Configuration;
using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingData.DatabaseLayer
{
    public class CustomerDatabaseAccess : ICustomerAccess
    {

        readonly string? _connectionString;
        public CustomerDatabaseAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CompanyConnection");
        }

        public CustomerDatabaseAccess(string inConnectionString)
        {
            _connectionString = inConnectionString;
        }
        public int CreateCustomer(Customer aCustomer)
        {
            int insertedId = -1;
            string insertString = "insert into Customer(firstName, lastName, email, phone) OUTPUT INSERTED.ID values(@FirstName, @LastName, @Email, @Phone)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand createCommand = new SqlCommand(insertString, con, transaction))
                        {
                            // Prepare SQL
                            SqlParameter fNameParam = new SqlParameter("@FirstName", aCustomer.FirstName);
                            createCommand.Parameters.Add(fNameParam);
                            SqlParameter lNameParam = new SqlParameter("@LastName", aCustomer.LastName);
                            createCommand.Parameters.Add(lNameParam);
                            SqlParameter emailParam = new SqlParameter("@Email", aCustomer.Email);
                            createCommand.Parameters.Add(emailParam);
                            SqlParameter phoneParam = new SqlParameter("@Phone", aCustomer.Phone);
                            createCommand.Parameters.Add(phoneParam);

                            // Execute save and read generated key (ID)
                            insertedId = (int)createCommand.ExecuteScalar();
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

            return insertedId;
        }
        //Method to delete a customer where we search on the id
        public bool DeleteCustomerById(int id)
        {
            bool isDeleted = false;

            // Delete bookings associated with the customer
            string deleteBookingsString = "DELETE FROM Booking WHERE customerID = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlCommand deleteBookingsCommand = new SqlCommand(deleteBookingsString, con))
                {
                    deleteBookingsCommand.Parameters.AddWithValue("@Id", id);

                    deleteBookingsCommand.ExecuteNonQuery();
                }

                // Delete the customer
                string deleteCustomerString = "DELETE FROM Customer WHERE id = @Id";

                using (SqlCommand deleteCustomerCommand = new SqlCommand(deleteCustomerString, con))
                {
                    deleteCustomerCommand.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = deleteCustomerCommand.ExecuteNonQuery();
                    isDeleted = (rowsAffected > 0);
                }
            }

            return isDeleted;
        }

        //Method to get a list of all customers.
        public List<Customer> GetAllCustomers()
        {
            List<Customer> foundCustomers;
            Customer readCustomer;
            //
            string queryString = "select id, firstName, lastName, email, phone from Customer";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                con.Open();
                // Execute read
                SqlDataReader customerReader = readCommand.ExecuteReader();
                // Collect data
                foundCustomers = new List<Customer>();
                while (customerReader.Read())
                {
                    readCustomer = GetCustomerFromReader(customerReader);
                    foundCustomers.Add(readCustomer);
                }
            }
            return foundCustomers;
        }
        //Method get a customer by it's ID
        public Customer GetCustomerById(int id)
        {
            Customer foundCustomer;
            //
            string queryString = "select id, firstName, lastName, email, phone from Customer where id = @Id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                // Prepace SQL
                SqlParameter idParam = new SqlParameter("@Id", id);
                readCommand.Parameters.Add(idParam);
                //
                con.Open();
                // Execute read
                SqlDataReader customerReader = readCommand.ExecuteReader();
                foundCustomer = new Customer();
                while (customerReader.Read())
                {
                    foundCustomer = GetCustomerFromReader(customerReader);
                }
            }
            return foundCustomer;
        }
        //Method to update a customer
        public bool UpdateCustomer(Customer customerToUpdate)
        {
            bool isUpdated = false;
            string updateString = "UPDATE Customer SET firstName = @FirstName, lastName = @LastName, email = @Email, phone = @Phone WHERE id = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand updateCommand = new SqlCommand(updateString, con, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Id", customerToUpdate.Id);
                            updateCommand.Parameters.AddWithValue("@FirstName", customerToUpdate.FirstName);
                            updateCommand.Parameters.AddWithValue("@LastName", customerToUpdate.LastName);
                            updateCommand.Parameters.AddWithValue("@Email", customerToUpdate.Email);
                            updateCommand.Parameters.AddWithValue("@Phone", customerToUpdate.Phone);

                            int rowsAffected = updateCommand.ExecuteNonQuery();

                            if (isUpdated = (rowsAffected > 0))
                            {
                                // Commit the transaction
                                transaction.Commit();
                                return isUpdated;
                            }
                            else
                            {
                                // No rows affected, rollback the transaction
                                transaction.Rollback();
                                return false;
                            }
                        }
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
        //Method to create a customer from the sqldata
        private Customer GetCustomerFromReader(SqlDataReader customerReader)
        {
            Customer foundCustomer;
            int tempId;
            string tempFirstName, tempLastName, tempEmail, tempPhone;
            // Fetch values
            tempId = customerReader.GetInt32(customerReader.GetOrdinal("id"));
            tempFirstName = customerReader.GetString(customerReader.GetOrdinal("firstName"));
            tempLastName = customerReader.GetString(customerReader.GetOrdinal("lastName"));
            tempEmail = customerReader.GetString(customerReader.GetOrdinal("email"));
            tempPhone = customerReader.GetString(customerReader.GetOrdinal("phone"));
            // Create object
            foundCustomer = new Customer(tempId, tempFirstName, tempLastName, tempEmail, tempPhone);
            return foundCustomer;
        }

        //Method to get a customer by phone Number
        public Customer GetCustomerByPhone(string phone)
        {
            Customer foundCustomer;
            //
            string queryString = "select id, firstName, lastName, email, phone from Customer where phone = @Phone";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                // Prepace SQL
                SqlParameter idParam = new SqlParameter("@Phone", phone);
                readCommand.Parameters.Add(idParam);
                //
                con.Open();
                // Execute read
                SqlDataReader customerReader = readCommand.ExecuteReader();
                foundCustomer = new Customer();
                while (customerReader.Read())
                {
                    foundCustomer = GetCustomerFromReader(customerReader);
                }
            }
            return foundCustomer;
        }


    }

}

