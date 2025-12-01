using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplashGoJunpro.Api.Data;
using SplashGoJunpro.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("notification")]
    public async Task<IActionResult> Notification([FromBody] MidtransNotification notification)
    {
        Console.WriteLine($"Received notification: {notification.order_id}, status: {notification.transaction_status}");

        string status = notification.transaction_status switch
        {
            "settlement" => "Paid",
            "pending" => "Pending",
            "expire" => "Expired",
            "cancel" => "Cancelled",
            _ => "Unknown"
        };

        var db = new NeonDB();
        string sql = @"UPDATE bookings SET status = @Status WHERE order_code = @OrderCode";
        var parameters = new Dictionary<string, object>
        {
            { "@Status", status },
            { "@OrderCode", notification.order_id } 
        };

        int rows = await db.ExecuteAsync(sql, parameters);
        Console.WriteLine($"Rows affected: {rows}");

        return Ok();
    }
}
