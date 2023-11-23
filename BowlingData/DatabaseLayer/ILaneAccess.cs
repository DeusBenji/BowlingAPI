using BowlingData.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingData.DatabaseLayer
{
    public interface ILaneAccess
    {
        Lane GetLaneById(int id);
        List<Lane> GetAllLanes();
        int CreateLane(Lane aLane);
        bool UpdateLane(Lane laneToUpdate);
        bool DeleteLaneById(int id);
    }
}
