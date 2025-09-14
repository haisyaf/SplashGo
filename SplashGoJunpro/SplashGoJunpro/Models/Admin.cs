using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class Admin : User
    {
        public override bool Login(string email, string password)
        {
            // Reuse User's login logic
            return base.Login(email, password);

            // Additional admin-specific login logic can be added here
            //...
        }


        // Admin can confirm a booking
        public void ConfirmBooking(Booking booking)
        {
            if (IsLoggedIn)
                booking.Confirm();
        }

        // Admin can reject a booking
        public void RejectBooking(Booking booking)
        {
            if (IsLoggedIn)
                booking.Reject();
        }

        // Admin can cancel a booking
        public void CancelBooking(Booking booking)
        {
            if (IsLoggedIn)
                booking.Cancel();
        }

        // Admin can update destination info
        public void UpdateDestination(Destination destination, string name, string location, string desc)
        {
            if (IsLoggedIn)
                destination.UpdateInfo(name, location, desc);
        }
    }
}


