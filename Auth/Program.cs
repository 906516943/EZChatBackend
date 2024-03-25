using Auth.Core.Externals;
using Auth.Core.Models;
using Auth.Core.Repos;
using Auth.Core.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConn")!));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("ASPNETCORE_HTTP_PORTS"));
builder.Configuration.AddJsonFile("/apiServers.json");
builder.Services.Configure<ApiServers>(builder.Configuration.GetSection("apiServers"));


//configure http clients
var configuration = builder.Configuration;

builder.Services.AddHttpClient<IUserApi, UserApi>(x => 
{
    x.BaseAddress = new Uri(configuration.GetSection("apiServers").GetValue<string>("UserApi")!);
});



var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

app.Run();
