var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
