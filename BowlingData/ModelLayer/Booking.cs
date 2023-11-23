using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlingData.ModelLayer
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public int HoursToPlay { get; set; }
        public int NoOfPlayers { get; set; }
        public Customer? Customer { get; set; }
        public int PriceId { get; set; }
        public int LaneId { get; set; }

        // Empty constructor
        public Booking() { }

        // Constructor with parameters
        public Booking(DateTime startDateTime, int hoursToPlay, int noOfPlayers, Customer? cutomer)
        {
            StartDateTime = startDateTime;
            HoursToPlay = hoursToPlay;
            NoOfPlayers = noOfPlayers;
            Customer = cutomer;

        }

        // Reused constructor with the addition of the Id parameter
        public Booking(int id, DateTime startDateTime, int hoursToPlay, int noOfPlayers, Customer? customer) : this(startDateTime, hoursToPlay, noOfPlayers, customer)

        {
            Id = id;
        }

    }
}
