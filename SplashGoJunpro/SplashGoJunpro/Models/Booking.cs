using System;

namespace SplashGoJunpro.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string OrderCode { get; set; }
        public int UserId { get; set; }
        public int DestinationId { get; set; }
        public Destination Destination { get; set; }

        public string FullName { get; set; }
        public string IdNumber { get; set; }
        public string MobileNumber { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.Now;
        public int PaxCount { get; set; }
        public decimal TotalPrice { get; set; }

        public string Status { get; private set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public string SnapToken { get; set; }

        // Constructor for new booking
        public Booking(Destination destination)
        {
            Destination = destination;
            DestinationId = destination.DestinationId;
            Status = "Pending";
            CreatedAt = DateTime.Now;
        }


        // Status transitions
        public void Confirm() => Status = "Confirmed";
        public void Reject() => Status = "Rejected";
        public void Cancel() => Status = "Cancelled";
        public void MarkPaid() => Status = "Paid";

        public string GetStatus() => Status;
    }
}
