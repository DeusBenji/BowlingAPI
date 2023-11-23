using BowlingApiService.DTOs;
using BowlingData.ModelLayer;

namespace BowlingApiService.ModelConversion
{
    public class LaneDtoConvert
    {
        // Convert from Lane objects to LaneDto objects
        public static List<LaneDto>? FromLaneCollection(List<Lane> inLanes)
        {
            List<LaneDto>? aLaneReadDtoList = null;
            if (inLanes != null)
            {
                aLaneReadDtoList = new List<LaneDto>();
                LaneDto? tempDto;
                foreach (Lane aLanes in inLanes)
                {
                    if (aLanes != null)
                    {
                        tempDto = FromLane(aLanes);
                        aLaneReadDtoList.Add(tempDto);
                    }
                }
            }
            return aLaneReadDtoList;
        }

        // Convert from Lane object to LaneDto object
        public static LaneDto? FromLane(Lane inLanes)
        {
            LaneDto? aLanesReadDto = null;
            if (inLanes != null)
            {
                aLanesReadDto = new LaneDto(inLanes.LaneNumber);
                aLanesReadDto.Id = inLanes.Id;
            }
            return aLanesReadDto;
        }

        // Convert from LaneDto object to Lane object
        public static Lane? ToLane(LaneDto inDto)
        {
            Lane? aLanes = null;
            if (inDto != null)
            {
                aLanes = new Lane(inDto.LaneNumber);
            }
            return aLanes;
        }
        // Convert from LaneDto object to Lane object
        public static Lane? ToLane(LaneDto inDto, int idToUpdate)
        {
            Lane? aLanes = null;
            if (inDto != null)
            {
                aLanes = new Lane(inDto.LaneNumber);
                aLanes.Id = idToUpdate;
            }
            return aLanes;
        }

    }
}
