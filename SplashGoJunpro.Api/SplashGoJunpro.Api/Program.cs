var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Allow Kestrel to use Railway's assigned PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

// Routing
app.UseRouting();
app.MapControllers();

app.Run();
