using Microsoft.AspNetCore.Mvc;
using SplashGoJunpro.Api.Models;
using SplashGoJunpro.Api.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost("notification")]
    public async Task<IActionResult> Notification([FromBody] MidtransNotification notification)
    {
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
            { "@OrderCode", notification.order_code }
        };

        await db.ExecuteAsync(sql, parameters);

        return Ok();
    }
}
