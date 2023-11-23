using BowlingData.ModelLayer;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Numerics;

namespace BowlingData.DatabaseLayer
{
    public class PriceDatabaseAccess : IPriceAccess
    {
        readonly string? _connectionString;

        public PriceDatabaseAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CompanyConnection");
        }

        public PriceDatabaseAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
        //Method to create a price.
        public int CreatePrice(Price aPrice)
        {
            int insertedId = -1;
            string insertString = "INSERT INTO Price (NormalPrice, Weekday) OUTPUT INSERTED.ID VALUES (@NormalPrice, @Weekday)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Set the transaction for the command
                        SqlCommand createCommand = new SqlCommand(insertString, con, transaction);
                        createCommand.Parameters.AddWithValue("@NormalPrice", aPrice.NormalPrice);
                        createCommand.Parameters.AddWithValue("@Weekday", aPrice.Weekday);

                        insertedId = (int)createCommand.ExecuteScalar();

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
        //Method to delete a price
        public bool DeletePriceById(int id)
        {
            bool isDeleted = false;
            //
            string deleteString = "DELETE FROM Price WHERE Id = @Id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand deleteCommand = new SqlCommand(deleteString, con))
            {
                deleteCommand.Parameters.AddWithValue("@Id", id);

                con.Open();
                int rowsAffected = deleteCommand.ExecuteNonQuery();

                isDeleted = (rowsAffected > 0);
            }

            return isDeleted;
        }
        //Method to get a list of all prices
        public List<Price> GetAllPrices()
        {
            List<Price> foundPrices;
            Price readPrice;
            //
            string queryString = "select Id, NormalPrice, Weekday from Price";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                con.Open();
                // Execute read
                SqlDataReader priceReader = readCommand.ExecuteReader();
                // Collect data
                foundPrices = new List<Price>();
                while (priceReader.Read())
                {
                    readPrice = GetPriceFromReader(priceReader);
                    foundPrices.Add(readPrice);
                }
            }
            return foundPrices;
        }
        //Method to get a price by it's id
        public Price GetPriceById(int id)
        {
            Price foundPrice;
            //
            string queryString = "select Id, NormalPrice, Weekday from Price where Id = @Id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                //Prepare SQL
                SqlParameter idParam = new SqlParameter("@Id", id);
                readCommand.Parameters.Add(idParam);
                //
                con.Open();
                //Execute reead
                SqlDataReader priceReader = readCommand.ExecuteReader();
                foundPrice = new Price();
                while (priceReader.Read())
                {
                    foundPrice = GetPriceFromReader(priceReader);
                }
            }
            return foundPrice;
        }
        //Method to update a price
        public bool UpdatePrice(Price priceToUpdate)
        {
            bool isUpdated = false;
            string updateString = "UPDATE Price SET NormalPrice = @NormalPrice, Weekday = @Weekday WHERE Id = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // Start a transaction to handle concurrency
                using (SqlTransaction transaction = con.BeginTransaction())
                {
                    try
                    {
                        // Set the transaction for the command
                        SqlCommand updateCommand = new SqlCommand(updateString, con, transaction);
                        updateCommand.Parameters.AddWithValue("@Id", priceToUpdate.Id);
                        updateCommand.Parameters.AddWithValue("@NormalPrice", priceToUpdate.NormalPrice);
                        updateCommand.Parameters.AddWithValue("@Weekday", priceToUpdate.Weekday);

                        int rowsAffected = updateCommand.ExecuteNonQuery();
                        isUpdated = (rowsAffected > 0);

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

            return isUpdated;
        }
        //Method to create a price object from the sqldata
        private Price GetPriceFromReader(SqlDataReader priceReader)
        {
            Price foundPrice;
            int tempID;
            double tempNormalPrice;
            string tempWeekDay;
            // Fetch values
            tempID = priceReader.GetInt32(priceReader.GetOrdinal("Id"));
            tempNormalPrice = priceReader.GetDouble(priceReader.GetOrdinal("NormalPrice"));
            tempWeekDay = priceReader.GetString(priceReader.GetOrdinal("Weekday"));
            //Create Price object
            foundPrice = new Price(tempID, tempNormalPrice, tempWeekDay);
            return foundPrice;
        }

    }
}
