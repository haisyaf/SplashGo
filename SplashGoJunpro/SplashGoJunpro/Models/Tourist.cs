using System.Collections.Generic;

namespace SplashGoJunpro.Models
{
    public class Tourist
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string IDNumber { get; set; }  // Passport or National ID
        public string PhoneNumber { get; set; }

        private List<Booking> Bookings { get; set; } = new List<Booking>();

        public Booking MakeBooking(Destination destination)
        {
            var booking = new Booking(destination);
            Bookings.Add(booking);
            return booking;
        }

        public bool CancelBooking(Booking booking)
        {
            if (Bookings.Contains(booking))
            {
                booking.Cancel();
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
