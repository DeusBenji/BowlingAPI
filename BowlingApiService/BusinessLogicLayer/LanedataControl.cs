using BowlingApiService.DTOs;
using BowlingData.DatabaseLayer;
using BowlingData.ModelLayer;
using System.Linq.Expressions;

namespace BowlingApiService.BusinessLogicLayer
{
    public class LanedataControl : ILaneData
    {
        private readonly ILaneAccess _laneAcces;

        public LanedataControl(ILaneAccess inLaneAccess)
        {
            _laneAcces = inLaneAccess;
        }

        // Adds a new lane to the database
        public int Add(LaneDto newLane)
        {
            int insertedId = 0;
            try
            {
                Lane? foundLane = ModelConversion.LaneDtoConvert.ToLane(newLane);
                if (foundLane != null)
                {
                    // Create the lane in the database and get the inserted ID
                    insertedId = _laneAcces.CreateLane(foundLane);
                }
            }
            catch
            {
                // If an exception occurs, set the inserted ID to -1
                insertedId = -1;
            }
            return insertedId;
        }

        // Deletes a lane from the database
        public bool Delete(int id)
        {
            try
            {
                // Delete the lane from the database by ID
                bool isDeleted = _laneAcces.DeleteLaneById(id);
                return isDeleted;
            }
            catch
            {
                // If an exception occurs, return false
                return false;
            }
        }

        // Retrieves a lane from the database by ID
        public LaneDto? Get(int id)
        {
            LaneDto? foundLaneDto;
            try
            {
                // Get the lane from the database by ID
                Lane? foundLane = _laneAcces.GetLaneById(id);
                // Convert the retrieved Lane object to LaneDto
                foundLaneDto = ModelConversion.LaneDtoConvert.FromLane(foundLane);
            }
            catch
            {
                // If an exception occurs, set foundLaneDto to null
                foundLaneDto = null;
            }
            return foundLaneDto;
        }

        // Retrieves all lanes from the database
        public List<LaneDto>? Get()
        {
            List<LaneDto>? foundDtos;
            try
            {
                // Get all lanes from the database
                List<Lane>? foundLanes = _laneAcces.GetAllLanes();
                // Convert the list of Lane objects to LaneDto objects
                foundDtos = ModelConversion.LaneDtoConvert.FromLaneCollection(foundLanes);

                if (foundDtos != null)
                {
                    // Ensure data consistency by fetching each Lane object from the database and updating the LaneDto's ID
                    foreach (var dto in foundDtos)
                    {
                        Lane lane = _laneAcces.GetLaneById(dto.Id);
                        dto.Id = lane.Id;
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

        // Updates a lane in the database
        public bool Put(LaneDto laneToUpdate, int idToUpdate)
        {
            try
            {
                // Convert the updated LaneDto object to Lane object
                Lane? updatedLane = ModelConversion.LaneDtoConvert.ToLane(laneToUpdate, idToUpdate);
                // Update the lane in the database
                return _laneAcces.UpdateLane(updatedLane);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine(ex);
                // If an exception occurs, return false
                return false;
            }
        }
    }
}
