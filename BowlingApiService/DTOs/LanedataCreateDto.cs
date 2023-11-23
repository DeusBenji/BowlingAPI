namespace BowlingApiService.DTOs
{
    public class LanedataCreateDto
    {
        public LanedataCreateDto() { }
        public LanedataCreateDto(int InLaneNumber)
        {
            LaneNumber = InLaneNumber;
        }
        public int LaneNumber { get; set; }
    }
}

