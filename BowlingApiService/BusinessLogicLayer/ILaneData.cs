using BowlingApiService.DTOs;

namespace BowlingApiService.BusinessLogicLayer
{
    public interface ILaneData
    {
        LaneDto? Get(int id);
        List<LaneDto>? Get();
        int Add(LaneDto laneToAdd);
        bool Put(LaneDto laneToUpdate, int intIdToUpdate);
        bool Delete(int id);

    }
}
