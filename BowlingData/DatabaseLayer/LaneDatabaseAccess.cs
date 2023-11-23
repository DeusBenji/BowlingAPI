using Microsoft.Extensions.Configuration;
using BowlingData.ModelLayer;
using System.Data.SqlClient;
using BowlingData.DatabaseLayer;
using System.Collections.Generic;


namespace BowlingData.DatabaseLayer
{
    public class LaneDatabaseAccess : ILaneAccess
    {
        readonly string? _connectionString;
        public LaneDatabaseAccess(IConfiguration configuration) 
        {
            _connectionString = configuration.GetConnectionString("CompanyConnection");
        }

        public LaneDatabaseAccess(string inConnectionString) 
        {
            _connectionString = inConnectionString;
        }
        //Method to create a lane
        public int CreateLane(Lane aLane)
        {
            int insertedId = -1;
            //
            string insertString = "insert into Lane(LaneNumber) OUTPUT INSERTED.ID values(@LaneNumber)";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand CreateCommand = new SqlCommand(insertString, con))
            {
                SqlParameter aLaneNrParam = new("@LaneNumber", aLane.LaneNumber);
                CreateCommand.Parameters.Add(aLaneNrParam);
                con.Open();
                // Execute save and read generated key (ID)
                insertedId = (int)CreateCommand.ExecuteScalar();
            }
            return insertedId;
        }
        //Method to delete a lane by it's Id
        public bool DeleteLaneById(int id)
        {
            bool isDeleted = false;
            //
            string deleteString = "DELETE FROM Lane WHERE Id = @Id";
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
        //Method to get a list of all the lanes.
        public List<Lane> GetAllLanes()
        {
            List<Lane> foundLanes;
            Lane readLane;
            //
            string queryString = "SELECT Id, laneNumber FROM Lane";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                con.Open();
                // Execute read
                SqlDataReader laneReader = readCommand.ExecuteReader();
                // Collect data
                foundLanes = new List<Lane>();
                while (laneReader.Read())
                {
                    readLane = GetLaneFromReader(laneReader);
                    foundLanes.Add(readLane);
                }
            }
            return foundLanes;
        }

        //Method to find a lane på it's Id
        public Lane GetLaneById(int id)
        {
            Lane foundLane;
            //
            string queryString = "select Id, laneNumber from Lane where id = @Id";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand readCommand = new SqlCommand(queryString, con))
            {
                //Prepare SQL
                SqlParameter idParam = new SqlParameter("@Id", id);
                readCommand.Parameters.Add(idParam);
                //
                con.Open();
                //Execute reead
                SqlDataReader laneReader = readCommand.ExecuteReader();
                foundLane = new Lane();
                while(laneReader.Read()) 
                { 
                   foundLane = GetLaneFromReader(laneReader);
                }
            }
            return foundLane;

        }
        //Method to update a lane.
        public bool UpdateLane(Lane laneToUpdate)
        {
            bool isUpdated = false;
            string updateString = "UPDATE Lane SET laneNumber = @LaneNumber WHERE Id = @Id";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand updateCommand = new SqlCommand(updateString, con))
            {
                updateCommand.Parameters.AddWithValue("@Id", laneToUpdate.Id);
                updateCommand.Parameters.AddWithValue("@LaneNumber", laneToUpdate.LaneNumber);

                con.Open();
                int rowsAffected = updateCommand.ExecuteNonQuery();

                if (isUpdated = (rowsAffected > 0))
                {
                    return isUpdated;
                }
                else
                {
                    return false;
                }
            }
        }
        //Method to create a lane object form the sqlData
        private Lane GetLaneFromReader(SqlDataReader laneReader) 
        {
            Lane foundLane;
            int tempID, tempLaneNumber;
            // Fetch values
            tempID = laneReader.GetInt32(laneReader.GetOrdinal("Id"));
            tempLaneNumber = laneReader.GetInt32(laneReader.GetOrdinal("laneNumber"));
            //Create lane object
            foundLane = new Lane(tempID, tempLaneNumber);
            return foundLane;
        }
    }
}
