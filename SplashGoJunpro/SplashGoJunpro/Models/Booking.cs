using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; private set; }
        public Destination Destination { get; set; }

        // Constructor to set initial status
        public Booking(Destination destination)
        {
            Destination = destination;
            Status = "Pending";
        }

        public void Confirm()
        {
            Status = "Confirmed";
        }

        public void Reject()
        {
            Status = "Rejected";
        }

        public void Cancel()
        {
            Status = "Cancelled";
        }

        public string GetStatus()
        {
            return Status;
        }
    }
}


