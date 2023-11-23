namespace BowlingApiService.DTOs
{
    public class LaneDto
    {
        public int Id { get; set; }
        public int? LaneNumber { get; set; }

        // Empty Constructor
        public LaneDto() { }

        // Constructor with LaneNumber parameter
        public LaneDto(int? inLaneNumber)
        {
            LaneNumber = inLaneNumber;
        }
        // Constructor with Id and LaneNumber parameters
        public LaneDto(int id, int? inLaneNumber) : this(inLaneNumber)
        {
            Id = id;
        }
    }
}
