namespace BowlingApiService.DTOs
{
    public class PriceDto
    {
        public int Id { get; set; }
        public double? NormalPrice { get; set; }
        public string? Weekday { get; set; }

        // Empty constructor
        public PriceDto() { }

        // Constructor with parameters
        public PriceDto(double? normalPrice, string? weekday)
        {
            NormalPrice = normalPrice;
            Weekday = weekday;
        }
        //Reuses constructor with id parameter
        public PriceDto(int id, double? normalPrice, string? weekday) : this(normalPrice, weekday)
        {
            Id = id;
        }
    }
}

