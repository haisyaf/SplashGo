using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class Tourist : User
    {
        public string PassportNumber { get; set; }
        private List<Booking> Bookings { get; set; } = new List<Booking>();

        public Booking MakeBooking(Destination d)
        {
            var booking = new Booking(d); // now uses constructor
            Bookings.Add(booking);
            return booking;
        }

        public bool CancelBooking(Booking b)
        {
            if (Bookings.Contains(b))
            {
                b.Cancel();
                return true;
            }
            return false;
        }

        public List<Booking> ViewBookings()
        {
            return Bookings;
        }
    }
}
