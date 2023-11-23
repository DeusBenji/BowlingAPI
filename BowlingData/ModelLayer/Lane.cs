using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingData.ModelLayer
{
    public class Lane
    {
        public int Id { get; set; }
        public int? LaneNumber { get; set; }

        // Empty Constructor
        public Lane() { }

        // Constructor with LaneNumber parameter
        public Lane(int? inLaneNumber)
        {
            LaneNumber = inLaneNumber;
        }
        // Constructor with Id and LaneNumber parameters
        public Lane(int id, int? inLaneNumber) : this(inLaneNumber)
        {
            Id = id;
        }

        // Check if the Lane is empty (no LaneNumber)
        public bool IsLaneEmpty
        {
            get
            {
                if (LaneNumber == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
