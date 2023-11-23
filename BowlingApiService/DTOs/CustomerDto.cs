namespace BowlingApiService.DTOs
{
    public class CustomerDto
    {
        public CustomerDto() { }

        public CustomerDto(string? inFirstName, string? inLastName, string? inEmail, string? inPhone)
        {
            FirstName = inFirstName;
            LastName = inLastName;
            Email = inEmail;
            Phone = inPhone;
        }
        public CustomerDto(int inId, string? inFirstName, string? inLastName, string? inEmail, string? inPhone) : this(inFirstName, inLastName, inEmail, inPhone)
        {
            Id = inId;
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int Id { get; set; }
        public string? FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}
